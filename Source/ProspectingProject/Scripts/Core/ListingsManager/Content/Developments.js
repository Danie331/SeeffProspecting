
app.buildDevelopmentsListing = function () {
    app.clearListingSelection();
    $("#listingFieldsContainer").show();
    var content = $("#listingFieldsContent");
    content.append(app.buildLocationSelect());
    content.append(app.buildDevelopmentName());
    content.append(app.buildAddressInformation());
    content.append(app.buildDescription());
    content.append(app.buildAgentBranchSelector());
    content.append(app.buildDevelopmentListingTypes());
    content.append(app.buildDevelopmentCategories());
    content.append(app.buildDevelopmentPropertyTypesWidget());
}

app.buildDevelopmentListingTypes = function () {
    return "<label for='listingTypeInput' class='fieldAlignmentShortWidth'>Listing Type:</label>\
        <select id='listingTypeInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>\
            <option value='ResidentialEstate'>Residential Estate</option>\
            <option value='CommercialEstate'>Commercial Estate</option>\
            <option value='ResidentialNewDevelopment'>Residential New Development</option>\
            <option value='CommercialNewDevelopment'>Commercial New Development</option>\
        </select>\
        <p class='vertical-spacer' />";
}

app.buildDevelopmentName = function () {
    return "<label for='developmentNameInput' class='fieldAlignmentShortWidth'>Name:</label>\
                    <input id='developmentNameInput' class='fieldAlignmentLongWidth' data-parsley-required data-parsley-length='[1, 100]' />\
                    <p class='vertical-spacer' />";
}

app.buildDevelopmentCategories = function () {
    return "<label for='categoryInput' class='fieldAlignmentShortWidth'>Category:</label>\
        <select id='categoryInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>\
            <option value='1'>Apartment Block</option>\
            <option value='2'>Affordable Housing Development</option>\
            <option value='3'>Coastal Estate</option>\
            <option value='4'>Equestrian Estate</option>\
            <option value='5'>Game Farm Estate</option>\
            <option value='6'>Golf Estate</option>\
            <option value='7'>Nature Estate</option>\
            <option value='8'>Residential Estate</option>\
            <option value='9'>Retirement Estate</option>\
            <option value='10'>Wine Estate</option>\
            <option value='11'>Leaseback</option>\
            <option value='12'>Eco Estate</option>\
            <option value='13'>Building</option>\
            <option value='14'>Business Park</option>\
            <option value='15'>Hotel Investment</option>\
            <option value='16'>Industrial Development</option>\
            <option value='17'>Industrial Park</option>\
            <option value='18'>Medical Centre</option>\
            <option value='19'>Mixed Use Development</option>\
            <option value='20'>Office Development</option>\
            <option value='21'>Office Park</option>\
            <option value='22'>Retail Development</option>\
            <option value='23'>Retail Park</option>\
            <option value='24'>Shopping Centre</option>\
         </select>\
        <p class='vertical-spacer' />";
}

app.buildDevelopmentPropertyTypesWidget = function () {
    return "<input type='button' id='addDevelopmentPropertyTypeBtn' value='Add Types' class='fieldAlignmentExtraShortWidth' />\
                   <div id='developmentPropertyTypesContainer' />";

}

app.getDevelopmentPropertyTypeOptions = function () {
    return "<option value=''></option>\
            <option value='1'>Apartment</option>\
            <option value='2'>Contract</option>\
            <option value='3'>Duplex</option>\
            <option value='4'>Flat</option>\
            <option value='5'>House</option>\
            <option value='6'>Simplex</option>\
            <option value='7'>Townhouse</option>\
            <option value='8'>Vacant Land</option>\
            <option value='9'>Villa</option>\
            <option value='10'>Units</option>\
            <option value='11'>Hotel Room</option>\
            <option value='12'>Equestrian Property</option>\
            <option value='13'>Leaseback</option>\
            <option value='14'>Chalet</option>\
            <option value='15'>Building</option>\
            <option value='16'>Business</option>\
            <option value='17'>Factory</option>\
            <option value='18'>Investment</option>\
            <option value='19'>Office</option>\
            <option value='20'>Mini Factory</option>\
            <option value='21'>Retail</option>\
            <option value='22'>Showroom</option>\
            <option value='23'>Warehouse</option>\
            <option value='24'>Medical Centre</option>\
            <option value='25'>Shopping Centre</option>";
}

app.addDevelopmentPropertyTypeRow = function () {
    var container = $("#developmentPropertyTypesContainer");
    var rowItem = "<div class='development-row-item' >\
                    <label class='fieldAlignmentShortWidth'>Property Type:</label>\
                    <select class='development-property-type-item centered-aligned'>" +
        app.getDevelopmentPropertyTypeOptions() +
        "</select>\
                    <p class='vertical-spacer' />\
                    <label for='pricedFromInput' class='fieldAlignmentShortWidth'>Priced From (R):</label>\
                    <input id='pricedFromInput' class='fieldAlignmentExtraShortWidth development-property-type-item-price' />\
                    <p class='vertical-spacer' />\
                    <label for='developmentPropertyTypeNumber' class='fieldAlignmentShortWidth'>Number:</label>\
                    <input id='developmentPropertyTypeNumber' class='fieldAlignmentExtraShortWidth development-property-type-item-number' type='number' />\
                        <p class='vertical-spacer' />\
                        <label for='developmentPropertyTypeSizeFrom' class='fieldAlignmentShortWidth'>Size From:</label>\
                    <input id='developmentPropertyTypeSizeFrom' class='fieldAlignmentExtraShortWidth development-property-type-item-size-from' type='number' />\
                        <p class='vertical-spacer' />\
                        <label for='developmentPropertyTypeSizeTo' class='fieldAlignmentShortWidth'>Size To:</label>\
                    <input id='developmentPropertyTypeSizeTo' class='fieldAlignmentExtraShortWidth development-property-type-item-size-to' type='number' />\
                   </div>";
    container.append("<hr>").append(rowItem);
}

app.getDevelopmentPropertyRows = function () {
    var items = [];
    $('.development-row-item').each(function (idx, item) {
        var propertyType = $(item).find('.development-property-type-item').first().find('option:selected').text();
        var price = $(item).find('.development-property-type-item-price').first().val();
        var number = $(item).find('.development-property-type-item-number').first().val();
        var sizeFrom = $(item).find('.development-property-type-item-size-from').first().val();
        var sizeTo = $(item).find('.development-property-type-item-size-to').first().val();

        items.push({ PropertyType: propertyType, Price: price, Number: number, SizeFrom: sizeFrom, SizeTo: sizeTo });
    });

    return items;
}

app.buildDevelopmentsSummary = function () {
    var content = `${app.buildListingSummaryRow('locationSelector')}\
            ${app.buildListingSummaryRow('developmentNameInput')}\
            ${app.buildListingSummaryRow('streetNameInput')}\
            ${app.buildListingSummaryRow('streetNoInput')}\
            ${currentProperty.SS_FH == "SS" ? (app.buildListingSummaryRow('complexNameInput') + app.buildListingSummaryRow('unitNoInput')) : ''}\
            ${app.buildListingSummaryRow('descriptionInput')}\
            ${app.buildListingSummaryRow('agentInput')}\
            ${app.buildListingSummaryRow('branchInput')}\
            ${app.buildListingSummaryRow('listingTypeInput')}\
            ${app.buildListingSummaryRow('categoryInput')}`;

    var propertyTypes = app.getDevelopmentPropertyRows();
    propertyTypes.forEach(function (item) {
        content += "<hr>";
        content += "<p class='vertical-spacer' />";
        content += `<label class='fieldAlignmentShortWidth'>Property Type:</label><span class='centered-aligned red-text'>${item.PropertyType}</span>`;
        content += "<p class='vertical-spacer' />";
        content += `<label class='fieldAlignmentShortWidth'>Priced From (R):</label><span class='centered-aligned red-text'>${item.Price}</span>`;
        content += "<p class='vertical-spacer' />";
        content += `<label class='fieldAlignmentShortWidth'>Number:</label><span class='centered-aligned red-text'>${item.Number}</span>`;
        content += "<p class='vertical-spacer' />";
        content += `<label class='fieldAlignmentShortWidth'>Size From:</label><span class='centered-aligned red-text'>${item.SizeFrom}</span>`;
        content += "<p class='vertical-spacer' />";
        content += `<label class='fieldAlignmentShortWidth'>Size To:</label><span class='centered-aligned red-text'>${item.SizeTo}</span>`;
    });

    return content;
}