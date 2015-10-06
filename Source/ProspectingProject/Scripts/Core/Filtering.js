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
    return "This feature is coming soon...";
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
    var resetSuburbFilterBtn = $("<input type='button' id='resetSuburbFilterBtn' value='Reset Suburb & Refresh Data' style='cursor:pointer;display:inline-block;float:right' />");

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
    if (!currentSuburb)
        return;

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Preparing Data For Filtering...</p>' });
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
    loadSuburbAndFilter();
}

function loadSuburbAndFilter() {
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
        return;
    }

    clearSuburbBySuburbId(currentSuburb.SuburbId);
    currentSuburb.IsInitialised = false;
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

function handleResetSuburbFiltering(refreshData) {
    toggleFilterMode(false);
    toggleFilterMode(true);

    if (refreshData) {
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