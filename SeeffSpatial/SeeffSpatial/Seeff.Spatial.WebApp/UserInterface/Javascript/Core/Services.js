
$(function () {
    $.extend(application, {
        services: {
            post: function (URL, input, loadingMsg, callback) {
                $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">' + loadingMsg + '</p>' });
                input = JSON.stringify(input);
                $.ajax({
                    url: URL,
                    type: 'POST',
                    data: input,
                    contentType: "application/json"
                }).done(function (data) {
                    if (callback) {
                        callback(data);
                    }
                    $.unblockUI();
                });
            },
            serviceControllers: {
                validateSuburbPolygon: function (suburbModel, callback) {
                    // NB: Consider all the rules for polygon validation!!!
                    var endpoint = application.utilities.buildHomeURL('/api/Home/ValidateSuburb');
                    application.services.post(endpoint, suburbModel, "Validating. Please wait...", callback);
                },
                saveSuburb: function (suburbModel, callback) {
                    var endpoint = application.utilities.buildHomeURL('/api/Home/SaveSuburb');
                    application.services.post(endpoint, suburbModel, "Saving. Please wait...", callback);
                }
            },
            serviceModels: {
                buildSuburbModel: function (activeSuburb) {
                    var suburbModel = { SeeffAreaID: null, AreaName: null, LicenseID: null, TerritoryID: null, PolyWKT: null, CentroidWKT: null };
                    if (activeSuburb) {
                        suburbModel.PolyWKT = application.Google.getPolyAsGeographyString(activeSuburb.PolygonInstance);
                        suburbModel.SeeffAreaID = activeSuburb.SeeffAreaID;
                        suburbModel.AreaName = activeSuburb.AreaName;
                        suburbModel.LicenseID = activeSuburb.LicenseID;
                        suburbModel.TerritoryID = activeSuburb.TerritoryID;
                    }

                    return suburbModel;
                }
            }
        }
    });
});