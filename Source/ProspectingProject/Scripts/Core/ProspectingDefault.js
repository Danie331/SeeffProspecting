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

function initEventHandlers() {
    $.contextMenu({
        selector: '.context-menu-prospect',
        build: function ($trigger, e) {
            return {
                callback: function (key, options) {
                    switch (key) {
                        case "Search Lightstone here":
                            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Lightstone results...</p>' });
                            findGoogleAddressAtLatLng(currentClickLatLng, function (data) { buildLightstoneMatchesContent(data); });
                            break;
                        //case "Find Google address here":
                        //    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Google address...</p>' });
                        //    findGoogleAddressAtLatLng(currentClickLatLng, function (data) { buildGoogleAddressContent(data); });
                        //    break;
                        //case "Capture address here":
                        //    buildManualCaptureAddress(null, 'create');
                        //    break;
                    }
                },
                items: {
                    "Search Lightstone here": { name: "Search Lightstone here", icon: "lightstone" },
                    //"Find Google address here": { name: "Find Google address here", icon: "google" }//,
                    //"Capture address here": { name: "Capture address here", icon: "capture" },
                }
            };
        }
    });
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

function findGoogleAddressAtLatLng(currentClickLatLng, actionOnSuccess) {
    var lat = currentClickLatLng.lat();
    var lng = currentClickLatLng.lng();
    // Use reverse geo-coding to try obtain the address..
    var latLngPair = lat + ',' + lng;
    var url = 'https://maps.googleapis.com/maps/api/geocode/json?latlng=' + latLngPair + '&sensor=true&key=AIzaSyDWHlk3fmGm0oDsqVaoBM3_YocW5xPKtwA';
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json"
    }).done(function(data) {actionOnSuccess(data); });
}

function buildLightstoneMatchesContent(data) {

    function getAddress(data) {
        if (data && data.results.length > 0) {
            var suburb = data.results[0].address_components[2].short_name;
            var streetName = data.results[0].address_components[1].long_name;
            streetName = streetName.replace(' Street', '').replace(' Road', '').replace(' Drive', '').replace(' Avenue', '').replace(' Crescent', '').replace(' Way', '').replace(' Lane', '').replace(' Close', '').replace(' Circle', '').trim();
            streetName = streetName.replace(' Straat', '').trim();
            var streetOrUnitNo = data.results[0].address_components[0].short_name;
            return { StreetName: streetName, StreetOrUnitNo: streetOrUnitNo };
        }

        return null;
    }

    var address = getAddress(data);
    // POST the partial address to the server to determine all possible matches
    if (address) {
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({
                Instruction: 'get_matching_addresses',
                StreetName: address.StreetName,
                StreetOrUnitNo: address.StreetOrUnitNo,
                LatLng: { Lat: currentClickLatLng.lat(), Lng: currentClickLatLng.lng() },
                SeeffAreaId: currentSuburb.SuburbId
            }),
            dataType: "json",
        }).done(function (matchesData) {
            var content = generateOutputFromLightstone(matchesData);
            showPopupAtLocation(currentClickLatLng, content);
        });
    }
}

function generateOutputFromLightstone(data) {

    function buildAddress(area, showPropId) {
        if (showPropId) {
            return area.StreetOrUnitNo + ' ' + area.StreetName + ', ' + area.Suburb + ', ' + area.City + ' (PropertyID:' + area.LightstonePropId + ')';
        }        
        return area.StreetOrUnitNo + ' ' + area.StreetName + ', ' + area.Suburb + ', ' + area.City;
    }

    function toggleCreateProspectBtn(lightstoneIdExists) {
        $('#createProspectBtn').attr("disabled", lightstoneIdExists);
    }

    var div = $("<div />");
    if (data.PropertyMatches && data.PropertyMatches.length > 0 && !data.ErrorMessage) {

        if (data.IsSectionalScheme) {
            div.append("The following sectional scheme(s) were found here: " + data.SectionalScheme);
            div.append("<br />");
            div.append("Address: " + buildAddress(data.PropertyMatches[0], false));
            div.append("<br />");
            div.append("Number of units that will be created: " + data.PropertyMatches.length);
            div.append("<p />");

            var disabled = data.SSExists ? 'disabled' : '';
            var createBtn = $('<input type="button" id="createSSProspectBtn" value="Create Sectional Scheme Here" ' + disabled + ' />');
            div.append(createBtn);

            $('body').unbind('click.createSS').on('click.createSS', '#createSSProspectBtn', function () {
                closeInfoWindow();

                // Add each to spiderfier, adjust saving to save "SS" (and loading SS as well), supress "marker click"
                var units = [];
                //var ownedByCompanies = false;
                for (var i = 0; i < data.PropertyMatches.length; i++)
                {
                    var match = data.PropertyMatches[i];
                    var record = newProspectingRecord(match);
                    units.push(record);
                    //if (!ensureAllIDNumbersValidForAllContacts(record)) {
                    //    ownedByCompanies = true;
                    //}
                    //createProspectingRecord(record, false);
                }

                //if (!ownedByCompanies) {
                    createSectionalTitle(units, function () {
                        // Center the map (and offset it to the right to accomodate the panel (if open))
                        var pos = calcMapCenterWithOffset(units[0].LatLng.Lat, units[0].LatLng.Lng, -200, 0);
                        map.setCenter(pos);
                    });
                //}
                //else {
                //    alert('Unable to create sectional title at this address: One or more units are owned by companies.');
                //}
            });
        }
        else {
            div.append("The following addresses were found at this location: <br />");
            div.append("<p/>");
            var form = $('<form id="areaMatchesForm" />');
            form.empty();
            div.append(form);
            $.each(data.PropertyMatches, function (index, area) {
                var radioItemId = 'radioitem_' + index;
                area.ItemId = index;
                var selected = index == 0 ? 'checked' : '';
                var areaRadioItem = $('<input type="radio" name="radio_list" id="' + radioItemId + '" value="' + index + '" ' + selected + '/><label for="' + radioItemId + '">' + buildAddress(area, true) + '</label>');

                //areaRadioItem.append(buildAddress(area, true));
                form.append(areaRadioItem);

                // Add click handler to the radio item
                $('body').unbind('click.' + radioItemId).on('click.' + radioItemId, '#' + radioItemId, function () {
                    var lightstoneIDExists = area.LightstoneIdExists ||
                                    (currentProperty && currentProperty.LightstonePropertyId == area.LightstonePropId);
                    toggleCreateProspectBtn(lightstoneIDExists);
                });

                form.append("<br />");
            });
            form.append('<p />');

            var lightstoneIDExists = data.PropertyMatches[0].LightstoneIdExists ||
                                    (currentProperty && currentProperty.LightstonePropertyId == data.PropertyMatches[0].LightstonePropId);
            var disabledAttr = lightstoneIDExists ? 'disabled' : '';
            var selectorBtn = $('<input type="button" id="createProspectBtn" value="Create Prospect Here" ' + disabledAttr + '/>');
            form.append(selectorBtn);

            $('body').unbind('click.createProspect').on('click.createProspect', '#createProspectBtn', function () {
                var selectedItem = $('input[name="radio_list"]:checked', '#areaMatchesForm').val();
                if (!selectedItem) return;
                var a = $.grep(data.PropertyMatches, function (ar) {
                    return ar.ItemId == selectedItem;
                })[0];

                closeInfoWindow();
                var record = newProspectingRecord(a);
                //if (ensureAllIDNumbersValidForAllContacts(record)) {
                createProspectingRecord(record, true, function () {
                    // Center the map (and offset it to the right to accomodate the panel (if open))
                    var pos = calcMapCenterWithOffset(a.LatLng.Lat, a.LatLng.Lng, -200, 0);
                    map.setCenter(pos);
                });
                //}
                //else {
                //    alert('Unable to create a prospect here at this stage: The property is owned by a company.');
                //}
            });
        }
    }
    else {
        if (data.ErrorMessage) {
            div.append(data.ErrorMessage);
        }
        else {
            div.append("No Lightstone data found for this location.");
        }
    }

    return div;
}

function ensureAllIDNumbersValidForAllContacts(prospectingRecord) {
    // ss
    if (prospectingRecord.Owners && prospectingRecord.Owners.length > 0) {
        for (var i = 0; i < prospectingRecord.Owners.length; i++) {
            var owner = prospectingRecord.Owners[i];
            if (isCKNumber(owner.IdNumber)) {
                return false;
            }
        }
    }

    if (prospectingRecord.Contacts && prospectingRecord.Contacts.length > 0) {
        for (var i = 0; i <  prospectingRecord.Contacts.length; i++) {
            var contact = prospectingRecord.Contacts[i];
            if (isCKNumber(contact.IdNumber)) {
                return false;
            }
        }
    }

    return true;
}

function buildGoogleAddressContent(data) {
    var div = $("<div />");
    div.append("The following address was obtained from Google for the location clicked:");
    div.append("<p />");
    
    if (data && data.results.length > 0) {
        div.append(data.results[0].formatted_address);
        div.append("<p/>");

        //var captureAddressBtn = $("<input type='button' id='captureGoogleAddressBtn' value='Capture Address' />");
        //$('body').unbind('click.gotoCaptureScreen').on('click.gotoCaptureScreen', '#captureGoogleAddressBtn', function () {
        //    var content = buildManualCaptureAddress(data, 'create');
        //    infowindow.setContent(content[0].outerHTML);
        //});
        //div.append(captureAddressBtn);
    }
    else {
        div.append("No results found.");
    }

    showPopupAtLocation(currentClickLatLng, div);
}

function buildManualCaptureAddress(data, action) {

    function validateForm() {

        var streetNo = $('#streetNoInput').val();
        var streetName = $('#streetNameInput').val();
        var suburbName = $('#suburbNameInput').val();
        var cityTown = $('#cityTownInput').val();
        return streetNo.length > 0 && streetName.length > 0 && suburbName.length > 0 && cityTown.length > 0;
    }

    var div = $("<div class='info-window' />");
    div.append("Captured address:");
    div.append("<p/>");
    div.append("<label style='display:inline-block;width:100px;'>Street no.:</label><input type='text' id='streetNoInput' size='30' />");
    div.append("<br/>");
    div.append("<label style='display:inline-block;width:100px;'>Street name:</label><input type='text' id='streetNameInput' size='30' />");
    div.append("<br />");
    div.append("<label style='display:inline-block;width:100px;'>Suburb:</label><input type='text' id='suburbNameInput' size='30' />");
    div.append("<br />");
    div.append("<label style='display:inline-block;width:100px;'>City or Town:</label><input type='text' id='cityTownInput' size='30' />");
    div.append("<p />");

    var actionBtnName = '';
    switch (action) {
        case "create": actionBtnName = 'Create Prospect';
            break;
        case "update": actionBtnName = 'Update';
            break;
    }
    var createProspectBtn = $("<input type='button' id='captureNewAddressBtn' value='" + actionBtnName + "' />");
    $('body').unbind('click.createProspectFromAddress').on('click.createProspectFromAddress', '#captureNewAddressBtn', function () {
        if (validateForm()) {
            var streetNoInput = $('#streetNoInput').val();
            var streetNameInput = $('#streetNameInput').val();
            var suburbNameInput = $('#suburbNameInput').val();
            var cityTownInput = $('#cityTownInput').val();

            if (action == 'create') {
                var recordForInsert = {
                    PropertyAddress: streetNameInput + ', ' + suburbNameInput + ', ' + cityTownInput,
                    StreetOrUnitNo: streetNoInput,
                    LatLng: { Lat: currentClickLatLng.lat(), Lng: currentClickLatLng.lng() },
                    LightstonePropertyId: null,
                    LightstoneIDOrCKNo: null
                };

                createProspectingRecord(recordForInsert);
                closeInfoWindow();
            }
            else if (action == 'update') {
                // At this stage can only update the address portion og the property.
                var recordForUpdate = {
                    PropertyAddress: streetNameInput + ', ' + suburbNameInput + ', ' + cityTownInput,
                    StreetOrUnitNo: streetNoInput,
                    ProspectingPropertyId: currentProperty.ProspectingPropertyId
                };

                updateProspectingRecord(recordForUpdate);
            }
        } else {
            alert("Some details are missing.");
        }
    });
    //div.append(createProspectBtn);
    if (data) {
        var packet = data;
        if (data.results && data.results.length > 0) {
            var handle = data.results[0].address_components;
            packet.StreetOrUnitNo = handle[0].long_name;
            packet.StreetName = handle[1].long_name;
            packet.Suburb = handle[3].long_name;
            packet.CityTown = handle[4].long_name;
        }
        
        div.find('#streetNoInput').attr('value', packet.StreetOrUnitNo);
        div.find('#streetNameInput').attr('value', packet.StreetName);
        div.find('#suburbNameInput').attr('value', packet.Suburb);
        div.find('#cityTownInput').attr('value', packet.CityTown);
        return div;
    }
    else {
        showPopupAtLocation(currentClickLatLng, div);
    }
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

function performPersonLookup(idNumber, checkForExisting) {

    if (checkForExisting) {
        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Looking for existing contact...</p>' });
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({ Instruction: 'check_for_existing_contact', PhoneNumbers: [], EmailAddresses: [], IdNumber: idNumber }),
            dataType: "json",
        }).done(function (contact) {
            $.unblockUI();
            if (contact && contact.PropertiesOwned && contact.PropertiesOwned.length > 0) {
                var propertiesOtherThanCurrentProp = $.grep(contact.PropertiesOwned, function (p) {
                    return p.LightstonePropertyId != currentProperty.LightstonePropertyId;
                });

                if (propertiesOtherThanCurrentProp.length > 0) {
                    showDialogExistingContactFound(contact, function ()
                    {
                        currentPersonContact = contact;
                        saveContact(currentPersonContact, currentProperty, function (data) {
                            //alert('Contact saved successfully!');
                            showSavedSplashDialog('Contact Saved!');       
                            addOrUpdateContactToCurrentProperty(data);
                            buildPersonContactMenu(currentProperty.Contacts, false);

                            if (currentPersonContact.IsPOPIrestricted) {
                                // If the popi option was selected, then delete all their contact info
                                handleSaveContactDetails([], []);
                            }

                            //This function is no longer needed as the details are fetched from the server 
                            //when the user clicks on the property in a suburb. 2014-10-08
                            //updateSuburbContactsList();

                            //if (saveDetailsFunction) {
                            //    saveDetailsFunction();
                            //}
                        });
                    });
                }
                else {
                    doLookup();
                }
            }
            else {
                doLookup();
            }
        });
    } else {
        doLookup();
    }

    function doLookup() {
        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Performing Enquiry...</p>' });
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({ Instruction: "get_prop_owner_details", LightstoneIDOrCKNo: idNumber, ProspectingPropertyId: currentProperty.ProspectingPropertyId }),
            success: function (data, textStatus, jqXHR) {
                $.unblockUI();
                if (textStatus == "success" && data) {
                    if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                        alert(data.ErrorMsg);
                    }
                    currentMarker.ContactInfoPacket = data;
                    if (data.AvailableTracePsCredits != null) {
                        // If the AvailableTracePsCredits is not null, then update the availableCredit variable
                        availableCredit = data.AvailableTracePsCredits;
                        $('#availableCreditLabel').text(availableCredit);
                    }
                    //updatePropertyInfoMenu(data);
                    //updateOwnerDetailsEditorWithBrandNewContact(currentTracePSInfoPacket, currentTracePSContactRows);
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
}

function getDetailsForProperty(prospectingProperty) {

    var prospectingPropId = prospectingProperty.ProspectingPropertyId;
    var lightstonePropID = prospectingProperty.LightstonePropertyId;
    var lightstoneIdOrCkNo = prospectingProperty.LightstoneIDOrCKNo;
    // If attemtping to do a search on a manually entered ID number, use it instead.
    // In this case we must first validate the ID number and proceed only if valid..
    var manuallyEnteredID = $('#knownIdTextbox').val();
    if (manuallyEnteredID.length > 0) {
        if (validIDNumber(manuallyEnteredID)) {
            lightstoneIdOrCkNo = manuallyEnteredID;
        }
        else {
            $.unblockUI();
            alert('The ID number you are trying to search on is not valid.');
            return;
        }
    }

    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_prop_owner_details", ProspectingPropertyId: prospectingPropId, LightstoneId: lightstonePropID, LightstoneIDOrCKNo: lightstoneIdOrCkNo }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }
                currentMarker.ContactInfoPacket = data;
                if (data.AvailableTracePsCredits != null) {
                    // If the AvailableTracePsCredits is not null, then update the availableCredit variable
                    availableCredit = data.AvailableTracePsCredits;
                }
                updatePropertyInfoMenu(data);               
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

function loadSuburb(suburbId,showSeeffCurrentListings, actionAfterLoad) {

    var suburb = getSuburbById(suburbId);
    if (suburb == null) {
        suburb = newSuburb();
        suburb.SuburbId = suburbId;
    }

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading ' + suburb.SuburbName + '...</p>' });
    if (!suburb.IsInitialised) {
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({ Instruction: "load_suburb", SuburbId: suburbId }),
            success: function (data, textStatus, jqXHR) {
                if (textStatus == "success" && data.PolyCoords.length > 0) {
                    initialiseAndDisplaySuburb(suburb, data, showSeeffCurrentListings);
                    centreMap(suburb);
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
        centreMap(suburb);
        $.unblockUI();
        if (actionAfterLoad) {
            actionAfterLoad();
        }
    }
}

function setCurrentMarker(suburb, property) {
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_existing_prospecting_property", PropertyId: property.ProspectingPropertyId }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }
                testpp = data;

                function changeBgColour(id, colour) {
                    var id = "#unit" + id;
                    var row = $(id);
                    row.data('color', colour)
                    row.css('background-color', 'lightblue');
                };

                $.each(suburb.ProspectingProperties, function (idx, pp) {
                    if (pp.ProspectingPropertyId == testpp.ProspectingPropertyId) {
                        if (testpp.Prospected) {
                            if (pp.SS_FH == "FH") {
                                pp.Marker.setIcon('Assets/marker_icons/prospecting/prospected.png');
                            } else {
                                ////"#009900" : "#FBB917";
                                //var id = "unit" + id;
                                //var newtr = ""; //buildUnitContentRow(pp)
                                //$(id).parent().replaceWith(newtr);
                                changeBgColour(testpp.LightstonePropertyId, "#009900");
                            };
                        }
                        else {
                            if (pp.SS_FH == "FH") {
                                pp.Marker.setIcon('Assets/marker_icons/prospecting/unprospected.png');
                            } else {
                                ////var id = "#unit" + testpp.LightstonePropertyId;
                                ////var row = $(id);
                                ////row.css('background-color', "#FBB917");
                                //var id = "unit" + id;
                                //var newtr = ""; //buildUnitContentRow(pp)
                                //$(id).parent().replaceWith(newtr);
                                changeBgColour(testpp.LightstonePropertyId, "#FBB917");
                            };
                        };
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
    
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_existing_prospecting_property", PropertyId: marker.ProspectingProperty.ProspectingPropertyId }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }
                
                marker.ProspectingProperty = null;

                marker.ProspectingProperty = data;
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

function loadProspectingProperty(marker) {
    openInfoWindow(marker, function () {
        if (marker.ProspectingProperty.SS_FH == "SS") {
            currentProperty = null;
        }
        else {
            currentProperty = marker.ProspectingProperty;
            updateOwnerDetailsEditor();
            updatePropertyNotesDiv();
            if (currentProperty.Contacts.length || currentProperty.ContactCompanies.length) {
                showMenu("contactdetails");
            }
        }

        updatePropertyInfoMenu();
    });
}


function prepareUnits(units) {
    for (var u=0;u<units.length;u++) {        
        units[u].ContactPersons = getContactPersons(units[u]);
        units[u].ContactCompanies = getContactCompanies(units[u]);
    }
    return units;
}

function createSectionalTitle(units, actionWhenDone) {
    var seeffAreaId = currentSuburb ? currentSuburb.SuburbId : null;

    units = prepareUnits(units);
    var inputPacket = {
        Instruction: "create_sectional_title",
        Units: units,
        SeeffAreaId: seeffAreaId
    };

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Creating sectional title...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify(inputPacket),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success") {
                if (data[0].CreateError) {
                    alert(data[0].CreateError);
                } else {
                    units = data;
                    var suburb = $.grep(suburbsInfo, function (s) {
                        return s.SuburbId == units[0].SeeffAreaId;
                    })[0];

                    //  Create a unique identifier by which this SS will be identified
                    //var ss_id = generateUniqueID();
                    if (suburb != currentSuburb) {
                        if (currentSuburb) {
                            clearSuburbBySuburbId(currentSuburb.SuburbId);
                        }
                        //Load for SS
                        loadSuburb(suburb.SuburbId, null, function () {           
                            currentProperty = $.grep(suburb.ProspectingProperties, function (p) {
                                return p.LightstonePropertyId == units[0].LightstonePropertyId;
                            })[0];
                            if (!currentProperty) {
                                for (var u = 0; u < units.length; u++) {
                                    var unit = units[u];
                                    //unit.ss_id = ss_id;

                                    suburb.ProspectingProperties.push(unit);
                                    var marker = createMarkerForProperty(unit);
                                    marker.Suburb = suburb;
                                    suburb.Markers.push(marker);
                                    suburb.MarkerClusterer.addMarker(marker);
                                    suburb.VisibleMarkers.push(marker);
                                    updateSuburbSelectionStats(suburb);
                                    unit.Marker.setIcon(getIconForMarker(unit.Marker));
                                    //enableSpiderfier(suburb, [marker]);                                
                                    currentProperty = unit;
                                }
                            }
                            new google.maps.event.trigger(currentProperty.Marker, 'click');
                            if (actionWhenDone) {
                                actionWhenDone();
                            }
                        });
                    }
                    else {
                        for (var u = 0; u < units.length; u++) {
                            var unit = units[u];
                            //unit.ss_id = ss_id;

                            suburb.ProspectingProperties.push(unit);
                            var marker = createMarkerForProperty(unit);
                            marker.Suburb = suburb;
                            suburb.Markers.push(marker);
                            suburb.MarkerClusterer.addMarker(marker);
                            suburb.VisibleMarkers.push(marker);
                            updateSuburbSelectionStats(suburb);
                            unit.Marker.setIcon(getIconForMarker(unit.Marker));
                            enableSpiderfier(suburb, [marker]);                            
                            currentProperty = unit;
                        }
                        new google.maps.event.trigger(currentProperty.Marker, 'click');
                        if (actionWhenDone) {
                            actionWhenDone();
                        }
                    }
                }
            } else {
                alert('Could not create sectional scheme. Please contact support.');
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

function getContactPersons(property) {
    return $.grep(property.Owners, function (o) {
        return o.ContactEntityType == 1;
    });
}

function getContactCompanies(property) {
    return $.grep(property.Owners, function (o) {
        return o.ContactEntityType == 2;
    });
}

function createProspectingRecord(property, showLinkedOwnershipDialog, actionWhenDone) {

    var seeffAreaId = currentSuburb ? currentSuburb.SuburbId : null;
    var inputPacket = {
        Instruction: "create_new_prospecting_property", LightstoneId: property.LightstonePropertyId, PropertyAddress: property.PropertyAddress,
        StreetOrUnitNo: property.StreetOrUnitNo, LatLng: property.LatLng, LightstoneIDOrCKNo: property.LightstoneIDOrCKNo,
        LightstoneRegDate: property.LightstoneRegDate, SeeffAreaId: seeffAreaId, LastPurchPrice: property.LastPurchPrice,
        ErfNo: property.ErfNo,

        // Sectional Schemes
        SS_FH: property.SS_FH,
        SSName: property.SSName,
        SSNumber: property.SSNumber,
        Unit: property.Unit,
        SS_ID: property.SS_ID,

        Owners: property.Owners, //remove
        ContactPersons: getContactPersons(property),
        ContactCompanies: getContactCompanies(property)
    };

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Creating new prospect...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify(inputPacket),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success") {
                if (data.CreateError) {
                    alert(data.CreateError);
                } else {            
                    property = data;
                    var suburb = $.grep(suburbsInfo, function (s) {
                        return s.SuburbId == property.SeeffAreaId;
                    })[0];
                    if (!suburb) {
                        alert("The prospecting property you created falls outside of your available suburbs!");
                        return;
                    }
                    if (suburb != currentSuburb) {
                        if (currentSuburb) {
                            clearSuburbBySuburbId(currentSuburb.SuburbId);
                        }
                        //load for FH
                        loadSuburb(suburb.SuburbId, null, function ()
                        {                          
                            currentProperty = $.grep(suburb.ProspectingProperties, function (pp) {
                                return pp.LightstonePropertyId == property.LightstonePropertyId;
                            })[0];
                            if (!currentProperty) {
                                suburb.ProspectingProperties.push(property);
                                var marker = createMarkerForProperty(property);
                                marker.Suburb = suburb;
                                suburb.Markers.push(marker);
                                suburb.MarkerClusterer.addMarker(marker);
                                suburb.VisibleMarkers.push(marker);
                                updateSuburbSelectionStats(suburb);
                                property.Marker.setIcon(getIconForMarker(property.Marker));
                                //enableSpiderfier(suburb, [marker]);
                                currentProperty = property;
                            }
                            new google.maps.event.trigger(currentProperty.Marker, 'click');

                            if (showLinkedOwnershipDialog) {
                                linkAndShowExistingOwners(property);
                            }

                            if (actionWhenDone) {
                                actionWhenDone();
                            }
                        });
                    }
                    else {
                        suburb.ProspectingProperties.push(property);
                        var marker = createMarkerForProperty(property);
                        marker.Suburb = suburb;
                        suburb.Markers.push(marker);
                        suburb.MarkerClusterer.addMarker(marker);
                        suburb.VisibleMarkers.push(marker);
                        updateSuburbSelectionStats(suburb);
                        property.Marker.setIcon(getIconForMarker(property.Marker));
                        //enableSpiderfier(suburb, [marker]);

                        new google.maps.event.trigger(marker, 'click');
                        currentProperty = property;

                        if (showLinkedOwnershipDialog) {
                            linkAndShowExistingOwners(property);
                        }

                        if (actionWhenDone) {
                            actionWhenDone();
                        }
                    }
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
                actionToExecuteAfterwards(data);
            }
        },
        error: function (textStatus, errorThrown) { },
        dataType: "json"
    });
}

function handleSavePropertyNotesComments() {
    if (currentProperty) {
        var comments = $('#propertyNotesCommentsTextArea').val();
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({ Instruction: 'save_property_notes', ProspectingPropertyId: currentProperty.ProspectingPropertyId, CommentsNotes: comments }),
            dataType: "json",
        }).done(function () {
            currentProperty.Comments = comments;
            showSavedSplashDialog('Comments Saved!');
        });
    }
}

function addOrUpdateContactToCurrentProperty(newContact) {

    var contactExists = $.grep(currentProperty.Contacts, function (c) {
        return c.ContactPersonId == newContact.ContactPersonId;
    });

    if (contactExists.length == 0) {
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

function updatePropertyNotesDiv() {
    var propNotesDiv = $('#propertyNotesDiv');
    propNotesDiv.css('display', currentProperty ? 'block' : 'none');

    if (currentProperty) {
        var notesContent = currentProperty && currentProperty.Comments != null ? currentProperty.Comments : "Add notes for this property here.";
        $('#propertyNotesCommentsTextArea').val(notesContent);
    }
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
        //This function is no longer needed as the details are fetched from the server 
        //when the user clicks on the property in a suburb. 2014-10-08. This record was 
        //already commented out by Danie
        //updateSuburbContactsList();
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

function getMarkersThatWillSpiderfyWithProp(suburb, ssUnits) {
    var markersWillSpiderfy = [];
    var sub
    for(var i=0;i<suburb.Markers.length;i++) {
        var marker = suburb.Markers[i];
        if ($.inArray(marker.LightstonePropertyId, ssUnits) > -1) {
            markersWillSpiderfy.push(marker);
        }
    }

    return markersWillSpiderfy;
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
        if (property.SS_FH == "SS") {
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

function allUnitsFullyProspected(units) {
    for (var u = 0; u < units.length; u++) {
        var unit = units[u];
        if (!hasAnyContactDetails(unit.ProspectingProperty)) {
            return false;
        }
    }

    return true;
}
/* Remove */
function hasAnyContactDetails(property) {

    function contactHasDetails(contact) {
        if (!contact.PhoneNumbers && !contact.EmailAddresses) return false;
        if ((contact.PhoneNumbers && contact.PhoneNumbers.length == 0) && (contact.EmailAddresses && contact.EmailAddresses.length == 0)) return false;

        return true;
    }

    if (!property.Contacts || property.Contacts.length == 0) return false;

    for (var c = 0; c < property.Contacts.length; c++) {
        var contact = property.Contacts[c];
        if (contactHasDetails(contact)) {
            return true;
        }
    }

    return false;
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

    return marker;
}

function buildInfoWindowContentForSS(unit) {

    function buildUnitContentRow(unit) {

        function getBGColourForRow(unit) {
            //if (unit.Contacts && unit.Contacts.length) {
            //    hasContactsWithDetails = $.grep(unit.Contacts, function (c) {
            //        return (c.PhoneNumbers && c.PhoneNumbers.length) || (c.EmailAddresses && c.EmailAddresses.length);
            //    }).length > 0;
            //}

            //Changed to work with Prospected 2014-10-07
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

        var unitRegDate = 'Reg date: ' + formatDate(unit.LightstoneRegDate);
        var unitLastSalePrice = 'Last sale price: ' + formatRandValue(unit.LastPurchPrice);
        var ssDoorNo = unit.SSDoorNo ? ' (Door no. ' + unit.SSDoorNo + ')' : '';
        var unitContent = "Unit " + unit.Unit + ssDoorNo + '<br />' +
                           unitRegDate + '<br />' +
                           unitLastSalePrice;
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

        $('body').unbind('click.' + id).on('click.' + id, '#' + id, function () {
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
        });

        return tr;
    }

    // First find all units that this unit belongs with
    // Going forward we must use a unique identifer for this SS that is set at creation
    // Backward compatability: if this identifer is not present then we must revert to using the SSName
    var ssUnits = [];
    ssUnits = $.grep(currentSuburb.ProspectingProperties, function (pp) {
        if (!pp.SS_ID) return false;
        return pp.SS_ID == unit.SS_ID;
    });
    if (ssUnits.length == 0) { // backwards compatibility
        ssUnits = $.grep(currentSuburb.ProspectingProperties, function (pp) {
            return pp.SSNumber == unit.SSNumber;
        });
    }
    ssUnits.sort(function (x, y) { return x.Unit - y.Unit;});
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
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_existing_prospecting_property", PropertyId: unit.ProspectingPropertyId }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }

                unit = null;
                unit = data;

                currentProperty = unit;
                updateOwnerDetailsEditor();
                updatePropertyNotesDiv();
                if (currentProperty.Contacts && currentProperty.Contacts.length > 0) {
                    showMenu("contactdetails");
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

function buildContentForInfoWindow(property) {
    var div = $("<div class='info-window' />");   
    // test for fh, test in chrome, mouille point, sort, fixed header
    if (property.SS_FH == 'SS') {
        var header = $("<label>" + property.SSName + "</label>");
        div.append(header);
        div.append("<br />");
        var ss = buildInfoWindowContentForSS(property);
        div.append(ss);
        //div.append("<br />");
        //div.append("Unit " + property.Unit + " in " + property.SSName + " (SS number: " + property.SSNumber + ")");
        //div.append("<br />");
        //div.append("Lightstone Property ID: " + property.LightstonePropertyId);
        //div.append("<br />");
        //div.append("Lightstone Reg. Date: " + property.LightstoneRegDate);
        //div.append("<br />");
        //div.append("Erf No.: " + (property.ErfNo ? property.ErfNo : "n/a"));
        //div.append("<br />");
        //div.append("Last sale price: " + (property.LastPurchPrice ? formatRandValue(property.LastPurchPrice) : "n/a"));
    }
    else { // FH
        var address = property.StreetOrUnitNo + " " + property.PropertyAddress;
        div.append(address);

        var contacts = "Number of contacts captured: " + property.Contacts.length;
        div.append("<br />");
        div.append(contacts);
        if (property.LightstoneRegDate) {
            div.append("<br />");
            div.append("Reg. date (from Lightstone): " + formatDate(property.LightstoneRegDate));
        }
        div.append("<br />");
        div.append("Erf No.: " + (property.ErfNo ? property.ErfNo : "n/a"));
        div.append("<br />");
        div.append("Last sale price: " + (property.LastPurchPrice ? formatRandValue(property.LastPurchPrice) : "n/a"));

        // Append the owner info
        div.append("<br />");
        div.append("Property Contacts:");
        div.append("<br />");
        if (property.Contacts.length) {
            $.each(property.Contacts, function (idx, c) {

                //var relationshipToProp = $.grep(c.PersonPropertyRelationships, function (rel) {
                //    return rel.Key == property.ProspectingPropertyId;
                //})[0];
                
                //var isOwner = relationshipToProp ? (relationshipToProp.Value == 1) : null;  // TODO: do this properly!!!!!!!
                //if (isOwner) {
                    div.append(c.Firstname + " " + c.Surname + " (ID Number:" + c.IdNumber + ")");
                    div.append("<br />");
                //}
            });
        }
        if (property.ContactCompanies.length) {
            $.each(property.ContactCompanies, function (idx, c) {
                div.append(c.CompanyName + " (" + c.CKNumber + ")");
                div.append("<br />");
            });
        }

        // Edit functionality
        //var editAddressBtn = $("<input type='button' id='editAddressBtn' value='View Full Address..' />");
        //$('body').unbind('click.updatePropertyAddress').on('click.updatePropertyAddress', '#editAddressBtn', function () {
        //    var content = buildManualCaptureAddress(stripPropertyAddress(property), 'update');
        //    infowindow.setContent(content[0].outerHTML);
        //});

        //div.append("<p />");
        //div.append(editAddressBtn);
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
        updatePropertyNotesDiv();

        currentTracePSInfoPacket = null;
        currentTracePSContactRows = null;
    }
}

function tryGetContactInfoForProperty() {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Performing Enquiry...</p>' });
    getDetailsForProperty(currentProperty);
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


function searchContacts() {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Searching for matching properties...</p>' });

    var idNumber = $('#idNumberSearchTextbox').val();
    var phoneNumber = $('#phoneNumberSearchTextbox').val();
    var emailAddress = $('#emailAddressSearchTextbox').val();
    var propertyAddress = $('#propertyAddressSearchTextbox').val();
    var streetOrUnitNo = $('#streetOrUnitNoSearchTextbox').val();

    var currentSuburbId = currentSuburb ? currentSuburb.SuburbId : null;

    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({
            Instruction: 'search_for_matches',
            CurrentSuburbId: currentSuburbId,
            IDNumber: idNumber,
            PhoneNumber: phoneNumber,
            EmailAddress: emailAddress,
            PropertyAddress: propertyAddress,
            StreetOrUnitNo: streetOrUnitNo
        }),
        dataType: "json",
    }).done(function (data) {
        $.unblockUI();
        if (data.MatchingPropertiesOutsideCurrentSuburb.length == 0 && data.ProspectingProperties.length == 0) {            
            showSavedSplashDialog('No matches found');
        }
        else {
            if (data.ProspectingProperties.length > 0) {              
                initCurrentSearchResults(data.ProspectingProperties);
            }

            if (data.MatchingPropertiesOutsideCurrentSuburb.length > 0) {
                showDialogOtherResults(data.MatchingPropertiesOutsideCurrentSuburb);
            }
            // TODO: other matches + no current suburb + click searched marker
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

function performLightstoneSearch() {

    function atLeastOneFieldPopulated() {
        return deedTown.length > 0 || suburb.length > 0 || streetName.length > 0 || streetNo.length > 0 || complexName.length > 0 || erfNo.length > 0 || estateName.length > 0;
    }

    var deedTown = $('#deedTownInput').val().trim();
    var suburb = $('#suburbInput').val().trim();
    var streetName = $('#streetNameInput').val().trim();
    var streetNo = $('#streetNoInput').val().trim();
    var complexName = $('#complexNameInput').val().trim();
    var erfNo = $('#erfNoInput').val().trim();
    var estateName = $('#estateNameInput').val().trim();

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
                    EstateName: estateName
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

    var content = generateOutputFromLightstone(result);
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
