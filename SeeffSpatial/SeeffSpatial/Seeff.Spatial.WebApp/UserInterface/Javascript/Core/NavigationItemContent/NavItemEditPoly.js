
$(function () {
    $.extend(application.panel, {
        navItemEditPoly: {
            buildContent: function () {
                var container = $("#contentContainerContent");
                if (application.panel.navItemEditPoly.contentCache) {
                    container.html(application.panel.navItemEditPoly.contentCache);
                } else {
                    container.html('').load("UserInterface/HTML/EditPoly.html", function (content) {

                        application.panel.navItemAreaSelection.getSuburbs();

                        application.panel.navItemEditPoly.contentCache = content;
                        container.html(application.panel.navItemEditPoly.contentCache);
                    });
                }                
            },
            controllers: {
                validateArea: function (callback) {
                    var activeSuburb = application.stateManager.activeSuburb;
                    if (activeSuburb) {
                        var suburbModel = application.services.serviceModels.buildSuburbModel(activeSuburb);           
                        application.services.serviceControllers.validateSuburbPolygon(suburbModel, function (result) {
                            application.panel.navItemEditPoly.controllers.validateSuburbCallback(result);
                            if (callback) {
                                callback(result);
                            }
                        });
                    }
                    var activeLicense = application.stateManager.activeLicense;
                    if (activeLicense) {
                        var licenseModel = application.services.serviceModels.buildLicenseModel(activeLicense);
                        application.services.serviceControllers.validateLicensePolygon(licenseModel, function (result) {
                            application.panel.navItemEditPoly.controllers.validateLicenseCallback(result);
                            if (callback) {
                                callback(result);
                            }
                        });
                    }
                },
                validateSuburbCallback: function (result) {
                    var container = $("#areaValidationResult");
                    if (result.IsValid) {
                        container.html('Validation successful');
                    } else {
                        container.html('Validation failed - Reason: ' + application.utilities.defaultIfNullOrUndef(result.ValidationMessage, 'unspecified') + ' <br />');
                        if (result.ConflictingPolys) {
                            $.each(result.ConflictingPolys, function (idx, area) {
                                var targetSuburb = application.user.SeeffAreaCollectionLookup[area.PolyID];
                                if (targetSuburb) {
                                    var radioItem = application.panel.navItemAreaSelection.constructSuburbRadioItem(targetSuburb);
                                    container.append(radioItem).append('<br />');
                                } else {
                                    container.append(area.AreaName + " - restart the application to access.").append('<br />');
                                }
                            });
                        }
                    }
                },
                validateLicenseCallback: function(result) {
                    var container = $("#areaValidationResult");
                    if (result.IsValid) {
                        container.html('Validation successful');
                    } else {
                        container.html('Validation failed - Reason: ' + application.utilities.defaultIfNullOrUndef(result.ValidationMessage, 'unspecified') + ' <br />');
                    }
                },
                saveArea: function () {
                    application.panel.navItemEditPoly.controllers.validateArea(function (result) {
                        var activeSuburb = application.stateManager.activeSuburb;
                        if (activeSuburb && result.IsValid) {
                            // path: go to server to save via spatial service call, save to DB, reload from DB and save to Glbal Cache (test this properly), reload suburb on front-end with new centroid etc.
                            var suburbModel = application.services.serviceModels.buildSuburbModel(activeSuburb);
                            application.services.serviceControllers.saveSuburb(suburbModel, application.panel.navItemEditPoly.controllers.saveSuburbCallback);
                        }
                        var activeLicense = application.stateManager.activeLicense;
                        if (activeLicense && result.IsValid) {
                            var licenseModel = application.services.serviceModels.buildLicenseModel(activeLicense);
                            application.services.serviceControllers.saveLicense(licenseModel, application.panel.navItemEditPoly.controllers.saveLicenseCallback);
                        }
                    });                   
                },
                saveSuburbCallback: function (result) {
                    var container = $("#areaSaveResult");
                    if (result.Successful) {
                        container.html('Successfully Saved!');
                        application.stateManager.handleExitEditPolyMode();
                        application.stateManager.updateActiveSuburb(result.SuburbResult);
                        application.stateManager.handleEnterPolyEditMode();
                        //
                    } else {
                        container.html("Error occurred while saving: - " + result.SaveMessage);
                    }
                },
                saveLicenseCallback: function (result) {
                    var container = $("#areaSaveResult");
                    if (result.Successful) {
                        container.html('Successfully Saved!');
                        application.stateManager.handleExitEditPolyMode();
                        application.stateManager.updateActiveLicense(result.LicenseResult);
                        application.stateManager.handleEnterPolyEditMode();
                        //
                    } else {
                        container.html("Error occurred while saving: - " + result.SaveMessage);
                    }
                }
            }
        }
    });
});