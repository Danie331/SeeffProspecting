
app.buildCommercialListing = function () {
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
    content.append(app.buildCommercialPropertyTypes());
}


app.buildCommercialPropertyTypes = function () {
    return "<label for='propertyTypeInput' class='fieldAlignmentShortWidth'>Property Type:</label>\
        <select id='propertyTypeInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>\
            <option value='1'>Building</option>\
            <option value='2'>Bed &amp; Breakfast</option>\
            <option value='3'>Business</option>\
            <option value='4'>Factory</option>\
            <option value='5'>Farm</option>\
            <option value='6'>Hotel</option>\
            <option value='7'>Industrial Yard</option>\
            <option value='8'>Investment</option>\
            <option value='9'>Office</option>\
            <option value='10'>Mini Factory</option>\
            <option value='11'>Retail</option>\
            <option value='12'>Small Holding</option>\
            <option value='13'>Showroom</option>\
            <option value='14'>Vacant Land</option>\
            <option value='15'>Warehouse</option>\
            <option value='16'>Serviced Office</option>\
            <option value='17'>Apartment Block</option>\
            <option value='18'>Guesthouse</option>\
            <option value='19'>Place of Worship</option>\
            <option value='20'>Storage Unit</option>\
            <option value='21'>Airport Hanger</option>\
            <option value='22'>Workshop</option>\
            <option value='23'>Medical Suite</option>\
            <option value='24'>Service Station</option>\
            <option value='25'>Training Facility</option>\
        </select>\
        <p class='vertical-spacer' />";
}
