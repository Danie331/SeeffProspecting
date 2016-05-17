

$(function () {
    $.extend(application.panel, {
        navItemEditTerritory: {
            buildContent: function () {
                var container = $("#contentContainerContent");
                if (application.panel.navItemEditTerritory.contentCache) {
                    container.html(application.panel.navItemEditTerritory.contentCache);
                } else {
                    container.html('').load("UserInterface/HTML/EditTerritory.html", function (content) {
                        application.panel.navItemEditTerritory.contentCache = content;
                        container.html(application.panel.navItemEditTerritory.contentCache);
                    });
                }
            },
            controllers: {
                validateTerritory: function (callback) {
                    var activeTerritory = application.stateManager.activeTerritory;
                    if (activeTerritory) {
                        var territoryModel = application.services.serviceModels.buildTerritoryModel(activeTerritory);
                        application.services.serviceControllers.validateTerritoryPolygon(territoryModel, function (result) {
                            application.panel.navItemEditTerritory.controllers.validateTerritoryCallback(result);
                            if (callback) {
                                callback(result);
                            }
                        });
                    }
                },
                validateTerritoryCallback: function (result) {
                    var container = $("#territoryValidationResult");
                    if (result.IsValid) {
                        container.html('Validation successful');
                    } else {
                        container.html('Validation failed - Reason: ' + application.utilities.defaultIfNullOrUndef(result.ValidationMessage, 'unspecified') + ' <br />');
                    }
                },
                saveTerritory: function () {
                    application.panel.navItemEditTerritory.controllers.validateTerritory(function (result) {
                        var activeTerritory = application.stateManager.activeTerritory;
                        if (activeTerritory && result.IsValid) {
                            var territoryModel = application.services.serviceModels.buildTerritoryModel(activeTerritory);
                            application.services.serviceControllers.saveTerritory(territoryModel, application.panel.navItemEditTerritory.controllers.saveTerritoryCallback);
                        }
                    });
                },
                saveTerritoryCallback: function (result) {
                    var container = $("#territorySaveResult");
                    if (result.Successful) {
                        container.html('Successfully Saved!');
                        application.stateManager.handleExitEditPolyMode();
                        application.stateManager.updateActiveTerritory(result.TerritoryResult);
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