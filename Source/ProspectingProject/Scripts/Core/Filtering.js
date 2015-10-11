var filterMode = false;

var currentSuburbForFiltering = null;

function buildFilteringMenu() {
    var div = $("<div class='contentdiv' id='filteringDiv' style='display:none' />");
    div.empty();

    var filteringSticker = $("#filterMode");
    var mapControl = map.controls[google.maps.ControlPosition.TOP_RIGHT];
    mapControl.push(filteringSticker[0]);

    return div;
}

function toggleFilteringMenu() {
    var container = $("#filteringDiv");
    if (currentSuburb == null) {
        container.empty();
        return;
    }
    if (currentSuburbForFiltering != currentSuburb) {
        currentSuburbForFiltering = currentSuburb;
        container.empty();

        var expander = buildContentExpanderForFiltering();
        container.append(expander.construct());
        expander.open('filterByContactDetailsTab');
    }
    container.css('display', 'block');
}

function buildContentExpanderForFiltering() {
    $('#filterByContactDetailsTab').empty();
    var filterByContactDetailsTab = buildContentExpanderItem('filterByContactDetailsTab', 'Assets/contact.png', "Filter By Contact Details", buildContactsFilterTab());

    $('#filterByPropertyDetailsTab').empty();
    var filterByPropertyDetailsTab = buildContentExpanderItem('filterByPropertyDetailsTab', 'Assets/fh_edit.png', "Filter By Property Details", buildPropertyDetailsFilterTab());

    return new ContentExpanderWidget('#contentarea', [filterByContactDetailsTab, filterByPropertyDetailsTab], "filteringExpander");
}

function buildPropertyDetailsFilterTab() {
    var container = $("<div />");

    var containerFieldset = $("<fieldset style='border:1px solid gray'></fieldset>");
    containerFieldset.append("<legend style='padding: 0.2em 0.5em; border:1px solid gray; color:gray;font-size:90%;'>Filtering Options</legend>");

    var haveRadio = $("<label><input id='propertiesFilterHave' type='radio' name='propertiesFilterSelector' checked />Show properties that <i>are</i></label>");
    var doNotHaveRadio = $("<label><input id='propertiesFilterDoNotHave' type='radio' name='propertiesFilterSelector' />Show properties that <i>are not</i></label>");

    var inclusiveFilter = $("<label><input id='inclusiveFilterOption2' type='radio' name='inclusiveExclusiveOptions2' checked />Inclusive (matches <i>any</i> of the criteria specified)</label>");
    var exclusiveFilter = $("<label><input id='exclusiveFilterOption2' type='radio' name='inclusiveExclusiveOptions2' />Exclusive (matches <i>all</i> of the criteria specified)</label>");

    var fhFilter = $("<label class='fieldAlignmentExtraShortWidth'>Free-hold</label><input type='checkbox' id='fhFilterInput' checked />");
    var ssFilter = $("<label class='fieldAlignmentShortWidth'>Sectional Title</label><input type='checkbox' id='ssFilterInput' checked />");

    var newRegistrationsFilter = $("<label class='fieldAlignment'>New Registrations</label><input type='checkbox' id='newRegistrationsFilterInput' checked />");

    var shortTermRentalFilter = $("<label class='fieldAlignment'>Short-term Rental</label><input type='checkbox' id='shortTermRentalFilterInput' checked />");
    var longTermRentalFilter = $("<label class='fieldAlignment'>Long-term Rental</label><input type='checkbox' id='longTermRentalFilterInput' checked />");
    var agriFilter = $("<label class='fieldAlignment'>Agricultural</label><input type='checkbox' id='agriFilterInput' checked />");
    var commFilter = $("<label class='fieldAlignment'>Commercial</label><input type='checkbox' id='commFilterInput' checked />");
    var investmentFilter = $("<label class='fieldAlignment'>Investment</label><input type='checkbox' id='investmentFilterInput' checked />");

    containerFieldset
    .append(fhFilter)
    .append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
    .append(ssFilter)
    .append("<p />")
    .append(haveRadio)
   .append("<br />")
   .append(doNotHaveRadio)
   .append("<p />")
   .append(inclusiveFilter)
   .append("<br />")
   .append(exclusiveFilter);

    var performFilteringBtn = $("<input type='button' id='performFilteringBtn2' value='Filter' style='cursor:pointer;display:inline-block;float:left' />");
    var resetSuburbFilterBtn = $("<input type='button' id='resetSuburbFilterBtn2' value='Refresh & Reload Suburb' style='cursor:pointer;display:inline-block;float:right' />");

    container
        .append(containerFieldset)
        .append("<p />")
        .append(newRegistrationsFilter)
        .append("<hr />")
        .append(shortTermRentalFilter)
        .append("<br />")
        .append(longTermRentalFilter)
        .append("<br />")
        .append(agriFilter)
        .append("<br />")
        .append(commFilter)
        .append("<br />")
        .append(investmentFilter)
                .append("<hr />")
        .append("<p />")
        .append(performFilteringBtn).append(resetSuburbFilterBtn);

    performFilteringBtn.click(handleFilterPropertiesByPropertyDetails);
    resetSuburbFilterBtn.click(function () { handleResetSuburbFiltering(true); });

    return container;
}

function handleFilterPropertiesByPropertyDetails() {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Filtering...</p>' });
    handleResetSuburbFiltering(false);
    loadSuburbAndFilterByPropertyDetails();
}

function buildContactsFilterTab() {
    var container = $("<div />");

    var intro = $("<span>These filters apply to the currently active suburb. For sectional titles, matching units will be high-lighted in <label style='color:blue'>BLUE</label>.</span>");

    var email = $("<label class='fieldAlignment'>Email address</label><input type='checkbox' id='emailFilterInput' />");
    var cell = $("<label class='fieldAlignment'>Cell no.</label><input type='checkbox' id='cellFilterInput' />");
    var landline = $("<label class='fieldAlignment'>Landline no.</label><input type='checkbox' id='landlineFilterInput' />");
    var primaryEmail = $("<label class='fieldAlignment'>Primary email address</label><input type='checkbox' id='primaryEmailFilterInput' />");
    var primaryCell = $("<label class='fieldAlignment'>Primary cell no.</label><input type='checkbox' id='primaryCellFilterInput' />");
    var primaryLandline = $("<label class='fieldAlignment'>Primary landline no.</label><input type='checkbox' id='primaryLandlineFilterInput' />");

    var containerFieldset = $("<fieldset style='border:1px solid gray'></fieldset>");
    containerFieldset.append("<legend style='padding: 0.2em 0.5em; border:1px solid gray; color:gray;font-size:90%;'>Filtering Options</legend>");

    var haveRadio = $("<label><input id='contactsFilterHave' type='radio' name='contactDetailFilterSelector' checked />Show properties with contacts <i>having</i></label>");
    var doNotHaveRadio = $("<label><input id='contactsFilterDoNotHave' type='radio' name='contactDetailFilterSelector' />Show properties with contacts who <i>don't have</i></label>");

    var inclusiveFilter = $("<label><input id='inclusiveFilterOption' type='radio' name='inclusiveExclusiveOptions' checked />Inclusive (matches <i>any</i> of the criteria specified)</label>");
    var exclusiveFilter = $("<label><input id='exclusiveFilterOption' type='radio' name='inclusiveExclusiveOptions' />Exclusive (matches <i>all</i> of the criteria specified)</label>");

    containerFieldset.append(haveRadio)
    .append("<br />")
    .append(doNotHaveRadio)
    .append("<p />")
    .append(inclusiveFilter)
    .append("<br />")
    .append(exclusiveFilter);

    var performFilteringBtn = $("<input type='button' id='performFilteringBtn' value='Filter' style='cursor:pointer;display:inline-block;float:left' />");
    var resetSuburbFilterBtn = $("<input type='button' id='resetSuburbFilterBtn' value='Refresh & Reload Suburb' style='cursor:pointer;display:inline-block;float:right' />");

    container
        .append(containerFieldset)
    .append("<p />")    
    .append(cell)
        .append("<br />")
    .append(landline)
        .append("<br />")
        .append(email)
        .append("<br /><hr />")
    .append(primaryCell)
        .append("<br />")
    .append(primaryLandline)
    .append("<br />")
    .append(primaryEmail)
            .append("<br /><hr />")
    .append(performFilteringBtn).append(resetSuburbFilterBtn);

    performFilteringBtn.click(handleFilterPropertiesByContactDetails);
    resetSuburbFilterBtn.click(function () { handleResetSuburbFiltering(true); });

    return container;
}

function toggleFilterMode(value) {
    if (value != null) {
        filterMode = value;
    }
    else {
        filterMode = !filterMode;
    }

    if (!filterMode) {
        if (currentSuburb) {
            var filterWasPerformed = false;
            $.each(currentSuburb.ProspectingProperties, function (idx, pp) {
                if (pp.Whence == "from_filter") {
                    pp.Whence = null;
                    filterWasPerformed = true;
                }
            });

            if (filterWasPerformed) {
                clearSuburbBySuburbId(currentSuburb.SuburbId);
                initialiseAndDisplaySuburb(currentSuburb, null, false);
            }
        }
    }

    var filterSticker = $("#filterMode");
    if (filterMode) {
        toggleMultiSelectMode(false);
        filterSticker.css('display', 'block');
        closeInfoWindow();
    } else {
        filterSticker.css('display', 'none');
    }
}

function prepareDataForFiltering(callbackFn) {
    if (!currentSuburb || !currentSuburb.RequiresStatsUpdate)
        return;

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Preparing Suburb For Filtering...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "update_suburb_statistics", SuburbId: currentSuburb.SuburbId })
    }).done(function() {
        $.unblockUI();
        if (callbackFn) {
            callbackFn();
        }
    });
}

function contactsWithCell(property) {
    return property.HasContactWithCell;
}

function contactsWithPrimaryCell(property) {
    return property.HasContactWithPrimaryCell;
}

function contactsWithEmail(property) {
    return property.HasContactWithEmail;
}

function contactsWithPrimaryEmail(property) {
    return property.HasContactWithPrimaryEmail;
}

function contactsWithLandline(property) {
    return property.HasContactWithLandline;
}

function contactsWithPrimaryLandline(property) {
    return property.HasContactWithPrimaryLandline;
}

function handleFilterPropertiesByContactDetails() {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Filtering...</p>' });
    handleResetSuburbFiltering(false);
    loadSuburbAndFilterByContactDetails();
}

function loadSuburbAndFilterByContactDetails() {
    var filterPropertiesHaving = $('#contactsFilterHave').is(':checked');
    var filterByCell = $('#cellFilterInput').is(':checked');
    var filterByEmail = $('#emailFilterInput').is(':checked');
    var filterByLandline = $('#landlineFilterInput').is(':checked');

    var filterByPrimaryCell = $('#primaryCellFilterInput').is(':checked');
    var filterByPrimaryEmail = $('#primaryEmailFilterInput').is(':checked');
    var filterByPrimaryLandline = $('#primaryLandlineFilterInput').is(':checked');

    var exclusive = $("#exclusiveFilterOption").is(':checked');

    if (!filterByCell && !filterByEmail && !filterByLandline && !filterByPrimaryCell && !filterByPrimaryEmail && !filterByPrimaryLandline) {
        alert('Please select at least one criterion to filter by');
        $.unblockUI();
        return;
    }

    clearSuburbBySuburbId(currentSuburb.SuburbId);
    if (currentSuburb.RequiresStatsUpdate) {
        currentSuburb.RequiresStatsUpdate = false;
        currentSuburb.IsInitialised = false;
    }

    loadSuburb(currentSuburb.SuburbId, false, function () {
        currentProperty = null;
        $.unblockUI();
    }, false, function (markers) {
        var filteredMarkers = [];
        $.each(markers, function (idx, marker) {
            var addMarker = false;
            var candidateMarkerForExclusiveFilter = true;
            if (filterByCell) {
                // Having
                if (filterPropertiesHaving) {
                    if (contactsWithCell(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!contactsWithCell(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (filterByPrimaryCell) {
                if (filterPropertiesHaving) {
                    if (contactsWithPrimaryCell(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!contactsWithPrimaryCell(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (filterByEmail) {
                if (filterPropertiesHaving) {
                    if (contactsWithEmail(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!contactsWithEmail(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (filterByPrimaryEmail) {
                if (filterPropertiesHaving) {
                    if (contactsWithPrimaryEmail(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!contactsWithPrimaryEmail(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (filterByLandline) {
                if (filterPropertiesHaving) {
                    if (contactsWithLandline(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!contactsWithLandline(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (filterByPrimaryLandline) {
                if (filterPropertiesHaving) {
                    if (contactsWithPrimaryLandline(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!contactsWithPrimaryLandline(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (exclusive) {
                if (addMarker && candidateMarkerForExclusiveFilter == true) {
                    marker.ProspectingProperty.Whence = 'from_filter';
                    filteredMarkers.push(marker);
                }
            }
            else { // not an exclusive filter
                if (addMarker) {
                    marker.ProspectingProperty.Whence = 'from_filter';
                    filteredMarkers.push(marker);
                }
            }
        });
        return filteredMarkers;
    });
}

function propertyIsFH(property) {
    return property.SS_FH == "FH";
}

function propertyIsSS(property) {
    return (property.SS_FH == "SS") || (property.SS_FH == "FS");
}

function propertyIsNewRegForUpdate(property) {
    return property.LatestRegDateForUpdate != null;
}

function propertyIsShortTermRental(property) {
    return property.IsShortTermRental == true;
}

function propertyIsLongTermRental(property) {
    return property.IsLongTermRental == true;
}

function propertyIsAgri(property) {
    return property.IsAgricultural == true;
}

function propertyIsComm(property) {
    return property.IsCommercial == true;
}

function propertyIsInvestment(property) {
    return property.IsInvestment == true;
}

function loadSuburbAndFilterByPropertyDetails() {
    var filterPropertiesHaving = $('#propertiesFilterHave').is(':checked');
    var exclusive = $("#exclusiveFilterOption2").is(':checked');

    var fhFilter = $('#fhFilterInput').is(':checked');
    var ssFilter = $('#ssFilterInput').is(':checked');

    if (!fhFilter && !ssFilter) {
        alert('Please select at least one property type to filter on');
        $.unblockUI();
        return;
    }

    var newRegistrationsFilter = $('#newRegistrationsFilterInput').is(':checked');

    var shortTermRentalFilter = $("#shortTermRentalFilterInput").is(':checked');
    var longTermRentalFilter = $("#longTermRentalFilterInput").is(':checked');
    var agriFilter = $("#agriFilterInput").is(':checked');
    var commFilter = $("#commFilterInput").is(':checked');
    var investmentFilter = $("#investmentFilterInput").is(':checked');

    clearSuburbBySuburbId(currentSuburb.SuburbId);
    if (currentSuburb.RequiresStatsUpdate) {
        currentSuburb.RequiresStatsUpdate = false;
        currentSuburb.IsInitialised = false;
    }

    loadSuburb(currentSuburb.SuburbId, false, function () {
        currentProperty = null;
        $.unblockUI();
    }, false, function (markers) {
        var filteredMarkers = [];
        $.each(markers, function (idx, marker) {
            var addMarker = false;
            var candidateMarkerForExclusiveFilter = true;
            
            if (!fhFilter) {
                if (propertyIsFH(marker.ProspectingProperty)) {
                    return;
                }
            }

            if (!ssFilter) {
                if (propertyIsSS(marker.ProspectingProperty)) {
                    return;
                }
            }

            if (newRegistrationsFilter) {
                if (filterPropertiesHaving) {
                    if (propertyIsNewRegForUpdate(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyIsNewRegForUpdate(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (shortTermRentalFilter) {
                if (filterPropertiesHaving) {
                    if (propertyIsShortTermRental(marker.ProspectingProperty)) {
                        addMarker = true;
                    }
                    else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyIsShortTermRental(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (longTermRentalFilter) {
                if (filterPropertiesHaving) {
                    if (propertyIsLongTermRental(marker.ProspectingProperty)) {
                        addMarker = true;
                    }
                    else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyIsLongTermRental(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (agriFilter) {
                if (filterPropertiesHaving) {
                    if (propertyIsAgri(marker.ProspectingProperty)) {
                        addMarker = true;
                    }
                    else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyIsAgri(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (commFilter) {
                if (filterPropertiesHaving) {
                    if (propertyIsComm(marker.ProspectingProperty)) {
                        addMarker = true;
                    }
                    else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyIsComm(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (investmentFilter) {
                if (filterPropertiesHaving) {
                    if (propertyIsInvestment(marker.ProspectingProperty)) {
                        addMarker = true;
                    }
                    else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyIsInvestment(marker.ProspectingProperty)) {
                        addMarker = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (exclusive) {
                if (addMarker && candidateMarkerForExclusiveFilter == true) {
                    marker.ProspectingProperty.Whence = 'from_filter';
                    filteredMarkers.push(marker);
                }
            }
            else { // not an exclusive filter
                if (addMarker) {
                    marker.ProspectingProperty.Whence = 'from_filter';
                    filteredMarkers.push(marker);
                }
            }
        });
        return filteredMarkers;
    });
}

function handleResetSuburbFiltering(refreshData) {
    toggleFilterMode(false);
    toggleFilterMode(true);

    if (refreshData) {
        currentSuburb.RequiresStatsUpdate = true;
        prepareDataForFiltering(function () {
            clearSuburbBySuburbId(currentSuburb.SuburbId);
            currentSuburb.IsInitialised = false;
            loadSuburb(currentSuburb.SuburbId, false, function () {
                currentProperty = null;
                $.unblockUI();
            }, false, false);
        });
    }
}