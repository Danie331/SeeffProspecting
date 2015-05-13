var currentTracePSInfoPacket = null;
var currentTracePSContactRows = null;
var activityExpanderWidget = null;
var followupExpanderWidget = null;
var activityTypes = null;
var businessUnitUsers = null;
var activityFollowupTypes = null;
var initialLoad = true;
var globalFollowUps = [];
var busyLoadingFollowups = false;

// Overridden from MenuBuilder.js
function fixElementHeightForIE(elementId, percHeight) {
    var maxHeight = $(document).height();
    var percentage = percHeight ? percHeight : 0.9;
    var newValue = percentage * maxHeight - 100;
    $('#' + elementId).css('height', newValue + 'px');
}

function createProspectingMenu(userData) {
    var menuItem = createMenuItem("Suburb Selection", "suburbselector", buildSuburbSelectionHtml(), function () { toggleMultiSelectMode(false); }, null);
    appendMenuItemContent(menuItem.MenuItemContent);
    fixElementHeightForIE('suburbsDiv');
    menuItems.push(menuItem);

    menuItem = createMenuItem("Contact Details", "contactdetails", buildContactDetailsDiv(), function () { toggleMultiSelectMode(false); }, null);
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);

    menuItem = createMenuItem("Activity Report", "activityreport", buildActivityReport(), function () { toggleMultiSelectMode(false); handleActivityReportClick(); }, null);
    appendMenuItemContent(menuItem.MenuItemContent);
    fixElementHeightForIE('contentactivityContainer', 0.8);
    menuItems.push(menuItem);

    menuItem = createMenuItem("Follow-up", "followup", buildFollowupReport(userData.FollowupActivities), function () { toggleMultiSelectMode(false); handleFollowupReportClick(); }, userData.TotalFollowups);
    appendMenuItemContent(menuItem.MenuItemContent);
    fixElementHeightForIE('contentfollowupContainer', 0.8);
    menuItems.push(menuItem);


    var fcc = $('#contentfollowupContainer');
    fcc.unbind('scroll').bind('scroll', function (e) {
        loadMoreFollowups();
    });
    // Check this thing not rebinding each time, test chrome, test mouse wheel + click, 


    menuItem = createMenuItem("Lightstone Search", "lightstonesearch", buildSearchMenu(), function () { toggleMultiSelectMode(false); }, null);
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);

    if (prospectingContext.LoggedInUser == 'a2c48f98-14fb-425e-bbd2-312cfb89980c' || prospectingContext.LoggedInUser == '62a85a9d-be7a-4fad-b704-a55edb1d338f') {
        menuItem = createMenuItem("Communication", "communication", buildCommunicationMenu(), handleSMSMenuItemClick, null);
        appendMenuItemContent(menuItem.MenuItemContent);
        menuItems.push(menuItem);
    }

    // If the user has followups, default to the followups screen instead of the suburb selection
    if (userData.FollowupActivities.length == 0) {
        showMenu("suburbselector");
    } else {
        showMenu("followup");
    }
}

function loadMoreFollowups() {
    if (!busyLoadingFollowups) {
        busyLoadingFollowups = true;

        loadFollowups(function (followups) {
            buildFollowupReport(followups);
            busyLoadingFollowups = false;
        }, false);
    }
}

//function setFollowupMenuItemCount(value) {
//    if (value > 10) {
//        value = '10+';
//    }
//    var element = $('#followup').next();
//    element.text(value);
//}

function buildFollowupReport(followups) {

    //setFollowupMenuItemCount(globalFollowUps.length);

    //globalFollowUps.length = 0;
    $.each(followups, function (idx, el) {
        globalFollowUps.push(el);
    });

    if ($('#followupContent').length) {
        // IF the div already exists (was created already), empty and repopulate it, other build the Ui from scratch
        //$('#followupContent').empty();
        performFollowupFiltering(globalFollowUps, $('#followupContent'));
    } else {

        function buildFollowupFilter() {
            var filterDiv = $("<div />");
            var followupTypeFilter = $("<label class='fieldAlignment'>Followup Activity Type: </label><select id='followupActivityTypeSelect' />");
            filterDiv.append(followupTypeFilter);

            followupTypeFilter.change(function () {
                performFollowupFiltering(globalFollowUps, $('#followupContent'));
            });

            return filterDiv;
        }

        function buildFollowupContainer() {
            var followupContent = $("<div id='followupContent' /> ");
            performFollowupFiltering(globalFollowUps, followupContent);
            return followupContent;
        }

        $('#followupFilter').empty();
        var followupFilter = buildContentExpanderItem('followupFilter', '', "Follow-up Filter", buildFollowupFilter());
        $('#followupContainer').empty();
        var fcc = buildFollowupContainer();
        var followupContainer = buildContentExpanderItem('followupContainer', '', "Today's Follow-up", fcc);

        followupExpanderWidget = new ContentExpanderWidget('#contentarea', [followupFilter, followupContainer], "followupExpander");
        var followupOuterDiv = $("<div class='contentdiv' id='followupOuterDiv' />");
        followupOuterDiv.empty();
        followupOuterDiv.append(followupExpanderWidget.construct());

        return followupOuterDiv;
    }
}

function performFollowupFiltering(sourceFollowups, jContainerElement) {

    jContainerElement.empty();

    if (activityTypes == null || businessUnitUsers == null) {
        loadActivityLookupData(function (lookupData) {
            activityTypes = lookupData.ActivityTypes;
            businessUnitUsers = lookupData.BusinessUnitUsers;
            activityFollowupTypes = lookupData.ActivityFollowupTypes;

            populateFollowupFilters();
        });
    } else {
        populateFollowupFilters();
    }

    var followupTypeSelect = $('#followupActivityTypeSelect').val();
    var followupTypeText = $('#followupActivityTypeSelect option:selected').text();
    // Perform filtering
   var displayItems = $.grep(sourceFollowups, function (fol) {
        var meetsCriteria = true;

        if (followupTypeSelect != null && followupTypeSelect != -1) {
            if (fol.ActivityFollowupTypeId != null) {
                meetsCriteria = fol.ActivityFollowupTypeId == followupTypeSelect;
            } else {
                // If the followup does not have a ActivityFollowUpTypeId then it is a parent activity, therefore the follow-up filters do not apply to
                // this follow-up, the original activity filters apply. We need to look at the "text" property of the select box instead: this is a hack -> ultimately we need to share activity types and follow-up activity types in the DB
                // TODO: fix this.
                meetsCriteria = fol.ActivityTypeName == followupTypeText;
            }
            if (!meetsCriteria) return false;
        }

        return meetsCriteria;
    });

    $.each(displayItems, function (idx, followup) {
        var formattedItem = buildFollowupDisplayItem(followup);
        jContainerElement.append(formattedItem);
        jContainerElement.append("<hr />");
    });

    function buildFollowupDisplayItem(followup) {
        function formatLabelValue(label, value) {
            var container = $("<div style='padding:5px' />");
            if (value == null) value = "n/a";

            var isDate = new Date(value) !== "Invalid Date" && !isNaN(new Date(value)) ? true : false;
            if (isDate) {
                value = value.substring(0, 10);
            }
            value = value.replace(/\n/g, '<br />');

            if (label !== '') label = label + ": ";
            if (label != '' && value.length > 24) {
                value = value.substring(0,24) + '...';
            }

            container.append(label).append(value);
            return container[0].outerHTML;
        }

        var containerDiv = $("<div class='followup-item-container' />");
        containerDiv.attr('id', 'followup_' + followup.ActivityLogId);

        var itemTypeName = followup.FollowupActivityTypeName ? followup.FollowupActivityTypeName : followup.ActivityTypeName;
        var followupDateActivityTypeContainer = $("<div id='followupDateActivityTypeContainer' style='border: 1px solid white !important;background-color:#955ba5;color:#FFFFFF' />");
        var followupDate = $("<div style='width:50%;display:inline-block;float:left;'>" + formatLabelValue('Follow-up Date', followup.FollowupDate) + " </div>");
        var activityType = $("<div style='width:50%;display:inline-block;'>" + formatLabelValue('Activity Type', itemTypeName) + " </div>");
        followupDateActivityTypeContainer.append(followupDate).append(activityType);

        var comment = $("<div style='display:block;background-color:#F0E68C;border: 1px solid white !important;'>" + formatLabelValue('', followup.Comment) + " </div>");

        var relatedToCreatedByContainer = $("<div id='relatedToCreatedByContainer' style='border:1px solid white !important;background-color:#F0E68C;' />");
        var relatedToContactPersonName = followup.RelatedToContactPerson != null ? followup.RelatedToContactPerson.Fullname : null;
        var relatedTo = $("<div style='width:50%;display:inline-block;float:left;'>" + formatLabelValue('Related To', relatedToContactPersonName) + " </div>");
        var createdBy = $("<div style='width:50%;display:inline-block;'>" + formatLabelValue('Created By', followup.CreatedByUsername) + " </div>");
        relatedToCreatedByContainer.append(relatedTo).append(createdBy);

        var contactPers = followup.RelatedToContactPerson;
        var primaryContactNoField = null, primaryEmailAddressField = null;
        if (contactPers != null) {
            if (contactPers.PhoneNumbers != null && contactPers.PhoneNumbers.length > 0) {
                var primaryContactNo = $.grep(contactPers.PhoneNumbers, function (ph) {
                    return ph.IsPrimary == true;
                })[0];
                if (!primaryContactNo && contactPers.PhoneNumbers.length == 1) {
                    primaryContactNo = contactPers.PhoneNumbers[0];
                }
                primaryContactNoField = $("<div style='display:block;background-color:#F0E68C;border: 1px solid white !important;'></div>");
                if (primaryContactNo) {
                    primaryContactNoField.append($("<div style='padding:5px' />").append("<img style='display:inline-block;vertical-align:middle' src='Assets/phone_icon.png' /><label style='display:inline-block;vertical-align:middle;padding-left:20px'>" + format10DigitPhoneNumber(primaryContactNo.ItemContent) + "</label>"));
                } else {
                    var selectPrimaryContactNoLink = $("<a href='' style='text-decoration:underline!important;'>Please select a primary contact number for this person.</a>");
                    primaryContactNoField.append($("<div style='padding:5px' />").append(selectPrimaryContactNoLink));
                    selectPrimaryContactNoLink.click(function (e) {
                        e.preventDefault();
                        showDialogSelectPrimaryContactDetail(contactPers.PhoneNumbers, function (primaryContactNoId) {
                            primaryContactNoField.empty();
                            primaryContactNo = $.grep(contactPers.PhoneNumbers, function (ph) {
                                return ph.ItemId == primaryContactNoId;
                            })[0];
                            primaryContactNoField.append("<img style='display:inline-block;vertical-align:middle' src='Assets/phone_icon.png' /><label style='display:inline-block;vertical-align:middle;padding-left:20px'>" + format10DigitPhoneNumber(primaryContactNo.ItemContent) + "</label>");
                        });
                    });
                }
            }
            if (contactPers.EmailAddresses != null && contactPers.EmailAddresses.length > 0) {
                var primaryEmailAddress = $.grep(contactPers.EmailAddresses, function (em) {
                    return em.IsPrimary == true;
                })[0];
                if (!primaryEmailAddress && contactPers.EmailAddresses.length == 1) {
                    primaryEmailAddress = contactPers.EmailAddresses[0];
                }
                primaryEmailAddressField = $("<div style='display:block;background-color:#F0E68C;border: 1px solid white !important;'></div>");
                if (primaryEmailAddress) {
                    var emailAddressAnchor = "<a href='mailto:" + primaryEmailAddress.ItemContent + "'>" + primaryEmailAddress.ItemContent + "</a>";
                    primaryEmailAddressField.append($("<div style='padding:5px' />").append("<img style='display:inline-block;vertical-align:middle' src='Assets/email_icon.png' /><label style='display:inline-block;vertical-align:middle;padding-left:20px'>" + emailAddressAnchor + "</label>"));
                } else {
                    var selectPrimaryEmailAddressLink = $("<a href='' style='text-decoration:underline!important;'>Please select a primary email address for this person.</a>");
                    primaryEmailAddressField.append($("<div style='padding:5px' />").append(selectPrimaryEmailAddressLink));
                    selectPrimaryEmailAddressLink.click(function (e) {
                        e.preventDefault();
                        showDialogSelectPrimaryContactDetail(contactPers.EmailAddresses, function (primaryContactNoId) {
                            primaryEmailAddressField.empty();
                            primaryEmailAddress = $.grep(contactPers.EmailAddresses, function (em) {
                                return em.ItemId == primaryContactNoId;
                            })[0];
                            primaryEmailAddressField.append("<img style='display:inline-block;vertical-align:middle' src='Assets/email_icon.png' /><label style='display:inline-block;vertical-align:middle;padding-left:20px'>" + primaryEmailAddress.ItemContent + "</label>");
                        });
                    });
                }
            }
        }

        var propertyAddress = $("<div style='display:block;background-color:#F0E68C;border: 1px solid white !important;'>" + formatLabelValue('', followup.PropertyAddress) + " </div>");

        var feedbackItem = $("<div style='display:block;background-color:white;border: 1px solid white !important;padding:1px'></div>");
        var feedbackLeftSpacer = $("<div style='width:50%;display:inline-block;'></div>");
        var feedbackRightSpacer = $("<div style='width:50%;display:inline-block;'></div>");
        var feedbackBtn = $("<button type='text' id='feedbackFollowupBtn' style='cursor:pointer;vertical-align:middle;float:right'><img src='Assets/add_activity.png' style='vertical-align:middle;margin-right:5px' /><label style='vertical-align:middle'>Feedback</label></button>");
        feedbackRightSpacer.append(feedbackBtn);
        feedbackItem.append(feedbackLeftSpacer).append(feedbackRightSpacer);
        feedbackBtn.click(function (e) {
            e.preventDefault();
            handleAddFollowupActivity(followup, function () {
                //sourceFollowups = $.grep(sourceFollowups, function (fol) {
                //    return fol != followup;
                //});

                var index = sourceFollowups.indexOf(followup);
                sourceFollowups.splice(index, 1);

                performFollowupFiltering(sourceFollowups, jContainerElement);
                //setFollowupMenuItemCount(sourceFollowups.length);
            });
        });

        // view property button
        var viewPropertyBtn = $("<button type='text' id='viewPropertyFollowupBtn' style='cursor:pointer;vertical-align:middle;float:right;margin-right:2px'><img src='Assets/lightstone.png' style='vertical-align:middle;margin-right:5px' /><label style='vertical-align:middle'>View Property</label></button>");
        feedbackRightSpacer.append(viewPropertyBtn);
        viewPropertyBtn.click(function (e) {
            e.preventDefault();
            // Find the suburb this property belongs to:
            var targetSuburb = null;
            targetSuburb = $.grep(suburbsInfo, function (sub) {
                return sub.SuburbId == followup.SeeffAreaId;
            })[0];

            // Load the suburb
            globalZoomLevel = 20;
            $('#suburbLink' + targetSuburb.SuburbId).trigger('click', function () {
                var targetProperty = $.grep(currentSuburb.ProspectingProperties, function (pp) {
                    return pp.LightstonePropertyId == followup.LightstonePropertyId;
                })[0];
                var marker = targetProperty.Marker;
                try {
                    centreMap(marker.Suburb, marker, true);
                    new google.maps.event.trigger(marker, 'click', function () {
                        debugger;
                    });
                } catch (e) { }
                // Set current proeprty, what about SS?
            });
        });

        containerDiv.append(followupDateActivityTypeContainer);
        containerDiv.append(comment);
        containerDiv.append(relatedToCreatedByContainer);
        if (primaryContactNoField) {
            containerDiv.append(primaryContactNoField);
        }
        if (primaryEmailAddressField) {
            containerDiv.append(primaryEmailAddressField);
        }
        containerDiv.append(propertyAddress);
        containerDiv.append(feedbackItem);

        return containerDiv;
    }
}

function handleAddFollowupActivity(followup, callback) {

    var loadActivities = activityTypes == null || businessUnitUsers == null ? true : false;

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "get_existing_prospecting_property", LightstonePropertyId: followup.LightstonePropertyId, LoadActivities: loadActivities }),
        success: function (data, textStatus, jqXHR) {
            $.unblockUI();
            if (textStatus == "success" && data) {
                if (!handleResponseIfServerError(data)) {
                    return;
                }

                if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                    alert(data.ErrorMsg);
                }

                if (loadActivities) {
                    activityTypes = data.ActivityBundle.ActivityTypes;
                    businessUnitUsers = data.ActivityBundle.BusinessUnitUsers;
                    activityFollowupTypes = data.ActivityBundle.ActivityFollowupTypes;
                }

                var parentActivityActivityType = followup.ActivityTypeId;
                showDialogAddActivity({ ActivityTypes: activityFollowupTypes, BusinessUnitUsers: businessUnitUsers, PropertyContacts: data.Contacts }, { RelatedTo: followup.RelatedToContactPersonId, Property: data, ParentActivityId: followup.ActivityLogId, IsChildActivity: true, ParentActivityActivityType: parentActivityActivityType }, callback);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(jqXHR.status);
            alert(jqXHR.responseText);
        },
        dataType: "json"
    });
}

function handleActivityReportClick() {
    clearActivityReport();
    if (currentProperty) {
        loadExistingProspectAddActivity(currentProperty, null, function () {
            $('#recentActivitiesDiv').css('display', 'none');
            $('#activitiesOuterDiv').css('display', 'block');

            activityExpanderWidget.open('activityContainer');
            // Populate related-to
            $('#relatedToSelect').empty();
            $('#relatedToSelect').append($("<option />").val(-1).text(''));
            $.each(currentProperty.Contacts, function (idx, el) {
                $('#relatedToSelect').append($("<option />").val(el.ContactPersonId).text(el.Firstname + ' ' + el.Surname));
            });

            // Populate the Activity No dropdown
            $('#activityNoSelect').empty();
            $('#activityNoSelect').append($("<option />").val(-1).text(''));
            if (currentProperty.ActivityBundle != null && currentProperty.ActivityBundle.Activities.length > 0) {
                $.each(currentProperty.ActivityBundle.Activities, function (idx, el) {
                    if (el.ParentActivityId == null) {
                        $('#activityNoSelect').append($("<option />").val(el.ActivityLogId).text(el.ActivityLogId));
                    }
                });
            }
            performActivityFiltering();
        });
    } else {        
         $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Recent Activities...</p>' });
         $.ajax({
             type: "POST",
             url: "RequestHandler.ashx",
             data: JSON.stringify({ Instruction: "load_activities_for_user" }),
             success: function (data, textStatus, jqXHR) {
                 $.unblockUI();
                 if (textStatus == "success" && data) {
                     if (!handleResponseIfServerError(data)) {
                         return;
                     }

                     if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                         alert(data.ErrorMsg);
                     }

                     showRecentActivitiesForUser(data.Activities);
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

function handleFollowupReportClick() {
    resetFollowupFilters();
    if (followupExpanderWidget != null) {
        followupExpanderWidget.open('followupContainer');
        if (!initialLoad) { // because followups are already loaded before startup
            loadFollowups(function (followups) {
                buildFollowupReport(followups);
            }, true);
        }
    }
    initialLoad = false;
}

function buildSearchMenu() {
    var searchDiv = $("<div id='lightstoneSearchDiv' class='contentdiv' style='padding-right:10px;font-size:12px' />");

    searchDiv.append("<label class='fieldAlignmentShortWidth' for='deedTownInput'>Deed Town</label>\
                      <input type='text' name='deedTownInput' id='deedTownInput' style='height:12px;font-size:12px' size='60' ><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='suburbInput'>Suburb</label>\
                      <input type='text' name='suburbInput' id='suburbInput' style='height:12px;font-size:12px' size='60'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='streetNameInput'>Street Name</label>\
                      <input type='text' name='streetNameInput' id='streetNameInput' style='height:12px;font-size:12px' size='60'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='streetNoInput'>Street Number</label>\
                      <input type='text' name='streetNoInput' id='streetNoInput' style='height:12px;font-size:12px' size='60'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='complexNameInput'>Complex Name</label>\
                      <input type='text' name='complexNameInput' id='complexNameInput' style='height:12px;font-size:12px' size='60'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='estateNameInput'>Estate Name</label>\
                      <input type='text' name='estateNameInput' id='estateNameInput' style='height:12px;font-size:12px' size='60'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='erfNoInput'>ERF Number</label>\
                      <input type='text' name='erfNoInput' id='erfNoInput' size='30' style='height:12px;font-size:12px'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='portionNoInput'>Portion Number</label>\
                      <input type='text' name='portionNoInput' id='portionNoInputBox' size='30' style='height:12px;font-size:12px' disabled><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='propertyIdInput'>Property ID</label>\
                      <input type='text' name='propertyIdInput' id='propertyIdInput' size='30' style='height:12px;font-size:12px'><p style='margin:3px;' />\
                      <hr />\
                      <label class='fieldAlignmentShortWidth' for='ownerNameInput'>Owner Name</label>\
                      <input type='text' name='ownerNameInput' id='ownerNameInput' size='30' style='height:12px;font-size:12px'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='ownerIDnoInput'>Owner ID No.</label>\
                      <input type='text' name='ownerIDnoInput' id='ownerIDnoInput' size='30' style='height:12px;font-size:12px'><p style='margin:3px;' />");

    // Portion number only valid when erf no present
    $('#contentarea').on('keyup', '#erfNoInput', function () {        
        var str = $(this).val();
        if (str.length > 0) {
            $('#portionNoInputBox').removeAttr("disabled");
        } else {
            $('#portionNoInputBox').val('');
            $('#portionNoInputBox').attr('disabled', 'disabled');            
        }        
    });

    var searchBtn = $("<input type='button' id='searchLightstoneMatchesBtn' value='Search..' style='float:right;' />");
    searchDiv.append(searchBtn);
    searchDiv.append("<p style='padding:10px;' />");
    var searchResultsDiv = $("<div id='lightstoneResultsDiv' style='height:350px;overflow-y:auto;' />");
    searchDiv.append(searchResultsDiv);

    // Must try to infer province

    searchBtn.click(function () {
        performLightstoneSearch();
    });

    return searchDiv;
}

function clearLightstoneSearchResults() {
    var resultsDiv = $('#lightstoneResultsDiv');
    resultsDiv.empty();
    return resultsDiv;
}

function createLightstoneSearchResultsDiv(results) {

    var resultsDiv = clearLightstoneSearchResults();
    
    if (results.length == 0) {
        resultsDiv.append("No results found.");
        return;
    }

    var currentResultDiv = null;
    $.each(results, function (idx, result) {

        if (result.IsSectionalScheme) {
            var propMatches = result.PropertyMatches;
            var ssId = propMatches[0].SS_UNIQUE_IDENTIFIER;
            var resultDiv = $("<div id='" + ssId + "' style='border:1px solid;border-radius:3px;cursor:pointer;' />");
            resultDiv.hover(function () {
                $(this).css('background-color', '#b0c4de');
            }, function () {
                if (!currentResultDiv || currentResultDiv.attr("id") != $(this).attr("id")) {
                    $(this).css('background-color', 'white');
                }
            });
            resultDiv.click(function () {
                if (currentResultDiv) {
                    currentResultDiv.css('background-color', 'white');
                }

                currentResultDiv = $(this);
                showSearchedPropertyOnMap(result);
            });

            var resultContent = "Sectional Title: " + propMatches[0].SSName + "<br />" +
                propMatches[0].StreetOrUnitNo + " " + propMatches[0].StreetName + ", " + propMatches[0].Suburb + ", " + propMatches[0].City;
            resultDiv.append(resultContent);

            resultsDiv.append(resultDiv);
        }
        else if (result.IsFarm) {
            var frmPortion = result.PropertyMatches[0];
            var frmId = 'FRM_' + frmPortion.LightstonePropId;
            var resultDiv = $("<div id='" + frmId + "' style='border:1px solid;border-radius:3px;cursor:pointer;' />");
            resultDiv.hover(function () { $(this).css('background-color', '#b0c4de'); }, function () {
                if (!currentResultDiv || currentResultDiv.attr("id") != $(this).attr("id")) {
                    $(this).css('background-color', 'white');
                }
            });
            resultDiv.click(function () {
                if (currentResultDiv) {
                    currentResultDiv.css('background-color', 'white');
                }

                currentResultDiv = $(this);
                //resultDiv.css('background-color', '#b0c4de');
                showSearchedPropertyOnMap(result);
            });

            var resultContent = "Farm: " + frmPortion.FarmName + " [Erf no.: " + frmPortion.ErfNo + ", Portion: " + frmPortion.Portion + "]";
            resultDiv.append(resultContent);

            resultsDiv.append(resultDiv);
        }
        else { // FH
            var fhProperty = result.PropertyMatches[0];
            var fhId = "FH" + "_" + fhProperty.LightstonePropId;
            var resultDiv = $("<div id='" + fhId + "' style='border:1px solid;border-radius:3px;cursor:pointer;' />");
            resultDiv.hover(function () { $(this).css('background-color', '#b0c4de'); }, function () {
                if (!currentResultDiv || currentResultDiv.attr("id") != $(this).attr("id")) {
                    $(this).css('background-color', 'white');
                }
            });
            resultDiv.click(function () {
                if (currentResultDiv) {
                    currentResultDiv.css('background-color', 'white');
                }

                currentResultDiv = $(this);
                //resultDiv.css('background-color', '#b0c4de');
                showSearchedPropertyOnMap(result);
            });

            var resultContent = "Free-hold: " + fhProperty.StreetOrUnitNo + " " + fhProperty.StreetName + ", " + fhProperty.Suburb + ", " + fhProperty.City;
            resultDiv.append(resultContent);

            resultsDiv.append(resultDiv);
        }        
    });
}

function resetActivityFilters() {
    $('#activityNoSelect').val(-1);
    $('#activityTypeSelect').val(-1);
    $('#allocatedToSelect').val(-1);
    $('#relatedToSelect').val(-1);

    $('#createdFromDate').val('');
    $('#createdToDate').val('');
    $('#followupFromDate').val('');
    $('#followupToDate').val('');
}

function resetFollowupFilters() {
    $('#followupActivityTypeSelect').val(-1);
}

function buildActivityReport() {

    function buildActivityFilter() {
        var filterDiv = $("<div />");
        var activityNoFilter = $("<label class='fieldAlignment'>Activity No.: </label><select id='activityNoSelect' />");
        var activityTypeFilter = $("<label class='fieldAlignment'>Activity Type: </label><select id='activityTypeSelect' />");       
        var allocatedToFilter = $("<label class='fieldAlignment'>Allocated To: </label><select id='allocatedToSelect' />");        
        var relatedToFilter = $("<label class='fieldAlignment'>Related To: </label><select id='relatedToSelect' />");        

        var createdDatesdiv = $("<div  />");
        var createdfrom = $("<label class='fieldAlignment' for='createdFromDate'>Created From: </label><input type='text' id='createdFromDate' />");
        var createdto = $("<label class='fieldAlignment' for='createdToDate'> To: </label><input type='text' id='createdToDate' />");
        createdDatesdiv.append(createdfrom).append("<br />").append(createdto);
        createdfrom.datepicker({ dateFormat: 'DD, d MM yy' });
        createdto.datepicker({ dateFormat: 'DD, d MM yy' });
        var createdDatesFilter = createdDatesdiv;

        var followupDatesDiv = $("<div  />");
        var followfrom = $("<label class='fieldAlignment' for='followupFromDate'>Follow-up Created From: </label><input type='text' id='followupFromDate' />");
        var followupto = $("<label class='fieldAlignment' for='followupToDate'> To: </label><input type='text' id='followupToDate' />");
        followupDatesDiv.append(followfrom).append("<br />").append(followupto);
        followfrom.datepicker({ dateFormat: 'DD, d MM yy' });
        followupto.datepicker({ dateFormat: 'DD, d MM yy' });
        var followupDatesFilter = followupDatesDiv;

        filterDiv.append(activityNoFilter).append("<br />").append(activityTypeFilter).append("<br />").append(allocatedToFilter).append("<br />").append(relatedToFilter).append("<p />").append(createdDatesdiv).append("<br />").append(followupDatesDiv);

        activityNoFilter.change(performActivityFiltering);
        activityTypeFilter.change(performActivityFiltering);
        allocatedToFilter.change(performActivityFiltering);
        relatedToFilter.change(performActivityFiltering);
        createdfrom.change(performActivityFiltering);
        createdto.change(performActivityFiltering);
        followfrom.change(performActivityFiltering);
        followupto.change(performActivityFiltering);

        return filterDiv;
    }

    function buildActivityContainer() {
        var activityFilterContent = $("<div id='activityContent' /> ");
        return activityFilterContent;
    }
   
    $('#activityFilter').empty();
    var activityFilter = buildContentExpanderItem('activityFilter', '', "Activity Filter", buildActivityFilter());
    $('#activityContainer').empty();
    var activityContainer = buildContentExpanderItem('activityContainer', '', "Activity Report", buildActivityContainer());

    activityExpanderWidget = new ContentExpanderWidget('#contentarea', [activityFilter, activityContainer], "activitiesExpander");
    var activitiesOuterDiv = $("<div class='contentdiv' id='activitiesOuterDiv' />");
    activitiesOuterDiv.empty();
    activitiesOuterDiv.append(activityExpanderWidget.construct());
    //activityExpanderWidget.setExpandable('activityContainer', false);

    return activitiesOuterDiv;
}

function populateActivityTypeAndAllocatedToDropdowns() {
    // Populate activities
    var activityTypeSelect = $('#activityTypeSelect');
    if (activityTypeSelect.children().length == 0) {
        activityTypeSelect.append($("<option />").val(-1).text(''));
        $.each(activityTypes, function (idx, el) {
            activityTypeSelect.append($("<option />").val(el.Key).text(el.Value));
        });
    }

    // Populate Allocated To
    var allocatedToSelect = $('#allocatedToSelect');
    if (allocatedToSelect.children().length == 0) {
        allocatedToSelect.append($("<option />").val(-1).text(''));
        $.each(businessUnitUsers, function (idx, el) {
            allocatedToSelect.append($("<option />").val(el.UserGuid).text(el.UserName + " " + el.UserSurname));
        });
    }
}

function populateFollowupFilters() {
    var followupActivityTypeSelect = $('#followupActivityTypeSelect');
    if (followupActivityTypeSelect.children().length == 0) {
        followupActivityTypeSelect.append($("<option />").val(-1).text(''));
        $.each(activityFollowupTypes, function (idx, el) {
            followupActivityTypeSelect.append($("<option />").val(el.Key).text(el.Value));
        });
    }
}

function showRecentActivitiesForUser(activities) {
    $('#activitiesOuterDiv').css('display', 'none');
    var recentActivitiesDiv = $('#recentActivitiesDiv');
    if (recentActivitiesDiv.length) {
        recentActivitiesDiv.empty();
        recentActivitiesDiv.css('display', 'block');
    } else {
        recentActivitiesDiv = $("<div class='contentdiv' id='recentActivitiesDiv' />");
        $('#activityreport_content').append(recentActivitiesDiv);
    }

    if (activities.length) {
        activities.sort(function (a, b) { return new Date(b.CreatedDate) - new Date(a.CreatedDate); });
                      
        recentActivitiesDiv.append("Recent activities created by you:");
        recentActivitiesDiv.append("<p />");
        $.each(activities, function (idx, activity) {
            var formattedActivity = buildActivityDisplayItem(activity);
            recentActivitiesDiv.append(formattedActivity);
            recentActivitiesDiv.append("<hr />");
        });
    }
    else {
        recentActivitiesDiv.append("You have no recent activities.");
    }
}

    // This function always targets the ActivityBundle of the current property
function performActivityFiltering() {
    $('#activitiesExpander').css('display', 'block');

    if (activityTypes == null || businessUnitUsers == null) {
        loadActivityLookupData(function (lookupData) {
            activityTypes = lookupData.ActivityTypes;
            businessUnitUsers = lookupData.BusinessUnitUsers;
            activityFollowupTypes = lookupData.ActivityFollowupTypes;

            populateActivityTypeAndAllocatedToDropdowns(); //populateFollowupFilters
        });
    } else {
        populateActivityTypeAndAllocatedToDropdowns();
    }

    var actvityNoFilter = $('#activityNoSelect').val();
    var activityTypeFilter = $('#activityTypeSelect').val();
    var allocatedToFilter = $('#allocatedToSelect').val();
    var relatedToFilter = $('#relatedToSelect').val();
    var dateCreatedFrom = $('#createdFromDate').val();
    var dateCreatedTo = $('#createdToDate').val();
    var followupDateFrom = $('#followupFromDate').val();
    var followupDateTo = $('#followupToDate').val();

    var activities = currentProperty.ActivityBundle.Activities;
    // Perform filtering
    var parentActivityList = [];
    // First order activities by ID in ascending order
    activities.sort(function (a, b) {
        return a.ActivityLogId - b.ActivityLogId;
    });
    activities = $.grep(activities, function (act) {
        var meetsCriteria = true;

        // TODO:
        if (act.ActivityFollowupTypeId != null && activityTypeFilter != -1) {
            return false;
        }

        if (actvityNoFilter != null && actvityNoFilter != -1) {
            meetsCriteria = act.ActivityLogId == actvityNoFilter || act.ParentActivityId == actvityNoFilter || ($.inArray(act.ParentActivityId, parentActivityList) > -1);
            if (meetsCriteria) {
                parentActivityList.push(act.ActivityLogId);
            }
            if (!meetsCriteria) return false;
        }

        if (activityTypeFilter != null && activityTypeFilter != -1) {
            meetsCriteria = act.ActivityTypeId == activityTypeFilter;
            if (!meetsCriteria) return false;
        }

        if (allocatedToFilter != null && allocatedToFilter != -1) {
            meetsCriteria = act.AllocatedTo == allocatedToFilter;
            if (!meetsCriteria) return false;
        }

        if (relatedToFilter != null && relatedToFilter != -1) {
            meetsCriteria = act.ContactPersonId == relatedToFilter;
            if (!meetsCriteria) return false;
        }

        if (dateCreatedFrom != null && dateCreatedFrom != '') {
            meetsCriteria = new Date(act.CreatedDate) >= new Date(dateCreatedFrom);
            if (!meetsCriteria) return false;
        }

        if (dateCreatedTo != null && dateCreatedTo != '') {
            var d2 = new Date(dateCreatedTo);
            meetsCriteria = new Date(act.CreatedDate) <= d2;
            if (!meetsCriteria) return false;
        }

        if (followupDateFrom != null && followupDateFrom != '') {
            meetsCriteria = new Date(act.FollowUpDate) >= new Date(followupDateFrom);
            if (!meetsCriteria) return false;
        }

        if (followupDateTo != null && followupDateTo != '') {
            meetsCriteria = new Date(act.FollowUpDate) <= new Date(followupDateTo);
            if (!meetsCriteria) return false;
        }

        return meetsCriteria;
    });

    // Finally order by CreatedDate desc
    activities.sort(function (a, b) { return new Date(b.CreatedDate) - new Date(a.CreatedDate); });

    // Begin rendering
    var activityReportContainer = $('#activityContent');
    activityReportContainer.empty();
    var addActivityBtn = $("<button type='text' id='addActivityFromActivityReport' style='cursor:pointer;display:inline-block;vertical-align:middle'><img src='Assets/add_activity.png' style='display:inline-block;vertical-align:middle;margin-right:5px' /><label style='vertical-align:middle'>Add Activity</label></button>");
    activityReportContainer.append(addActivityBtn);
    addActivityBtn.click(function (e) {
        e.preventDefault();
        showDialogAddActivity({
            ActivityTypes: activityTypes, BusinessUnitUsers: businessUnitUsers, PropertyContacts: currentProperty.Contacts
        }, null, function () {
            loadExistingProspectAddActivity(currentProperty, null, function () {
                activityExpanderWidget.open('activityContainer');
                // Populate related-to
                $('#relatedToSelect').empty();
                $('#relatedToSelect').append($("<option />").val(-1).text(''));
                $.each(currentProperty.Contacts, function (idx, el) {
                    $('#relatedToSelect').append($("<option />").val(el.ContactPersonId).text(el.Firstname + ' ' + el.Surname));
                });
                performActivityFiltering();
            });
        });
    });
    activityReportContainer.append("<p />");

    $.each(activities, function (idx, activity) {
        var formattedActivity = buildActivityDisplayItem(activity);
        activityReportContainer.append(formattedActivity);
        activityReportContainer.append("<hr />");
    });

}

function buildContactDetailsDiv() {
    var contactDetailsDiv = $("<div id='contactDetailsDiv' style='display:none;' />");
    return contactDetailsDiv;
}

function buildActivityDisplayItem(activity) {

    function formatLabelValue(label, value) {
        var container = $("<div style='padding:5px' />");
        if (value == null) value = "n/a";

        var isDate = new Date(value) !== "Invalid Date" && !isNaN(new Date(value)) ? true : false;
        if (isDate) {
            var datetime = value.split('T');
            var dateportion = datetime[0];
            var timeportion = datetime[1].substring(0, 5);
            if (timeportion == '00:00') timeportion = '';
            value = dateportion + " " + timeportion;
        }
        value = value.replace(/\n/g, '<br />');

        if (label !== '') label = label + ": ";
        container.append(label).append(value);
        return container[0].outerHTML;
    }

    var containerDiv = $("<div />");

    var activityAndFollowupContainer = $("<div id='activityAndFollowupContainer' style='border: 1px solid white !important;background-color:#F0E68C;' />");
    var activityTypeName = activity.ActivityFollowupTypeId != null ? activity.ActivityFollowupTypeName : activity.ActivityType;
    var activityType = $("<div style='width:50%;display:inline-block;float:left;'>" + formatLabelValue('Activity Type', activityTypeName) + " </div>");
    var followupDate = $("<div style='width:50%;display:inline-block;'>" + formatLabelValue('Follow-up Date', activity.FollowUpDate) + " </div>");
    activityAndFollowupContainer.append(activityType).append(followupDate);

    if (activity.Comment == null) activity.Comment = '';
    var parentActivityIndicator = '';
    if (activity.ParentActivityId != null) {
        parentActivityIndicator = "<br />" + "<label style='color:red'>Activity (" + activity.ActivityLogId + ") related to (" + activity.ParentActivityId + ")</label>";
    } else {
        // else this is parent activity
        parentActivityIndicator = "<br />" + "<label style='color:red'>Activity (" + activity.ActivityLogId + ")</label>";
    }
    var comment = $("<div style='display:block;background-color:#F0E68C;border: 1px solid white !important;'>" + formatLabelValue('', activity.Comment + parentActivityIndicator) + " </div>");

    var createdDateCreatedByContainer = $("<div id='createdDateCreatedByContainer' style='border:1px solid white !important;background-color:#F0E68C;' />");
    var createdDate = $("<div style='width:50%;display:inline-block;float:left;'>" + formatLabelValue('Created', activity.CreatedDate) + " </div>");
    var createdBy = $("<div style='width:50%;display:inline-block;'>" + formatLabelValue('Created By', activity.CreatedBy) + " </div>");
    createdDateCreatedByContainer.append(createdDate).append(createdBy);

    var allocatedTo = $("<div style='display:block;background-color:#F0E68C;border:1px solid white;'>" + formatLabelValue('Allocated To', activity.AllocatedToName) + " </div>");

    var contactPersonName = activity.RelatedToContactPersonName ? activity.RelatedToContactPersonName : "n/a";
    var relatedTo = $("<div style='display:block;background-color:#F0E68C;border:1px solid white;'>" + formatLabelValue('Related To', contactPersonName) + " </div>");

    var viewPropertyContainer = $("<div style='display:block;background-color:white;border: 1px solid white !important;padding:1px'></div>");
    var viewPropertyLeftSpacer = $("<div style='width:50%;display:inline-block;'></div>");
    var viewPropertyRightSpacer = $("<div style='width:50%;display:inline-block;'></div>");
    var viewPropertyBtn = $("<button type='text' id='viewPropertyBtn' style='cursor:pointer;vertical-align:middle;float:right'><img src='Assets/lightstone.png' style='vertical-align:middle;margin-right:5px' /><label style='vertical-align:middle'>View Property</label></button>");
    viewPropertyRightSpacer.append(viewPropertyBtn);
    viewPropertyContainer.append(viewPropertyLeftSpacer).append(viewPropertyRightSpacer);
    viewPropertyBtn.click(function (e) {
        e.preventDefault();
        // Find the suburb this property belongs to:
        var targetSuburb = null;
        targetSuburb = $.grep(suburbsInfo, function (sub) {
            return sub.SuburbId == activity.SeeffAreaId;
        })[0];

        // Load the suburb
        globalZoomLevel = 20;
        $('#suburbLink' + targetSuburb.SuburbId).trigger('click', function () {
            var targetProperty = $.grep(currentSuburb.ProspectingProperties, function (pp) {
                return pp.LightstonePropertyId == activity.LightstonePropertyId;
            })[0];
            var marker = targetProperty.Marker;
            try {
                centreMap(marker.Suburb, marker, true);
                new google.maps.event.trigger(marker, 'click');
            } catch (e) {}
            // Set current proeprty, what about SS?
        });
    });

    containerDiv.append(activityAndFollowupContainer);
    containerDiv.append(comment);
    containerDiv.append(createdDateCreatedByContainer);
    if (activity.FollowUpDate != null && activity.CreatedByGuid != activity.AllocatedTo) {
        containerDiv.append(allocatedTo);
    } 
    containerDiv.append(relatedTo);
    if (currentProperty == null) {
        containerDiv.append(viewPropertyContainer);
    }

    return containerDiv;
}

    // Overridden from MenuBuilder.js
function buildSuburbsSummaryContent() {
    return "The following suburbs are available to you for prospecting.";
}

    // Overridden from MenuBuilder.js
function buildSuburbsSelectionHeaderHtml() {
    var tableHeader = $("<table class='prospectingSuburbsTbl' />");
    tableHeader.append("<tr><td id='th_suburb'>Suburb</td><td id='th_withdetails'>Prospected Properties</td></tr>");

    return tableHeader[0].outerHTML;
}

    // Overridden from MenuBuilder.js
function buildSuburbSelectionHtml() {

    var html = "<div id='suburbsInfoDiv' class='contentdiv'>"
                + "<div id='suburbsSummaryDiv' style='font-size:12px;'>" + buildSuburbsSummaryContent() + "</div><p/>"
                + buildSuburbsSelectionHeaderHtml()
                + "<div id='suburbsDiv'>"
                + buildSuburbsSelectionHtml()
                + "</div></div>";

    return html;
}

    // Overridden from MenuBuilder.js
function buildSuburbsSelectionHtml() {
    var suburbsTbl = $("<table class='prospectingSuburbsContentTbl' />");
    for (var a = 0; a < suburbsInfo.length; a++) {
        var suburbName = suburbsInfo[a].SuburbName;
        var suburbId = suburbsInfo[a].SuburbId;
        var numPropsWithContactDetails = suburbsInfo[a].TotalFullyProspected;
        var tr = $("<tr id='" + "row" + suburbId + "' />");

        var linkId = "suburbLink" + suburbId;
        var radioBtnId = 'suburbRadio' + suburbId;
        var suburbBtn = $("<input type='radio' name='suburbsSelect' id='" + radioBtnId + "'><a href='' id='" + linkId + "'>" + suburbName + "</a></input>");
        $('#contentarea').on('click', '#' + linkId, function (event, callbackFn) {
            suburbSelect(event, $(this).attr('id'), callbackFn);
        });

        $('#contentarea').on('click', '#' + radioBtnId, function (event) {
            suburbSelect(event, $(this).attr('id'));
        });

        var tdSuburb = $("<td style='padding:0px;' />");
        tdSuburb.append(suburbBtn);

        var tdPropertiesWithContactDetails = $("<td style='padding:0px;text-align:center;' />");
        var withDetailsId = "withDetailLabel" + suburbId;
        var propertiesWithContactDetailsCnt = $("<label id='" + withDetailsId + "'>(" + numPropsWithContactDetails + ")</label>");
        tdPropertiesWithContactDetails.append(propertiesWithContactDetailsCnt);

        tr.append(tdSuburb);
        tr.append(tdPropertiesWithContactDetails);
        suburbsTbl.append(tr);
    }

    function suburbSelect(event, itemId, callbackFn) {
        for (var a = 0; a < suburbsInfo.length; a++) {
            clearSuburbBySuburbId(suburbsInfo[a].SuburbId);
        }

        var areaId = itemId.replace('suburbLink', '').replace('suburbRadio', '');
        // Only check radio button if the Suburb link was clicked
        if (itemId.indexOf('suburbLink') > -1) {
            event.preventDefault();
            $('#suburbRadio' + areaId).prop('checked', true);
        }

        // Hack to reset global zoom level
        if (!callbackFn) {
            globalZoomLevel = 13;
        }
        loadSuburb(areaId, false, callbackFn, true);
    }

    return suburbsTbl[0].outerHTML;
}

    function updateSuburbSelectionStats(suburb) {
        var totalPropsWithContacts = 0;
        var totalPropsWithoutContactDetails = 0;

        $.each(suburb.ProspectingProperties, function (idx, prop) {
            var contacts = prop.Contacts ? prop.Contacts : [];
            var hasDetails = false;
            $.each(contacts, function (idx2, c) {
            if ((c.PhoneNumbers && c.PhoneNumbers.length > 0) || (c.EmailAddresses && c.EmailAddresses.length > 0)) {
                hasDetails = true;
            }
            });

        if (hasDetails) {
            totalPropsWithContacts++;
        }
        else {
            totalPropsWithoutContactDetails++;
        }
        });

    var withDetailsLabel = $('#withDetailLabel' +suburb.SuburbId);
    var withoutDetailsLabel = $('#withoutDetailsLabel' +suburb.SuburbId);

    withDetailsLabel.text("(" + totalPropsWithContacts + ")");
    withoutDetailsLabel.text("(" + totalPropsWithoutContactDetails + ")");
}

    function buildContactsResultsDiv(infoPacket) {

        currentTracePSInfoPacket = infoPacket;
        currentTracePSContactRows =[];
        // Check for an error first
        if (infoPacket.ErrorMsg) {
            $('#errLabel').css('display', 'block');
            $('#errLabel').html(infoPacket.ErrorMsg);
        }
        else {
            var div = $('#propertyContactResultsDiv');
            div.css('display', 'block');
            $('#contactPersonNameLabel').text("Name: " +infoPacket.OwnerName);
            $('#contactPersonSurnameLabel').text("Surname: " +infoPacket.OwnerSurname);
            $('#contactIDorCKnoLabel').text("ID number: " +infoPacket.IdNumber);

        var table = $('#propertyOwnerContactInfoTbl');
            table.empty();
            table.append("<tr><th>Contact no.</th><th>Type</th><th>Date</th><th>Use</th></tr>");
            if (infoPacket.ContactRows && infoPacket.ContactRows.length > 0) {
            $.each(infoPacket.ContactRows, function (index, row) {
                var tr = $("<tr />");
                tr.append("<td>" + row.Phone + "</td>");
                tr.append("<td>" + row.Type + "</td>");
                tr.append("<td>" + row.Date + "</td>");

                row.RowId = index;
                var checkbox = buildInputCheckbox("", "contactrow_" +index, "left", 2, 2, true, function () { 
            });

                tr.append("<td>" +checkbox[0].outerHTML + "</td>");

                table.append(tr);
            });
        }

            // Event handler and Go to Edit Person Details pane
            var gotoEditOwnerDetailsBtn = $("<input type='button' id='gotoEditOwnerDetailsBtn' value='Create new contact..' />");
            gotoEditOwnerDetailsBtn.unbind('click').bind('click', function () {
            showMenu('contactdetails');

            currentTracePSContactRows = $.grep(currentTracePSInfoPacket.ContactRows, function (cr) {
                return $('#contactrow_' +cr.RowId).is(':checked');
            });
            updateOwnerDetailsEditorWithBrandNewContact(currentTracePSInfoPacket, currentTracePSContactRows);
            });
        div.append('<p />');
        if (div.find('#gotoEditOwnerDetailsBtn').length == 0) {
            div.append(gotoEditOwnerDetailsBtn);
        }
    }
}

    // Overridden
    // This function should display all info relating to properties, and allow user to select which info to use.
    function updatePropertyInfoMenu(infoPacket) {
        $('#propertyInfoDiv').css('display', 'none');
        $('#propertyContactResultsDiv').css('display', 'none');
        $('#performEnquiryDiv').css('display', 'none');
        $('#errLabel').css('display', 'none');
        $('#lightstoneHistoryDiv').css('display', 'none');
        $('#previousEnquiryLabel').css('display', 'none');

        $('#knownIdTextbox').val('');
        // Clear the menu if theres no marker currently selected
        if (!currentMarker || !currentProperty) {
            return;
        }

        //$('#previousEnquiryLabel').css('display', currentProperty.HasTracePSEnquiry ? 'block' : 'none');

        //displayHistoryIfLightstoneListing();

        // If there is no incoming info packet and the current marker doesn't have any cached info against it then enable the button to do the enquiry.
        if (!infoPacket && !currentMarker.ContactInfoPacket) {
            var enquiryDiv = $('#performEnquiryDiv');
            enquiryDiv.css('display', 'block');
            $('#propertyContactResultsDiv').css('display', 'none');
            $('#propertyInfoDiv').css('display', 'block');

            return;
        }

        // If there is an incoming info packet, update the cached copy for the current marker
        if (infoPacket) {
            currentMarker.ContactInfoPacket = infoPacket;
            $('#availableCreditLabel').text(availableCredit);
        }

        // If there is no incoming info but there is a cached packet already stored in the current marker, use it
        if (!infoPacket && currentMarker.ContactInfoPacket) {
            infoPacket = currentMarker.ContactInfoPacket;
        }

        if (infoPacket.OwnerName || infoPacket.ContactRows.length > 0) {
            $('#performEnquiryDiv').css('display', 'none');
        }
        else {
            $('#performEnquiryDiv').css('display', 'block');
        }
        buildContactsResultsDiv(infoPacket);
        $('#propertyInfoDiv').css('display', 'block');
    }

    function showContentForItem(itemId) {

        if (!itemId) return;

        $.each(menuItems, function (index, item) {
            var itemContentDiv = $('#' + item.MenuItemId + "_content");
            itemContentDiv.css('display', 'none');
        });

        var menuItem = $.grep(menuItems, function (item, index) {
            return item.MenuItemId == itemId;
        })[0];

        var itemContentDiv = $('#' + menuItem.MenuItemId + "_content");
        itemContentDiv.css('display', 'block');
    }


    function appendMenuItemContent(itemContent) {

        var contentArea = $('#contentarea');
        contentArea.append(itemContent);
    }

    function showMenu(activeItem) {
        //menu.accordion({
        //    active: activeItem,
        //    collapsible: true,
        //    heightStyle: "content"
        //}).draggable({ handle: 'h3' });
        $('#' + activeItem).trigger("click");
    }

    function clearMenuSelection() {

        $.each(menuItems, function (index, item) {
            //item.MenuItemDiv.find("img").remove();
            item.MenuItemDiv.css('background-color', '');
        });
    }

    function buildInputCheckbox(itemText, itemId, position, spacesBefore, spacesBetween, checked, clickHandler) {
        var checkedOption = checked ? "checked" : "";

        var ckb = $("<div>" + addNbsp(spacesBefore) + "<input style='cursor:pointer;' type='checkbox' name='checkbox' id='" + itemId + "' " + checkedOption + "/><label for='" + itemId + "'>" + itemText + addNbsp(spacesBetween) + "</label></div>");
        menu.on("click", "#" + itemId, clickHandler);
        return ckb;
    }

    function addNbsp(numberSpaces) {
        var s = '';
        for (var i = 0; i < numberSpaces; i++) {
            s += "&nbsp;";
        }
        return s;
    }