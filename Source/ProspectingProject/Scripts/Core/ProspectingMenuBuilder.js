var currentTracePSInfoPacket = null;
var currentTracePSContactRows = null;


function createProspectingMenu() {
    var menuItem = createMenuItem("Suburb Selection", "suburbselector", buildSuburbSelectionHtml());
    appendMenuItemContent(menuItem.MenuItemContent);
    fixElementHeightForIE('suburbsDiv');
    menuItems.push(menuItem);

    //menuItem = createMenuItem("Owner Lookup", "ownersearch", buildOwnerLookup());
    //appendMenuItemContent(menuItem.MenuItemContent);
    //menuItems.push(menuItem);

    menuItem = createMenuItem("Contact Details", "contactdetails", buildContactDetailsDiv());
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);

    menuItem = createMenuItem("Property Notes", "propertynotes", buildPropertyNotesDiv());
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);

    menuItem = createMenuItem("Lightstone Search", "lightstonesearch", buildSearchMenu());
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);

    showMenu("suburbselector");
}

function buildSearchMenu() {
    var searchDiv = $("<div id='lightstoneSearchDiv' class='contentdiv' style='padding-right:10px;' />");

    searchDiv.append("<label for='deedTownInput'>Deed Town</label><br />\
                      <input type='text' name='deedTownInput' id='deedTownInput' style='width:100%;' ><p />\
                      <label for='suburbInput'>Suburb</label><br />\
                      <input type='text' name='suburbInput' id='suburbInput' style='width:100%;'><p />\
                      <label for='streetNameInput'>Street Name</label><br />\
                      <input type='text' name='streetNameInput' id='streetNameInput' style='width:100%;'><p />\
                      <label for='streetNoInput'>Street Number</label><br />\
                      <input type='text' name='streetNoInput' id='streetNoInput' style='width:100%;'><p />\
                      <label for='complexNameInput'>Complex Name</label><br />\
                      <input type='text' name='complexNameInput' id='complexNameInput' style='width:100%;'><p />\
                      <label for='erfNoInput'>ERF Number</label><br />\
                      <input type='text' name='erfNoInput' id='erfNoInput' style='width:100%;'><p />\
                      <label for='estateNameInput'>Estate Name</label><br />\
                      <input type='text' name='estateNameInput' id='estateNameInput' style='width:100%;'><p />");

    var searchBtn = $("<input type='button' id='searchLightstoneMatchesBtn' value='Search..' style='float:right;' />");
    searchDiv.append(searchBtn);
    searchDiv.append("<p style='padding:10px;' />");
    var searchResultsDiv = $("<div id='lightstoneResultsDiv' style='height:300px;overflow-y:auto;' />");
    searchDiv.append(searchResultsDiv);

    // Must try to infer province

    searchBtn.click(function () {
        performLightstoneSearch();
    });

    return searchDiv;
}

function createLightstoneSearchResultsDiv(results) {

    var resultsDiv = $('#lightstoneResultsDiv');
    resultsDiv.empty();
    
    if (results.length == 0) {
        resultsDiv.append("No results found.");
        return;
    }

    $.each(results, function (idx, result) {

        if (result.IsSectionalScheme) {
            var propMatches = result.PropertyMatches;
            var ssId = propMatches[0].SSName + "_" + propMatches[0].StreetName;
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
        else {
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

function buildPropertyNotesDiv(prospectingProperty) {
    var notesDiv = $("<div id='propertyNotesDiv' class='contentdiv' style='padding-right:10px;display:none;' />");
    notesDiv.append("Capture comments and notes about this property below:");
    notesDiv.append("<p/>");    
    notesDiv.append("<textarea id='propertyNotesCommentsTextArea' style='width:100%;' rows='32'></textarea>");

    notesDiv.append("<p />");
    var saveBtn = $("<input type='button' id='propertyNotesCommentsSaveBtn' value='Save..' />");
    notesDiv.append(saveBtn);
    saveBtn.click(function () { handleSavePropertyNotesComments(); });

    return notesDiv;
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
    tableHeader.append("<tr><td id='th_suburb'>Suburb</td><td id='th_withdetails'>Properties with contact details</td></tr>");

    return tableHeader[0].outerHTML;
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

function buildOwnerLookup() {
    var propInfoDiv = $("<div id='propertyInfoDiv' />");

    var buttonDiv = $("<div id='performEnquiryDiv' style='font-family: Verdana;font-size: 12px;margin: 10px 10px;display:none;' />");
    buttonDiv.append("Use the button below to perform a search for contact details of the property owner. The cost of such an enquiry, if successful, is 60c (excl. VAT)");
    buttonDiv.append("<p />");

    var availableCreditLabel = $("<label id='availableCreditLabel' />");
    buttonDiv.append("You have ");
    buttonDiv.append(availableCreditLabel);
    buttonDiv.append(" Prospecting credits available.");
    availableCreditLabel.text(availableCredit);

    buttonDiv.append("<p />");
    buttonDiv.append("<label id='previousEnquiryLabel' style='display:none;'>Note: A previous enquiry has been made against this property.</label>");
    buttonDiv.append("<p />");
    var getContactInfoBtn = $("<input type='button' id='getContactInfoBtn' value='Search for owner details' />");
    buttonDiv.append(getContactInfoBtn);
    getContactInfoBtn.click(tryGetContactInfoForProperty);
    buttonDiv.append("<p />");

    buttonDiv.append("- OR -");
    buttonDiv.append("<br />");
    buttonDiv.append("Search using the ID number of a known contact person: <input type='text' id='knownIdTextbox' />");
    buttonDiv.append("<br />");
    buttonDiv.append("<hr /><br />");

    var div = $("<div id='propertyContactResultsDiv' style='font-family: Verdana;font-size: 12px;margin: 10px 10px;display:none;' />");
    div.append("The following details were found for the owner of this property:");
    div.append("<br /><br />");
    div.append("<label id='contactPersonNameLabel'></label>");
    div.append("<br />");
    div.append("<label id='contactPersonSurnameLabel'></label>");
    div.append("<br />");
    div.append("<label id='contactIDorCKnoLabel'></label>");
    div.append("<br /><br />");

    //menu.on('click', '#getContactInfoBtn', tryGetContactInfoForProperty);

    var table = $("<table id='propertyOwnerContactInfoTbl' style='width:100%;' />");
    div.append(table);
    var errLbl = $("<div id='errLabel' style='font-family: Verdana;font-size: 12px;margin: 10px 10px;' />");
    div.append(errLbl);

    var lightstoneHistoryDiv = $("<div id='lightstoneHistoryDiv' style='font-family: Verdana;font-size: 12px;margin: 10px 10px;display:none;' />");

    propInfoDiv.append(buttonDiv);
    propInfoDiv.append(div);
    propInfoDiv.append(lightstoneHistoryDiv);

    return propInfoDiv;
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
        $('#contactIDorCKnoLabel').text("ID number: " + infoPacket.IdCkNo);

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