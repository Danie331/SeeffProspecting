
$(function () {
    $.extend(application.panel, {
        navItemEditSuburb: {
            buildContent: function () {
                var container = $("#contentContainerContent");
                if (application.panel.navItemEditSuburb.contentCache) {
                    container.html(application.panel.navItemEditSuburb.contentCache);
                } else {
                    container.html('').load("UserInterface/HTML/EditSuburb.html", function (content) {

                        application.panel.navItemAreaSelection.getSuburbs();

                        application.panel.navItemEditSuburb.contentCache = content;
                        container.html(application.panel.navItemEditSuburb.contentCache);
                    });
                }
            },
            controllers: {
                validateArea: function (callback) {
                    var activeSuburb = application.stateManager.activeSuburb;
                    if (activeSuburb) {
                        var suburbModel = application.services.serviceModels.buildSuburbModel(activeSuburb);
                        application.services.serviceControllers.validateSuburbPolygon(suburbModel, function (result) {
                            application.panel.navItemEditSuburb.controllers.validateSuburbCallback(result);
                            if (callback) {
                                callback(result);
                            }
                        });
                    }
                    var activeLicense = application.stateManager.activeLicense;
                    if (activeLicense) {
                        var licenseModel = application.services.serviceModels.buildLicenseModel(activeLicense);
                        application.services.serviceControllers.validateLicensePolygon(licenseModel, function (result) {
                            application.panel.navItemEditSuburb.controllers.validateLicenseCallback(result);
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
                validateLicenseCallback: function (result) {
                    var container = $("#areaValidationResult");
                    if (result.IsValid) {
                        container.html('Validation successful');
                    } else {
                        container.html('Validation failed - Reason: ' + application.utilities.defaultIfNullOrUndef(result.ValidationMessage, 'unspecified') + ' <br />');
                    }
                },
                saveArea: function () {
                    application.panel.navItemEditSuburb.controllers.validateArea(function (result) {
                        var activeSuburb = application.stateManager.activeSuburb;
                        if (activeSuburb && result.IsValid) {
                            // path: go to server to save via spatial service call, save to DB, reload from DB and save to Glbal Cache (test this properly), reload suburb on front-end with new centroid etc.
                            var suburbModel = application.services.serviceModels.buildSuburbModel(activeSuburb);
                            application.services.serviceControllers.saveSuburb(suburbModel, application.panel.navItemEditSuburb.controllers.saveSuburbCallback);
                        }
                        var activeLicense = application.stateManager.activeLicense;
                        if (activeLicense && result.IsValid) {
                            var licenseModel = application.services.serviceModels.buildLicenseModel(activeLicense);
                            application.services.serviceControllers.saveLicense(licenseModel, application.panel.navItemEditSuburb.controllers.saveLicenseCallback);
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
                },
                deleteSuburb: function () {
                    var activeSuburb = application.stateManager.activeSuburb;
                    if (activeSuburb) {
                        var yesNo = window.confirm("Are you sure you want to delete this suburb's polygon?");
                        if (yesNo == true) {
                            var suburbModel = application.services.serviceModels.buildSuburbModel(activeSuburb);
                            application.services.serviceControllers.deleteSuburb(suburbModel, function (result) {
                                var resultDiv = $("#deleteSuburbResult");
                                if (result.Successful) {
                                    var index = -1;
                                    application.user.SeeffAreaCollectionLookup = {};
                                    $.each(application.user.SeeffAreaCollection, function (idx, sub) {
                                        if (sub.SeeffAreaID == activeSuburb.SeeffAreaID) {
                                            // Find the index of the suburb to remove from the client cache
                                            index = idx;
                                        } else {
                                            // Otherwise just re-index the lookup array again.
                                            var areaID = '' + sub.SeeffAreaID;
                                            application.user.SeeffAreaCollectionLookup[areaID] = sub;
                                        }
                                    });
                                    if (index > -1) application.user.SeeffAreaCollection.splice(index, 1);

                                    application.stateManager.handleExitEditPolyMode();
                                    //application.panel.navItemAreaSelection.contentCache = ''; // needed?
                                    activeSuburb.PolygonInstance.setMap(null);
                                    application.stateManager.setActiveSuburb(null);

                                    resultDiv.empty().append("Deleted Successfully.");
                                } else {
                                    resultDiv.empty().append("Problem occurred deleting suburb, please contact support - " + result.Message);
                                }
                            });
                        }
                    }
                }
            }
        }
    });
});