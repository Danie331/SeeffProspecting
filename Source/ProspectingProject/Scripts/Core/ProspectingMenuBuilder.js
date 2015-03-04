var currentTracePSInfoPacket = null;
var currentTracePSContactRows = null;

// Overridden from MenuBuilder.js
function fixElementHeightForIE(elementId) {
    var maxHeight = $(document).height();
    var newValue = 0.9 * maxHeight - 100;
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

    menuItem = createMenuItem("Property Notes", "propertynotes", buildPropertyNotesDiv());
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);

    menuItem = createMenuItem("Lightstone Search", "lightstonesearch", buildSearchMenu());
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);

    showMenu("suburbselector");
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