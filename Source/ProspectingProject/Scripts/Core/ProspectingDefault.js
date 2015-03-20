/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// ProspectingDefault.js - core javascript for Prospecting
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Reference to the currently selected marker
var currentMarker = null, tempIndicatorMarker = null;

// The property currently being prospected: this has a value when an orange dot is created (prospecting shell is created when an empty record is inserted into the prospecting_property table).
// The shell is created under the following conditions:
// 1) A lightstone property is clicked that has not previously been clicked.
// 2) An unknown point is being interrogated.
// Additional notes: The ability to add contacts or view existing contacts should always be tied to currentProperty not being null.
//                   If currentProperty is null, you should never be able to create, edit or view contacts - the two are directly linked.        
var currentProperty = null;

// Stores the lat/long of the point the last point the user clicked
var currentClickLatLng = null;

// Stores a reference to the current suburb
var currentSuburb = null;

// Currently right-clicked marker
var rightClickedProperty = null;

function initEventHandlers() {
    $.contextMenu({
        selector: '.context-menu-prospect',
        build: function ($trigger, e) {
            return {
                callback: function (key, options) {
                    switch (key) {
                        case "Search Lightstone here":
                            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Lightstone results...</p>' });
                            buildLightstoneMatchesContent();
                            break;
                    }
                },
                items: {
                    "Search Lightstone here": { name: "Search Lightstone here", icon: "lightstone" },
                }
            };
        }
    });

    $.contextMenu({
        selector: '.context-menu-rightclick-property',
        build: function ($trigger, e) {
            return {
                callback: function (key, options) {
                    switch (key) {
                        case "Add Activity":
                            handlePropertyRightClick();
                            break;
                    }
                },
                items: { "Add Activity": { name: "Add Activity", icon: "add_activity" } }
            };
        }
    });
}

function handlePropertyRightClick() {
    if (rightClickedProperty != null) {
        loadExistingProspectAddActivity(rightClickedProperty, null, null);
    }
    
    rightClickedProperty = null;
}

function showPopupAtLocation(loc, contentHtml) {
    closeInfoWindow();

    tempIndicatorMarker = new google.maps.Marker({
        position: new google.maps.LatLng(loc.lat(), loc.lng()),
        map: map
    });

    var html = "<div class='info-window'>" + contentHtml.html() + "</div>";
    infowindow = new google.maps.InfoWindow({ content: html  });
    google.maps.event.addListener(infowindow, 'closeclick', function () {
        if (tempIndicatorMarker) {
            tempIndicatorMarker.setMap(null);
            tempIndicatorMarker = null;
        }
    });

    infowindow.open(map, tempIndicatorMarker);
    $.unblockUI();
}

function buildLightstoneMatchesContent() {
    clearLightstoneSearchResults(); // This is to ensure that any existing results from a lightstone search are removed from front-end because the server cache is receiving new data making the existing results stale. 
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({
            Instruction: 'get_matching_addresses',
            LatLng: { Lat: currentClickLatLng.lat(), Lng: currentClickLatLng.lng() },
            SeeffAreaId: currentSuburb.SuburbId
        }),
        dataType: "json",
    }).done(function (matchesData) {
        var content = generateOutputFromLightstone(matchesData);
        showPopupAtLocation(currentClickLatLng, content);
    });
}

function generateOutputFromLightstone(data) {

    function buildAddress(area, showPropId) {
        if (showPropId) {
            return area.StreetOrUnitNo + ' ' + area.StreetName + ', ' + area.Suburb + ', ' + area.City + ' (PropertyID:' + area.LightstonePropId + ')';
        }        
        return area.StreetOrUnitNo + ' ' + area.StreetName + ', ' + area.Suburb + ', ' + area.City;
    }

    var div = $("<div />");
    if (data.length != 0) {
        div.append('<span class="searchResultsHeader">The following Lightstone complexes and properties were found at this location:<p /></span>');
        var form = $('<form id="areaMatchesForm" />');
        form.empty();        
        var itemIndex = {};
        $.each(data, function (index, entity) {
            var itemDescription = '';
            var entityAlreadyExists = false;
            if (entity.IsSectionalScheme) {
                var numUnits = entity.PropertyMatches.length;
                itemDescription = entity.SectionalScheme + ' (' + numUnits + ' units) : ' + buildAddress(entity.PropertyMatches[0], false);
                entityAlreadyExists = entity.Exists;
            }
            else if (entity.IsFarm) {
                var frmPortion = entity.PropertyMatches[0];
                itemDescription = "Farm: " + frmPortion.FarmName + " [Erf no.: " + frmPortion.ErfNo + ", Portion: " + frmPortion.Portion + "]";
                entityAlreadyExists = frmPortion.LightstoneIdExists || (currentProperty && currentProperty.LightstonePropertyId == frmPortion.LightstonePropId);
            }
            else {
                // Must be an FH. FRM add here.
                var freehold = entity.PropertyMatches[0]; 
                itemDescription = buildAddress(freehold, true);
                entityAlreadyExists = freehold.LightstoneIdExists || (currentProperty && currentProperty.LightstonePropertyId == freehold.LightstonePropId);
            }
            // Only add the search result if it does not already exist
            if (!entityAlreadyExists) {
                var checkItemId = 'checkitem_' + index;
                var areaCheckItem = $('<input type="checkbox" name="check_list" id="' + checkItemId + '" value="' + index + '" checked /><label for="' + checkItemId + '">' + itemDescription + '</label>');
                itemIndex[checkItemId] = entity;
                form.append(areaCheckItem);
                form.append("<br />");
            }
        });        

        form.append("<p />");
        var createBtn = $('<input type="button" id="createProspectBtn" value="Create Prospect(s)" />');
        form.append(createBtn);

        // If no items were added, show it.
        if ($.isEmptyObject(itemIndex)) {
            div.empty();
            div.append('All properties and complexes have already been prospected at this location.');
            createBtn.attr('disabled', 'disabled');
        }

        div.append(form);

        $('body').unbind('click.createProspect').on('click.createProspect', '#createProspectBtn', function () {
            var selectedEntities = [];
            $('#areaMatchesForm input:checked').each(function () {
                var id = $(this).attr('id');
                var entity = itemIndex[id];
                selectedEntities.push(entity);
            });

            if (selectedEntities.length > 0) {
                closeInfoWindow();
                $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Creating new prospects...</p>' });

                createProspectingEntities(selectedEntities, function (data) {
                    var dataObject = $.parseJSON(data);
                    if (dataObject.CreationErrorMsg) {
                        $.unblockUI();
                        alert(dataObject.CreationErrorMsg);
                    }
                    else {
                        if (currentSuburb) {
                            clearSuburbBySuburbId(currentSuburb.SuburbId);
                            currentSuburb.IsInitialised = false;
                        }
                        var seeffSuburbId = dataObject.SeeffAreaId;
                        loadSuburb(seeffSuburbId, false, function () {
                            var countProspectsCreated = Object.keys(dataObject.SectionalSchemes).length + dataObject.FhProperties.length;
                            if (countProspectsCreated == 1) {
                                // if only one unit was created, show its info bubble
                                var targetProp = $.grep(currentSuburb.ProspectingProperties, function(pp, x){
                                    return pp.LightstonePropertyId == dataObject.TargetProspect.LightstonePropertyId;
                                })[0];
                                
                                currentProperty = targetProp;
                                new google.maps.event.trigger(currentProperty.Marker, 'click');
                            } else {
                                currentProperty = null;
                            }
                        }, false);
                    }
                },
                function (data) {
                    $.unblockUI();
                    alert(data.CreationErrorMsg);
                });
            }
        });
    }
    else {
        if (data.CreationErrorMsg) {
            div.append(data.CreationErrorMsg);
        }
        else {
            div.append("No Lightstone data found for this location.");
        }
    }

    return div;
}

function createProspectingEntities(selectedEntities, callbackSuccess, callbackFail) {
    var inputData = { Instruction: 'create_new_prospects', SectionalSchemes: [], FHProperties: [] };
    $.each(selectedEntities, function (index, entity) {
        if (entity.IsSectionalScheme) {
            inputData.SectionalSchemes.push(entity);
        }
        else {
            // Must be FH
            inputData.FHProperties.push(entity);
        }
    });

    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        timeout: 240000,
        data: JSON.stringify(inputData)
    })
        .done(function (data) {
            if (callbackSuccess) callbackSuccess(data);
        })
        .fail(function (data) {
            if (callbackFail) callbackFail(data);
        });
}

function fromLatLngToPoint(latLng, map) {
    var topRight = map.getProjection().fromLatLngToPoint(map.getBounds().getNorthEast());
    var bottomLeft = map.getProjection().fromLatLngToPoint(map.getBounds().getSouthWest());
    var scale = Math.pow(2, map.getZoom());
    var worldPoint = map.getProjection().fromLatLngToPoint(latLng);
    return new google.maps.Point((worldPoint.x - bottomLeft.x) * scale, (worldPoint.y - topRight.y) * scale);
}

function handleMapClick(event) {
    currentClickLatLng = new google.maps.LatLng(event.latLng.lat(), event.latLng.lng());

    var point = fromLatLngToPoint(currentClickLatLng, map);
    $('.context-menu-prospect').contextMenu({ x: point.x, y: point.y });
}

function performPersonLookup(idNumber, lookupType) {

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Performing Enquiry...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_prop_owner_details", LightstoneIDOrCKNo: idNumber, PersonLookupType: lookupType, ProspectingPropertyId: currentProperty.ProspectingPropertyId }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }
                currentMarker.ContactInfoPacket = data;
                if (data.WalletBalance != null) {
                    // If the AvailableTracePsCredits is not null, then update the availableCredit variable
                    availableCredit = data.WalletBalance;
                    $('#availableCreditLabel').text(availableCredit);
                }
                if (data.EnquirySuccessful) {
                    populateContactLookupInfo(data);
                }
            } else {
                alert('Could not complete request.');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(jqXHR.status);
            alert(jqXHR.responseText);
        },
        dataType: "json"
    });
}

function loadSuburb(suburbId,showSeeffCurrentListings, actionAfterLoad, mustCentreMap) {

    var suburb = getSuburbById(suburbId);
    if (suburb == null) {
        suburb = newSuburb();
        suburb.SuburbId = suburbId;
    }
    suburb.IsInitialised = false; // Adding this line in here to force obtaining the latest data from the database each load (this is important because the contacts for the property could be updated elsewhere and changes must reflect)

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading ' + suburb.SuburbName + '...</p>' });
    if (!suburb.IsInitialised) {
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({ Instruction: "load_suburb", SuburbId: suburbId }),
            success: function (data, textStatus, jqXHR) {
                if (textStatus == "success" && data.PolyCoords.length > 0) {
                    initialiseAndDisplaySuburb(suburb, data, showSeeffCurrentListings);
                    if (mustCentreMap !== false) {
                        centreMap(suburb);
                    }
                    $.unblockUI();
                    if (actionAfterLoad) {
                        actionAfterLoad();
                    }
                } else {
                    $.unblockUI();
                    alert('No data found for this area on the map. Please contact support.');
                }
            },
            error: function (textStatus, errorThrown) {
                alert(textStatus.responseText);
                alert(errorThrown);
            },
            dataType: "json"
        });
    } else {
        initialiseAndDisplaySuburb(suburb, null, showSeeffCurrentListings);
        if (mustCentreMap !== false) {
            centreMap(suburb);
        }
        $.unblockUI();
        if (actionAfterLoad) {
            actionAfterLoad();
        }
    }
}

function changeBgColour(id, colour) {
    var id = "#unit" + id;
    var row = $(id);
    row.data('color', colour)
    row.css('background-color', 'lightblue');
}

function setCurrentMarker(suburb, property) {
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_existing_prospecting_property", LightstonePropertyId: property.LightstonePropertyId,LoadActivities: false }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }
                testpp = data;

                $.each(suburb.ProspectingProperties, function (idx, pp) {
                    if (pp.ProspectingPropertyId == testpp.ProspectingPropertyId) {
                        pp.Prospected = testpp.Prospected;
                        if (testpp.Prospected) {
                            if (pp.SS_FH == "FH") {
                                pp.Marker.setIcon('Assets/marker_icons/prospecting/prospected.png');
                            } else {
                                changeBgColour(testpp.LightstonePropertyId, "#009900");
                            };
                        }
                        else {
                            if (pp.SS_FH == "FH") {
                                pp.Marker.setIcon('Assets/marker_icons/prospecting/unprospected.png');
                            } else {
                                changeBgColour(testpp.LightstonePropertyId, "#FBB917");
                            }
                        }
                    };
                    
                });

            } else {
                alert('Could not complete request.');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(jqXHR.status);
            alert(jqXHR.responseText);
        },
        dataType: "json"
    });
}

function markerClick() {
    closeInfoWindow();
    $('#propertyInfoDiv').css('display', 'none');
    var marker = $(this)[0];
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Property Info...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_existing_prospecting_property", LightstonePropertyId: marker.ProspectingProperty.LightstonePropertyId, LoadActivities: false }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }

                updateExistingPropertyFromProperty(marker.ProspectingProperty, data);
                currentMarker = marker;
                loadProspectingProperty(marker);
            } else {
                alert('Could not complete request.');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(jqXHR.status);
            alert(jqXHR.responseText);
        },
        dataType: "json"
    });
}

/// NB property input must either be FH or a unit of a SS
function loadExistingProspectAddActivity(property, defaultSelection, callbackFunction) {
    //closeInfoWindow();
    $('#propertyInfoDiv').css('display', 'none');
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_existing_prospecting_property", LightstonePropertyId: property.LightstonePropertyId, LoadActivities: true }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }

                var activityBundle = data.ActivityBundle;
                activityBundle.PropertyContacts = data.Contacts;

                updateExistingPropertyFromProperty(property, data);
                currentMarker = property.Marker;
                currentProperty = property;
                updateOwnerDetailsEditor();
                //updatePropertyInfoMenu();  // NB CHECK THIS

                if (callbackFunction) {
                    callbackFunction(activityBundle);
                } else {
                    if (currentProperty.SS_FH != 'SS' && currentProperty.SS_FH != 'FS') {
                        openInfoWindow(property.Marker, function () {
                            // openInfoWindow calls closeInfoWindow which resets a whole bunch of globals, so re-init them here
                            currentProperty = property;
                            currentMarker = property.Marker;
                            updateOwnerDetailsEditor();

                            if (currentProperty.Contacts.length || currentProperty.ContactCompanies.length) {
                                showMenu("contactdetails");                                
                            }
                        });
                    }

                    if (defaultSelection && defaultSelection.RelatedTo != null) {
                        currentPersonContact = $.grep(currentProperty.Contacts, function (c) {
                            return c.ContactPersonId == defaultSelection.RelatedTo;
                        })[0];
                        openExpanderWidget(currentPersonContact);
                    }
                    showDialogAddActivity(activityBundle, defaultSelection);
                }
            } else {
                alert('Could not complete request.');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(jqXHR.status);
            alert(jqXHR.responseText);
        },
        dataType: "json"
    });
}

function loadFollowups(callbackFn) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "load_followups" }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }

                if (callbackFn) {
                    callbackFn(data);
                }
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(jqXHR.status);
            alert(jqXHR.responseText);
        },
        dataType: "json"
    });
}

function showDialogAddActivity(inputPacket, defaultSelection, callback) {
    var parentActivityId = null;
    var targetProperty = currentProperty;
    var childActivity = false;
    var parentActivityActivityType = null;

    var div = $('#addActivityDialog');
    div.empty();
    div.append("<p /><p />");
    div.append("<label class='fieldAlignmentShortWidth' for='activityInput'>Activity</label>");
    var activity = $("<select id='activityInput' style='width:300px;' /><p style='margin:6px;' />");
    div.append(activity);
    div.append("<label class='fieldAlignmentShortWidth' for='allocatedToInput'>Allocated To</label>");
    var allocatedTo = $("<select id='allocatedToInput' style='width:300px;' /><p style='margin:6px;' />");
    div.append(allocatedTo);
    div.append("<label class='fieldAlignmentShortWidth' for='followupDateInput'>Follow-up Date</label>");
    var followupDate = $("<input type='text' name='followupDateInput' id='followupDateInput' style='height:12px;font-size:12px;width:295px;' /><p style='margin:6px;' />");
    div.append(followupDate);
    followupDate.datepicker({ dateFormat: 'DD, d MM yy', minDate: 0 });
    div.append("<label class='fieldAlignmentShortWidth' for='relatedToInput'>Related To</label>");
    var relatedTo = $("<select id='relatedToInput' style='width:300px;' /><p style='margin:6px;' />");
    div.append(relatedTo);
    div.append("<label for='commentInput'>Comment</label><p />");
    var comment = $("<textarea id='commentText' rows='8' style='width:98%;' />");
    div.append(comment);

    var buttonsDiv = $("<div style='float:right' />");
    var saveButton = $("<input type='button' id='saveActivityBtn' value='Save and close' style='margin:5px;cursor:pointer' />");
    buttonsDiv.append(saveButton);
    div.append(buttonsDiv);

    populateDropdowns();
    setDefaults();

    activity.change(function () {
        var text = $(this).children(':selected').text();
        if (text == 'Closed for now') {
            followupDate.val('');
            followupDate.prop('disabled', true);
        } else {
            followupDate.prop('disabled', false);
        }
    });
    
    var title = parentActivityActivityType != null ? 'Follow-up on property' : 'Add Activity To Property';
    div.dialog({
        show: 'fade',
        position: ['center', 'center'],
        hide: { effect: "fadeOut", duration: 500 },
        width: 'auto',
        height: 'auto',
        resizable: false,
        open: function (event, ui) {
            saveButton.click(function () {
                if (validateInputs()) {
                    saveActivity();
                }
            });
        },
        modal: true,
        title: title
    });

    function populateDropdowns() {

        // Populate activities
        $('#activityInput').append($("<option />").val(-1).text(''));
        $.each(inputPacket.ActivityTypes, function (idx, el) {
            $('#activityInput').append($("<option />").val(el.Key).text(el.Value));
        });

        // Populate Allocated To
        $('#allocatedToInput').append($("<option />").val(-1).text(''));
        $.each(inputPacket.BusinessUnitUsers, function (idx, el) {
            $('#allocatedToInput').append($("<option />").val(el.UserGuid).text(el.UserName + " " + el.UserSurname));
        });

        // Populate related-to
        $('#relatedToInput').append($("<option />").val(-1).text(''));
        $.each(inputPacket.PropertyContacts, function (idx, el) {
            $('#relatedToInput').append($("<option />").val(el.ContactPersonId).text(el.Firstname + ' ' + el.Surname));
        });
    }

    function setDefaults() {
        if (defaultSelection) {
            if (defaultSelection.RelatedTo) {
                $('#relatedToInput').val(defaultSelection.RelatedTo);
            }
            if (defaultSelection.Property) {
                targetProperty = defaultSelection.Property;
            }
            if (defaultSelection.ParentActivityId) {
                parentActivityId = defaultSelection.ParentActivityId;
            }
            if (defaultSelection.IsChildActivity) {
                childActivity = true;
            }
            if (defaultSelection.ParentActivityActivityType) {
                parentActivityActivityType = defaultSelection.ParentActivityActivityType;
            }
        }
    }

    function validateInputs() {
        var activityValue = activity.val();
        if (activityValue == '-1') {
            var activityElement = document.getElementById("activityInput");
            tooltip.pop(activityElement, 'You must specify a type', { showDelay: 1, hideDelay: 100, calloutPosition: 0.5 });
            return false;
        }
        var allocatedToValue = allocatedTo.val();
        var followupDateValue = followupDate.val();
        if (allocatedToValue != '-1' && followupDateValue == '') {
            var followupDateElement = document.getElementById("followupDateInput");
            tooltip.pop(followupDateElement, 'You must specify a follow-up date if you are allocating this to someone.', { showDelay: 1, hideDelay: 100, calloutPosition: 0.5, maxWidth: 200 });
            return false;
        }
        return true;
    }

    function saveActivity() {
        
        var activityType = $('#activityInput').val();
        var allocatedTo = $('#allocatedToInput').val();
        var followupDate = $('#followupDateInput').val();
        var relatedTo = $('#relatedToInput').val();
        var comment = $('#commentText').val();

        var inputPacket;
        if (!childActivity) {
            inputPacket = {
                Instruction: "save_activity",
                IsForInsert: true,
                LightstonePropertyId: targetProperty.LightstonePropertyId,
                ActivityTypeId: activityType,
                AllocatedTo: allocatedTo != '-1' ? allocatedTo : null,
                FollowupDate: followupDate != '' ? followupDate : null,
                ContactPersonId: relatedTo != '-1' ? relatedTo : null,
                Comment: comment != '' ? comment : null,
                ParentActivityId: parentActivityId
            };
        } else {
            inputPacket = {
                Instruction: "save_activity",
                IsForInsert: true,
                LightstonePropertyId: targetProperty.LightstonePropertyId,
                ActivityFollowupTypeId: activityType,
                AllocatedTo: allocatedTo != '-1' ? allocatedTo : null,
                FollowupDate: followupDate != '' ? followupDate : null,
                ContactPersonId: relatedTo != '-1' ? relatedTo : null,
                Comment: comment != '' ? comment : null,
                ParentActivityId: parentActivityId,
                ActivityTypeId: parentActivityActivityType,
            };
        }

        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify(inputPacket),
            success: function (data, textStatus, jqXHR) {
                $('#addActivityDialog').dialog('close');
                if (callback) {
                    callback();
                }
                showSavedSplashDialog("Activity Saved!"); // + test against SS
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(jqXHR.status);
                alert(jqXHR.responseText);
            },
            dataType: "json"
        });
    }
}

function markerRightClick(event) {
    // Find the marker underneath the click
    var marker = $(this)[0];
    rightClickedProperty = null;
    if (marker.ProspectingProperty.SS_FH != 'SS' && marker.ProspectingProperty.SS_FH != 'FS') {
        rightClickedProperty = marker.ProspectingProperty;

        var clickLoc = new google.maps.LatLng(event.latLng.lat(), event.latLng.lng());
        var point = fromLatLngToPoint(clickLoc, map);
        $('.context-menu-rightclick-property').contextMenu({ x: point.x, y: point.y });
    }
}

function loadProspectingProperty(marker) {
    currentMarker = marker;
    openInfoWindow(marker, function () {
        if (marker.ProspectingProperty.SS_FH == "SS" || marker.ProspectingProperty.SS_FH == "FS") {
            currentProperty = null;
        }
        else {
            currentProperty = marker.ProspectingProperty;
            currentProperty.Marker = marker;
            updateOwnerDetailsEditor();
            //updatePropertyNotesDiv();
            if (currentProperty.Contacts.length || currentProperty.ContactCompanies.length) {
                showMenu("contactdetails");
            }

            updateProspectedStatus();
        }

        updatePropertyInfoMenu();
    });
}

// Left here for legacy only - can remove in a future version.
function linkAndShowExistingOwners(property) {
    var otherPropsOwnedByTheseOwners = [];
    for (var o = 0; o < property.Contacts.length; o++) {
        var owner = property.Contacts[o];
        if (owner.PropertiesOwned) {
            for (var o2 = 0; o2 < owner.PropertiesOwned.length; o2++) {
                var prop = owner.PropertiesOwned[o2];
                if (prop.LightstonePropertyId != property.LightstonePropertyId) {

                    var containsProp = false;
                    $.each(otherPropsOwnedByTheseOwners, function (i, p) {
                        if (p.LightstonePropertyId == prop.LightstonePropertyId) {
                            containsProp = true;
                        }
                    });
                    if (!containsProp) {
                        otherPropsOwnedByTheseOwners.push(prop);
                    }
                }
            }
        }
    }

    if (otherPropsOwnedByTheseOwners.length > 0) {
        showDialogExistingPropertiesFound(otherPropsOwnedByTheseOwners);
    }
}

function updateProspectingRecord(record) {

    var inputPacket = {
        Instruction: "update_prospecting_property",
        PropertyAddress: record.PropertyAddress,
        StreetOrUnitNo: record.StreetOrUnitNo,
        SSDoorNo: record.SSDoorNo,
        SS_FH: record.SS_FH,
        ProspectingPropertyId: record.ProspectingPropertyId
    };

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Updating address...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify(inputPacket),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success") {
                if (!data) {
                    alert('Unable to update this property at this time.');
                } else {
                    if (record.SS_FH == "SS") {
                        currentProperty.SSDoorNo = record.SSDoorNo;
                    } else {
                        currentProperty.PropertyAddress = record.PropertyAddress;
                        currentProperty.StreetOrUnitNo = record.StreetOrUnitNo;
                    }
                    //alert('Address details updated successfully.');
                    showSavedSplashDialog('Details updated!');
                }
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $.unblockUI();
            alert(jqXHR.status);
            alert(jqXHR.responseText);
        },
        dataType: "json"
    });
}

function saveContact(contact, property, actionToExecuteAfterwards) {
    var inputPacket = null;
    if (contact.ContactCompanyId) {
        inputPacket = { Instruction: "save_contact", ContactPerson: contact, ProspectingPropertyId: property.ProspectingPropertyId, ContactCompanyId: contact.ContactCompanyId };
    }
    else {
        inputPacket = {Instruction: "save_contact", ContactPerson: contact, ProspectingPropertyId: property.ProspectingPropertyId, ContactCompanyId: null };
    }
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify(inputPacket),
        success: function (data, textStatus, jqXHR) {
            if (textStatus == "success") {

                if (data.ContactIsCompromised) {
                    showDialogUserDeletedDetailsWarning();
                }
                else {
                    actionToExecuteAfterwards(data);
                }
            }
        },
        error: function (textStatus, errorThrown) { },
        dataType: "json"
    });
}

function addOrUpdateContactToCurrentProperty(newContact) {

    var contactExists = $.grep(currentProperty.Contacts, function (c) {
        return c.ContactPersonId == newContact.ContactPersonId;
    });

    if (contactExists.length == 0) {

        if (newContact.IsPOPIrestricted) {
            newContact.PhoneNumbers = [];
            newContact.EmailAddresses = [];
        }

        currentProperty.Contacts.push(newContact);
    } else {
        var contact = contactExists[0];
        contact.Firstname = newContact.Firstname;
        contact.Surname = newContact.Surname;
        contact.Title = newContact.Title;
        contact.IdNumber = newContact.IdNumber;
        contact.PersonPropertyRelationships = newContact.PersonPropertyRelationships;
        contact.PhoneNumbers = newContact.PhoneNumbers;
        contact.EmailAddresses = newContact.EmailAddresses;
        contact.Comments = newContact.Comments;
        contact.IsPOPIrestricted = newContact.IsPOPIrestricted;

        if (contact.IsPOPIrestricted) {
            contact.PhoneNumbers = [];
            contact.EmailAddresses = [];
        }
    }
}

function clearSuburbBySuburbId(suburbId) {
    var suburb = getSuburbById(suburbId);

    if (suburb.IsInitialised) {
        // Close open info window if any
        closeInfoWindow();
        suburb.MarkerClusterer.clearMarkers();
        suburb.Markers.length = 0;
        suburb.VisibleMarkers = null;
        if (suburb.SeeffCurrentListings) {
            suburb.SeeffCurrentListings.length = 0;
        }
        suburb.Polygon.setMap(null);
        suburb.Spiderfier.clearMarkers();
        suburb.Visible = false;
    }
}


function updateOwnerDetailsEditor() {
    var contactDetailsPane = $('#contactDetailsDiv');
    contactDetailsPane.empty();
    contactDetailsPane.css('display', 'block');
    if (currentMarker) {
        contactDetailsPane.append(buildPersonContactMenu(currentProperty.Contacts));
    }
}

function loadActivityLookupData(callback) {    
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "load_activity_lookup_data" }),
        success: function (data, textStatus, jqXHR) {
            if (textStatus == "success" && data) {
                if (callback) {
                    callback(data);
                }
            }
            else {
                alert('Error retrieving activity lookup data.');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(jqXHR.status);
            alert(jqXHR.responseText);
        },
        dataType: "json"
    });
}

function updateOwnerDetailsEditorWithBrandNewContact(infoPacket, contactRows) {
    var contactDetailsPane = $('#contactDetailsDiv');
    contactDetailsPane.empty();
    contactDetailsPane.css('display', 'block');
    if (currentMarker) {
        contactDetailsPane.append(buildPersonContactMenu(currentProperty.Contacts));
        openAndPopulateNewContactPerson(infoPacket, contactRows);
    }
}

function enableSpiderfier(suburb, visibleMarkers) {

    visibleMarkers = $.grep(visibleMarkers, function (m) {
        return m.ProspectingProperty.SS_FH == "FH"; // We must change this to only spiderfy FH not SS anymore. SS handled separately.
    });


    if (suburb.Spiderfier) {
        var currentMarkers = suburb.Spiderfier.getMarkers();
        var markersToAdd = [];
        $.each(visibleMarkers, function (idx, m) {
            var mustAdd = true;
            $.each(currentMarkers, function (idx2, m2) {
                if (m == m2) {
                    mustAdd = false;
                }
            });

            if (mustAdd) {
                markersToAdd.push(m);
            }
        });

        $.each(markersToAdd, function (idx3, m3) {
            suburb.Spiderfier.addMarker(m3);
        });
    }
    else {
        var spiderfier = new OverlappingMarkerSpiderfier(map, { circleFootSeparation: 70, keepSpiderfied: true, circleSpiralSwitchover: 15, nearbyDistance: 1 });
        for (var i = 0; i < visibleMarkers.length; i++) {
            spiderfier.addMarker(visibleMarkers[i]);
        }

        spiderfier.addListener("spiderfy", function (markers) {
            //closeInfoWindow();

            $.each(markers, function (idx, m) {
                m.MarkerIsSpiderfied = true;
                m.setIcon(getIconForMarker(m));
            });
        });

        spiderfier.addListener('unspiderfy', function (markers) {
            $.each(markers, function (idx, m) {
                m.MarkerIsSpiderfied = false;
                m.setIcon(getIconForMarker(m));
            });
        });

        suburb.Spiderfier = spiderfier;
    }
}

function initialiseAndDisplaySuburb(suburb, data, showSeeffCurrentListings, searchFunc) {

    closeInfoWindow();
    if (data) {
        if (data.PolyCoords) {
            suburb.PolyCoords = data.PolyCoords;
        }
        if (data.ProspectingProperties) {
            suburb.ProspectingProperties = data.ProspectingProperties;
        }
        if (data.LocationName) {
            suburb.Name = data.LocationName;
        }
        if (data.LocationID) {
            suburb.SuburbId = data.LocationID;
        }

        suburb.IsInitialised = true;
        suburb.VisibleMarkers = [];
    }

    if (suburb.IsInitialised) {

        if (infowindow) {
            infowindow.Marker = null;
        }
        suburb.Visible = true;

        updatePropertyInfoMenu();
        createMarkersForSuburb(suburb, showSeeffCurrentListings);
        var visibleMarkers = suburb.Markers;

        if (searchFunc) {
            visibleMarkers = searchFunc(visibleMarkers);
        }

        enableMarkerClustering(suburb, visibleMarkers);
        enableSpiderfier(suburb, visibleMarkers);
        suburb.VisibleMarkers = visibleMarkers;

        drawPolygonForSuburb(suburb);

        currentSuburb = suburb;
    }
}

function drawPolygonForSuburb(suburb) {
    var coords = [];
    for (var j = 0; j < suburb.PolyCoords.length; j++) {
        var lat = suburb.PolyCoords[j].Lat;
        var lng = suburb.PolyCoords[j].Lng;
        coords.push(new google.maps.LatLng(lat, lng));
    }

    // The last coordinate set to add will be the starting point 
    coords.push(new google.maps.LatLng(suburb.PolyCoords[0].Lat, suburb.PolyCoords[0].Lng));
    var poly = new google.maps.Polygon({
        paths: coords,
        editable: false,
        fillOpacity: '0.0'
    });

    poly.setMap(map);
    suburb.Polygon = poly;
    poly.Suburb = suburb;

    google.maps.event.addListener(poly, 'click', function (event) {
        currentSuburb = poly.Suburb;
        handleMapClick(event);
    });
}

function getIconForMarker(marker) {
    var path = 'Assets/marker_icons/prospecting/';
    // Otherwise at this stage it *might* still be a lightstone property, or prospected property, or a prospected lightstone property.
    var property = marker.ProspectingProperty; // property is actually a "listing"
    try {
        var fullPathToIcon = buildPathToIcon(property);
        return fullPathToIcon;
    } catch (e) {
        return 'Assets/unknown.png';
    }

    function buildPathToIcon(property) {        
        if (property.SS_FH == "SS" || property.SS_FH == "FS") {
            if (!marker.MarkerIsSpiderfied) {
                return path += 'ss_unprospected.png';
            }
        }       
        // Any other FH types
        return path += getPathIdentifierForFH(property);
    }

    function getPathIdentifierForFH(property) {
        if (property.ProspectingPropertyId) {
            if (property.Prospected) {
                // Fully prospected
                return 'prospected.png';
            }
            return 'unprospected.png';
        }

        return 'unprospected.png';
    }
}

function getAllMarkersThatWillSpiderfy(property) {

    var suburb = property.Marker.Suburb;
    var allPropsWithSameLatLong = $.grep(suburb.ProspectingProperties, function (lis, index) {
        return lis.LatLong.Lat == property.LatLong.Lat && lis.LatLong.Lng == property.LatLong.Lng;
    });

    if (allPropsWithSameLatLong.length > 0) {

        var markersForListings = [];
        $.each(allPropsWithSameLatLong, function (index, lis) {

            var marker = lis.Marker;
            if (marker) {
                if ($.inArray(marker, markersForListings) == -1) {
                    markersForListings.push(marker);
                }
            }
        });

        return markersForListings;
    }

    return [];
}

function centreMap(suburb) {

    if (!suburb.IsInitialised) {
        return;
    }

    var center = null;
    if (suburb.ProspectingProperties && suburb.ProspectingProperties.length > 0) {
        center = new google.maps.LatLng(suburb.ProspectingProperties[0].LatLng.Lat, suburb.ProspectingProperties[0].LatLng.Lng);
    }
    else {
        center = new google.maps.LatLng(suburb.PolyCoords[0].Lat, suburb.PolyCoords[0].Lng);
    }

    map.setZoom(13);
    map.setCenter(center);
}

function createMarkersForSuburb(suburb, showSeeffCurrentListings) {
    var markersForProperties = [];
    var uniquePropIds = [];
    for (var i = 0; i < suburb.ProspectingProperties.length; i++) {
        // Create a new google maps marker from the lat/long of the listing
        var property = suburb.ProspectingProperties[i];
        var marker = createMarkerForProperty(property);
        marker.Suburb = suburb;
        markersForProperties.push(marker);
    }

    suburb.Markers = markersForProperties;
    $.each(markersForProperties, function (idx, m) { m.setIcon(getIconForMarker(m)); });
}

function createMarkerForProperty(property) {
    var lat = property.LatLng.Lat;
    var lng = property.LatLng.Lng;
    var marker = new google.maps.Marker({ position: new google.maps.LatLng(lat, lng) });

    property.Marker = marker;
    marker.LightstonePropertyId = property.LightstonePropertyId;
    marker.ProspectingProperty = property;
    google.maps.event.addListener(marker, 'click', markerClick);
    google.maps.event.addListener(marker, 'rightclick', markerRightClick);

    return marker;
}

function buildInfoWindowContentForSS(unit) {

    function buildUnitContentRow(unit) {

        function getBGColourForRow(unit) {
            var hasContactsWithDetails = false;
            if (unit.Prospected) {
                hasContactsWithDetails = true;
            }
            return hasContactsWithDetails ? "#009900" : "#FBB917";
        }

        var id = "unit" + unit.LightstonePropertyId;
        var tr = $("<tr class='unitrow' />");
        var color = getBGColourForRow(unit);
        var bgcolor = "background-color:" + color;

        var unitPropId = 'Property ID: ' + unit.LightstonePropertyId;
        var unitRegDate = 'Reg date: ' + formatDate(unit.LightstoneRegDate);
        var unitLastSalePrice = 'Last sale price: ' + formatRandValue(unit.LastPurchPrice);
        var ssDoorNo = unit.SSDoorNo ? ' (Door no. ' + unit.SSDoorNo + ')' : '';
        var unitContent = '';
        if (unit.SS_FH == 'FS') {
            unitContent = 'Property ID: ' + unit.LightstonePropertyId + '<br />' +
                          'ERF no.: ' + unit.ErfNo;
        } else {
            unitContent = "Unit " + unit.Unit + ssDoorNo + '<br />' +
                           unitPropId + '<br />' +
                           unitRegDate + '<br />' +
                           unitLastSalePrice;
        }
        
        var td = tr.append($("<td id='" + id + "' class='unittd' data-color='" + color + "' style='cursor:pointer;width:150px;text-align:left;" + bgcolor + "' />").append(unitContent));

        $('body').unbind('mouseover.' + id).on('mouseover.' + id, '#' + id, function () {
            var row = $('#' + id);
            row.css('background-color', 'lightblue');
        });

        $('body').unbind('mouseout.' + id).on('mouseout.' + id, '#' + id, function () {
            var row = $('#' + id);
            var color = row.data('color');
            var color2 = row.data('color2');
            if (!color2) {
                // If we mouse out an already clicked row, do not change color
                row.css('background-color', color);
            }
        });

        $('body').unbind('mousedown.' + id).on('mousedown.' + id, '#' + id, function (e) {
            if (e.which == 1) {
                // Reset all color2 attributes of all other rows
                var row = $('#' + id);
                $('.unittd').not(row).each(function (idx, val) {
                    var r = $(val);
                    r.data('color2', '');
                    var color = r.data('color');
                    r.css('background-color', color);
                });
                row.data('color2', 'lightblue');

                openSSUnitInfo(unit);
            }
            else if (e.which == 3) {
                rightClickedProperty = unit;
                $('.context-menu-rightclick-property').contextMenu({ x: e.pageX, y: e.pageY });
            }
        });

        return tr;
    }

    // First find all units that this unit belongs with
    // Going forward we must use a unique identifer for this SS that is set at creation
    // Backward compatability: if this identifer is not present then we must revert to using the SSName
    var ssUnits = [];
    ssUnits = $.grep(currentSuburb.ProspectingProperties, function (pp) {
        if (!pp.SS_UNIQUE_IDENTIFIER) return false;
        return pp.SS_UNIQUE_IDENTIFIER == unit.SS_UNIQUE_IDENTIFIER;
    });

    $.each(ssUnits, function (i, u) {
        if (u.SS_FH == 'FS') u.Unit = 99999999;
    });
    ssUnits.sort(function (x, y) {
        return x.Unit - y.Unit;
    });

    var tableOfUnits = $("<table id='ssUnitsTbl' class='info-window' style='display: block;max-height:250px;overflow-y:auto;width:200px;' />");
    tableOfUnits.empty();

    for (var i = 0; i < ssUnits.length; i++) {
        var u = ssUnits[i];
        var unitContent = buildUnitContentRow(u);
        tableOfUnits.append(unitContent);
    }

    return tableOfUnits;
}

function openSSUnitInfo(unit) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Property Info...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_existing_prospecting_property", LightstonePropertyId: unit.LightstonePropertyId, LoadActivities: false }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }

                updateExistingPropertyFromProperty(unit, data);
                currentProperty = unit;
                updateOwnerDetailsEditor();
                //updatePropertyNotesDiv();
                if (currentProperty.Contacts && currentProperty.Contacts.length > 0) {
                    showMenu("contactdetails");
                }

                updateProspectedStatus();

            } else {
                alert('Could not complete request.');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(jqXHR.status);
            alert(jqXHR.responseText);
        },
        dataType: "json"
    });
}

// This function will update an object that already exists on the front-end with an incoming object's data - it does not change the existing reference.
function updateExistingPropertyFromProperty(existingProp, newProp) {
    existingProp.ProspectingPropertyId = newProp.ProspectingPropertyId;
    existingProp.LatLng = newProp.LatLng;
    existingProp.LightstonePropertyId = newProp.LightstonePropertyId;
    existingProp.PropertyAddress = newProp.PropertyAddress;
    existingProp.StreetOrUnitNo = newProp.StreetOrUnitNo;
    existingProp.SeeffAreaId = newProp.SeeffAreaId;
    existingProp.Contacts = newProp.Contacts;
    existingProp.ContactCompanies = newProp.ContactCompanies;
    existingProp.LightstoneIDOrCKNo = newProp.LightstoneIDOrCKNo;
    existingProp.LightstoneRegDate = newProp.LightstoneRegDate;
    existingProp.Comments = newProp.Comments;
    existingProp.LastPurchPrice = newProp.LastPurchPrice;
    existingProp.SS_FH = newProp.SS_FH;
    existingProp.SSName = newProp.SSName;
    existingProp.SSNumber = newProp.SSNumber;
    existingProp.Unit = newProp.Unit;
    existingProp.SS_ID = newProp.SS_ID;
    existingProp.SS_UNIQUE_IDENTIFIER = newProp.SS_UNIQUE_IDENTIFIER;
    existingProp.SSDoorNo = newProp.SSDoorNo;
    existingProp.Prospected = newProp.Prospected;
    existingProp.ErfNo = newProp.ErfNo;
    existingProp.FarmName = newProp.FarmName;
    existingProp.Portion = newProp.Portion;
    existingProp.LightstoneSuburb = newProp.LightstoneSuburb;
    existingProp.ActivityBundle = newProp.ActivityBundle;
}

function buildContentForInfoWindow(property) {
    var div = $("<div class='info-window' />");   
    // test for fh, test in chrome, mouille point, sort, fixed header
    if (property.SS_FH == 'SS' || property.SS_FH == 'FS') {
        var header = $("<label>" + property.SSName + "</label>");
        div.append(header);
        div.append("<br />");
        var ss = buildInfoWindowContentForSS(property);
        div.append(ss);
    }
    else { // FH and FRM
        var address = property.StreetOrUnitNo + " " + property.PropertyAddress;
        if (property.SS_FH == 'FRM') {
            address = 'Farm: ' + property.FarmName + " (" + property.LightstoneSuburb + ")";
        }
        div.append(address);

        var contacts = "Number of contacts captured: " + property.Contacts.length;
        div.append("<br />");
        div.append(contacts);
        div.append("<br />");
        div.append("Property ID: " + property.LightstonePropertyId);
        if (property.LightstoneRegDate) {
            div.append("<br />");
            div.append("Reg. date (from Lightstone): " + formatDate(property.LightstoneRegDate));
        }
        div.append("<br />");
        div.append("Erf No.: " + (property.ErfNo ? property.ErfNo : "n/a"));
        div.append("<br />");
        div.append("Portion: " + (property.Portion ? property.Portion : "n/a"));
        div.append("<br />");
        div.append("Last sale price: " + (property.LastPurchPrice ? formatRandValue(property.LastPurchPrice) : "n/a"));

        // Append the owner info
        div.append("<br />");
        div.append("Property Contacts:");
        div.append("<br />");
        if (property.Contacts.length) {
            $.each(property.Contacts, function (idx, c) {

                div.append(c.Firstname + " " + c.Surname + " (ID Number:" + c.IdNumber + ")");
                div.append("<br />");
            });
        }
        if (property.ContactCompanies.length) {
            $.each(property.ContactCompanies, function (idx, c) {
                div.append(c.CompanyName + " (" + c.CKNumber + ")");
                div.append("<br />");
            });
        }
    }  
   
    return div[0].outerHTML;
}

function stripPropertyAddress(property) {
    var streetOrUnitNo = property.StreetOrUnitNo;
    var streetPortion = property.PropertyAddress.split(',')[0].trim();
    var suburbPortion = property.PropertyAddress.split(',')[1].trim();
    var townPortion = property.PropertyAddress.split(',')[2].trim();

    return { StreetOrUnitNo: streetOrUnitNo, StreetName: streetPortion, Suburb: suburbPortion, CityTown: townPortion };
}

function openInfoWindow(marker, actionAfterOpening) {
    closeInfoWindow();

    function createInfoWindowForMarker(marker) {
        infowindow = new google.maps.InfoWindow();
        infowindow.setContent(buildContentForInfoWindow(marker.ProspectingProperty));
        infowindow.setPosition(marker.getPosition());
        infowindow.open(map);
        infowindow.Marker = marker;

        google.maps.event.addDomListener(infowindow, 'closeclick', function () {
            closeInfoWindow();
        });
    }

    function updatePropertyAddressForProperty(property, address) {
        if (address) {
            prop.PropertyAddress = address.PropertyAddress;
            prop.StreetOrUnitNo = address.StreetOrUnitNo;
        }
    }

    var prop = marker.ProspectingProperty;
    if (prop.PropertyAddress == 'n/a') {
        getGoogleAddress(prop.LatLng.Lat, prop.LatLng.Lng, function (address)
        {
            updatePropertyAddressForProperty(prop, address);
            createInfoWindowForMarker(marker);
            actionAfterOpening();
        });
    }
    else {
        createInfoWindowForMarker(marker);
        actionAfterOpening();
    }
}

function getGoogleAddress(lat, lng, actionToExecuteAfterwards) {

    function getAddress(data) {
        if (data && data.results.length > 0) {
            var streetName = data.results[0].address_components[1].short_name + ', ' + data.results[0].address_components[2].short_name + ', ' + data.results[0].address_components[3].short_name + ' (from Google)';
            var streetOrUnitNo = data.results[0].address_components[0].short_name;
            return { PropertyAddress: streetName, StreetOrUnitNo: streetOrUnitNo };
        }

        return null;
    }

    var latLngPair = lat + ',' + lng;
    var url = 'https://maps.googleapis.com/maps/api/geocode/json?latlng=' + latLngPair + '&sensor=true&key=AIzaSyDWHlk3fmGm0oDsqVaoBM3_YocW5xPKtwA';
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json"
    }).done(function (data) {
        var address = getAddress(data);
        actionToExecuteAfterwards(address);
    });
}

function closeInfoWindow() {
    if (infowindow) {
        infowindow.close();
        infowindow = null;
        currentMarker = null;
        currentProperty = null;
        currentPersonContact = null;
        if (tempIndicatorMarker) {
            tempIndicatorMarker.setMap(null);
            tempIndicatorMarker = null;
        }
        updatePropertyInfoMenu();
        updateOwnerDetailsEditor();
        clearActivityReport();
        resetFollowupFilters();
        //updatePropertyNotesDiv();

        currentTracePSInfoPacket = null;
        currentTracePSContactRows = null;
    }
}

function clearActivityReport() {
    $('#activitiesExpander').css('display', 'none');
    resetActivityFilters();
}

function showSavedSplashDialog(text) {

    if (text) {
        $('#itemSavedSplashText').text(text);
    }

    $('#itemSavedDialogSplash').dialog({
        show: 'fade',
        position: ['center', 'center'],
        hide: { effect: "fadeOut", duration: 1000 },
        open: function (event, ui) {
            $('#itemSavedDialogSplash').siblings(".ui-dialog-titlebar").hide();
            setTimeout(function () {
                $('#itemSavedDialogSplash').dialog('close');
            }, 1000);
        }
    });
}

function applyProspectingSearch(visibleMarkers, matchedProperties) {
    return $.grep(visibleMarkers, function (vm) {

        for (var i = 0; i < matchedProperties.length; i++) {
            var match = matchedProperties[i];
            if (vm.ProspectingProperty.ProspectingPropertyId == match.ProspectingPropertyId) {
                return true;
            }
        }

        return false;
    });
}

function initCurrentSearchResults(results) {
    if (!currentSuburb) return;

    if (currentSuburb) {
        clearSuburbBySuburbId(currentSuburb.SuburbId);
        initialiseAndDisplaySuburb(currentSuburb, null, false, function (vm) { return applyProspectingSearch(vm, results);});
    }
}

function resetSearch() {
    // Clear the form and reset the suburb
    $('#idNumberSearchTextbox').val('');
    $('#phoneNumberSearchTextbox').val('');
    $('#emailAddressSearchTextbox').val('');
    $('#propertyAddressSearchTextbox').val('');
    $('#streetOrUnitNoSearchTextbox').val('');

    if (currentSuburb) {
        //currentSuburb.IsInitialised = false; // Flag for re-initialisation
        //loadSuburb(currentSuburb.SuburbId, false);
        initialiseAndDisplaySuburb(currentSuburb, null, false);
    }
}

function showDialogOtherResults(otherMatches) {        

    function populateOtherSearchResultsContentDiv() {
        var intro = !currentSuburb ? 'The following matches were found:' : 'The following matches were found outside your currently selected suburb';
        var div = $('#searchResultsContent');
        div.empty();
        div.append(intro);
        div.append("<p />");
    }

    $("#searchResultsDialog").dialog(
            {
                modal: true,
                closeOnEscape: true,
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                width: 'auto',
                buttons: { "Close": function () { $(this).dialog("close"); } },
                position: ['right', 'center']
            });
}

function showDialogExistingPropertiesFound(properties) {
    if (properties && properties.length > 0) {
        var contentDiv = $('#existingContactContent');
        contentDiv.empty();
        contentDiv.append("One or more owner(s) of this property also own the following properties: <p />");

        $.each(properties, function (idx, prop) {
            contentDiv.append(prop.StreetOrUnitNo + " " + prop.PropertyAddress);
            contentDiv.append("<br />");
        });

        contentDiv.append("<br />");
        contentDiv.append("The owners have been linked. <br />");

        $("#existingContactFoundDialog").dialog(
                {
                    modal: true,
                    closeOnEscape: true,
                    open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                    width: 'auto',
                    buttons: { "Ok": function () { $(this).dialog("close"); } },
                    position: ['center', 'center']
                });
    }
}

function showDialogExistingContactFound(contact, proceedAction) {
    if (contact) {
        var contentDiv = $('#existingContactContent');
        contentDiv.empty();
        contentDiv.append("An existing contact with this ID number exists in the system. Would you like to link to this contact?: <p />");
        contentDiv.append("Person details: <br/>");
        contentDiv.append(contact.Firstname + " " + contact.Surname + " (ID number: " + contact.IdNumber + ")");
        contentDiv.append("<br />");

        $("#existingContactFoundDialog").dialog(
               {
                   modal: true,
                   closeOnEscape: true,
                   open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                   width: 'auto',
                   buttons: { "Proceed?": function () { proceedAction(); $(this).dialog("close"); }, "Cancel": function () { $(this).dialog("close"); } },
                   position: ['center', 'center']
               });
    }
}

function showDialogUserDeletedDetailsWarning() {
    var div = $('#userIsCompromisedContent');
    div.empty();
    div.append("The system has detected that you are deleting contact details and have reached the limit for this session.");
    div.append("<p/>");
    div.append("As a safety precaution, you will be logged out. For any questions please contact support.");

    $("#userIsCompromisedDialog").dialog(
         {
             modal: true,
             closeOnEscape: false,
             open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
             width: 'auto',
             buttons: { "Proceed with log-off": function () { window.location = '/LogOut.aspx'; $(this).dialog("close"); }},
             position: ['center', 'center']
         });
}

function performLightstoneSearch() {

    function atLeastOneFieldPopulated() {
        return deedTown.length > 0 ||
            suburb.length > 0 ||
            streetName.length > 0 ||
            streetNo.length > 0 ||
            complexName.length > 0 ||
            erfNo.length > 0 ||
            estateName.length > 0 ||
            propertyID.length > 0 ||
            ownerName.length > 0 ||
            ownerIdNo.length > 0;
    }

    var deedTown = $('#deedTownInput').val().trim();
    var suburb = $('#suburbInput').val().trim();
    var streetName = $('#streetNameInput').val().trim();
    var streetNo = $('#streetNoInput').val().trim();
    var complexName = $('#complexNameInput').val().trim();
    var erfNo = $('#erfNoInput').val().trim();
    var portion = $('#portionNoInputBox').val().trim();
    var estateName = $('#estateNameInput').val().trim();
    var propertyID = $('#propertyIdInput').val().trim();
    var ownerName = $('#ownerNameInput').val().trim();
    var ownerIdNo = $('#ownerIDnoInput').val().trim();

    try{
        if (atLeastOneFieldPopulated()) {
            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Searching for matching properties...</p>' });
            $.ajax({
                type: "POST",
                url: "RequestHandler.ashx",
                data: JSON.stringify({
                    Instruction: 'search_for_matches',
                    DeedTown: deedTown,
                    Suburb: suburb,
                    StreetName: streetName,
                    StreetOrUnitNo: streetNo,
                    SSName: complexName,
                    ErfNo: erfNo,
                    Portion: portion,
                    EstateName: estateName,
                    PropertyID: propertyID,
                    OwnerName: ownerName,
                    OwnerIdNumber: ownerIdNo
                }),
                dataType: "json",
            }).done(function (results) {
                $.unblockUI();
                createLightstoneSearchResultsDiv(results);
            });
        }
    }
    catch (ex) {
        errorHandler(ex);
    }
}

function showSearchedPropertyOnMap(result) {  

    var content = generateOutputFromLightstone([result]);
    var firstResult = result.PropertyMatches[0];

    var latLng = new google.maps.LatLng(firstResult.LatLng.Lat, firstResult.LatLng.Lng);
    showPopupAtLocation(latLng, content);

    var pos = calcMapCenterWithOffset(latLng.lat(), latLng.lng(), -200, 0);
    if (pos) {
        map.setCenter(pos);
    }
    else {
        map.setCenter(latLng);
    }
}

// Rem to set primary contact detail in DB using same logic on front-end
function showDialogSelectPrimaryContactDetail(contactDetailArray, callbackFn) {
    if (contactDetailArray.length > 0) {
        var div = $('#selectPrimaryContactDetailDialog');
        div.empty();
        var testDetail = contactDetailArray[0].ContactItemType;
        switch (testDetail) {
            case 'PHONE':
                buildDialogForPhoneNumbers();
                break;
            case 'EMAIL':
                buildDialogForEmailAddresses();
                break;
            default: break;
        }

        var buttonsDiv = $("<div style='float:right' />");
        var saveButton = $("<input type='button' id='savePrimaryContactBtn' value='Save and close' style='margin:5px;cursor:pointer' />");
        buttonsDiv.append(saveButton);
        div.append(buttonsDiv);

        div.dialog({
            show: 'fade',
            position: ['center', 'center'],
            hide: { effect: "fadeOut", duration: 500 },
            width: 'auto',
            height: 'auto',
            resizable: false,
            open: function (event, ui) {
                saveButton.click(function () {
                    if (validateInputs()) {
                        var itemId = $('input[name=defaultItemName]:checked').attr("id");
                        itemId = itemId.replace('_','');
                        saveDefaultContactItem(itemId, callbackFn);
                    } else {
                        alert('Please select a primary contact detail');
                    }
                });
            },
            modal: true
        });

        function validateInputs() {
            var selectedItem = $('input[name=defaultItemName]:checked').val();
            return selectedItem ? true : false;
        }
    
        function buildDialogForPhoneNumbers() {
            div.append("Select a primary phone number for this contact:");
            div.append("<p />");
            
            $.each(contactDetailArray, function (idx, item) {
                var displayItem = buildItemRow(item);
                div.append(displayItem);
                div.append("<br />");
            });

            function buildItemRow(phoneNumberItem) {
                var id = '_' + phoneNumberItem.ItemId;
                return $("<input class='defaultItemClass' type='radio' id='" + id + "' name='defaultItemName' /><label for='" + id + "'>" + format10DigitPhoneNumber(phoneNumberItem.ItemContent) + "</label>");
            }
        }

        function buildDialogForEmailAddresses() {
            div.append("Select a primary email address for this contact:");
            div.append("<p />");

            $.each(contactDetailArray, function (idx, item) {
                var displayItem = buildItemRow(item);
                div.append(displayItem);
                div.append("<br />");
            });

            function buildItemRow(emaiLAddressItem) {
                var id = '_' + emaiLAddressItem.ItemId;
                return $("<input class='defaultItemClass' type='radio' id='" + id + "' name='defaultItemName' /><label for='" + id + "'>" + emaiLAddressItem.ItemContent + "</label>");
            }
        }

        function saveDefaultContactItem(itemId, callbackFn) {
            $.ajax({
                type: "POST",
                url: "RequestHandler.ashx",
                data: JSON.stringify({
                    Instruction: 'make_default_contact_detail',
                    ItemId: itemId
                }),
                dataType: "json",
            }).done(function () {
                div.dialog('close');
                showSavedSplashDialog("Saved!");
                if (callbackFn) {
                    callbackFn(itemId);
                }
            });
        }
    }
}
