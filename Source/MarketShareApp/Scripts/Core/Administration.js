
var msApp = msApp || {};

msApp.administration = {
    expanderWidget: null,
    candidateDeveloperResults: null,
    selectedAgency: null,
    buildAdminMenuHtml: function () {
        var container = $("<div style='padding:15px' />");

        $('#autoFateOption').empty();
        var autoFateOption = buildContentExpanderItem('autoFateOption', '', "Auto-Fate All Unfated Transactions", msApp.administration.buildAutoFatingHtml());
        $('#manageDevelopersOption').empty();
        var manageDevelopersOption = buildContentExpanderItem('manageDevelopersOption', '', "Manage Property Developers", msApp.administration.buildManageDevelopersHtml());

        if (initializationData.UserGuid == '62a85a9d-be7a-4fad-b704-a55edb1d338f' || initializationData.UserGuid == 'a2c48f98-14fb-425e-bbd2-312cfb89980c') {
            $('#manageAddNewAgenciesOption').empty();
            var manageAddNewAgenciesOption = buildContentExpanderItem('manageAddNewAgenciesOption', '', "Manage Agencies", msApp.administration.buildManageAddNewAgenciesHtml());
            msApp.administration.expanderWidget = new ContentExpanderWidget('#contentarea', [autoFateOption, manageDevelopersOption, manageAddNewAgenciesOption], "adminExpander");
        } else {
            msApp.administration.expanderWidget = new ContentExpanderWidget('#contentarea', [autoFateOption, manageDevelopersOption], "adminExpander");
        }
       
        container.empty();
        container.append(msApp.administration.expanderWidget.construct());

        return container;
    },
    buildAutoFatingHtml: function () {
        var div = $("<div />");
        var autoFateAllBtn = $("<input type='button' value='Auto-Fate' class='cool_btn_style' />");
        var autoFateDescriptionLbl = $("<label><i>Use this option to automatically assign a 'RESIDENTIAL' fating to all unfated transactions under your license</i></label>");
        div.append(autoFateDescriptionLbl);
        div.append("<p />");
        div.append(autoFateAllBtn);

        autoFateAllBtn.click(msApp.administration.eventHandlers.autoFateBtnClick);
        
        return div;
    },
    buildManageDevelopersHtml: function () {
        var div = $("<div />");
        var manageDevelopersLbl = $("<label><i>Use the textbox below to find sellers that are listed against more than 10 registrations on MarketShare and add them to a list of developers. Transactions for these sellers will be automatically fated as 'Developments'.</i></label>");
        div.append(manageDevelopersLbl);
        div.append("<p />");

        var input = $("<input id='developerSearchInput' type='text' style='width:60%;display:inline-block;float:left;vertical-align:middle' />");
        var searchBtn = $("<input type='button' value='Search' style='display:inline-block;vertical-align:middle;margin-left:2px' />");
        div.append(input).append(searchBtn).append('<br />').append('<hr />').append('<br />');
        searchBtn.click(msApp.administration.eventHandlers.searchForDevelopers);

        var resultsContainer = $("<div id='developerResultsContainer' />");
        div.append(resultsContainer);

        var viewDevelopersBtn = $("<input type='button' id='viewDevelopersBtn' value='List Of Developers' />");
        div.append(viewDevelopersBtn);
        viewDevelopersBtn.click(msApp.administration.eventHandlers.viewDevelopersList);

        return div;
    },
    initManageAgencies: function() {
        var items = [];
        $.each(initializationData.Agencies, function (idx, ag) {
            items.push({ label: ag.agency_name, value: ag.agency_id });
        });
        var x = $('#competingAgencySelector');
        var addBtn = $("#addNewAgencyBtn");
        x.bind("change keyup", function () {
            var value = $(this).val();
            var target = $.grep(items, function (i) {
                return i.label.toLowerCase() == value.toLowerCase();
            })[0];
            addBtn.prop('disabled', target !== undefined);
        });
        addBtn.unbind('click').bind('click', function () {
            var value = msApp.administration.selectedAgency;
            if (!value) return;
            // Save here
            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Saving. Please wait...</p>' });
            $.ajax({
                type: "POST",
                url: "RequestHandler.ashx",
                data: JSON.stringify({ Instruction: "save_new_agency", agency_name: value })
            }).done(function (result) {
                $.unblockUI();
                if (result != null) {
                    result = JSON.parse(result);
                    // Now add to in-memory cache
                    initializationData.Agencies.push(result);
                    // Update the control
                    items.push({ label: result.agency_name, value: result.agency_id });
                    x.autocomplete({
                        source: items
                    });
                    // splash screen
                    showSavedSplashDialog('Agency successfully added!');
                    // test in chrome, test that in-memory cache updated immediately and after restart.
                } else {
                    alert('Error occurred saving new agency. Please contact support.');
                }
                msApp.administration.selectedAgency = null;
            });
        });
       x.autocomplete({
               source: items,
               change: function (event, ui) {
                if (ui.item == null) {
                    //$(this).val('');
                    //$(this).focus();
                    //msApp.administration.selectedAgency = null;
                    //addBtn.prop('disabled', true);
                } else {
                    //addBtn.prop('disabled', false);
               }
       },
               select: function (event, ui) {
                event.preventDefault();
                $(this).val(ui.item.label);
                msApp.administration.selectedAgency = ui.item.value;
                addBtn.prop('disabled', true);
       },
               focus: function (event, ui) {
                event.preventDefault();
                $(this).val(ui.item.label);
               },
               search: function (event, ui) {
                   msApp.administration.selectedAgency = event.currentTarget.value;
               }
       });
    },
    buildManageAddNewAgenciesHtml: function() {
        var divContainer = $("<div />");
        divContainer.append($("<label><i>Use the searchbox below to search for existing competing agencies, and add them if they don't already exist.</i></label>"));
        divContainer.append('<p />');

        var competingAgencySelector = $("<label for='competingAgencySelector'>Search Agencies: </label><input id='competingAgencySelector' size=30 />");

        var divSelector = $("<div id='manageAgencyContainer' class='ui-widget'></div>");
        divSelector.append(competingAgencySelector);

        divContainer.append(divSelector);                            

        divContainer.append("<p />");
        var matchesFound = $("<div id='agencyMatchResultDiv' />");
        var addAgencyBtn = $("<input type='button' value='Add Agency' id='addNewAgencyBtn' disabled />");
        matchesFound.append(addAgencyBtn);
        divContainer.append(matchesFound);

        return divContainer;
    },
    eventHandlers: {
        autoFateBtnClick: function () {
            var msg = "Please note that this operation is irreversible.\n\n\Would you like to proceed?";
            var result = confirm(msg);
            if (result) {
                $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Working. Please wait...</p>' });
                $.ajax({
                    type: "POST",
                    url: "RequestHandler.ashx",
                    data: JSON.stringify({ Instruction: "auto_fate_all" })
                }).done(function (successFlag) {
                    $.unblockUI();
                    if (successFlag == "true") {
                        showSavedSplashDialog("Auto-fating completed successfully");
                        $.each(suburbsInfo, function (idx, suburb) {
                            if (suburb.IsInitialised) {
                                $.each(suburb.Listings, function (idx2, tran) {
                                    if (!tran.Fated) {
                                        tran.Fated = true;
                                        tran.MarketShareType = "R";

                                        if (tran.Marker) {
                                            if (tran.SS_FH == "SS") {
                                                var iconPath = getIconForMarker(tran.Marker);
                                                var allMarkersForSS = getAllMarkersThatWillSpiderfy(tran);
                                                $.each(allMarkersForSS, function (idx, el) {
                                                    el.setIcon(iconPath);
                                                });
                                            } else {
                                                tran.Marker.setIcon(getIconForMarker(tran.Marker));
                                            }
                                        }
                                    }
                                });
                                updateSuburbStats(suburb);
                                suburbsInfo.CanFilter = true;
                                showFilteringOptions(true);
                                generateStatisticsMenu(true);
                            } else {
                                // update of stats must still occur
                                updateSuburbStats(suburb);
                            }
                        });
                    } else {
                        alert("An error occurred processing your request. Please notify support for assistance.");
                    }
                });
            }
        },
        viewDevelopersList: function () {
            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading...</p>' });
            $.ajax({
                type: "POST",
                url: "RequestHandler.ashx",
                data: JSON.stringify({ Instruction: "retrieve_list_of_developers" })
            }).done(function (results) {
                if (results != null) {
                    results = JSON.parse(results);
                    var resultsDiv = $("<div id='listOfDevelopers' class='dialog' style='margin-right:0;font-family:verdana;font-size:12px;' />");
                    resultsDiv.empty();
                    for (var i = 0; i < results.length; i++) {
                        var resultText = results[i];
                        resultsDiv.append(resultText).append('<br />');
                    }
                    resultsDiv.dialog({
                        show: 'fade',
                        //position: ['center', 'center'],
                        hide: { effect: "fadeOut", duration: 500 },
                        width: '400',
                        height: '360',
                        title: 'List Of Developers'
                        // height title zindex
                    });
                    resultsDiv.parent().css('z-index', 999);
                } else {
                    alert('An error occurred retrieving list of developers. Please contact support.');
                }
                $.unblockUI();
            });
        },
        searchForDevelopers: function () {
            var searchText = $('#developerSearchInput').val();
            if (searchText) {
                $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Searching Database...</p>' });
                $.ajax({
                    type: "POST",
                    url: "RequestHandler.ashx",
                    data: JSON.stringify({ Instruction: "search_candidate_developers", SearchText: searchText })
                }).done(function (results) {
                    msApp.administration.candidateDeveloperResults = null;
                    if (results != null) {
                        msApp.administration.candidateDeveloperResults = JSON.parse(results);
                        var resultsContainer = $("#developerResultsContainer");
                        resultsContainer.empty();
                        if (msApp.administration.candidateDeveloperResults.length) {
                            for (var i = 0; i < msApp.administration.candidateDeveloperResults.length; i++) {
                                msApp.administration.candidateDeveloperResults[i].SearchID = i;
                                var id = 'developer_' + i;
                                var candidateDeveloperCheckbox = $("<input type='checkbox' id='" + id + "' class='candidate_developer' /><label for='" + id + "'>" + msApp.administration.candidateDeveloperResults[i].CandidateDeveloperName + "</label>");
                                resultsContainer.append(candidateDeveloperCheckbox);
                                resultsContainer.append("<br />");
                            }
                            var addDeveloperBtn = $("<input type='button' value='Add Developer(s)' class='cool_btn_style' style='display:block;float:right;' />");
                            resultsContainer.append(addDeveloperBtn);
                            addDeveloperBtn.click(msApp.administration.eventHandlers.addSellersToDeveloperList);
                        } else {
                            resultsContainer.append('No matching results found.');
                        }
                        resultsContainer.append("<br /><hr />");
                        // add dev btn, hr, no results found: dont add same developer twice, add to developers list and save, then autofate.
                    } else {
                        alert('Error occurred while searching for candidate developers. Please notify support.');
                    }
                    $.unblockUI();
                });
            }
        },
        addSellersToDeveloperList: function () {
            var checkedItems = $('.candidate_developer:checkbox:checked');
            if (checkedItems.length) {
                var candidateDevelopers = [];
                $.each(checkedItems, function (idx, checkedItem) {
                    var itemID = $(checkedItem).attr('id').replace('developer_', '');
                    var developer = $.grep(msApp.administration.candidateDeveloperResults, function (dev) {
                        return dev.SearchID == itemID;
                    })[0];
                    candidateDevelopers.push(developer.CandidateDeveloperName);
                });
                $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Saving...Please wait. This dialog will close when the process completes.</p>' });
                $.ajax({
                    type: "POST",
                    url: "RequestHandler.ashx",
                    data: JSON.stringify({ Instruction: "save_developers", DevelopersToSave: candidateDevelopers })
                }).done(function (result) {
                    $.unblockUI();
                    if (result != 'false') {
                        //
                    } else {
                        alert('An error occurred saving the developers list - please contact support.');
                    }
                });
            }
        }
    }
}

function showSavedSplashDialog(text) {
    var container = $("<div class='dialog' style='margin-right:0;font-family:verdana;font-size:12px'/>");
    if (text) {
        container.text(text);
    }

    container.dialog({
        show: 'fade',
        width: '350',
        //position: ['center', 'right'],
        hide: { effect: "fadeOut", duration: 2000 },
        buttons: { "OK": function () { container.dialog("close"); } },
        open: function (event, ui) {
            $('.ui-dialog').css('z-index', 10000);
            container.siblings(".ui-dialog-titlebar").hide();
            //setTimeout(function () {
            //    container.dialog('close');
            //}, 2000);
        }
    });
}