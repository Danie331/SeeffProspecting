
var NewListingModelBase = function (
    lightstoneId,
    lat,
    lng,
    erfNo,
    streetName,
    streetNo,
    sectionaTitleName,
    unitNo,
    locationId,
    description,
    agentId,
    branchId,
    status,
    price,
    listingType) {
    this.lightstone_id = lightstoneId;
    this.map_y_position = lng;
    this.map_x_position = lat;
    this.erf_number = erfNo;
    this.street_name = streetName;
    this.street_number = streetNo;
    if (sectionaTitleName && unitNo) {
        this.complex_name = sectionaTitleName;
        this.unit_number = unitNo;
    }
    this.location = locationId;
    this.description = description;
    this.agent = agentId;
    this.branch = branchId;
    this.status = status;
    this.price = (price + '').replace(/\s/g, "");
    this.listing_type = listingType;
};

NewListingModelBase.prototype.create = function () {
    return new NewListingModelBase(currentProperty.LightstonePropertyId,
        currentProperty.LatLng.Lat,
        currentProperty.LatLng.Lng,
        currentProperty.ErfNo,
        $("#streetNameInput").val(),
        $("#streetNoInput").val(),
        $("#complexNameInput").val(),
        $("#unitNoInput").val(),
        $("#locationSelector").val(),
        $("#descriptionInput").val(),
        $("#agentInput").val(),
        $("#branchInput").val(),
        $("#statusInput option:selected").text(),
        $("#priceInput").val(),
        $("#listingTypeInput option:selected").text());
}