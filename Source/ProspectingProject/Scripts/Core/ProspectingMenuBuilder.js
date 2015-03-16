var currentTracePSInfoPacket = null;
var currentTracePSContactRows = null;
var activityExpanderWidget = null;
var activityTypes = null;
var businessUnitUsers = null;

// Overridden from MenuBuilder.js
function fixElementHeightForIE(elementId, percHeight) {
    var maxHeight = $(document).height();
    var percentage = percHeight ? percHeight : 0.9;
    var newValue = percentage * maxHeight - 100;
    $('#' + elementId).css('height', newValue + 'px');
}

function createProspectingMenu() {
    var menuItem = createMenuItem("Suburb Selection", "suburbselector", buildSuburbSelectionHtml());
    appendMenuItemContent(menuItem.MenuItemContent);
    fixElementHeightForIE('suburbsDiv');
    menuItems.push(menuItem);

    menuItem = createMenuItem("Contact Details", "contactdetails", buildContactDetailsDiv());
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);

    menuItem = createMenuItem("Activity Report", "activityreport", buildActivityReport(), handleActivityReportClick);
    appendMenuItemContent(menuItem.MenuItemContent);
    fixElementHeightForIE('contentactivityContainer', 0.8);
    menuItems.push(menuItem);

    menuItem = createMenuItem("Lightstone Search", "lightstonesearch", buildSearchMenu());
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);

    showMenu("suburbselector");
}

function handleActivityReportClick() {
    clearActivityReport();
    if (currentProperty) {
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
    }
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

    $.each(results, function (idx, result) {

        if (result.IsSectionalScheme) {
            var propMatches = result.PropertyMatches;
            var ssId = propMatches[0].SS_UNIQUE_IDENTIFIER;
            var resultDiv = $("<div id='" + ssId + "' style='border:1px solid;border-radius:3px;cursor:pointer;' />");
            resultDiv.hover(function () { $(this).css('background-color', '#b0c4de'); }, function () { $(this).css('background-color', 'white'); });
            resultDiv.click(function () {
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
            resultDiv.hover(function () { $(this).css('background-color', '#b0c4de'); }, function () { $(this).css('background-color', 'white'); });
            resultDiv.click(function () {
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
            resultDiv.hover(function () { $(this).css('background-color', '#b0c4de'); }, function () { $(this).css('background-color', 'white'); });
            resultDiv.click(function () {
                showSearchedPropertyOnMap(result);
            });

            var resultContent = "Free-hold: " + fhProperty.StreetOrUnitNo + " " + fhProperty.StreetName + ", " + fhProperty.Suburb + ", " + fhProperty.City;
            resultDiv.append(resultContent);

            resultsDiv.append(resultDiv);
        }        
    });
}

function resetActivityFilters() {
    $('#activityTypeSelect').val(-1);
    $('#allocatedToSelect').val(-1);
    $('#relatedToSelect').val(-1);

    $('#createdFromDate').val('');
    $('#createdToDate').val('');
    $('#followupFromDate').val('');
    $('#followupToDate').val('');
}

function buildActivityReport(prospectingProperty) {

    function buildActivityFilter() {
        var filterDiv = $("<div />");
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

        filterDiv.append(activityTypeFilter).append("<br />").append(allocatedToFilter).append("<br />").append(relatedToFilter).append("<p />").append(createdDatesdiv).append("<br />").append(followupDatesDiv);

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

// This function always targets the ActivityBundle of the current property
function performActivityFiltering() {
    $('#activitiesExpander').css('display', 'block');

    if (activityTypes == null || businessUnitUsers == null) {
        loadActivityLookupData(function (lookupData) {
            activityTypes = lookupData.ActivityTypes;
            businessUnitUsers = lookupData.BusinessUnitUsers;

            // Populate activities
            $('#activityTypeSelect').append($("<option />").val(-1).text(''));
            $.each(activityTypes, function (idx, el) {
                $('#activityTypeSelect').append($("<option />").val(el.Key).text(el.Value));
            });

            // Populate Allocated To
            $('#allocatedToSelect').append($("<option />").val(-1).text(''));
            $.each(businessUnitUsers, function (idx, el) {
                $('#allocatedToSelect').append($("<option />").val(el.UserGuid).text(el.UserName + " " + el.UserSurname));
            });
        });
    }

    var activityTypeFilter = $('#activityTypeSelect').val();
    var allocatedToFilter = $('#allocatedToSelect').val();
    var relatedToFilter = $('#relatedToSelect').val();
    var dateCreatedFrom = $('#createdFromDate').val();
    var dateCreatedTo = $('#createdToDate').val();
    var followupDateFrom = $('#followupFromDate').val();
    var followupDateTo = $('#followupToDate').val();

    var activities = currentProperty.ActivityBundle.Activities;
    // Perform filtering
    activities = $.grep(activities, function (act) {
        var meetsCriteria = true;
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

    // Begin rendering
    var activityReportContainer = $('#activityContent');
    activityReportContainer.empty();
    var addActivityBtn = $("<button type='text' id='addActivityFromActivityReport' style='cursor:pointer;display:inline-block;vertical-align:middle'><img src='Assets/add_activity.png' style='display:inline-block;vertical-align:middle;margin-right:5px' /><label style='vertical-align:middle'>Add Activity</label></button>");
    activityReportContainer.append(addActivityBtn);
    addActivityBtn.click(function (e) {
        e.preventDefault();
        showDialogAddActivity({ ActivityTypes: activityTypes, BusinessUnitUsers: businessUnitUsers, PropertyContacts: currentProperty.Contacts }, null, function () {
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

    function buildActivityDisplayItem(activity) {

        function formatLabelValue(label, value) {
            var container = $("<div style='padding:5px' />");
            if (value == null) value = "n/a";

            var isDate = new Date(value) !== "Invalid Date" && !isNaN(new Date(value)) ? true : false;
            if (isDate) {
                value = value.substring(0,10);
            }
            value = value.replace(/\n/g, '<br />');
            
            if (label !== '') label = label + ": ";
            container.append(label).append(value);
            return container[0].outerHTML;
        }

        var containerDiv = $("<div />");

        var activityAndFollowupContainer = $("<div id='activityAndFollowupContainer' style='border: 1px solid white !important;background-color:#F0E68C;' />");
        var activityType = $("<div style='width:50%;display:inline-block;float:left;'>" + formatLabelValue('Activity Type', activity.ActivityType) + " </div>");
        var followupDate = $("<div style='width:50%;display:inline-block;'>" + formatLabelValue('Follow-up Date', activity.FollowUpDate) + " </div>");
        activityAndFollowupContainer.append(activityType).append(followupDate);

        var comment = $("<div style='display:block;background-color:#F0E68C;border: 1px solid white !important;'>" + formatLabelValue('', activity.Comment) + " </div>");

        var createdDateCreatedByContainer = $("<div id='createdDateCreatedByContainer' style='border:1px solid white !important;background-color:#F0E68C;' />");
        var createdDate = $("<div style='width:50%;display:inline-block;float:left;'>" + formatLabelValue('Created', activity.CreatedDate) + " </div>");
        var createdBy = $("<div style='width:50%;display:inline-block;'>" + formatLabelValue('Created By', activity.CreatedBy) + " </div>");
        createdDateCreatedByContainer.append(createdDate).append(createdBy);

        var allocatedTo = $("<div style='display:block;background-color:#F0E68C;border:1px solid white;'>" + formatLabelValue('Allocated To', activity.AllocatedToName) + " </div>");

        var contactPersonName = "n/a"; 
        if (activity.ContactPersonId) {
            var pp = $.grep(currentProperty.Contacts, function (pers) {
                return pers.ContactPersonId == activity.ContactPersonId;
            });

            if (pp.length > 0) contactPersonName = pp[0].Firstname + ' ' + pp[0].Surname;
        }
        var relatedTo = $("<div style='display:block;background-color:#F0E68C;border:1px solid white;'>" + formatLabelValue('Related To', contactPersonName) + " </div>");

        containerDiv.append(activityAndFollowupContainer);
        containerDiv.append(comment);
        containerDiv.append(createdDateCreatedByContainer);
        containerDiv.append(allocatedTo);
        containerDiv.append(relatedTo);

        return containerDiv;
    }
}

function buildContactDetailsDiv() {
    var contactDetailsDiv = $("<div id='contactDetailsDiv' style='display:none;' />");
    return contactDetailsDiv;
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
        $('#contentarea').on('click', '#' + linkId, function (event) {
            suburbSelect(event, $(this).attr('id'));
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

    function suburbSelect(event, itemId) {
        for (var a = 0; a < suburbsInfo.length; a++) {
            clearSuburbBySuburbId(suburbsInfo[a].SuburbId);
        }
        
        var areaId = itemId.replace('suburbLink', '').replace('suburbRadio', '');
        // Only check radio button if the Suburb link was clicked
        if (itemId.indexOf('suburbLink') > -1) {
            event.preventDefault();
            $('#suburbRadio' + areaId).prop('checked', true);
        }

        loadSuburb(areaId, false);
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

    var withDetailsLabel = $('#withDetailLabel' + suburb.SuburbId);
    var withoutDetailsLabel = $('#withoutDetailsLabel' + suburb.SuburbId);

    withDetailsLabel.text("(" + totalPropsWithContacts + ")");
    withoutDetailsLabel.text("(" + totalPropsWithoutContactDetails + ")");
}

function buildContactsResultsDiv(infoPacket) {

    currentTracePSInfoPacket = infoPacket;
    currentTracePSContactRows = [];
    // Check for an error first
    if (infoPacket.ErrorMsg) {
        $('#errLabel').css('display', 'block');
        $('#errLabel').html(infoPacket.ErrorMsg);
    }
    else {
        var div = $('#propertyContactResultsDiv');       
        div.css('display', 'block');
        $('#contactPersonNameLabel').text("Name: " + infoPacket.OwnerName);
        $('#contactPersonSurnameLabel').text("Surname: " + infoPacket.OwnerSurname);
        $('#contactIDorCKnoLabel').text("ID number: " + infoPacket.IdNumber);

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
                var checkbox = buildInputCheckbox("", "contactrow_" + index, "left", 2, 2, true, function () { });

                tr.append("<td>" + checkbox[0].outerHTML + "</td>");

                table.append(tr);
            });
        }

        // Event handler and Go to Edit Person Details pane
        var gotoEditOwnerDetailsBtn = $("<input type='button' id='gotoEditOwnerDetailsBtn' value='Create new contact..' />");
        gotoEditOwnerDetailsBtn.unbind('click').bind('click', function () {            
            showMenu('contactdetails');

            currentTracePSContactRows = $.grep(currentTracePSInfoPacket.ContactRows, function (cr) {
                return $('#contactrow_' + cr.RowId).is(':checked');
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