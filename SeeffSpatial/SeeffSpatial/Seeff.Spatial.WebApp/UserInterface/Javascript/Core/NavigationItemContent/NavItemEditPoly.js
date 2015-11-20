
$(function () {
    $.extend(application.panel, {
        navItemEditPoly: {
            buildContent: function () {
                var container = $("#contentContainerContent");
                if (application.panel.navItemEditPoly.contentCache) {
                    container.html(application.panel.navItemEditPoly.contentCache);
                } else {
                    container.html('').load("../HTML/EditPoly.html", function (content) {
                        application.panel.navItemEditPoly.contentCache = content;
                        container.html(application.panel.navItemEditPoly.contentCache);
                    });
                }                
            },
            controllers: {
                validateSuburb: function (callback) {
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
                },
                validateSuburbCallback: function (result) {
                    var container = $("#suburbValidationResult");
                    if (result.IsValid) {
                        container.html('Validation successful');
                    } else {
                        container.html('Validation failed - Reason: ' + application.utilities.defaultIfNullOrUndef(result.ValidationMessage, 'unspecified') + ' <br />');
                        if (result.ConflictingPolys) {
                            $.each(result.ConflictingPolys, function (idx, area) {
                                var targetSuburb = application.user.SeeffAreaCollectionLookup[area.PolyID];
                                if (targetSuburb) {
                                    var radioItem = application.panel.navItemSuburbSelection.constructSuburbRadioItem(targetSuburb);
                                    container.append(radioItem).append('<br />');
                                } else {
                                    container.append(area.AreaName + " - restart the application to access.").append('<br />');
                                }
                            });
                        }
                    }
                },
                saveSuburb: function () {
                    application.panel.navItemEditPoly.controllers.validateSuburb(function (result) {
                        var activeSuburb = application.stateManager.activeSuburb;
                        if (activeSuburb && result.IsValid) {
                            // path: go to server to save via spatial service call, save to DB, reload from DB and save to Glbal Cache (test this properly), reload suburb on front-end with new centroid etc.
                            var suburbModel = application.services.serviceModels.buildSuburbModel(activeSuburb);
                            application.services.serviceControllers.saveSuburb(suburbModel, application.panel.navItemEditPoly.controllers.saveSuburbCallback);
                        }
                    });                   
                },
                saveSuburbCallback: function (result) {
                    var container = $("#suburbSaveResult");
                    if (result.Successful) {
                        container.html('Suburb saved successfully!');
                        application.stateManager.handleExitEditPolyMode();
                        application.stateManager.updateActiveSuburb(result.SuburbResult);
                        application.stateManager.handleEnterPolyEditMode();
                        //
                    } else {
                        container.html("Error occurred while saving suburb: - " + result.SaveMessage);
                    }
                }
            }
        }
    });
});