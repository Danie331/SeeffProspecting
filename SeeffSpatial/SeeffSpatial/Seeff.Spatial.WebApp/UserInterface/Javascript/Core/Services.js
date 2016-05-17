
$(function () {
    $.extend(application, {
        services: {
            post: function (URL, input, loadingMsg, callback) {
                if (loadingMsg) {
                    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">' + loadingMsg + '</p>' });
                }
                input = JSON.stringify(input);
                $.ajax({
                    url: URL,
                    type: 'POST',
                    data: input,
                    contentType: "application/json"
                }).done(function (data) {
                    $.unblockUI();
                    if (callback) {
                        callback(data);
                    }
                    //$.unblockUI();
                });
            },
            get: function (URL, loadingMsg, callback) {
                $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">' + loadingMsg + '</p>' });
                $.ajax({
                    url: URL,
                    type: 'GET',
                    contentType: "application/json"
                }).done(function (data) {
                    if (callback) {
                        callback(data);
                    }
                    $.unblockUI();
                });
            },
            getSynchronously: function (URL, loadingMsg, callback) {               
                setTimeout(function () { $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">' + loadingMsg + '</p>', fadeIn: 0 }); }, 500);
                    $.ajax({
                        url: URL,
                        type: 'GET',
                        async: false,
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
                },
                retrieveUnmappedSuburbs: function (callback) {
                    var endpoint = application.utilities.buildHomeURL('/api/Home/RetrieveUnmappedSuburbs');
                    application.services.get(endpoint, "Loading. Please wait...", callback);
                },
                validateLicensePolygon: function (licenseModel, callback) {
                    var endpoint = application.utilities.buildHomeURL('/api/Home/ValidateLicense');
                    application.services.post(endpoint, licenseModel, "Validating. Please wait...", callback);
                },
                saveLicense: function (licenseModel, callback) {
                    var endpoint = application.utilities.buildHomeURL('/api/Home/SaveLicense');
                    application.services.post(endpoint, licenseModel, "Validating. Please wait...", callback);
                },
                getSuburbs: function (callback) {
                    var endpoint = application.utilities.buildHomeURL('/api/Home/GetSuburbs');
                    application.services.get(endpoint, "Retrieving Suburbs. Please wait...", callback);
                },
                deleteSuburb: function (suburbModel, callback) {
                    var endpoint = application.utilities.buildHomeURL('/api/Home/DeleteSuburb');
                    application.services.post(endpoint, suburbModel, "Deleting. Please wait...", callback);
                },
                getLicenseName: function (licenseModel, callback) {
                    var endpoint = application.utilities.buildHomeURL('/api/Home/GetLicenseName');
                    application.services.post(endpoint, licenseModel, null, callback);
                },
                retrieveOrphanRecords: function (licenseModel, callback) {
                    var endpoint = application.utilities.buildHomeURL('/api/Home/GetOrphanedProperties');
                    application.services.post(endpoint, licenseModel, "Loading. Please wait...", callback);
                },
                exportLicenseToKML: function (exportLicenseModel, callback) {
                    var endpoint = application.utilities.buildHomeURL('/api/Home/ExportLicenseToKML');
                    application.services.post(endpoint, exportLicenseModel, "Generating Model. Please wait...", callback);
                },
                getSuburbsUnderMaintenance: function (callback) {
                    var endpoint = application.utilities.buildHomeURL('/api/Home/GetSuburbsUnderMaintenance');
                    application.services.get(endpoint, "Refreshing. Please wait...", callback);
                },
                validateTerritoryPolygon: function (territoryModel, callback) {
                    var endpoint = application.utilities.buildHomeURL('/api/Home/ValidateTerritory');
                    application.services.post(endpoint, territoryModel, "Validating. Please wait...", callback);
                },
                saveTerritory: function (territoryModel, callback) {
                    var endpoint = application.utilities.buildHomeURL('/api/Home/SaveTerritory');
                    application.services.post(endpoint, territoryModel, "Saving. Please wait...", callback);
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
                },
                buildLicenseModel: function (license) {
                    var licenseModel = { LicenseID: null, TerritoryID: null, PolyWKT: null, CentroidWKT: null };
                    if (license) {
                        licenseModel.PolyWKT = application.Google.getPolyAsGeographyString(license.PolygonInstance);
                        licenseModel.LicenseID = license.LicenseID;
                        licenseModel.TerritoryID = license.TerritoryID;
                    }

                    return licenseModel;
                },
                buildTerritoryModel: function (territory) {
                    var territoryModel = { TerritoryID: null, PolyWKT: null, CentroidWKT: null };
                    if (territory) {
                        territoryModel.PolyWKT = application.Google.getPolyAsGeographyString(territory.PolygonInstance);
                        territoryModel.TerritoryID = territory.TerritoryID;
                    }

                    return territoryModel;
                }
            }
        }
    });
});