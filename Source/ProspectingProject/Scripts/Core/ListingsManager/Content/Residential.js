
app.buildResidentialListing = function () {
    app.clearListingSelection();
    $("#listingFieldsContainer").show();
    var content = $("#listingFieldsContent");
    content.append(app.buildLocationSelect());
    content.append(app.buildAddressInformation());
    content.append(app.buildDescription());
    content.append(app.buildAgentBranchSelector());
    content.append(app.buildStatusSelector());
    content.append(app.buildPriceInput());
    content.append(app.buildListingTypeSelect());
    content.append(app.buildResidentialPropertyTypeSelector());
}

app.buildResidentialPropertyTypeSelector = function () {
    return "<label for='propertyTypeInput' class='fieldAlignmentShortWidth'>Property Type:</label>\
        <select id='propertyTypeInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>\
            <option value='1'>Apartment</option>\
            <option value='2'>Apartment Block</option>\
            <option value='3'>Bed &amp; Breakfast</option>\
            <option value='4'>Bungalow</option>\
            <option value='5'>Cluster</option>\
            <option value='6'>Duplex</option>\
            <option value='7'>Farm</option>\
            <option value='8'>Freehold</option>\
            <option value='9'>Freestanding</option>\
            <option value='10'>Flat</option>\
            <option value='11'>Garden Cottage</option>\
            <option value='12'>Gated Estate</option>\
            <option value='13'>House</option>\
            <option value='14'>Lodge</option>\
            <option value='15'>Package Home</option>\
            <option value='16'>Penthouse</option>\
            <option value='17'>Room</option>\
            <option value='18'>Sectional Title</option>\
            <option value='19'>Simplex</option>\
            <option value='20'>Small Holding</option>\
            <option value='21'>Townhouse</option>\
            <option value='22'>Vacant Land</option>\
            <option value='23'>Villa</option>\
            <option value='24'>Retirement Unit</option>\
            <option value='25'>Guest House</option>\
            <option value='26'>Hotel Room</option>\
            <option value='27'>Equestrian Property</option>\
            <option value='28'>Studio Apartment</option>\
            <option value='29'>Leaseback</option>\
            <option value='30'>Hotel</option>\
            <option value='31'>Club</option>\
            <option value='32'>Golf Estate</option>\
            <option value='33'>Maisonette</option>\
            <option value='34'>Duet</option>\
        </select>\
        <p class='vertical-spacer' />";
}

app.buildResidentialSummary = function () {
    return `${app.buildListingSummaryRow('locationSelector')}\
            ${app.buildListingSummaryRow('streetNameInput')}\
            ${app.buildListingSummaryRow('streetNoInput')}\
            ${currentProperty.SS_FH == "SS" ? (app.buildListingSummaryRow('complexNameInput') + app.buildListingSummaryRow('unitNoInput')) : ''}\
            ${app.buildListingSummaryRow('descriptionInput')}\
            ${app.buildListingSummaryRow('agentInput')}\
            ${app.buildListingSummaryRow('branchInput')}\
            ${app.buildListingSummaryRow('statusInput')}\
            ${app.buildListingSummaryRow('priceInput')}\
            ${app.buildListingSummaryRow('listingTypeInput')}\
            ${app.buildListingSummaryRow('propertyTypeInput')}`;
}