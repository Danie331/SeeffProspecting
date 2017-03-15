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
        container.append('Load a suburb to see available filtering options.');
        container.css('display', 'block');
        return;
    }
    if (currentSuburbForFiltering != currentSuburb) {
        currentSuburbForFiltering = currentSuburb;
        container.empty();

        var expander = buildContentExpanderForFiltering();
        container.append(expander.construct());
        expander.open('filterByActivitiesAndFollowups');
    }
    container.css('display', 'block');
}

function buildContentExpanderForFiltering() {
    $('#filterByActivitiesAndFollowups').empty();
    var filterByActivitiesAndFollowups = buildContentExpanderItem('filterByActivitiesAndFollowups', 'Assets/activities-followups-filtering.png', "Activities & Follow-ups For Suburb", buildActivitiesAndFollowupsFilterTab());

    $('#filterByContactDetailsTab').empty();
    var filterByContactDetailsTab = buildContentExpanderItem('filterByContactDetailsTab', 'Assets/contact.png', "Contact Details Filters", buildContactsFilterTab());

    $('#filterByPropertyDetailsTab').empty();
    var filterByPropertyDetailsTab = buildContentExpanderItem('filterByPropertyDetailsTab', 'Assets/fh_edit.png', "Property Details Filters", buildPropertyDetailsFilterTab());

    return new ContentExpanderWidget('#contentarea', [filterByActivitiesAndFollowups, filterByContactDetailsTab, filterByPropertyDetailsTab], "filteringExpander");
}

function buildActivitiesAndFollowupsFilterTab() {
    var container = $("<div />");

    var activityTypesCombo = new app.comboSelect('Activity Types', 'activityTypesComboFilter', prospectingContext.UserActivityTypes, function (item) { return item.Key; }, function (item) { return item.Value; });
    var followupActivityTypesCombo = new app.comboSelect('Follow-up Types', 'followupTypesComboFilter', prospectingContext.UserFollowupTypes, function (item) { return item.Key; }, function (item) { return item.Value; });

    var filterTypesContainer = $("<div style='margin-bottom:10px' />");
    var rb1 = $("<input type='radio' name='rg1' checked />");
    filterTypesContainer.append(rb1).append($("<div style='display:inline-block;margin-left:10px' />").append(activityTypesCombo.getElement()));
    var rb2 = $("<input type='radio' name='rg1' style='margin-left:30px' />");
    filterTypesContainer.append(rb2).append($("<div style='display:inline-block;margin-left:10px' />").append(followupActivityTypesCombo.getElement()));
    container.append(filterTypesContainer);
    followupActivityTypesCombo.enable(false);

    rb1.change(function () {
        activityTypesCombo.enable(this.checked);
        followupActivityTypesCombo.enable(!this.checked);
    });

    rb2.change(function () {
        followupActivityTypesCombo.enable(this.checked);
        activityTypesCombo.enable(!this.checked);
    });

    var filterUsersContainer = $("<div />");
    var createdBy = $("<label class='fieldAlignmentMediumWidth'>Created By: </label><select><option /></select>");
    var allocatedTo = $("<label class='fieldAlignmentMediumWidth'>Allocated To: </label><select><option /></select>");
    filterUsersContainer.append(createdBy);
    filterUsersContainer.append('<br />');
    filterUsersContainer.append(allocatedTo);
    container.append(filterUsersContainer);

    var followupDateContainer = $("<div />");
    var followupFromInput = $("<input size='17' readonly='true' />");
    var followupDateFrom = $("<label class='fieldAlignmentMediumWidth'>Follow-up From: </label>");
    var followupToInput = $("<input size='17' readonly='true' />");
    var followupDateTo = $("<label>To: </label>");
    followupDateContainer.append($("<div  style='display:inline-block;'>").append(followupDateFrom).append(followupFromInput));
    followupDateContainer.append($("<div style='display:inline-block;margin-left:12px'>").append(followupDateTo).append(followupToInput));
    container.append(followupDateContainer);

    followupFromInput.datepicker({ dateFormat: 'd MM yy', changeMonth: true, changeYear: true });
    followupToInput.datepicker({ dateFormat: 'd MM yy', changeMonth: true, changeYear: true });

    $.each(prospectingContext.BusinessUnitUsers, function (idx, el) {
        createdBy.next().append($("<option />").val(el.UserGuid).text(el.UserName + " " + el.UserSurname));
        allocatedTo.next().append($("<option />").val(el.UserGuid).text(el.UserName + " " + el.UserSurname));
    });

    var createdDatesContainer = $("<div />");
    var createdFromInput = $("<input size='17' readonly='true' />");
    var createdFrom = $("<label class='fieldAlignmentMediumWidth'>Created From: </label>");
    var createdToInput = $("<input size='17' readonly='true' />");
    var createdTo = $("<label>To: </label>");
    createdDatesContainer.append($("<div  style='display:inline-block;'>").append(createdFrom).append(createdFromInput));
    createdDatesContainer.append($("<div style='display:inline-block;margin-left:12px'>").append(createdTo).append(createdToInput));
    container.append(createdDatesContainer);

    createdFromInput.datepicker({ dateFormat: 'd MM yy', changeMonth: true, changeYear: true });
    createdToInput.datepicker({ dateFormat: 'd MM yy', changeMonth: true, changeYear: true });
    
    var filterBtn = $("<input type='button' value='Filter' style='cursor:pointer;display:inline-block;float:left' />");
    var resetBtn = $("<input type='button' value='Reset' style='cursor:pointer;display:inline-block;float:right' />");
    var buttonContainer = $("<div />");
    buttonContainer.append(filterBtn).append(resetBtn);
    container.append('<br />').append(buttonContainer);
    container.append('<br />').append('<hr style="margin-top:12px" />');

    resetBtn.click(function () {
        activityTypesCombo.setDefault();
        followupActivityTypesCombo.setDefault();
        createdBy.next()[0].selectedIndex = 0;
        allocatedTo.next()[0].selectedIndex = 0;
        followupFromInput.datepicker('setDate', null);
        followupToInput.datepicker('setDate', null);
        createdFromInput.datepicker('setDate', null);
        createdToInput.datepicker('setDate', null);

        handleResetSuburbFiltering(true);
    });

    var outputGrid = $("<div id='pivotTable' style='display:block;width:100%;overflow-x:auto' />");
    container.append(outputGrid);

    filterBtn.click(function () {
        if (!activityTypesCombo.hasSelectedOption() && !followupActivityTypesCombo.hasSelectedOption()) {
            tooltip.pop(followupActivityTypesCombo.getElement()[0], 'At least one option from either Activity Types or Follow-up Types must be selected', { showDelay: 1, hideDelay: 100, calloutPosition: 0.5, maxWidth: 500 });
            return;
        }
        var followupFromDate = followupFromInput.val();
        var followupToDate = followupToInput.val();
        if (followupFromDate && followupToDate && new Date(followupToDate) < new Date(followupFromDate)) {
            tooltip.pop(followupToInput[0], 'To date must be greater than From date', { showDelay: 1, hideDelay: 100, calloutPosition: 0.5, maxWidth: 500 });
            return;
        }
        var createdFromDate = createdFromInput.val();
        var createdToDate = createdToInput.val();
        if (createdFromDate && createdToDate && new Date(createdToDate) < new Date(createdFromDate)) {
            tooltip.pop(createdToInput[0], 'To date must be greater than From date', { showDelay: 1, hideDelay: 100, calloutPosition: 0.5, maxWidth: 500 });
            return;
        }

        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Generating Data Model. Please wait...</p>' });
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({
                Instruction: 'filter_activities_followups_for_business_unit',
                ShowingActivityTypes: rb1.is(':checked'),
                ActivityTypes: activityTypesCombo.getSelection(),
                ActivityFollowupTypes: followupActivityTypesCombo.getSelection(),
                CreatedBy: createdBy.next().val(),
                AllocatedTo: allocatedTo.next().val(),
                FollowupDateFrom: followupFromInput.val(),
                FollowupDateTo: followupToInput.val(),
                CreatedDateFrom: createdFromInput.val(),
                CreatedDateTo: createdToInput.val(),
                CurrentSuburbID: currentSuburb.SuburbId
            }),
            dataType: "json"
        }).done(function (data) {
            handleFilterByActivityFollowupTypes(rb1.is(':checked'), data.OutputRows, data.FilteredProperties);
        });
    });            

    return container;
}

function handleFilterByActivityFollowupTypes(activityTypesOnly, rows, filteredProperties) {
    handleResetSuburbFiltering(false);
    var outputGrid = $("#pivotTable");
    if (!rows.length) {
        outputGrid.append("No results found.");
    } else {
        if (activityTypesOnly) {
            // activity types
            outputGrid.pivotUI(
                            rows,
                            {
                                rows: ["Created By"],
                                cols: ["Activity Type"]
                            },
                            true);
        }
        else {
            outputGrid.pivotUI(
                            rows,
                            {
                                rows: ["Allocated To"],
                                cols: ["Followup Type"]
                            },
                            true);
        }
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
            if (filteredProperties.indexOf(marker.LightstonePropertyId) > -1) {
                marker.ProspectingProperty.Whence = 'from_filter';
                filteredMarkers.push(marker);
            }
        });
        return filteredMarkers;
    });
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

    var newRegistrationsFilter = $("<label class='fieldAlignment'>New Registrations</label><input type='checkbox' id='newRegistrationsFilterInput'  />");

    var shortTermRentalFilter = $("<label class='fieldAlignment'>Short-term Rental</label><input type='checkbox' id='shortTermRentalFilterInput'  />");
    var longTermRentalFilter = $("<label class='fieldAlignment'>Long-term Rental</label><input type='checkbox' id='longTermRentalFilterInput'  />");
    var agriFilter = $("<label class='fieldAlignment'>Agricultural</label><input type='checkbox' id='agriFilterInput'  />");
    var commFilter = $("<label class='fieldAlignment'>Commercial</label><input type='checkbox' id='commFilterInput'  />");
    var investmentFilter = $("<label class='fieldAlignment'>Investment</label><input type='checkbox' id='investmentFilterInput'  />");

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
    var sendCommsToResultsBtn = $("<input type='button' id='sendCommsToResultsBtn' value='Send Communication' style='cursor:pointer;display:none;' />");

    var regDateSelector = populateRegYearInput($("<select id='regDateSelector' style='width:60px' />"));
    var salePriceSelector = populateSalePriceInput($("<select id='salePriceSelector' style='width:120px' />"));
    var latestRegDateAndSalePriceFilter = $("<div />")
                                .append($("<label class='fieldAlignment'>Year of last registration: </label>"))
                                .append(regDateSelector)
                                .append("<p />")
                                .append($("<label class='fieldAlignment'>Last sale price: </label>"))
                                .append(salePriceSelector);

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
        .append(latestRegDateAndSalePriceFilter)
        .append("<hr />")
        .append("<p />")
        .append(performFilteringBtn).append(resetSuburbFilterBtn);

    if (prospectingContext.UserHasCommAccess) {
        container.append("<br /><br />");
        container.append(sendCommsToResultsBtn);
        sendCommsToResultsBtn.click(function () {
            showMenu("communication");
            loadSuburbAndFilterByPropertyDetails();
        });
    }

    performFilteringBtn.click(handleFilterPropertiesByPropertyDetails);
    resetSuburbFilterBtn.click(function () { handleResetSuburbFiltering(true); });

    return container;
}

function populateRegYearInput(control) {
    control.append($("<option />"));
    var currentYear = new Date().getFullYear();
    for (var i = currentYear; i >= (currentYear - 50) ; i--) {
        control.append($("<option />").val(i).text(i));
    }
    return control;
}

function populateSalePriceInput(control) {
    control.append($("<option />"));
    control.append($("<option />").val(100).text('< R100k'));
    control.append($("<option />").val(500).text('R100k to R500k'));
    control.append($("<option />").val(1000000).text('R500k to R1m'));
    control.append($("<option />").val(1500000).text('R1m to R1.5m'));
    control.append($("<option />").val(2500000).text('R1.5m to R2.5m'));
    control.append($("<option />").val(5000000).text('R2.5m to R5m'));
    control.append($("<option />").val(10000000).text('R5m to R10m'));
    control.append($("<option />").val(100000000).text('R10m + '));

    return control;
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
        if ($("#pivotTable").length) {
            $("#pivotTable").empty();
        }
        $("#sendCommsToResultsBtn").hide();
    }
}

function prepareDataForFiltering(callbackFn) {
    if (!currentSuburb || !currentSuburb.RequiresStatsUpdate)
        return;

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Preparing Suburb For Filtering...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "enable_suburb_filtering", SuburbId: currentSuburb.SuburbId }),
        dataType: "json"
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

function propertyMatchesRegDate(property, regYear) {
    var regDate = property.LightstoneRegDate;
    if (!regDate) return false;
    var dateString = formatDate(regDate);
    var yearPortion = dateString.substring(0, 4);

    return yearPortion == regYear;
}

function propertyMatchesSalePrice(property, priceBracket) {
    var lastPurchPrice = property.LastPurchPrice;
    if (!lastPurchPrice) return false;

    switch (priceBracket) {
        case "100":
            return lastPurchPrice <= 100000;
            break;
        case "500":
            return (lastPurchPrice > 100000) && (lastPurchPrice <= 500000);
            break;
        case "1000000":
            return (lastPurchPrice > 500000) && (lastPurchPrice <= 1000000);
            break;
        case "1500000":
            return (lastPurchPrice > 1000000) && (lastPurchPrice <= 1500000);
            break;
        case "2500000":
            return (lastPurchPrice > 1500000) && (lastPurchPrice <= 2500000);
            break;
        case "5000000":
            return (lastPurchPrice > 2500000) && (lastPurchPrice <= 5000000);
            break;
        case "10000000":
            return (lastPurchPrice > 5000000) && (lastPurchPrice <= 10000000);
            break;
        case "100000000":
            return lastPurchPrice > 10000000;
            break;
    }
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

    var regDateSelector = $("#regDateSelector").val();
    var salePriceSelector = $("#salePriceSelector").val();

    clearSuburbBySuburbId(currentSuburb.SuburbId);
    if (currentSuburb.RequiresStatsUpdate) {
        currentSuburb.RequiresStatsUpdate = false;
        currentSuburb.IsInitialised = false;
    }

    var anyOptionsSelected = newRegistrationsFilter || shortTermRentalFilter || longTermRentalFilter || agriFilter || commFilter || investmentFilter
                             || (regDateSelector) || (salePriceSelector);

    if (anyOptionsSelected) {
        $("#sendCommsToResultsBtn").show();
    }

    loadSuburb(currentSuburb.SuburbId, false, function () {
        currentProperty = null;
        $.unblockUI();
    }, false, function (markers) {
        var filteredMarkers = [];
        $.each(markers, function (idx, marker) {
            var candidateMarkerForExclusiveFilter = true;
            var candidateMarkerForInclusiveFilter = !anyOptionsSelected;
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
                        candidateMarkerForInclusiveFilter = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyIsNewRegForUpdate(marker.ProspectingProperty)) {
                        candidateMarkerForInclusiveFilter = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (shortTermRentalFilter) {
                if (filterPropertiesHaving) {
                    if (propertyIsShortTermRental(marker.ProspectingProperty)) {
                        candidateMarkerForInclusiveFilter = true;
                    }
                    else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyIsShortTermRental(marker.ProspectingProperty)) {
                        candidateMarkerForInclusiveFilter = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (longTermRentalFilter) {
                if (filterPropertiesHaving) {
                    if (propertyIsLongTermRental(marker.ProspectingProperty)) {
                        candidateMarkerForInclusiveFilter = true;
                    }
                    else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyIsLongTermRental(marker.ProspectingProperty)) {
                        candidateMarkerForInclusiveFilter = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (agriFilter) {
                if (filterPropertiesHaving) {
                    if (propertyIsAgri(marker.ProspectingProperty)) {
                        candidateMarkerForInclusiveFilter = true;
                    }
                    else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyIsAgri(marker.ProspectingProperty)) {
                        candidateMarkerForInclusiveFilter = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (commFilter) {
                if (filterPropertiesHaving) {
                    if (propertyIsComm(marker.ProspectingProperty)) {
                        candidateMarkerForInclusiveFilter = true;
                    }
                    else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyIsComm(marker.ProspectingProperty)) {
                        candidateMarkerForInclusiveFilter = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (investmentFilter) {
                if (filterPropertiesHaving) {
                    if (propertyIsInvestment(marker.ProspectingProperty)) {
                        candidateMarkerForInclusiveFilter = true;
                    }
                    else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyIsInvestment(marker.ProspectingProperty)) {
                        candidateMarkerForInclusiveFilter = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (regDateSelector) {
                if (filterPropertiesHaving) {
                    if (propertyMatchesRegDate(marker.ProspectingProperty, regDateSelector)) {
                        candidateMarkerForInclusiveFilter = true;
                    }
                    else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyMatchesRegDate(marker.ProspectingProperty, regDateSelector)) {
                        candidateMarkerForInclusiveFilter = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (salePriceSelector) {
                if (filterPropertiesHaving) {
                    if (propertyMatchesSalePrice(marker.ProspectingProperty, salePriceSelector)) {
                        candidateMarkerForInclusiveFilter = true;
                    }
                    else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                } else {
                    if (!propertyMatchesSalePrice(marker.ProspectingProperty, salePriceSelector)) {
                        candidateMarkerForInclusiveFilter = true;
                    } else {
                        candidateMarkerForExclusiveFilter = false;
                    }
                }
            }

            if (exclusive) {
                if (candidateMarkerForExclusiveFilter == true) {
                    marker.ProspectingProperty.Whence = 'from_filter';
                    filteredMarkers.push(marker);
                }
            }
            else { // not an exclusive filter
                if (candidateMarkerForInclusiveFilter) {
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