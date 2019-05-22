
var app = app || {};

app.buildPropertyListingMenu = function () {
    var container = $("<div id='propertyListingContainer' class='contentdiv' style='display:none' />");
    container.empty();
    return container;
}

app.formatAddress = function () {
    if (currentProperty.SS_FH == "FH") {
        return `${currentProperty.StreetOrUnitNo} ${currentProperty.PropertyAddress}`;
    }
    if (currentProperty.SS_FH == "SS") {
        return `Unit ${currentProperty.Unit} ${currentProperty.SSDoorNo ? "(Door no. " + `${currentProperty.SSDoorNo})` : ""} ${currentProperty.SSName}`;
    }

    return `${currentProperty.StreetOrUnitNo} ${currentProperty.PropertyAddress}`;
}

app.buildListingDetailsTab = function () {
    var container = $("<div id='listingDetailsContainer' class='contentdiv' />");
    if (currentProperty.PropertyListingId) {
        // Property has an active listing
        app.getListingInfo(currentProperty.PropertyListingId, function (listingModel) {
            // may need to access container by ID here

        });
    } else {
        // No active listing, option to create and post a ListingModel
        container.append($(`<span style='display:block;'>No current listing found for ${app.formatAddress()}</span>`));
        var createListingBtn = $("<input type='button' id='newlistingbtn' class='bigRedButton' value='New Listing' />");
        container.append("<p />").append(createListingBtn).append("<p />");
        createListingBtn.click(app.handleNewListingClick);

        var listingCategorySelectorContainer = $("<div id='listingCategorySelectorContainer' />");
        var listingFieldsContainer = $("<div id='listingFieldsContainer' style='display:none'>\
                                            <hr>\
                                                <div id='listingFieldsContent'>\
                                                    \
                                                </div>\
                                            <hr>\
                                            <input type='button' id='createListingBtn' class='bigRedButton' value='Next >>' />\
                                        </div>");

        container.append(listingCategorySelectorContainer);
        container.append(listingFieldsContainer);
    }

    return container;
}

app.buildListingHistoryTab = function () {
    return '';
}

app.buildListingCategories = function () {
    app.clearListingSelection();
    var container = $("#listingCategorySelectorContainer");
    container.empty();
    var categorySelector = $("<label for='listingCategorySelector' class='fieldAlignmentShortWidth'>List under:</label>\
                                <select id='listingCategorySelector' class='centered-aligned'>\
                                            <option value=''></option>\
                                            <option value='residential'>Residential</option>\
                                            <option value='commercial'>Commercial</option>\
                                            <option value='developments'>Developments</option>\
                                            <option value='holiday'>Holiday</option>\
                                </select>");
    container.append(categorySelector);
    categorySelector.change(app.handleSelectListingCategory);
}

app.clearListingSelection = function () {
    $("#listingFieldsContent").empty();
    $("#listingFieldsContainer").hide();
}

app.buildLocationSelect = function () {
    return $("<label for='locationSelector' class='fieldAlignmentShortWidth'>P24 Suburb:</label>\
                                <select id='locationSelector' class='centered-aligned'><option value=''></option><option value='1'>Test</option></select>\
                                <p class='vertical-spacer' />");
}

app.buildAddressInformation = function () {
    var address = '';
    if (currentProperty.PropertyAddress.trim().indexOf(',') > 0) {
        address = toTitleCase(currentProperty.PropertyAddress.replace(/(^\s*,)|(,\s*$)/g, '').trim().split(',')[0]);
    }
    var content = `<label for='streetNameInput' class='fieldAlignmentShortWidth'>Street Name:</label>\
                    <input id='streetNameInput' type='text' class='fieldAlignmentLongWidth' value='${address}' />\
                    <p class='vertical-spacer' />\
                    <label for='streetNoInput' class='fieldAlignmentShortWidth'>Street Number:</label>\
                    <input id='streetNoInput' type='text' class='fieldAlignmentExtraShortWidth' value='${currentProperty.StreetOrUnitNo != 'n/a' ? currentProperty.StreetOrUnitNo : ''}' />\
                    <p class='vertical-spacer' />`;

    if (currentProperty.SS_FH == "SS") {
        content += `<label for='complexNameInput' class='fieldAlignmentShortWidth'>Complex Name:</label>\
                           <input id='complexNameInput' type='text' class='fieldAlignmentLongWidth' value='${currentProperty.SSName}' />\
                           <p class='vertical-spacer' />\
                            <label for='unitNoInput' class='fieldAlignmentShortWidth'>Unit:</label>\
                            <input id='unitNoInput' type='text' class='fieldAlignmentExtraShortWidth' value='${currentProperty.Unit}' />\
                            <p class='vertical-spacer' />`;
    }

    return content;
}

app.buildDescription = function () {
    return "<label for='descriptionInput' class='fieldAlignmentShortWidth'>Description:</label>\
                    <textarea id='descriptionInput' rows='8' class='fieldAlignmentLongWidth' style='vertical-align:middle'></textarea>\
                    <p class='vertical-spacer' />";
}

app.buildAgentBranchSelector = function () {
    return "<label for='agentInput' class='fieldAlignmentShortWidth'>Agent:</label>\
        <select id='agentInput' class='centered-aligned'><option value='1'>me@seeff.com</option></select>\
                    <p class='vertical-spacer' />\
                    <label for='branchInput' class='fieldAlignmentShortWidth'>Branch:</label>\
        <select id='branchInput' class='centered-aligned'><option value=''></option></select>\
                    <p class='vertical-spacer' />";
}

app.buildStatusSelector = function () {
    return "<label for='statusInput' class='fieldAlignmentShortWidth'>Status:</label>\
                    <select id='statusInput' class='centered-aligned'>\
                            <option value='Active'>Active</option>\
                            <option value='Pending'>Pending</option>\
                            <option value='Rented'>Rented</option>\
                            <option value='Sold'>Sold</option>\
                            <option value='Archived'>Archived</option>\
                            <option value='Valuation'>Valuation</option>\
                    </select>\
                    <p class='vertical-spacer' />";
}

app.buildPriceInput = function () {
    return "<label for='priceInput' class='fieldAlignmentShortWidth'>Price (R):</label>\
                    <input id='priceInput' class='fieldAlignmentExtraShortWidth' />\
                    <p class='vertical-spacer' />";
}

app.buildListingTypeSelect = function () {
    return "<label for='listingTypeInput' class='fieldAlignmentShortWidth'>Listing Type:</label>\
        <select id='listingTypeInput' class='centered-aligned'>\
            <option value=''></option>\
            <option value='ForSale'>For Sale</option>\
            <option value='ToLet'>To Let</option>\
        </select>\
        <p class='vertical-spacer' />";
}
