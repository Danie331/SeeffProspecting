
$(function () {
    $.extend(application.panel, {
        navItemEditLicense: {
            newLicensePolygon: null,
            buildContent: function () {
                var container = $("#contentContainerContent");
                if (application.panel.navItemEditLicense.contentCache) {
                    container.html(application.panel.navItemEditLicense.contentCache);
                } else {
                    container.html('').load("UserInterface/HTML/EditLicense.html", function (content) {
                        application.panel.navItemEditLicense.contentCache = content;
                        container.html(application.panel.navItemEditLicense.contentCache);
                    });
                }
            },
            controllers: {
                uploadNewKML: function () {
                    $('#attachKMLForLicenseFile').unbind('change').bind('change', function (evt) {
                        application.utilities.handleGetPolygonFromFile(evt, application.panel.navItemEditLicense.controllers.uploadNewKMLCallback);
                    });
                    $('#attachKMLForLicenseFile').trigger('click');
                },
                uploadNewKMLCallback: function (polygon) {
                    polygon.setMap(application.Google.map);
                    // set editable..? validation tests, re-positioning relative to old license (diff colour), confirmation dialog before save.
                    // reindex suburbs whose centroids fall inside new KML to the license (the REVERSE is not true- don't disasociate)
                    // Btn to re-associate a suburb with a new license (or null)
                    // Btn show all suburbs belong to this license.
                    // license not allowed to overlap another license and must be contained within a territory.
                    // test modify another license before saving so that validation fails on Save btn click.
                    // commit.
                    var step1Tick = $("#uploadLicenseKMLStep1CompleteTick");
                    step1Tick.removeClass('step-nonactive').addClass('step-active');

                    var createAreaStep2 = $("#editLicenseStep2");
                    createAreaStep2.removeClass('step-nonactive').addClass('step-active');

                    var startOverBtn = $("#resetUploadLicenseKMLBtn");
                    startOverBtn.removeClass('step-nonactive').addClass('step-active');

                    var startPolygonBtn = $("#uploadKMLForLicenseBtn");
                    startPolygonBtn.prop('disabled', true);

                    polygon.setOptions({ editable: true });
                    polygon.addListener('rightclick', function (event) {
                        if (event.path != null && event.vertex != null) {
                            var path = this.getPaths().getAt(event.path);
                            if (path.getLength() > 3) {
                                path.removeAt(event.vertex);
                            }
                        }
                    });

                    application.panel.navItemEditLicense.newLicensePolygon = polygon;

                    var infoWindow = new google.maps.InfoWindow();
                    infoWindow.setContent("KML upload");
                    infoWindow.setPosition(polygon.getPath().getArray()[0]);
                    infoWindow.open(application.Google.map);
                },
                validateLicensePolygon: function (callback) {
                    if (application.panel.navItemEditLicense.newLicensePolygon) {
                        var licenseModel = application.services.serviceModels.buildLicenseModel(application.stateManager.activeLicense);
                        licenseModel.PolyWKT = application.Google.getPolyAsGeographyString(application.panel.navItemEditLicense.newLicensePolygon);
                        application.services.serviceControllers.validateLicensePolygon(licenseModel, function (result) {
                            application.panel.navItemEditLicense.controllers.validateLicenseCallback(result);
                            if (callback) {
                                callback(result);
                            }
                        });
                    }
                },
                validateLicenseCallback: function (result) {
                    var validateBtn = $("#validateLicenseKMLPolyBtn");
                    var errorContainer = $("#editLicensePolyValidationResults");
                    var step2Tick = $("#editLicenseStep2CompleteTick");
                    var errorTick = $("#editLicenseStep2ErrorTick");
                    var step3 = $("#editLicenseStep3");

                    if (result.IsValid) {
                        application.panel.navItemEditLicense.newLicensePolygon.setOptions({ editable: false });
                        errorContainer.removeClass('step-active').addClass('step-nonactive');
                        step2Tick.removeClass('step-nonactive').addClass('step-active');
                        errorTick.removeClass('step-active').addClass('step-nonactive');
                        step3.removeClass('step-nonactive').addClass('step-active');
                        validateBtn.prop('disabled', true);
                    }
                    else {
                        errorContainer.html('Validation failed - Reason: ' + application.utilities.defaultIfNullOrUndef(result.ValidationMessage, 'unspecified') + ' <br />');
                    }
                },
                saveLicense: function () {
                    var yesNo = window.confirm("Warning: This operation will overwrite license ID " + application.stateManager.activeLicense.LicenseID + " polygon. Are you sure you want to proceed?");
                    if (yesNo == true) {
                        var licenseModel = application.services.serviceModels.buildLicenseModel(application.stateManager.activeLicense);
                        licenseModel.PolyWKT = application.Google.getPolyAsGeographyString(application.panel.navItemEditLicense.newLicensePolygon);
                        application.services.serviceControllers.saveLicense(licenseModel, application.panel.navItemEditLicense.controllers.saveLicenseCallback);
                    }
                },
                saveLicenseCallback: function (result) {
                    var step3Tick = $("#editLicenseStep3CompleteTick");
                    var errorTick = $("#editLicenseStep3ErrorTick");
                    var resultContainer = $("#saveLicenseResults");
                    var saveBtn = $("#overwriteLicenseBtn");
                    if (result.Successful) {
                        application.stateManager.activeLicense.PolyWKT = result.LicenseResult.PolyWKT;
                        application.stateManager.activeLicense.CentroidWKT = result.LicenseResult.CentroidWKT;
                        application.stateManager.activeLicense.Suburbs = result.LicenseResult.Suburbs;
                        application.panel.navItemEditLicense.newLicensePolygon.setMap(null);
                        application.panel.navItemEditLicense.newLicensePolygon = null;
                        application.stateManager.handleExitEditPolyMode();
                        application.Google.createLicensePoly(application.stateManager.activeLicense, { render: true });
                        application.Google.showLicenseInfoWindow(application.stateManager.activeLicense);

                        application.user.SeeffAreaCollection.length = 0;
                        application.user.SeeffAreaCollectionLookup = {};

                        step3Tick.removeClass('step-nonactive').addClass('step-active');
                        errorTick.removeClass('step-active').addClass('step-nonactive');
                        saveBtn.prop('disabled', true);
                        resultContainer.append("License updated successfully");
                    } else {
                        step3Tick.removeClass('step-active').addClass('step-nonactive');
                        errorTick.removeClass('step-nonactive').addClass('step-active');
                        saveBtn.prop('disabled', true);
                        resultContainer.append("An error occurred while saving. Please restart the application in case information was updated. Message - " + result.SaveMessage);
                    }
                },
                resetForm: function () {
                    if (application.panel.navItemEditLicense.newLicensePolygon) {
                        application.panel.navItemEditLicense.newLicensePolygon.setMap(null);
                        application.panel.navItemEditLicense.newLicensePolygon = null;
                    }
                    application.stateManager.handleExitEditPolyMode();
                    application.stateManager.handleExitCreateAreaMode();
                    application.stateManager.handleEnterCreateAreaMode();
                    application.panel.navItemEditLicense.buildContent();
                }
            }
        }
    });
});