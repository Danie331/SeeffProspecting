

app.createResidentialListingModel = function () {
    var baseModel = new NewListingModelBase().create();
    return Object.assign(baseModel, { property_type: $("#propertyTypeInput option:selected").text() });
}