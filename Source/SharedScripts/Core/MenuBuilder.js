
function buildOptionsForOrdinaryUser() {

    var canEditAtLeastOneSuburb = userCanEditAtLeastOneSuburb();
    var hasUnfatedTransactions = !allListingsFated();
    suburbsInfo.CanFilter = !hasUnfatedTransactions;
    showMenuForUser(canEditAtLeastOneSuburb, hasUnfatedTransactions);
}

function showMenuForUser(userCanEdit, unfatedTransactions) {

    var enableFilteringOptions;
    switch (userCanEdit) {
        
        case true:
            switch (unfatedTransactions) {
                case true:
                    showPopupUnfatedTransactions("popupUnfatedAreas");
                    enableFilteringOptions = false;
                    break;
                case false:
                    enableFilteringOptions = true;
                    break;
                default: break;
            }
            break;
        case false:
            switch (unfatedTransactions) {
                case true:
                    showPopupAdminRequired();
                    break;
                case false:
                    enableFilteringOptions = true;
                    break;
                default: break;
            }
            break;
        default: break;
    }

    var menuItem = createMenuItem("Suburb Selection", "suburbselector", buildSuburbSelectionHtml());
    appendMenuItemContent(menuItem.MenuItemContent);
    fixElementHeightForIE('suburbsDiv');
    //adjustSuburbTableWidths();
    menuItems.push(menuItem);

    menuItem = createMenuItem("Filter / Summary", "filterandsummary", buildFilterItemsAndSummaryInfo());
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);

    menuItem = createMenuItem("Property Information", "propertyinfo", buildPropertyInfoHtml());
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);

    menuItem = createMenuItem("Search", "search", buildSearchMenuHtml());
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);

    showFilteringOptions(enableFilteringOptions);
    generateStatisticsMenu(enableFilteringOptions);

    showMenu("suburbselector");
}

function fixElementHeightForIE(elementId) {

    // IE bug fix: height:100% of div does not work unless all parent divs heights are also 100%
    if (detectIE()) {
        var maxHeight = $(document).height();
        var newValue = 0.9 * maxHeight - 100;
        $('#' + elementId).css('height', newValue + 'px');
    }
}

function buildUtils() {
    var html = $("<div class='contentdiv'></div>");
    html.append("<input id='printmapBtn' type='button' value='Print map'></button>");

    menu.on('click', '#printmapBtn', function () {
        $('#closepanelbutton').trigger('click');
        window.print();
    });

    return html[0].outerHTML;
}

function buildLicenseSummaryHtml() {

    var tablePercentage = $("<table id='marketSharePercentage' style='width:100%;' />");   
    var tableValue = $("<table id='marketShareValue' style='width:100%;' />");
    var tableMarketshareTypes = $("<table id='tableMarketshareTypes' style='width:100%;' />");

    var licSummaryDiv = $("<div id='licSummaryDiv' style='font-family: Verdana;font-size: 12px;margin: 10px 10px;'></div>");    
    //licSummaryDiv.append("<hr>");
    licSummaryDiv.append(tableMarketshareTypes);
    licSummaryDiv.append("<br /><br /><hr>");
    licSummaryDiv.append("<div style='font-size:12px;'>Please note the values below are for primary residential listings only and calculated according to your filter selection criteria.</div><hr><br />");
    //licSummaryDiv.append("<br /><br /><hr>");
    licSummaryDiv.append(tablePercentage);
    licSummaryDiv.append("<br />");
    licSummaryDiv.append(tableValue);

    return licSummaryDiv[0].outerHTML;
}

function generateStatisticsMenu(canShow) {
    var licSummaryDiv = $('#licSummaryDiv');
    licSummaryDiv.css('display', canShow ? 'block' : 'none');

    var tablePercentage = licSummaryDiv.find('#marketSharePercentage');
    tablePercentage.empty();
    tablePercentage.append("<tr><th>Agency</th><th>No. Registrations</th><th>% Registrations</th></tr>");

    var tableValue = licSummaryDiv.find('#marketShareValue');
    tableValue.empty();
    tableValue.append("<tr><th>Agency</th><th>Total value</th><th>% value</th></tr>");

    var tableMarketshareTypes = licSummaryDiv.find('#tableMarketshareTypes');
    tableMarketshareTypes.empty();
    tableMarketshareTypes.append("<tr><th>Property type</th><th>No. Registrations</th><th>% Registrations</th></tr>");

    // Build some stats
    var totalListingsToCalculate = getTotalVisibleListings(true);
    var statistics = buildListingsByAgencyStatistics();
    var grandTotal = 0;
    $.each(statistics, function (index, stat) {

        var statPerc = 0;
        if (totalListingsToCalculate > 0) {
            statPerc = stat.Value / totalListingsToCalculate * 100;
        }

        var tr = $("<tr />");
        tr.append(createTd(stat.AgencyName));
        tr.append(createTd(stat.Value));
        tr.append(createTd(parseFloat(statPerc).toFixed(2) + " %"));

        tablePercentage.append(tr);
        grandTotal += stat.Value;
    });
    var totalTr = $("<tr />");
    totalTr.append(createTd("Total", "font-weight:800"));
    totalTr.append(createTd(grandTotal));
    tablePercentage.append(totalTr);

    statistics = buildTotalValueStatistics();
    var totalValueAllAgencies = getTotalMarketValueByAgency();
    grandTotal = 0;
    $.each(statistics, function (index, stat) {
        var tr = $("<tr />");
        tr.append(createTd(stat.AgencyName));
        tr.append(createTd(formatRandValue(stat.Value / 1000) + "k"));
        tr.append(createTd(parseFloat(stat.Value / totalValueAllAgencies * 100).toFixed(2) + " %"));

        tableValue.append(tr);
        grandTotal += stat.Value / 1000;
    });
    totalTr = $("<tr />");
    totalTr.append(createTd("Total", "font-weight:800"));
    totalTr.append(createTd(formatRandValue(grandTotal) + "k"));
    tableValue.append(totalTr);

    statistics = buildListingsByMarketShareTypes();
    var totalVisibleListings = getTotalVisibleListings(false);
    grandTotal = 0;
    $.each(statistics, function (index, stat) {
        var tr = $("<tr />");

        tr.append(createTd(stat.PropertyType));
        tr.append(createTd(stat.Value));
        tr.append(createTd(parseFloat(stat.Value / totalVisibleListings * 100).toFixed(2) + " %"));

        tableMarketshareTypes.append(tr);
        grandTotal += stat.Value;
    });
    totalTr = $("<tr />");
    totalTr.append(createTd("Total", "font-weight:800"));
    totalTr.append(createTd(grandTotal));
    tableMarketshareTypes.append(totalTr);
}

function showFilteringOptions(enableFilteringOptions) {
    var filteringOptionsDiv = $('#filteroptionsdiv');
    filteringOptionsDiv.css('display', enableFilteringOptions ? 'block' : 'none');
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
    $('#' + activeItem).trigger( "click" );
}

// 
// Filtering
function buildFilterItemsAndSummaryInfo() {
    var fh = buildInputCheckbox("FH", "FH_filter", "left", 2, 2, true, handleFilterItemClick);
    var ss = buildInputCheckbox("SS", "SS_filter", "right", 2, 2, true, handleFilterItemClick);
    var res = buildInputCheckbox("Res", "R_filter", "left", 2, 3, true, handleFilterItemClick);
    var comm = buildInputCheckbox("Com", "C_filter", "left", 2, 3, true, handleFilterItemClick);
    var agri = buildInputCheckbox("Agri", "A_filter", "left", 2, 3, true, handleFilterItemClick);
    var dev = buildInputCheckbox("Dev", "D_filter", "left", 2, 3, true, handleFilterItemClick);
    var other = buildInputCheckbox("Other", "O_filter", "left", 2, 3, true, handleFilterItemClick);
    var pending = buildInputCheckbox("Pending", "P_filter", "left", 2, 3, true, handleFilterItemClick);

    //var _2011 = buildInputCheckbox("2011", "2011_filter", "left", 2, 2, true, handleFilterItemClick);
    var _2012 = buildInputCheckbox("2012", "2012_filter", "left", 2, 2, true, handleFilterItemClick);
    var _2013 = buildInputCheckbox("2013", "2013_filter", "left", 2, 2, true, handleFilterItemClick);
    var _2014 = buildInputCheckbox("2014", "2014_filter", "left", 2, 2, true, handleFilterItemClick);
    var _2015 = buildInputCheckbox("2015", "2015_filter", "left", 2, 2, true, handleFilterItemClick);

    var seeffCurrentForSale = buildInputCheckbox("Seeff for sale", "forsale_filter", "left", 2, 2, true, handleFilterItemClick);
    var seeffCurrentForRent = buildInputCheckbox("Seeff for rent", "forrent_filter", "left", 2, 2, true, handleFilterItemClick);

    var withAgencyAssigned = buildInputCheckbox("With agency assigned", "withagencyassigned_filter", "left", 2, 2, true, handleFilterItemClick);
    var withoutAgencyAssigned = buildInputCheckbox("Without agency assigned", "withoutagencyassigned_filter", "left", 2, 2, true, handleFilterItemClick);

    var filterOptionsHtml = "<div class='contentdiv' id='filteroptionsdiv'>" +                 
                 "<div style='display:inline-block;border-right:1px solid #000;float:left;margin: 0px 10px;'>" + res[0].outerHTML + comm[0].outerHTML + agri[0].outerHTML + dev[0].outerHTML + other[0].outerHTML + pending[0].outerHTML + "</div>" +
                 "<div style='display:inline-block;border-right:1px solid #000;float:left;margin: 0px 5px;'>" + _2012[0].outerHTML + _2013[0].outerHTML + _2014[0].outerHTML + _2015[0].outerHTML + "</div>" +
                  "<div style='display:inline-block;float:left;border-right:1px solid #000;margin: 0px 5px;'>" + fh[0].outerHTML + ss[0].outerHTML + "</div>" +
                  "<div style='display:inline-block;float:left;border-right:1px solid #000;margin: 0px 5px;'>" + seeffCurrentForSale[0].outerHTML + seeffCurrentForRent[0].outerHTML + "</div>" +
                  "<div style='display:inline-block;float:left;border-right:1px solid #000;margin: 0px 5px;'>" + withAgencyAssigned[0].outerHTML + withoutAgencyAssigned[0].outerHTML + "</div>" +
                  "</div>";

    filterSets.push(["FH_filter", "SS_filter"]);
    filterSets.push(["R_filter", "C_filter", "A_filter", "D_filter", "O_filter", "P_filter"]);
    filterSets.push(["2012_filter", "2013_filter", "2014_filter", "2015_filter"]);
    // The filter set below is handled differently to the rest. See filtering.js
    filterSets.push(["forsale_filter", "forrent_filter"]);
    filterSets.push(["withagencyassigned_filter", "withoutagencyassigned_filter"]);

    return filterOptionsHtml + "<div style='float:left;'><hr />" + buildLicenseSummaryHtml() + "</div>";
}

function buildSuburbSelectionHtml() {

    var html = "<div id='suburbsInfoDiv' class='contentdiv'>"
                + "<div id='suburbsSummaryDiv' style='font-size:12px;'>" + buildSuburbsSummaryContent() + "</div><p/>"
                + buildSuburbsSelectionHeaderHtml()
                + "<div id='suburbsDiv' style='height:90%;overflow: auto;'>"
                + buildSuburbsSelectionHtml()
                + "</div></div>";

    return html;
}

function buildSuburbsSelectionHtml() {
    var suburbsTbl = $("<table class='suburbsContentTbl' />");

    for (var a = 0; a < suburbsInfo.length; a++) {
        var fatedCount = suburbsInfo[a].FatedCount;
        var unfatedCount = suburbsInfo[a].UnfatedCount;
        var seeffCurrentCount = suburbsInfo[a].SeeffCurrentListingCount;
        var suburbName = suburbsInfo[a].SuburbName;

        if (fatedCount == null) {
            fatedCount = 'n/a';
        }
        if (unfatedCount == null) {
            unfatedCount = 'n/a';
        }

        var suburbId = suburbsInfo[a].SuburbId;
        var tr = $("<tr id='" + "row" + suburbId + "' />");

        var fatedElement = buildInputCheckbox('(' + fatedCount + ')', "fated" + suburbId, "left", 2, 2, false, handleSuburbItemSelect);
        var tdFated = "<td>" + fatedElement[0].outerHTML + "</td>";

        var unfatedElement = buildInputCheckbox('(' + unfatedCount + ')', "unfated" + suburbId, "left", 2, 2, false, handleSuburbItemSelect);
        var tdUnfated = "<td>" + unfatedElement[0].outerHTML + "</td>";

        var seeffCurrentListingsElement = buildInputCheckbox('(' + seeffCurrentCount + ')', "seeffcurrentlistings" + suburbId, "left", 2, 2, false, handleSuburbItemSelect);
        var tdSeeffCurrent = "<td>" + seeffCurrentListingsElement[0].outerHTML + "</td>";

        var suburbIdString = "suburb" + suburbId;
        var suburbBtn = $("<a href='' id='" + suburbIdString + "'>" + suburbName + "</a>");
        $('#contentarea').on('click', '#' + suburbIdString, function (event) {

            event.preventDefault();
            // select both fated/unfated, and center the map
            var areaId = $(this).attr('id').replace('suburb', '');

            loadDataForSuburb(areaId, true, true, true);
            var suburb = getSuburbById(areaId);
            if (suburb) {
                centreMap(suburb);
            }
        });

        var tdSuburb = $("<td style='padding:0px;' />");
        tdSuburb.append(suburbBtn);

        tr.append(tdFated);
        tr.append(tdUnfated);
        tr.append(tdSeeffCurrent);
        tr.append(tdSuburb);        
        suburbsTbl.append(tr);
    }

    return suburbsTbl[0].outerHTML;
}

function buildSuburbsSelectionHeaderHtml() {

    var tableHeader = $("<table class='suburbsHeaderTbl' />");
    tableHeader.append("<tr><td id='th_fated'>Fated</td><td id='th_unfated'>Unfated</td> <td id='th_current_seeff_listings'>Seeff.com Listings</td> <td id='th_suburb'>Suburb</td></tr>");

    return tableHeader[0].outerHTML;
}

function buildSuburbsSummaryContent() {

    var totalListings = 0;
    var totalUnfated = 0;
    for (var a = 0; a < suburbsInfo.length; a++) {

        totalListings += suburbsInfo[a].FatedCount + suburbsInfo[a].UnfatedCount;
        totalUnfated += suburbsInfo[a].UnfatedCount;
    }

    return "You have in total " + totalUnfated + " unfated listings out of a total of " + totalListings + ".";
}

function buildInputCheckbox(itemText, itemId, position, spacesBefore, spacesBetween, checked, clickHandler) {
    var checkedOption = checked ? "checked" : "";

    var ckb = $("<div>" + addNbsp(spacesBefore) + "<input style='cursor:pointer;' type='checkbox' name='checkbox' id='" + itemId + "' " + checkedOption  + "/><label for='" + itemId + "'>" + itemText + addNbsp(spacesBetween) + "</label></div>");
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

// Area Market Share
function buildMarketShareTypesHtml(selectedValue) {
    var marketShareTypes = loadMarketShareTypes();
    var typesHtml = '';
    if (!selectedValue || selectedValue == '') {
        typesHtml = "<option id='nothing' value='' />";
    }

    for (var t = 0; t < marketShareTypes.length; t++) {
        var type = marketShareTypes[t].substring(0, 1);

        typesHtml += type === selectedValue ? "<option id='" + type + "' value='" + type + "' selected>" + marketShareTypes[t] + "</option>"
                                            : "<option id='" + type + "' value='" + type + "'>" + marketShareTypes[t] + "</option>";
    }

    return typesHtml;
}

function buildAgencyNamesList(listing) {   

    // Get agencies available for current suburb
    var currentSuburb = markerIsSelected() ? infowindow.Marker.Suburb : null;
    
    var agenciesHtml = '';

    var privateSale = $.grep(allAgencies, function (agency, index) {
        return agency.agency_name == 'Private sale';
    })[0];

    if (listing.Agency == privateSale.agency_id) {
        agenciesHtml = "<option id='nothing' value='' /><option id='AgencyId" + privateSale.agency_id + "' value='" + privateSale.agency_id + "' selected>" + privateSale.agency_name + "</option>";
    }
    else {
        agenciesHtml += "<option id='nothing' value='' /><option id='AgencyId" + privateSale.agency_id + "' value='" + privateSale.agency_id + "'>" + privateSale.agency_name + "</option>";
    }

    var auction = $.grep(allAgencies, function (agency, index) {
        return agency.agency_name == 'Auction';
    })[0];

    if (listing.Agency == auction.agency_id) {
        agenciesHtml += "<option id='AgencyId" + auction.agency_id + "' value='" + auction.agency_id + "' selected>" + auction.agency_name + "</option>";
    }
    else {
        agenciesHtml += "<option id='AgencyId" + auction.agency_id + "' value='" + auction.agency_id + "'>" + auction.agency_name + "</option>";
    }

    if (currentSuburb && currentSuburb.SelectedAgencies) {
        var sortedAgencies = getSortedAgencies(currentSuburb.SelectedAgencies);
        for (var a = 0; a < currentSuburb.SelectedAgencies.length; a++) {
            var agencyId = currentSuburb.SelectedAgencies[a];
            var agencyName = getAgencyName(agencyId);

            agenciesHtml += agencyId == listing.Agency ? "<option id='AgencyId" + agencyId + "' value='" + agencyId + "' selected>" + agencyName + "</option>"
                                                   : "<option id='AgencyId" + agencyId + "' value='" + agencyId + "'>" + agencyName + "</option>";
        }

        return agenciesHtml;
    }

    return '';
}

function getSortedAgencies(agencyIDs) {

    return agencyIDs.sort(function (a, b) {
        var agency1 = getAgencyName(a);
        var agency2 = getAgencyName(b);
        if (agency1 == agency2) {
            return 0;
        }

        return agency1 > agency2 ? 1 : -1;
    });
}

function buildSearchMenuHtml() {
    var header = $('<div style="font-family: Verdana;font-size: 12px;margin: 10px 10px;">\
                  Enter one or more values in the fields below to search for matching properties. <br />\
                  Please note that the search results will be limited to the suburbs under your license.</div>');
    header.append('<hr />');
    
    var searchDiv = $("<div id='lightstoneSearchDiv' class='contentdiv' style='padding-right:10px;font-size:12px' />");
    searchDiv.append("<label class='fieldAlignmentShortWidth' for='deedTownInput'>Deed Town</label>\
                      <input type='text' name='deedTownInput' id='deedTownInput' size='60' ><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='suburbInput'>Suburb</label>\
                      <input type='text' name='suburbInput' id='suburbInput' size='60'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='streetNameInput'>Street Name</label>\
                      <input type='text' name='streetNameInput' id='streetNameInput' size='60'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='streetNoInput'>Street Number</label>\
                      <input type='text' name='streetNoInput' id='streetNoInput' size='60' disabled><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='complexNameInput'>Complex Name</label>\
                      <input type='text' name='complexNameInput' id='complexNameInput' size='60'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='estateNameInput'>Estate Name</label>\
                      <input type='text' name='estateNameInput' id='estateNameInput' size='60'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='erfNoInput'>ERF Number</label>\
                      <input type='text' name='erfNoInput' id='erfNoInput' size='30'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='portionNoInput'>Portion Number</label>\
                      <input type='text' name='portionNoInput' id='portionNoInputBox' size='30' disabled><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='propertyIdInput'>Property ID</label>\
                      <input type='text' name='propertyIdInput' id='propertyIdInput' size='30'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='titleDeedInput'>Title Deed</label>\
                      <input type='text' name='titleDeedInput' id='titleDeedInput' size='30'><p style='margin:3px;' />\
                      <hr />\
                      <label class='fieldAlignmentShortWidth' for='buyerNameInput'>Name of buyer</label>\
                      <input type='text' name='buyerNameInput' id='buyerNameInput' size='30'><p style='margin:3px;' />\
                      <label class='fieldAlignmentShortWidth' for='sellerNameInput'>Name of seller</label>\
                      <input type='text' name='sellerNameInput' id='sellerNameInput' size='30'><p style='margin:3px;' /><p/>\
                      <input id='findMatches' value='Find matches' type='button' style='cursor:pointer;'></input>");

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

    // Street number only allowed when street name also given
    $('#contentarea').on('keyup', '#streetNameInput', function () {
        var str = $(this).val();
        if (str.length > 0) {
            $('#streetNoInput').removeAttr("disabled");
        } else {
            $('#streetNoInput').val('');
            $('#streetNoInput').attr('disabled', 'disabled');
        }
    });

    menu.on('click', '#findMatches', initSearch);

    searchDiv.append("<p style='padding:10px;' />");
    var searchResultsDiv = $("<div id='lightstoneResultsDiv' style='height:250px;overflow-y:auto;' />");
    searchDiv.append(searchResultsDiv);

    return header[0].outerHTML + searchDiv[0].outerHTML;
}

function createSearchResults(results) {

    var resultsDiv = $('#lightstoneResultsDiv');
    resultsDiv.empty();

    if (results.Count == 0) {
        resultsDiv.append("No results found.");
        return;
    }

    if (results.Count > 50) {
        resultsDiv.append("More than 50 results were found. Please refine your search criteria and try again...");
        return;
    }

    $.each(results.Data, function (idx, result) {

        if (result.ss_fh == "SS") {
            var ssId = "propid_" + result.property_id;
            var resultDiv = $("<div id='" + ssId + "' style='border:1px solid;border-radius:3px;cursor:pointer;' />");
            resultDiv.hover(function () { $(this).css('background-color', '#b0c4de'); }, function () { $(this).css('background-color', 'white'); });
            resultDiv.click(function () {
                openResultOnMap(result);
            });

            var resultContent = "Unit " + result.street_or_unit_no + " in " + result.property_address + " (Property ID: " + result.property_id + ")";                
            resultDiv.append(resultContent);
            resultsDiv.append(resultDiv);
        }       
        else if (result.ss_fh == "FH") { 
            var fhId = "propid_" + result.property_id;
            var resultDiv = $("<div id='" + fhId + "' style='border:1px solid;border-radius:3px;cursor:pointer;' />");
            resultDiv.hover(function () { $(this).css('background-color', '#b0c4de'); }, function () { $(this).css('background-color', 'white'); });
            resultDiv.click(function () {
                openResultOnMap(result);
            });

            var resultContent = result.street_or_unit_no + " " + result.property_address + " (Property ID: " + result.property_id + ")";
            resultDiv.append(resultContent);
            resultsDiv.append(resultDiv);
        }
    });
}

function openResultOnMap(searchResult) {

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Finding property...</p>' });
    // 'Click' the correct suburb
    loadDataForSuburb(searchResult.seeff_area_id + '', true, true, false, function () {

        $.unblockUI();

        var suburb = getSuburbById(searchResult.seeff_area_id);

        var target = $.grep(suburb.Listings, function (item) {
            return item.PropertyId == searchResult.property_id;
        })[0];
        // trigger the marker click event.
        openInfoWindow(target.Marker);
        var pos = calcMapCenterWithOffset(target.LatLong.Lat, target.LatLong.Lng, -200, 0);
        if (pos) {
            map.setCenter(pos);
        }
        else {
            map.setCenter(latLng);
        }
    });

    //setTimeout(function () {
    //    // Find the listing
    //    var suburb = getSuburbById(searchResult.seeff_area_id);
    //    // Find its marker
    //    var target = $.grep(suburb.Listings, function (item) {
    //        return item.PropertyId == searchResult.property_id;
    //    })[0];
    //    // trigger the marker click event.
    //    google.maps.event.trigger(target.Marker, 'click');
    //}, 1000);
}

function buildPropertyInfoHtml() {
    var html = "<div id='propInfoDiv' class='contentdiv' style='overflow: auto; display:none;margin: 10px 10px;' ></div>";
    return html;
}

function createTd(content, styles) {
    var css = styles ? 'style="' + styles + '"' : '';
    return "<td " + css + ">" + content + "</td>";
}

function updatePropertyInfoMenu(salesForListing) {
      
    var propInfoDiv = $('#propInfoDiv');
    if (salesForListing.length == 0) {
        propInfoDiv.css('display', 'none');
        return;
    }

    loadAgenciesForSuburb(function () { // Load all the agencies available for this suburb.        

        function createCombo(id, content, editable, selectionChangedHandler) {
            var disabledAttr = editable ? "" : "disabled";
            var combo = $("<select id='" + id + "'" + disabledAttr + " style='width:100px;'>" +
                        content +
                       "</select>");

            if (selectionChangedHandler) {
                propInfoDiv.on('change', '#' + id, selectionChangedHandler);
            }
            return combo[0].outerHTML;
        }

        propInfoDiv.off('change');
        propInfoDiv.empty();

        if (salesForListing[0].CanEdit) {
            var manageAgenciesBtnId = "propID" + salesForListing[0].PropertyId;
            propInfoDiv.append("<div style='display:inline-block;float:right;'><button id='" + manageAgenciesBtnId + "'>Manage Agencies</button></div>");
            $('#' + manageAgenciesBtnId).click(handleManageAgenciesClick);

            propInfoDiv.append("<div style='display:inline-block;width:100%;'><hr /></div>");
        }
        var table = $("<table style='width:100%;' />");
        table.append("<tr><th>Reg Date</th><th>Sale price</th><th>Agency</th><th>Market Share</th></tr>");

        $.each(salesForListing, function (index, listing) {
            // Add record to the table
            var tr = $("<tr />");
            tr.append(createTd(formatDate(listing.RegDate)));
            tr.append(createTd(formatRandValue(listing.PurchPrice)));

            var agencyComboHtml = '';
            if (listing.SeeffDeal) {
                var seeffId = getAgencyId('Seeff');
                var seeffHtml = '<option id="AgencyId' + seeffId + '" value="' + seeffId + '" selected>Seeff</option>';
                agencyComboHtml = createCombo("AgencyCombo" + listing.UniqueId, seeffHtml, false, null);
            }
            else {
                agencyComboHtml = createCombo("AgencyCombo" + listing.UniqueId, buildAgencyNamesList(listing), listing.CanEdit, handleAgencySelectionChanged);
            }
            tr.append(createTd(agencyComboHtml));
            tr.append(createTd(createCombo("MarketShareCombo" + listing.UniqueId, buildMarketShareTypesHtml(listing.MarketShareType), listing.CanEdit, handleMarketShareSelectionChanged)));
            table.append(tr);
        });

        //table.append("<tr><td style='border:0;'><button id='a' >Manage Agencies</button><td><td /><td /><td /></tr>");
        propInfoDiv.append(table);        
        propInfoDiv.css('display', 'block');

        appendContentIfMultiplePropsUnderSale(salesForListing);
    });    
}

function appendContentIfMultiplePropsUnderSale(listings) {
    var propInfoDiv = $('#propInfoDiv');
    // Check whether this property was sold together with one (or more) properties under the same transaction (ie same title deed and purch_price)
    var transactionHasMultipleProps = $.grep(listings, function (t) { return t.SaleIncludesOtherProperties == true; }).length > 0;
    if (transactionHasMultipleProps) {
        var html = '<div style="display: inline-block;padding-top:75px;">\
                            \<label>This sale includes additional properties or units. If you wish to merge (link) these items\
                           under the parent property, select the item\'s checkbox in the \'Linked to parent\' column. \
                           The linked items will not affect your market share.\
                   \</label><br /><br />' +
                   buildLinkedListingsTable(listings)
                   + '</div>';
        propInfoDiv.append(html);
    }

    function buildLinkedListingsTable(listings) {
        var firstListing = $.grep(listings, function (list) { return list.SaleIncludesOtherProperties == true;})[0]; // We don't care about the others as they aren't relevant to *this* sale.
        var suburb = firstListing.Marker.Suburb;
        // Get the listings linked to this one by title deed and purchase price
        var linkedListings = $.grep(suburb.Listings, function (listing) {
            return listing.TitleDeedNo == firstListing.TitleDeedNo && listing.PurchPrice == firstListing.PurchPrice
                                                                    && listing.PurchPrice != null;
        });
        // Sort the listings by property Id ascending.
        linkedListings.sort(function (x, y) { return x.PropertyId - y.PropertyId; });
        // Build the table. The first item in the list (smallest propertyId) will be designated as the parent.
        var parentListing = linkedListings.splice(0,1)[0];
        var table = $('<table id="linkedListingsTable" />');
        var headerRow = $('<tr />');
        headerRow.append('<th>Linked to parent</th><th>Property ID</th><th>Erf no.</th><th>Erf/unit size</th>');
        table.append(headerRow);

        // Add the parent
        var parentRow = $('<tr />');
        parentRow.append('<td>Parent</td><td>' + parentListing.PropertyId + '</td><td>' + parentListing.ErfNo + '</td><td>' + parentListing.ErfOrUnitSize + '</td>');
        table.append(parentRow);

        $.each(linkedListings, function (index, item) {
            var tr = buildRowItem(item);
            table.append(tr);
        });

        return table[0].outerHTML;

        function buildRowItem(listing) {
            var tr = $('<tr />');

            var itemId = "linkListing_" + listing.UniqueId;
            var isChecked = listing.ParentPropertyId == parentListing.PropertyId;
            var propId = listing.PropertyId;
            var erfNo = listing.ErfNo;
            var erfOrUnitSize = listing.ErfOrUnitSize;

            var checkedAttribute = isChecked ? 'checked' : '';
            var mergedCheckbox = $('<input id="' + itemId + '" name="' + itemId + '" type="checkbox" ' + checkedAttribute + ' />');
            //if (isChecked) {
            //    mergedCheckbox.append('(merged)');
            //}

            propInfoDiv.on('change', '#' + itemId, function () {
                //var uniqueId = itemId.replace('linkListing_', '');
                var parentPropId = this.checked ? parentListing.PropertyId : null;
                updateListingLinkedParent(listing, parentPropId);
            });

            tr.append('<td>' + mergedCheckbox[0].outerHTML + '</td>' + '<td>' + propId + '</td>' + '<td>' + erfNo + '</td>' + '<td>' + erfOrUnitSize + '</td>');
            return tr;
        }
    }
}

function updateCheckboxItemForLinkedListing(listing, parentPropId) {
    //var checkboxItem = $('#linkListing_' + listing.UniqueId);
    //if (parentPropId) {
    //    checkboxItem.text('(merged)');
    //} else {
    //    checkboxItem.text('');
    //}
}

function handleAgencySelectionChanged() {
    
    var combo = $(this);
    var uniqueId = combo.attr('id').replace("AgencyCombo", "");
    var selectedValue = combo.val();
    updateAgencyForListing(uniqueId, selectedValue ? selectedValue : -1);
}

function handleMarketShareSelectionChanged() {
    
    var combo = $(this);
    var uniqueId = combo.attr('id').replace("MarketShareCombo", "");
    var selectedValue = combo.val();
    updateMarketShareForListing(uniqueId, selectedValue);
}

function handleManageAgenciesClick(event) {
    //var propertyId = $(this).attr('id').replace("propID", "");
    event.preventDefault();
    buildManageAgenciesScreen();
}

// New panel menu
function showContentForItem(itemId) {

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

function clearMenuSelection() {

    $.each(menuItems, function (index, item) {
        //item.MenuItemDiv.find("img").remove();
        item.MenuItemDiv.css('background-color', '');
    });
}

function updateSuburbStats(suburb) {

    if (!suburb) return;

    suburb.FatedCount = 0;
    suburb.UnfatedCount = 0;
    for (var i = 0; i < suburb.Listings.length; i++) {

        if (suburb.Listings[i].IsCurrentSeeffListing) {
            continue; // do not take current listings into consideration
        }

        if (suburb.Listings[i].Fated) {
            suburb.FatedCount += 1;
        }
        else {
            suburb.UnfatedCount += 1;
        }
    }

    // Fating summary
    var suburbsSummaryDiv = $('#suburbsSummaryDiv');
    suburbsSummaryDiv.empty();
    suburbsSummaryDiv.append(buildSuburbsSummaryContent());

    updateInputCheckbox("fated" + suburb.SuburbId, "(" + suburb.FatedCount + ")");
    updateInputCheckbox("unfated" + suburb.SuburbId, "(" + suburb.UnfatedCount + ")");
}

function updateInputCheckbox(id, value) {

    $("label[for='" + id  + "']").text(value + '');
}