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

// global zoom level variable
var globalZoomLevel = 13;

function initEventHandlers() {
    $.contextMenu({
        selector: '.context-menu-prospect',
        build: function ($trigger, e) {
            var items = {};
            if (!filterMode) {
                items = { "Search Lightstone here": { name: "Search Lightstone here", icon: "lightstone" } };
            }
            
            return {
                callback: function (key, options) {
                    switch (key) {
                        case "Search Lightstone here":
                            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Searching Lightstone at this location...</p>' });
                            buildLightstoneMatchesContent();
                            break;
                    }
                },
                items: items
            };
        }
    });

    $.contextMenu({
        selector: '.context-menu-rightclick-property',
        build: function ($trigger, e) {
            var items = {
                "Add Activity": { name: "Add Activity", icon: "add_activity" },
                "Reload Owners": { name: "Reload Owners", icon: "reload_from_lightstone" }
            };
            if (prospectingContext.UserHasCommAccess) {
                if (rightClickedProperty.Prospected) {
                    items["New SMS Message"] = { name: "New SMS Message", icon: "new_sms_message" };
                    //items["New Email Message"] = { name: "New Email Message", icon: "new_email_message" };
                }
            }
            return {
                callback: function (key, options) {
                    switch (key) {
                        case "Add Activity":
                            handlePropertyRightClick();
                            break;
                        case "New SMS Message":
                            newMessageToProperty('SMS', rightClickedProperty.Marker);
                            break;
                        case "New Email Message":
                            newMessageToProperty('EMAIL', rightClickedProperty.Marker);
                            break;
                        case "Reload Owners":
                            handleReloadFromLightstone(function (marker) {
                                updateOwnershipOfProperty(marker, function (marker) {
                                    var prop = marker.ProspectingProperty;
                                    if (prop.SS_FH == 'SS' || prop.SS_FH == 'FS') {
                                        prop.Contacts = null;
                                        loadExistingSSUnit(prop, null);
                                    } else {
                                        prop.Contacts = null;
                                        loadExistingProspectReloadFromLightstone(prop);
                                    }
                                });
                            });
                            break;
                    }
                },
                items: items
            };
        }
    });
}

function handleReloadFromLightstone(proceedCallback) {
    if (rightClickedProperty != null) {
        var targetMarker = rightClickedProperty.Marker;
        var div = $('<div id="reloadFromLightstoneDialog" title="Reload From Lightstone"  style="font-family:Verdana;font-size:12px;" />').empty();
        div.append("<p /><p />");
        div.append("This operation will reload and update the property from Lightstone. If the current owners on record in Prospecting are the same as the Lightstone owners, all their details and contact information will be retained provided that the owners have valid RSA ID numbers.<p /> Beware using this function if the contacts associated with this property have invalid ID numbers or have been manually associated such as with tenants, as this operation will disassociate all non-owners from the property.");
        div.dialog(
                   {
                       modal: true,
                       closeOnEscape: false,
                       width: '550',
                       buttons: {
                           "Continue": function () { $(this).dialog("close"); proceedCallback(targetMarker); },
                           "Cancel": function () { $(this).dialog("close"); }
                       },
                       position: ['center', 'center']
                   });
    }
    rightClickedProperty = null;
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
        if (!handleResponseIfServerError(matchesData)) {
            return;
        }

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
        var selectAllCheckbox = $("<label style='display:inline-block;float:right'><input type='checkbox' id='selectAllNewProspects' checked />Select All</label>");
        form.append(selectAllCheckbox);
        $('body').unbind('change.selectAllProspects').on('change.selectAllProspects', '#selectAllNewProspects', function () {
            var selectAll = $('#selectAllNewProspects').is(":checked");
            $('#areaMatchesForm input:checkbox').not('#selectAllNewProspects').each(function (idx, item) { this.checked = selectAll; });
        });

        form.append("<p />");
        var createBtn = $('<input type="button" id="createProspectBtn" value="Create Properties" style="display:inline-block" />');
        form.append(createBtn);

        // If no items were added, show it.
        if ($.isEmptyObject(itemIndex)) {
            div.empty();
            div.append('All properties and complexes have already been prospected at this location.');
            createBtn.attr('disabled', 'disabled');
            selectAllCheckbox.attr('disabled', 'disabled');
        }

        div.append(form);

        $('body').unbind('click.createProspect').on('click.createProspect', '#createProspectBtn', function () {
            var selectedEntities = [];
            $('#areaMatchesForm input:checked').not('#selectAllNewProspects').each(function () {
                var id = $(this).attr('id');
                var entity = itemIndex[id];
                selectedEntities.push(entity);
            });

            if (selectedEntities.length > 0) {
                closeInfoWindow();
                $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Creating new prospects. Please note this process may take a few minutes to complete...</p>' });

                createProspectingEntities(selectedEntities, function (data) {
                    var dataObject = $.parseJSON(data);
                    if (dataObject.CreationErrorMsg) {
                        $.unblockUI();
                        // reload suburb 
                        handleCreateProspectsServerError(dataObject, selectedEntities);
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
                                try {
                                    centreMap(currentSuburb, currentProperty.Marker, false);
                                } catch (e) { }
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

function handleCreateProspectsServerError(dataObject, createEntities) {
    if (dataObject.CreationErrorMsg.indexOf('Property already exists in the system.') > -1) {      
        if (dataObject.SeeffAreaId) {
            alert('The property(ies) you are trying to create already exist in the system. Click OK to reload suburb...');
            var suburb = getSuburbById(dataObject.SeeffAreaId);
            if (suburb) {
                clearSuburbBySuburbId(suburb.SuburbId);
                suburb.IsInitialised = false;
            }
            if (currentSuburb) {
                clearSuburbBySuburbId(currentSuburb.SuburbId);
                currentSuburb.IsInitialised = false;
            }
            loadSuburb(dataObject.SeeffAreaId, false, null, false);
        } 
    }
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
        timeout: 320000,
        data: JSON.stringify(inputData)
    })
        .done(function (data) {
            if (!handleResponseIfServerError(data)) {
                return;
            }

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
    if (!multiSelectMode) {
        var point = fromLatLngToPoint(currentClickLatLng, map);
        $('.context-menu-prospect').contextMenu({ x: point.x, y: point.y });
    }
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
                if (!handleResponseIfServerError(data)) {
                    return;
                }
                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }
                currentMarker.ContactInfoPacket = data;
                if (data.WalletBalance != null) {
                    // If the AvailableTracePsCredits is not null, then update the availableCredit variable
                    availableCredit = data.WalletBalance;
                    $('#availableCreditLabel').text(availableCredit.toFixed(2));
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

function loadSuburb(suburbId,showSeeffCurrentListings, actionAfterLoad, mustCentreMap, filterFunction) {

    var suburb = getSuburbById(suburbId);
    if (suburb == null) {
        suburb = newSuburb();
        suburb.SuburbId = suburbId;
    }
    //suburb.IsInitialised = false; // Adding this line in here to force obtaining the latest data from the database each load (this is important because the contacts for the property could be updated elsewhere and changes must reflect)

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading ' + suburb.SuburbName + '...</p>' });
    if (!suburb.IsInitialised) {
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({ Instruction: "load_suburb", SuburbId: suburbId }),
            success: function (data, textStatus, jqXHR) {
                if (textStatus == "success") {
                    if (!handleResponseIfServerError(data)) {
                        return;
                    }

                    if (data.UnderMaintenance == true) {
                        $.unblockUI();
                        alert('This suburb is currently under maintenance and not available right now. Please try again in a few minutes.');
                        return;
                    }

                    if (data.PolyCoords.length > 0) {
                        initialiseAndDisplaySuburb(suburb, data, showSeeffCurrentListings, filterFunction);

                        $.unblockUI();
                        if (actionAfterLoad) {
                            actionAfterLoad();
                        } else {
                            if (mustCentreMap !== false) {
                                centreMap(suburb, null, true);
                            }
                        }
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
        initialiseAndDisplaySuburb(suburb, null, showSeeffCurrentListings, filterFunction);

        $.unblockUI();
        if (actionAfterLoad) {
            actionAfterLoad();
        } else {
            if (mustCentreMap !== false) {
                centreMap(suburb, null, true);
            }
        }
    }
}

function changeBgColour(id, colour) {
    var id = "#unit" + id;
    var row = $(id);
    row.data('color', colour)
    //row.css('background-color', 'lightblue');
}

function setCurrentMarker(suburb, property, callback) {
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_existing_prospecting_property", LightstonePropertyId: property.LightstonePropertyId,LoadActivities: false }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (!handleResponseIfServerError(data)) {
                    return;
                }

                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }
                testpp = data;

                $.each(suburb.ProspectingProperties, function (idx, pp) {
                    if (pp.ProspectingPropertyId == testpp.ProspectingPropertyId) {
                        pp.Prospected = testpp.Prospected;
                        pp.Marker.setIcon(getIconForMarker(pp.Marker));

                        updateExistingPropertyStats(pp, testpp);
                        //if (testpp.Prospected) {
                        //    if (pp.SS_FH == "FH") {
                        //        pp.Marker.setIcon('Assets/marker_icons/prospecting/prospected.png');
                        //    } else {
                        //        changeBgColour(testpp.LightstonePropertyId, "#009900");
                        //    };
                        //}
                        //else {
                        //    if (pp.SS_FH == "FH") {
                        //        pp.Marker.setIcon('Assets/marker_icons/prospecting/unprospected.png');
                        //    } else {
                        //        changeBgColour(testpp.LightstonePropertyId, "#FBB917");
                        //    }
                        //}
                    }                    
                });

                if (callback) {
                    callback(data);
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

function showChangeOfOwnershipDialog(freeholdOrSS, proceedCallback, cancelCallback) {
    var div = $("<div id='ownershipChangeDialog' title='Change Of Ownership' style='font-family:Verdana;font-size:12px;' />").empty();
    var deleteOldContactsFromLists = $("<label style='display:inline-block;vertical-align:middle;padding-right:10px'><input type='checkbox' id='deleteOldContactsFromLists' style='display:inline-block;vertical-align:middle;' />On update remove old owners from all my lists</label>");
    if (freeholdOrSS == 'FH') {        
        div.append("The ownership of this property has changed and the current information on record is stale. ")
        .append("Please update this property with the latest data from Lightstone. The existing contacts will be replaced by the new owners, however an activity will be created for the property containing details of the previous owners and their default contact information, should you wish to revert to that information at a later point in time.")
        .append("<p />")
        .append(deleteOldContactsFromLists);

        div.dialog(
                {
                    modal: true,
                    closeOnEscape: false,
                    width: '550',
                    open: function (event, ui) { $(".ui-dialog-titlebar-close", $(this).parent()).hide(); },
                    buttons: {
                        "Update ownership": function () { proceedCallback(); $(this).dialog("close").empty(); },
                        "Continue to property": function () { $(this).dialog("close").empty(); cancelCallback(); }
                    },
                    position: ['center', 'center']
                });
    }
    if (freeholdOrSS == 'SS') {
        div.append("The ownership of one or more units in this sectional title has changed.<br />")
        .append("It is recommended that you update the affected units with the latest data from Lightstone. <br />")
        .append("The affected units will appear highlighted in red.");
        //.append("<p />")
        //.append(deleteOldContactsFromLists);

        div.dialog(
                {
                    modal: true,
                    closeOnEscape: false,
                    width: 'auto',
                    open: function (event, ui) { $(".ui-dialog-titlebar-close", $(this).parent()).hide(); },
                    buttons: {
                        "Ok": function () { $(this).dialog("close").empty(); proceedCallback(); }
                    },
                    position: ['center', 'center']
                });
    }
}

function updateOwnershipOfProperty(marker, callbackFn) {
    var property = marker.ProspectingProperty;
    var deleteOldOwnersFromLists = $("#deleteOldContactsFromLists").length ? $("#deleteOldContactsFromLists").is(":checked") : false;
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Updating Property Ownership...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "update_property_ownership", LightstonePropertyId: property.LightstonePropertyId, DeleteOldContactsFromMyLists: deleteOldOwnersFromLists }),
        dataType: "json"
    }).done(function (data) {
        $.unblockUI();
        if (data == false) {
            alert("The system is unable to update the ownership of this property at this point in time because it has already been updated by another user or the Lightstone service is unavailable. Please try again later.");
        }
        callbackFn(marker);
    });
}

function markerClick(e) {
    closeInfoWindow();
    closeTransientInfoWindow(this);

    if (multiSelectMode) { // if we are in multi-select mode

        if (!commCustomSelectionEnabled) {
            showDialogCommCustomSelectionDisabled();
            return;
        }

        var marker = $(this)[0];
        if (marker.IsPartOfSelection) {
            removeMarkersFromSelection(marker);
        } else {
            var selectOnlyThisUnit = (e && e.TargetOnlySelectedProperty) ? true : false;
            var fromFilter = marker.ProspectingProperty.Whence == 'from_filter' ? true : false;
            addMarkerToSelection(marker, true, selectOnlyThisUnit, fromFilter);
        }
        return;
    }

    $('#propertyInfoDiv').css('display', 'none');
    var marker = $(this)[0];
    var prop = marker.ProspectingProperty;
    if (prop.SS_FH == 'SS' || prop.SS_FH == 'FS') {
        // SS
        var ssUnits = $.grep(currentSuburb.ProspectingProperties, function (pp) {
            if (!pp.SS_UNIQUE_IDENTIFIER) return false;
            return pp.SS_UNIQUE_IDENTIFIER == prop.SS_UNIQUE_IDENTIFIER;
        });
        var requiresOwnerUpdates = false;
        $.each(ssUnits, function (idx, unit) {
            if (unit.LatestRegDateForUpdate) {
                requiresOwnerUpdates = true;
            }
        });
        if (requiresOwnerUpdates) {
            showChangeOfOwnershipDialog('SS', function () { loadExistingProspectingProperty(marker, e); }, function () { });
        } else {
            loadExistingProspectingProperty(marker, e);
        }
    } else { // FH
        if (marker.ProspectingProperty.LatestRegDateForUpdate) {
            showChangeOfOwnershipDialog('FH', function () {
                updateOwnershipOfProperty(marker, function (marker) {
                    marker.ProspectingProperty.Contacts = null;
                    loadExistingProspectingProperty(marker, e);
                    marker.setIcon(getIconForMarker(marker));
                });
            }, function () { loadExistingProspectingProperty(marker, e); });
        } else {
            loadExistingProspectingProperty(marker, e);
        }
    }        
}

function loadExistingProspectingProperty(marker, eventData) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Property Info...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_existing_prospecting_property", LightstonePropertyId: marker.ProspectingProperty.LightstonePropertyId, LoadActivities: false }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (!handleResponseIfServerError(data)) {
                    return;
                }

                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }

                if (data.IsLockedByOtherUser == true) {
                    warnUserRecordIsLocked(data);
                } else {
                    updateExistingPropertyFromProperty(marker.ProspectingProperty, data);
                    currentMarker = marker;
                    loadProspectingProperty(marker, eventData);
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

function warnUserRecordIsLocked(propertyResult) {
    var datetime = propertyResult.LockedDateTime.split('T');
    var dateportion = datetime[0];
    var timeportion = datetime[1].substring(0, 5);
    if (timeportion == '00:00') timeportion = '';
    propertyResult.LockedDateTime = dateportion + " @ " + timeportion;

    var displayMsg = 'On ' + propertyResult.LockedDateTime + " " + propertyResult.LockedUsername + " locked this record. If you would like to work on the record please ask them to release it.";
    alert(displayMsg);
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
                if (!handleResponseIfServerError(data)) {
                    return;
                }

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

                            showMenu("contactdetails");
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

function loadExistingProspectReloadFromLightstone(property) {
    //closeInfoWindow();
    $('#propertyInfoDiv').css('display', 'none');
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_existing_prospecting_property", LightstonePropertyId: property.LightstonePropertyId }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (!handleResponseIfServerError(data)) {
                    return;
                }

                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }

                updateExistingPropertyFromProperty(property, data);
                currentMarker = property.Marker;
                currentProperty = property;
                updateOwnerDetailsEditor();
                //updatePropertyInfoMenu();  // NB CHECK THIS

                if (currentProperty.SS_FH != 'SS' && currentProperty.SS_FH != 'FS') {
                    openInfoWindow(property.Marker, function () {
                        // openInfoWindow calls closeInfoWindow which resets a whole bunch of globals, so re-init them here
                        currentProperty = property;
                        currentMarker = property.Marker;
                        updateOwnerDetailsEditor();

                        showMenu("contactdetails");
                    });
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

function loadFollowups(callbackFn, mustBlockUI) {
    if (mustBlockUI) {
        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading...</p>' });
    }

    var existingFollowups = [];
    //$('.followup-item-container').each(function (idx, fu) {
    //    var id = $(fu).attr('id');
    //    var activityLogId = id.replace('followup_','');
    //    existingFollowups.push(activityLogId);
    //});
    $.each(globalFollowUps, function (idx, fol) {
        existingFollowups.push(fol.ActivityLogId);
    });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "load_followups", ExistingFollowupItems: existingFollowups }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (!handleResponseIfServerError(data)) {
                    return;
                }

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
    var followupDate = $("<input type='text' name='followupDateInput' id='followupDateInput' style='height:12px;font-size:12px;width:295px;' readonly='true' /><p style='margin:6px;' />");
    div.append(followupDate);
    followupDate.datepicker({
        dateFormat: 'DD, d MM yy', minDate: 0, changeMonth: true,
        changeYear: true
    });
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
            if (el.Value == 'Valuation Done' || el.Value == 'Valuation Follow-up')
                return; // TODO: excluding this value here for backwards compatibility. This activity type must still be available for other operations like filtering, users should not be able to create new activities against this type. -- Remove from DB after a year or so.
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
                if (!handleResponseIfServerError(data)) {
                    return;
                }

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
    if (multiSelectMode) {
        return;
    }

    // Find the marker underneath the click
    var marker = $(this)[0];
    rightClickedProperty = null;
    if (marker.ProspectingProperty.SS_FH != 'SS' && marker.ProspectingProperty.SS_FH != 'FS') {
        if (marker.ProspectingProperty.LatestRegDateForUpdate) {
            // Disable right click if this property's ownership needs to be updated
            return;
        }

        rightClickedProperty = marker.ProspectingProperty;
        var clickLoc = new google.maps.LatLng(event.latLng.lat(), event.latLng.lng());
        var point = fromLatLngToPoint(clickLoc, map);
        $('.context-menu-rightclick-property').contextMenu({ x: point.x, y: point.y });
    }
}

function loadProspectingProperty(marker, eventData) {
    currentMarker = marker;
    openInfoWindow(marker, function () {
        if (marker.ProspectingProperty.SS_FH == "SS" || marker.ProspectingProperty.SS_FH == "FS") {
            currentProperty = null;
            if (marker.ProspectingProperty.Whence) {
                var container = $('#ssUnitsTbl');
                var unitInTable = $('#unit_row' + marker.ProspectingProperty.LightstonePropertyId);
                var offset = unitInTable.offset();
                var topOffset = offset.top - unitInTable.height();
                container.scrollTop(topOffset);

                if (marker.ProspectingProperty.Whence == 'from_comm_report') {
                    currentProperty = marker.ProspectingProperty;
                    updateOwnerDetailsEditor();
                    showMenu("contactdetails");

                    openDetailsForContactFromContext(marker.ProspectingProperty.TargetContactIdForComms);
                    marker.ProspectingProperty.Whence = null;
                }

                if (marker.ProspectingProperty.Whence == 'from_followup') {
                    if (eventData && eventData.FromFollowup) {
                        var followup = eventData.FromFollowup;
                        currentProperty = marker.ProspectingProperty;
                        updateOwnerDetailsEditor(eventData);
                        showMenu("contactdetails");

                        if (followup.RelatedToContactPersonId) {
                            openDetailsForContactFromContext(followup.RelatedToContactPersonId, eventData);
                        }
                    }
                    marker.ProspectingProperty.Whence = null;
                }

                if (marker.ProspectingProperty.Whence == 'from_activity') {
                    if (eventData && eventData.FromActivity) {
                        var activity = eventData.FromActivity;
                        currentProperty = marker.ProspectingProperty;
                        updateOwnerDetailsEditor();
                        showMenu("contactdetails");

                        if (activity.RelatedToContactPersonId) {
                            openDetailsForContactFromContext(activity.RelatedToContactPersonId);
                        }
                    }
                    marker.ProspectingProperty.Whence = null;
                }
            }
        }
        else {
            currentProperty = marker.ProspectingProperty;
            currentProperty.Marker = marker;
            updateOwnerDetailsEditor(eventData);
            showMenu("contactdetails");

            if (marker.ProspectingProperty.Whence == 'from_comm_report') {
                openDetailsForContactFromContext(marker.ProspectingProperty.TargetContactIdForComms);
                marker.ProspectingProperty.Whence = null;
            }

            if (marker.ProspectingProperty.Whence == 'from_followup') {
                if (eventData && eventData.FromFollowup) {
                    var followup = eventData.FromFollowup;
                    if (followup.RelatedToContactPersonId) {
                        openDetailsForContactFromContext(followup.RelatedToContactPersonId, eventData);
                    }
                }
                marker.ProspectingProperty.Whence = null;
            }

            if (marker.ProspectingProperty.Whence == 'from_activity') {
                if (eventData && eventData.FromActivity) {
                    var activity = eventData.FromActivity;
                    if (activity.RelatedToContactPersonId) {
                        openDetailsForContactFromContext(activity.RelatedToContactPersonId);
                    }
                }
                marker.ProspectingProperty.Whence = null;
            }
            
            updateProspectedStatus();
        }
        updatePropertyInfoMenu();
    });
}

function openDetailsForContactFromContext(contactPersonIdFromContext, context) {
    var contactPerson = $.grep(currentProperty.Contacts, function (c) {
        return contactPersonIdFromContext == c.ContactPersonId; // person or company
    })[0];
    if (contactPerson) {
        currentPersonContact = contactPerson;
        $('#contactsExpander').remove();
        openExpanderWidget(currentPersonContact, context);
    }
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

function updateProspectingRecord(record, property, callbackFn) {

    var inputPacket = {
        Instruction: "update_prospecting_property",
        PropertyAddress: record.PropertyAddress,
        StreetOrUnitNo: record.StreetOrUnitNo,
        SSDoorNo: record.SSDoorNo,
        SS_FH: record.SS_FH,
        ProspectingPropertyId: record.ProspectingPropertyId,
        TitleCaseSS: record.TitleCaseSS,

        ErfSize: record.ErfSize,
        DwellingSize: record.DwellingSize,
        Condition: record.Condition , 
        Beds:record.Beds ,
        Baths:record.Baths ,
        Receptions:record.Receptions ,
        Studies:record.Studies ,
        Garages:record.Garages ,
        ParkingBays:record.ParkingBays ,
        Pool:record.Pool ,
        StaffAccomodation: record.StaffAccomodation,

        IsShortTermRental: record.IsShortTermRental,
        IsLongTermRental: record.IsLongTermRental,
        IsCommercial: record.IsCommercial,
        IsAgricultural: record.IsAgricultural,
        IsInvestment: record.IsInvestment
    };

    if (!property) {
        property = currentProperty;
    }

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Updating Property Details...</p>' });
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
                    if (!handleResponseIfServerError(data)) {
                        return;
                    }

                    if (record.SS_FH == "SS") {
                        property.SSDoorNo = record.SSDoorNo;
                        if (record.SSName) {
                            property.SSName = record.SSName;
                        }
                    } else {
                        property.PropertyAddress = record.PropertyAddress;
                        property.StreetOrUnitNo = record.StreetOrUnitNo;
                    }

                    if (callbackFn) {
                        callbackFn();
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
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Saving Contact Information...</p>' });
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
                if (!handleResponseIfServerError(data)) {
                    return;
                }

                if (data.ContactIsCompromised) {
                    showDialogUserDeletedDetailsWarning();
                }
                else {
                    actionToExecuteAfterwards(data);
                    $.unblockUI();
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
        contact.PersonCompanyRelationshipType = newContact.PersonCompanyRelationshipType;
        contact.PhoneNumbers = newContact.PhoneNumbers;
        contact.EmailAddresses = newContact.EmailAddresses;
        contact.Comments = newContact.Comments;
        contact.IsPOPIrestricted = newContact.IsPOPIrestricted;
        contact.EmailOptout = newContact.EmailOptout;
        contact.SMSOptout = newContact.SMSOptout;
        contact.DoNotContact = newContact.DoNotContact;
        contact.EmailContactabilityStatus = newContact.EmailContactabilityStatus;

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

function getSuburbById(suburbId) {
    if (suburbsInfo) {
        var suburb = $.grep(suburbsInfo, function (s) {
            return s.SuburbId == suburbId;
        });

        if (suburb[0]) {
            return suburb[0];
        }
    }

    return null;
}

function updateOwnerDetailsEditor(context) {
    var contactDetailsPane = $('#contactDetailsDiv');
    contactDetailsPane.empty();
    contactDetailsPane.css('display', 'block');
    if (currentMarker) {
        contactDetailsPane.append(buildPersonContactMenu(currentProperty.Contacts, false, context));
    }
}

function loadActivityLookupData(callback) {    
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "load_activity_lookup_data" }),
        success: function (data, textStatus, jqXHR) {
            if (textStatus == "success" && data) {
                if (!handleResponseIfServerError(data)) {
                    return;
                }

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
        currentSuburb = suburb;
        createMarkersForSuburb(suburb, showSeeffCurrentListings);
        var visibleMarkers = suburb.Markers;

        if (searchFunc) {
            visibleMarkers = searchFunc(visibleMarkers);
        }

        enableMarkerClustering(suburb, visibleMarkers);
        enableSpiderfier(suburb, visibleMarkers);
        suburb.VisibleMarkers = visibleMarkers;

        drawPolygonForSuburb(suburb);
    }
}

function enableMarkerClustering(suburb, visibleMarkers) {
    var mcOptions = { maxZoom: 15, minimumClusterSize: 1 };
    var markerClusterer = new MarkerClusterer(map, visibleMarkers, mcOptions);

    suburb.MarkerClusterer = markerClusterer;
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
        //fillOpacity: '0.0',
        strokeColor: '#FF0000'
    });

    poly.setMap(map);
    suburb.Polygon = poly;
    poly.Suburb = suburb;

    google.maps.event.addListener(poly, 'click', function (event) {
        handleSuburbPolyClick(event, poly);
    });
}

function handleSuburbPolyClick(event, poly) {
    currentSuburb = poly.Suburb;
    handleMapClick(event);
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
                if (!marker.IsPartOfSelection) {
                    if (property.Prospected) {
                        changeBgColour(property.LightstonePropertyId, "#009900");
                    } else {
                        changeBgColour(property.LightstonePropertyId, "#FFFF00");
                    }
                    if (property.LatestRegDateForUpdate) {
                        changeBgColour(property.LightstonePropertyId, "#FF0000");
                    }
                    if (currentSuburb) {
                        var ssUnits = $.grep(currentSuburb.ProspectingProperties, function (pp) {
                            if (!pp.SS_UNIQUE_IDENTIFIER) return false;
                            return pp.SS_UNIQUE_IDENTIFIER == property.SS_UNIQUE_IDENTIFIER;
                        });
                        var requiresOwnerUpdates = false;
                        $.each(ssUnits, function (idx, unit) {
                            if (unit.LatestRegDateForUpdate) {
                                requiresOwnerUpdates = true;
                            }
                        });
                        if (requiresOwnerUpdates) {
                            return path += 'ss_attention_required.png';
                        }
                    }
                    return path += 'ss_unprospected.png';
                }
                else {
                    return path += 'ss_unprospected_bulk_select.png';
                }
            }
        }       
        // Any other FH types
        return path += getPathIdentifierForFH(property);
    }

    function getPathIdentifierForFH(property) {
        if (property.ProspectingPropertyId) {
            if (property.Prospected) {
                // Fully prospected
                if (!marker.IsPartOfSelection) {
                    if (property.LatestRegDateForUpdate) {
                        return 'prospected_attention_required.png';
                    }
                    return 'prospected.png';
                } else {
                    return 'prospected_bulk_select.png';
                }
            }

            if (!marker.IsPartOfSelection) {
                if (property.LatestRegDateForUpdate) {
                    return 'unprospected_attention_required.png';
                }
                return 'unprospected.png';
            } else {
                return 'unprospected_bulk_select.png';
            }
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

function centreMap(suburb, marker, mustSetZoom) {

    if (!suburb.IsInitialised) {
        return;
    }

    if (mustSetZoom) {
        map.setZoom(globalZoomLevel);
    }
    if (marker) {
        var mapMarkerPos = calcMapCenterWithOffset(marker.getPosition().lat(), marker.getPosition().lng(), -350, 0);
        if (!mapMarkerPos) {
            mapMarkerPos = marker.position;
        }
        map.setCenter(mapMarkerPos);
    } else {
        panToWithOffset(new google.maps.LatLng(suburb.PolyCoords[0].Lat, suburb.PolyCoords[0].Lng), -350, 0);
        //var pos = calcMapCenterWithOffset(suburb.PolyCoords[0].Lat, suburb.PolyCoords[0].Lng, -350, 0);
        //if (pos) {
        //    map.setCenter(pos);
        //}
        //else {
        //    map.setCenter(new google.maps.LatLng(suburb.PolyCoords[0].Lat, suburb.PolyCoords[0].Lng));
        //}
    }
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
    google.maps.event.addListener(marker, 'mouseover', function () {
       
        var that = this;
        clearTimeout(this.timer);
        this.timer = setTimeout(function () {
            if (that != currentMarker && !that.TransientInfoWindow) {
                showTransientInfoWindowOnHover(that);
            }
        }, 300);
        google.maps.event.addListenerOnce(this, 'mouseout', function () { clearTimeout(this.timer); closeTransientInfoWindow(this); });
    });
    //google.maps.event.addListener(marker, 'mouseout', function () {
    //    closeTransientInfoWindow(this);
    //});

    return marker;
}

function closeTransientInfoWindow(marker) {
    if (marker.TransientInfoWindow) {
        marker.TransientInfoWindow.close();
        marker.TransientInfoWindow = null;
    }
}

function buildInfoWindowContentForSS(unit) {

    function buildUnitContentRow(unit) {

        function getBGColourForRow(unit) {
            if (unit.Whence == 'from_followup' || unit.Whence == 'from_activity') {
                return "#CC00CC";
            }
            if (unit.Whence == 'from_comm_report') {
                return "#CC00CC";
            }
            if (unit.LatestRegDateForUpdate) {
                return "#FF0000";
            }
            var hasContactsWithDetails = false;
            if (unit.Prospected) {
                hasContactsWithDetails = true;
            }
            return hasContactsWithDetails ? "#009900" : "#FFFF00";
        }

        function buildUnitContent(unit) {
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
            if (unit.Contacts && unit.Contacts.length) {
                $.each(unit.Contacts, function (idx, c) {
                    unitContent = unitContent + '<br />' + c.Firstname + ' ' + c.Surname;
                });
            }
            return unitContent;
        }
        
        var unitRowId = 'unit_row' + unit.LightstonePropertyId;
        var tr = $("<tr class='unitrow' id='" + unitRowId + "' />");
        var id = "unit" + unit.LightstonePropertyId;
        var color = getBGColourForRow(unit);
        var bgcolor = "background-color:" + color;
        var td = tr.append($("<td id='" + id + "' class='unittd' data-color='" + color + "' style='cursor:pointer;width:200px;text-align:left;" + bgcolor + "' />").append(buildUnitContent(unit)));

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

                openSSUnitInfo(unit, function () {
                    row.empty();
                    row.append(buildUnitContent(unit));
                    // Update the icon for the SS
                    unit.Marker.setIcon(getIconForMarker(unit.Marker));

                    // Determine whether all units in the SS are now up-to-date, if so then reset their icons
                    var ssUnits = $.grep(currentSuburb.ProspectingProperties, function (pp) {
                        if (!pp.SS_UNIQUE_IDENTIFIER) return false;
                        return pp.SS_UNIQUE_IDENTIFIER == unit.SS_UNIQUE_IDENTIFIER;
                    });
                    var anyOtherUnitsNeedUpdate = false;
                    $.each(ssUnits, function (idx, ssunit) {
                        if (ssunit.LatestRegDateForUpdate) {
                            anyOtherUnitsNeedUpdate = true;
                        }
                    });
                    // If no other units need to be updated then revert to the normal icon.
                    if (!anyOtherUnitsNeedUpdate) {
                        $.each(ssUnits, function (idx, ssunit) {
                            ssunit.Marker.setIcon(getIconForMarker(ssunit.Marker));
                        });
                    }
                });
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

    var isFiltering = false;
    $.grep(currentSuburb.ProspectingProperties, function (pp) {
        if (pp.Whence && pp.Whence == 'from_filter')
            isFiltering = true;
    });
  
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

    var tableOfUnits = $("<table id='ssUnitsTbl' class='info-window' style='display: block;max-height:300px;overflow-y:auto;width:250px;' />");
    tableOfUnits.empty();

    for (var i = 0; i < ssUnits.length; i++) {
        var u = ssUnits[i];
        if (isFiltering) {
            if (u.Whence != 'from_filter')
                continue;
        }

        var unitContent = buildUnitContentRow(u);
        tableOfUnits.append(unitContent);
    }

    return tableOfUnits;
}

function loadExistingSSUnit(unit, callbackFn) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Property Info...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_existing_prospecting_property", LightstonePropertyId: unit.LightstonePropertyId, LoadActivities: false }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (!handleResponseIfServerError(data)) {
                    return;
                }

                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }

                if (data.IsLockedByOtherUser == true) {
                    warnUserRecordIsLocked(data);
                } else {
                    updateExistingPropertyFromProperty(unit, data);
                    currentProperty = unit;
                    updateOwnerDetailsEditor();
                    showMenu("contactdetails");

                    updateProspectedStatus();

                    if (callbackFn) {
                        callbackFn();
                    }
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

function openSSUnitInfo(unit, callbackFn) {

    if (unit.LatestRegDateForUpdate) {
        showChangeOfOwnershipDialog('FH', function () {
            updateOwnershipOfProperty(unit.Marker, function () {
                unit.Contacts = null;
                loadExistingSSUnit(unit, callbackFn);
            });
        },
        function () { loadExistingSSUnit(unit, callbackFn); });
    } else {
        loadExistingSSUnit(unit, callbackFn);
    }    
}

// This function will update an object that already exists on the front-end with an incoming object's data - it does not change the existing reference.
function updateExistingPropertyFromProperty(existingProp, newProp) {
    existingProp.ProspectingPropertyId = newProp.ProspectingPropertyId;
    existingProp.LatLng = newProp.LatLng;
    existingProp.LightstonePropertyId = newProp.LightstonePropertyId;
    existingProp.PropertyAddress = newProp.PropertyAddress;
    existingProp.StreetOrUnitNo = newProp.StreetOrUnitNo;
    existingProp.SeeffAreaId = newProp.SeeffAreaId;
    updateContactsOnProperty(existingProp, newProp);
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
    existingProp.LatestRegDateForUpdate = newProp.LatestRegDateForUpdate;
   // existingProp.HasMandate = newProp.HasMandate;

    updateExistingPropertyStats(existingProp, newProp);
}

function updateExistingPropertyStats(existingProp, newProp) {
    existingProp.HasContactWithCell = newProp.HasContactWithCell;
    existingProp.HasContactWithPrimaryCell = newProp.HasContactWithPrimaryCell;
    existingProp.HasContactWithEmail = newProp.HasContactWithEmail;
    existingProp.HasContactWithPrimaryEmail = newProp.HasContactWithPrimaryEmail;
    existingProp.HasContactWithLandline = newProp.HasContactWithLandline;
    existingProp.HasContactWithPrimaryLandline = newProp.HasContactWithPrimaryLandline;
}

function updateContactsOnProperty(existingProp, newProp) {
    //existingProp.Contacts = newProp.Contacts;
    if (existingProp.Contacts == null && newProp.Contacts != null) {
        existingProp.Contacts = newProp.Contacts;
    }
    else {
        if (existingProp.Contacts && newProp.Contacts) {
            $.each(newProp.Contacts, function (idx, con) {
                // Try find the existing contact and update it, otherwise add it
                var existingContact = $.grep(existingProp.Contacts, function (ex) {
                    return ex.ContactPersonId == con.ContactPersonId;
                })[0];
                if (existingContact) {
                    updateExistingContactFromContact(existingContact, con);
                } else {
                    existingProp.Contacts.push(con);
                }
            });
        }
    }

    function updateExistingContactFromContact(existingContact, newContact) {
        existingContact.PhoneNumbers = newContact.PhoneNumbers;
        existingContact.EmailAddresses = newContact.EmailAddresses;
    }
}

function buildContentForInfoWindow(property, infowindow, showStreetView) {
    var outerDiv = $("<div id='infoWindowStandard' class='info-window' style='display:none;' />");
    var div = $("<div style='float:right;display:inline-block;' />");
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

        if (property.Contacts) {
            var contacts = "Number of contacts captured: " + property.Contacts.length;
            div.append("<br />");
            div.append(contacts);
        } 
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
        if (property.Contacts) {
            div.append("<br />");
            div.append("Property Contacts:");
            div.append("<br />");
            if (property.Contacts.length) {
                $.each(property.Contacts, function (idx, c) {

                    div.append(c.Firstname + " " + c.Surname + " (ID Number:" + c.IdNumber + ")");
                    div.append("<br />");
                });
            }
        }
        else {
            div.append("<br />");
            div.append("(Click to retrieve contacts)");
        }
        if (property.ContactCompanies) {
            if (property.ContactCompanies.length) {
                $.each(property.ContactCompanies, function (idx, c) {
                    var regNo = c.CKNumber && c.CKNumber.indexOf('UNKNOWN_CK') == -1 ? " (" + c.CKNumber + ")" : '';
                    div.append(c.CompanyName + regNo);
                    div.append("<br />");
                });
            }
        }
    }

    // SS, chrome,
    if (showStreetView) {
        var imgUrl = 'https://maps.googleapis.com/maps/api/streetview?key=AIzaSyDWHlk3fmGm0oDsqVaoBM3_YocW5xPKtwA&size=200x150&location=' + property.LatLng.Lat + ',' + property.LatLng.Lng + '&fov=90&heading=235&pitch=10';
        var imgElement = $("<img id='streetview_" + property.LightstonePropertyId + "' style='cursor:pointer' />").attr('src', imgUrl);
        var streetViewDiv = $("<div style='padding-right:5px;float:left;display:inline-block;width:201px' />")
                            .append($("<div><span style='font-size:10px'>(Click the image to go to StreetView)</span></div>"))
                            .append(imgElement);
        outerDiv.append(streetViewDiv).append(div);

        $(imgElement).waitForImages(function () {
            outerDiv.css('display', 'block');
            infowindow.open(map);
        });

        $('body').on('click', '#streetview_' + property.LightstonePropertyId, function () {
            var streetView = map.getStreetView();
            streetView.setPosition(new google.maps.LatLng(property.LatLng.Lat, property.LatLng.Lng));
            //map.bindTo("center", streetView, "position");
            var streetViewLayer = new google.maps.ImageMapType({
                getTileUrl: function (coord, zoom) {
                    return "http://www.google.com/cbk?output=overlay&zoom=" + zoom + "&x=" + coord.x + "&y=" + coord.y + "&cb_client=api";
                },
                tileSize: new google.maps.Size(256, 256)
            });
            map.overlayMapTypes.insertAt(0, streetViewLayer);
            streetView.setVisible(true);
        });
    } else {
        outerDiv.append(div);
        outerDiv.css('display', 'block');
    }
   
    return outerDiv[0].outerHTML;
}

function stripPropertyAddress(property) {
    var streetOrUnitNo = property.StreetOrUnitNo;
    var streetPortion = property.PropertyAddress.split(',')[0].trim();
    var suburbPortion = property.PropertyAddress.split(',')[1].trim();
    var townPortion = '';
    try {
        townPortion = property.PropertyAddress.split(',')[2].trim();
    } catch (e) {
        townPortion = suburbPortion;
    }

    return { StreetOrUnitNo: streetOrUnitNo, StreetName: streetPortion, Suburb: suburbPortion, CityTown: townPortion };
}

function showTransientInfoWindowOnHover(marker) {
    var pp = marker.ProspectingProperty;
    var y_offset = pp.SS_FH == "FH" ? -21 : -30;
    var tempInfowindow = new google.maps.InfoWindow({ pixelOffset: new google.maps.Size(0, y_offset), disableAutoPan: true });
    tempInfowindow.setContent(buildContentForInfoWindow(marker.ProspectingProperty, tempInfowindow, false));
    tempInfowindow.setPosition(marker.getPosition());
    marker.TransientInfoWindow = tempInfowindow;

    tempInfowindow.open(map);
}

function openInfoWindow(marker, actionAfterOpening) {
    closeInfoWindow();

    function createInfoWindowForMarker(marker) {
        infowindow = new google.maps.InfoWindow();
        infowindow.setContent(buildContentForInfoWindow(marker.ProspectingProperty, infowindow, true));
        infowindow.setPosition(marker.getPosition());
        
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
        unlockCurrentProperty();
        currentProperty = null;
        currentPersonContact = null;
        currentPropertyForPropertyInformation = null;
        if (tempIndicatorMarker) {
            tempIndicatorMarker.setMap(null);
            tempIndicatorMarker = null;
        }
        updatePropertyInfoMenu();
        updateOwnerDetailsEditor();
        clearActivityReport();
        resetFollowupFilters();
        togglePropertyInformationMenu(true);
        //updatePropertyNotesDiv();

        currentTracePSInfoPacket = null;
        currentTracePSContactRows = null;
    }
}

function unlockCurrentProperty() {
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: 'unlock_prospecting_record' }),
        async: false,
        dataType: "json"
    }).done(function (data) {
        if (!handleResponseIfServerError(data)) {
            return;
        }
    });
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
            ownerFullname.trim().length > 0 ||
            ownerIdNo.length > 0;
    }

    function validateOwnerNameAndSurname() {
        if (!ownerFirstName.length && !ownerSurname.length) return true;

        if (ownerFirstName.length < 3 || ownerSurname.length < 3) return false;

        return true;
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
    var ownerFirstName = $('#ownerFirstNameInput').val().trim();
    var ownerSurname = $('#ownerSurnameInput').val().trim();
    var ownerIdNo = $('#ownerIDnoInput').val().trim();

    var ownerFullname = ownerFirstName + " " + ownerSurname;

    try {
        if (!validateOwnerNameAndSurname()) {
            alert('When searching by name, please specify both firstname and surname (without initials).');
            return;
        }
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
                    OwnerName: ownerFullname,
                    OwnerIdNumber: ownerIdNo
                }),
                dataType: "json",
            }).done(function (results) {
                $.unblockUI();
                if (!handleResponseIfServerError(results)) {
                    return;
                }

                createLightstoneSearchResultsDiv(results);
            });
        }
    }
    catch (ex) {
        errorHandler(ex);
    }
}

function showSearchedPropertyOnMap(result) { // test for ss (new and existing)
    if (result.SeeffAreaId == null) {
        // Find the seeff area id for this result
        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Locating Area...</p>' });
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({
                Instruction: 'find_area_id',
                Lat: result.PropertyMatches[0].LatLng.Lat,
                Lng: result.PropertyMatches[0].LatLng.Lng
            }),
            dataType: "json",
        }).done(function (data) {
            $.unblockUI();
           
            if (data != null && handleLocatePropertyFromSearch(data, result)) {
                result.SeeffAreaId = data.SuburbId;
            }
            else {
                // We did not find a suburb belonging to this user.
                var message;
                if (data == null) {
                    message = 'Unable to find a Seeff area that contains this property';
                }
                else {
                    var areaName = data.SuburbName ? "'" + data.SuburbName + "'" : "Seeff Area ID";
                    if (data.SuburbId == -1) {
                        message = "This property falls outside your available areas, furthermore Prospecting cannot find the area to which this property belongs. Please contact support if you believe this property falls within a valid Seeff suburb.";
                    } else {
                        message = "This property falls outside your available areas. Please ask your prospecting administrator to add " + areaName + " (" + data.SuburbId + ") to your list of areas under 'Prospecting Permissions' on BOSS.";
                    }
                }
                alert(message);
            }
        }).fail(function (f) { alert(f.responseText); });
    }
    else {
        handleLocatePropertyFromSearch(result.SeeffAreaId, result);
    }
}

function handleLocatePropertyFromSearch(seeffArea, searchResult) {
    var containingSuburb = $.grep(suburbsInfo, function (sub) {
        return sub.SuburbId == seeffArea.SuburbId;
    })[0];

    if (!containingSuburb) return null;

    globalZoomLevel = 17;
    $('#suburbLink' + containingSuburb.SuburbId).trigger('click', function () {
        var targetProperty = $.grep(currentSuburb.ProspectingProperties, function (pp) {
            var matchFound = false;
            $.each(searchResult.PropertyMatches, function (idx, m) {
                if (m.LightstonePropId == pp.LightstonePropertyId) {
                    matchFound = true;
                }
            });
            return matchFound;
        })[0];
        if (targetProperty) { // a matching property already exists in the system
            var marker = targetProperty.Marker;
            try {
                centreMap(marker.Suburb, marker, true);
                new google.maps.event.trigger(marker, 'click');
            } catch (e) { }
        } else {
            // This is an entirely new prospect
            var content = generateOutputFromLightstone([searchResult]);
            var firstResult = searchResult.PropertyMatches[0];
            map.setZoom(globalZoomLevel);
            var latLng = new google.maps.LatLng(firstResult.LatLng.Lat, firstResult.LatLng.Lng);
            showPopupAtLocation(latLng, content);

            var pos = calcMapCenterWithOffset(latLng.lat(), latLng.lng(), -350, 0);
            if (pos) {
                map.setCenter(pos);
            }
            else {
                map.setCenter(latLng);
            }
        }
        globalZoomLevel = 13;
    });

    return true;
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
            }).done(function (data) {
                if (!handleResponseIfServerError(data)) {
                    return;
                }

                div.dialog('close');
                showSavedSplashDialog("Saved!");
                if (callbackFn) {
                    callbackFn(itemId);
                }
            });
        }
    }
}

function initializeMenuHtml() {

    menu = $('#mainpanel');
    $('#openpanelbutton').click(function () {
        $('#openpanelbutton').css('display', 'none');
        $('#closepanelbutton').trigger('click');
    });

    $('#snappanelbutton').click(function () {
        menu.css({ 'left': '0', 'top': '0' });
    });

    togglePanel();

    $("#menuitempanel").resizable();

    // Wire up the log off button
    $("#logoffBtn").click(function (e) {
        e.preventDefault();
        var logoffDialog = $("<div title='Log off Prospecting' style='font-family:Verdana;font-size:12px;' />").empty()
            .append("Are you sure you want to return to BOSS?")
            .append("<p />")
            .append("Note: If you return to BOSS, you might need to log out and log in again to access Prospecting.")
            .dialog(
            {
                modal: true,
                closeOnEscape: false,
                buttons: {
                    "Yes": function () {
                        $(this).dialog("close");
                        unlockCurrentProperty();
                        window.history.back();
                    },
                    "No": function () { $(this).dialog("close"); }
                },
                position: ['center', 'center']
            });
    });
}

function togglePanel(actionAfterClosing) {

    $('#closepanelbutton').unbind('click').bind('click', function () {
        $('#mainpanel').css('min-width', '');
        $("#mainpanel").animate(
            { width: 'toggle' },
            {
                duration: 500,
                complete: function () {

                    $('#mainpanel').css('min-width', '45%');
                    if (slidePanelOpen) {
                        slidePanelOpen = false;
                        $('#openpanelbutton').css('display', 'block');
                    }
                    else {
                        slidePanelOpen = true;
                        $('#openpanelbutton').css('display', 'none');
                    }

                    if (actionAfterClosing) {
                        actionAfterClosing();
                    }
                }
            })
    });

    $('#closepanelbutton3').unbind('click').bind('click', function () {
        $('#legend').css('min-width', '');
        $('#legend').animate(
            { width: 'toggle' },
            {
                duration: 500,
                complete: function () {

                    $('#legend').css('min-width', '45%');
                    if (legendPanelOpen) {
                        legendPanelOpen = false;
                        //$('#openpanelbutton3').css('display', 'block');
                    }
                    //else {
                    //    legendPanelOpen = true;
                    //    $('#openpanelbutton3').css('display', 'none');
                    //}
                }
            }
        );
    });
}

function handleResponseIfServerError(responseObject) {
    if (!responseObject)
        return true;

    if (responseObject.SessionExpired) {
        window.location = "/NotAuthorised.aspx";
        return false;
    }

    if (responseObject.ErrorMessage) {
        $.unblockUI();
        var errorMessage = 'An error occurred processing your request: ' + responseObject.ErrorMessage;
        alert(errorMessage);
        return false;
    }

    return true;
}

function validateIdNumberFromService(value, callbackFn) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Please wait...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: 'validate_person_id', IdNumber: value }),
        dataType: "json",
    }).done(function (data) {
        $.unblockUI();
        callbackFn(data);
    });
}


function handleCreateReferral() {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Generating Referral Details...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({
            Instruction: 'generate_referral_details', Target: {
                ContactPerson: {
                    ContactPersonId: currentPersonContact.ContactPersonId
                },
                ProspectingPropertyId: currentProperty.ProspectingPropertyId,
                ContactCompanyId: currentPersonContact.ContactCompanyId
            }
        }),
        dataType: "json",
    }).done(function (data) {
        var summaryDialog = $("#referralsDialog");
        summaryDialog.empty();
        summaryDialog.append("Details of referral to be created:<p /><hr />");
        var deptTypeRadiogroup = $("<div style='display:inline-block' />");
        deptTypeRadiogroup.append($("<input type='radio' name='referralDeptType' id='salesReferral' style='display:inline-block;vertical-align:middle;margin-left:30px' /><label for='salesReferral' style='display:inline-block;vertical-align:middle;margin-right:50px'>Seller</label>"));
        deptTypeRadiogroup.append($("<input type='radio' name='referralDeptType' id='rentalsReferral' style='display:inline-block;vertical-align:middle' /><label for='rentalsReferral' style='display:inline-block;vertical-align:middle'>Landlord</label>"));
        summaryDialog.append("<label style='display:inline-block'>Select Type:</label>");
        summaryDialog.append(deptTypeRadiogroup);
        summaryDialog.append('<br /><hr />');
        var createdBy = "<label class='fieldAlignment'>Referral Created By: </label>" + "<label>" + prospectingContext.LoggedInUsername + "</label>";
        summaryDialog.append(createdBy);
        summaryDialog.append('<br />');
        var propertyDesc = "<label class='fieldAlignment' style='vertical-align:top;margin-top:3px'>Property Description: </label>" + "<label class='fieldAlignmentExtendedWidth' style='margin-top:3px'>" + data.property_desc + "</label>";
        summaryDialog.append(propertyDesc);
        summaryDialog.append('<br />');
        var propertyID = "<label class='fieldAlignment' style='margin-top:3px'>Lightstone Property ID: </label>" + "<label style='margin-top:3px'>" + data.property_id + "</label>";
        summaryDialog.append(propertyID);
        summaryDialog.append('<p />');
        var title = "<label class='fieldAlignment' style='margin-top:3px'>Contact's Title: </label>" + "<label style='margin-top:3px'>" + data.smart_pass_title + "</label>";
        summaryDialog.append(title);
        summaryDialog.append('<br />');
        var name = "<label class='fieldAlignment' style='margin-top:3px'>Contact's Name: </label>" + "<label style='margin-top:3px'>" + data.smart_pass_name + "</label>";
        summaryDialog.append(name);
        summaryDialog.append('<br />');
        var surname = "<label class='fieldAlignment' style='margin-top:3px'>Contact's Surname: </label>" + "<label style='margin-top:3px'>" + data.smart_pass_surname + "</label>";
        summaryDialog.append(surname);
        summaryDialog.append('<br />');
        var idNo = "<label class='fieldAlignment' style='margin-top:3px'>Contact's ID no.: </label>" + "<label style='margin-top:3px'>" + data.smart_pass_id_no + "</label>";
        summaryDialog.append(idNo);
        summaryDialog.append('<br />');
        var selectContactNoLabel = "<label class='fieldAlignment' style='margin-top:3px'>Contact Number: </label>";
        var contactNoSelector = $("<select id='referralPhoneSelector' style='margin-top:3px' />").empty().append("<option />");
        if (currentPersonContact.PhoneNumbers && currentPersonContact.PhoneNumbers.length) {
            $.each(currentPersonContact.PhoneNumbers, function (idx, ph) {
                var itemId = 'ph_' + ph.ItemId;
                // cell - intl dialing code - number - primary?
                var type = $.grep(prospectingContext.ContactPhoneTypes, function (i) {
                    return i.Key == ph.ItemType;
                })[0];
                var dialingCode = $.grep(prospectingContext.IntlDialingCodes, function (i) {
                    return i.Key == ph.IntDialingCode;
                })[0];
                var itemDesc = type.Value + ' (' + dialingCode.Value + ') - ' + ph.ItemContent;
                var option;
                if (ph.IsPrimary) {
                    itemDesc += " - primary";
                    option = $("<option value='" + itemId + "' style='color:red;' >" + itemDesc + "</option>");
                } else {
                    option = $("<option value='" + itemId + "'>" + itemDesc + "</option>");
                }
                contactNoSelector.append(option);
            });

            if (currentPersonContact.PhoneNumbers.length == 1) {
                var value = 'ph_' + currentPersonContact.PhoneNumbers[0].ItemId;
                contactNoSelector.find("option[value='" + value + "']").attr("selected", true);
            }
        }

        var selectEmailLabel = "<label class='fieldAlignment' style='margin-top:3px'>Email Address: </label>";
        var contactEmailSelector = $("<select id='referralEmailSelector' style='margin-top:3px' />").empty().append("<option />");
        if (currentPersonContact.EmailAddresses && currentPersonContact.EmailAddresses.length) {
            $.each(currentPersonContact.EmailAddresses, function (idx, em) {
                var itemId = 'em_' + em.ItemId;
                var type = $.grep(prospectingContext.ContactEmailTypes, function (i) {
                    return i.Key == em.ItemType;
                })[0];
                var itemDesc = type.Value + ' - ' + em.ItemContent;
                var option;
                if (em.IsPrimary) {
                    itemDesc += " - primary";
                    option = $("<option value='" + itemId + "' style='color:red;' >" + itemDesc + "</option>");
                } else {
                    option = $("<option value='" + itemId + "'>" + itemDesc + "</option>");
                }
                contactEmailSelector.append(option);
            });
            if (currentPersonContact.EmailAddresses.length == 1) {
                var value = 'em_' + currentPersonContact.EmailAddresses[0].ItemId;
                contactEmailSelector.find("option[value='" + value + "']").attr("selected", true);
            }
        }

        summaryDialog.append(selectContactNoLabel).append(contactNoSelector);
        summaryDialog.append('<br />');
        if (currentPersonContact.EmailAddresses && currentPersonContact.EmailAddresses.length) {
            summaryDialog.append(selectEmailLabel).append(contactEmailSelector);
            summaryDialog.append('<br />');
        }

        if (data.smart_pass_company) {
            var companyName = "<label class='fieldAlignment' style='margin-top:3px'>Company Name: </label>" + "<label style='margin-top:3px'>" + data.smart_pass_company + "</label>";
            summaryDialog.append(companyName);
        }
        summaryDialog.append('<p />');

        var commentLabel = "<label>Comment:</label>";
        summaryDialog.append(commentLabel);
        summaryDialog.append('<br />');

        var commentBox = $("<textarea id='referralComment' style='width:98%' rows='8' />").empty();
        summaryDialog.append(commentBox);
        summaryDialog.append('<p />');
        //summaryDialog.append("<hr />");

        //
        var referralFollowupContainer = $("<div />");
        var createReferralFollowupOption = $("<label style='display:inline-block;vertical-align:middle;padding-right:10px'><input type='checkbox' id='createReferralFollowupOption' />Create a Follow-up</label>");
        var referralFollowupDate = $("<input type='text' name='referralFollowupDateInput' id='referralFollowupDateInput' style='width:200px;display:inline-block;vertical-align:middle;' readonly='readonly' disabled /><p style='margin:6px;' />");
        referralFollowupDate.datepicker({ dateFormat: 'DD, d MM yy', minDate: 0 });
        referralFollowupContainer.append(createReferralFollowupOption)
                                 .append(referralFollowupDate);
        summaryDialog.append(referralFollowupContainer);
        createReferralFollowupOption.change(function () {
            var checkOption = $('#createReferralFollowupOption').is(':checked');
            if (checkOption) {
                $('#referralFollowupDateInput').prop('disabled', false);
                $('#referralFollowupDateInput').datepicker("show");
            } else {
                $('#referralFollowupDateInput').prop('disabled', true);
                $('#referralFollowupDateInput').datepicker("hide");
                $('#referralFollowupDateInput').datepicker('setDate', null);
            }
        });

        //summaryDialog.append("<hr />");
        summaryDialog.append('<br />');

        var errorDiv = $("<div id='referralErrors' />");
        summaryDialog.append(errorDiv);

        summaryDialog.dialog(
                        {
                            modal: true,
                            closeOnEscape: false,
                            width: '600',
                            buttons: {
                                "Create": function () {
                                    createClick(function () {
                                        $(summaryDialog).dialog("close");
                                        var successDialog = $("<div title='Success' style='font-family: Verdana; font-size: 12px;' />");
                                        successDialog.append("Referral created successfully. A corresponding activity containing a SmartPass link has been added to the property.");
                                        successDialog.dialog(
                                                            {
                                                                modal: true,
                                                                closeOnEscape: true,
                                                                width: '400',
                                                                buttons: {
                                                                    "OK": function () { $(this).dialog("close"); }
                                                                },
                                                                position: ['center', 'center']
                                                            });

                                    }); },
                                "Cancel": function () { $(this).dialog("close"); }
                            },
                            position: ['top', 'top']
                        });

        $.unblockUI();
    });

    function createClick(callback) {       
        var departmentRadioGroup = $('input[name=referralDeptType]').filter(':checked');
        var departmentType = null;
        if (departmentRadioGroup.length) {
            var test = departmentRadioGroup.attr('id');
            switch (test)
            {
                case "salesReferral": departmentType = "S"; break;
                case "rentalsReferral": departmentType = "R"; break;
            }
        }
        var comment = $("#referralComment").val();

        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Creating Referral...</p>' });
        var phoneContactDetailId = $('#referralPhoneSelector').val();
        if (phoneContactDetailId)
            phoneContactDetailId = phoneContactDetailId.replace('ph_', '');

        var emailContactDetailId = $('#referralEmailSelector').val();
        if (emailContactDetailId)
            emailContactDetailId = emailContactDetailId.replace('em_', '');
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({
                Instruction: 'create_referral', Target: {
                    ContactPerson: {
                        ContactPersonId: currentPersonContact.ContactPersonId,
                        PhoneNumbers: [{ ItemId: phoneContactDetailId }],
                        EmailAddresses: [{ ItemId: emailContactDetailId }]
                    },
                    ProspectingPropertyId: currentProperty.ProspectingPropertyId,
                    ContactCompanyId: currentPersonContact.ContactCompanyId
                },
                DepartmentType: departmentType,
                Comment: comment,
                CreateFollowup: $('#createReferralFollowupOption').is(':checked'),
                FollowupDate: $('#referralFollowupDateInput').val()
            }),
            dataType: "json",
        }).done(function (result) {
            $.unblockUI();
            if (result.InstanceValidationErrors && result.InstanceValidationErrors.length) {
                var errorDiv = $('#referralErrors').empty();
                errorDiv.append("<hr />");
                errorDiv.append("The following validation errors occurred. Please fix these or contact support:");
                errorDiv.append('<br />');
                $.each(result.InstanceValidationErrors, function (idx, err) {
                    var errLabel = $("<label style='color:red' />").append(err);
                    errorDiv.append(" - ").append(errLabel);
                    errorDiv.append('<br />');
                });
                return;
            }
            // Success!
            callback();
        });
    }
}