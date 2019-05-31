

app.createDevelopmentsListingModel = function () {
    var baseModel = new NewListingModelBase().create();
    var propertyTypes = [];
    var items = app.getDevelopmentPropertyRows();
    items.forEach(function (i) {
        propertyTypes.push({ property_type: i.PropertyType, priced_from: i.Price, number: i.Number, size_from: i.SizeFrom, size_to: i.SizeTo });
    });
    return Object.assign(baseModel, {
        name: $("#developmentNameInput").val(),
        category: $("#categoryInput option:selected").text(),
        property_types: propertyTypes 
    });
}