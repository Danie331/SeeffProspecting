
$(function () {
    $.extend(application.panel, {
        navItemCreateNewArea: {
            newAreaPolygon: null,
            buildContent: function () {
                var container = $("#contentContainerContent");
                if (application.panel.navItemCreateNewArea.contentCache) {
                    container.html(application.panel.navItemCreateNewArea.contentCache);
                } else {
                    container.html('').load("UserInterface/HTML/CreatePoly.html", function (content) {
                        application.panel.navItemCreateNewArea.contentCache = content;
                        container.html(application.panel.navItemCreateNewArea.contentCache);
                    });
                }
            },
            controllers: {
                startNewPolygon: function () {
                    application.Google.drawingManager.setMap(null);
                    application.Google.drawingManager = new google.maps.drawing.DrawingManager({
                        drawingMode: google.maps.drawing.OverlayType.POLYGON,
                        drawingControl: true,
                        drawingControlOptions: {
                            position: google.maps.ControlPosition.TOP_RIGHT,
                            drawingModes: [google.maps.drawing.OverlayType.POLYGON]
                        },
                        polygonOptions: {
                            strokeColor: '#FF0000',
                            strokeOpacity: 0.8,
                            strokeWeight: 1.5,
                            fillColor: '#FF0000',
                            fillOpacity: 0.4
                        }
                    });
                    application.Google.drawingManager.setMap(application.Google.map); //cancelation, exit this mode (transaction), green tick when step complete, edit once drawn, highlight steps as they become available
                    google.maps.event.addListener(application.Google.drawingManager, 'polygoncomplete', function (polygon) {
                        var step1Tick = $("#createPolyStep1CompleteTick");
                        step1Tick.removeClass('step-nonactive').addClass('step-active');

                        var createAreaStep2 = $("#createAreaStep2");
                        createAreaStep2.removeClass('step-nonactive').addClass('step-active');

                        var startOverBtn = $("#startNewPolyOverBtn");
                        startOverBtn.removeClass('step-nonactive').addClass('step-active');

                        var startPolygonBtn = $("#startPolygonBtn");
                        startPolygonBtn.prop('disabled', true);

                        var uploadNewKMLBtn = $("#uploadKMLBtn");
                        uploadNewKMLBtn.prop('disabled', true);

                        polygon.setOptions({ editable: true });
                        application.panel.navItemCreateNewArea.newAreaPolygon = polygon;
                        application.Google.drawingManager.setMap(null);
                    });
                },
                uploadNewKML: function () {
                    $('#attachKMLFile').unbind('change').bind('change', function (evt) {
                        application.utilities.handleGetPolygonFromFile(evt, application.panel.navItemCreateNewArea.controllers.uploadNewKMLCallback);
                    });
                    $('#attachKMLFile').trigger('click');
                },
                uploadNewKMLCallback: function(polygon) {
                    polygon.setMap(application.Google.map);
                    var step1Tick = $("#createPolyStep1CompleteTick");
                    step1Tick.removeClass('step-nonactive').addClass('step-active');

                    var createAreaStep2 = $("#createAreaStep2");
                    createAreaStep2.removeClass('step-nonactive').addClass('step-active');

                    var startOverBtn = $("#startNewPolyOverBtn");
                    startOverBtn.removeClass('step-nonactive').addClass('step-active');

                    var startPolygonBtn = $("#startPolygonBtn");
                    startPolygonBtn.prop('disabled', true);

                    var uploadNewKMLBtn = $("#uploadKMLBtn");
                    uploadNewKMLBtn.prop('disabled', true);

                    polygon.setOptions({ editable: true });
                    application.panel.navItemCreateNewArea.newAreaPolygon = polygon;

                    var infoWindow = new google.maps.InfoWindow();
                    infoWindow.setContent("KML upload");
                    infoWindow.setPosition(polygon.getPath().getArray()[0]);
                    infoWindow.open(application.Google.map);
                },
                validateNewPolygon: function () {
                    if (application.panel.navItemCreateNewArea.newAreaPolygon) {
                        var suburbModel = application.services.serviceModels.buildSuburbModel();
                        suburbModel.PolyWKT = application.Google.getPolyAsGeographyString(application.panel.navItemCreateNewArea.newAreaPolygon);
                        suburbModel.AreaName = "temp";
                        application.services.serviceControllers.validateSuburbPolygon(suburbModel, function (result) {
                            var step2Tick = $("#createPolyStep2CompleteTick");
                            var errorTick = $("#createPolyStep2ErrorTick");
                            var errorContainer = $("#newAreaValidationResults");
                            var step3 = $("#createAreaStep3");
                            var validateBtn = $("#ValidateNewPolyBtn");
                            
                            if (result.IsValid) {
                                application.panel.navItemCreateNewArea.newAreaPolygon.setOptions({ editable: false });
                                errorContainer.removeClass('step-active').addClass('step-nonactive');
                                step2Tick.removeClass('step-nonactive').addClass('step-active');
                                errorTick.removeClass('step-active').addClass('step-nonactive');
                                step3.removeClass('step-nonactive').addClass('step-active');
                                validateBtn.prop('disabled', true);
                                // populate the licenseIDSelect 
                                var licenseIDSelect = $("#licenseIDSelect");
                                licenseIDSelect.empty().append("<option value='' selected='selected'></option>");
                                $.each(application.user.SeeffLicenses, function (idx, lic) {
                                    var option = $('<option/>', { value: lic.LicenseID }).text(lic.LicenseID);
                                    licenseIDSelect.append(option);
                                });
                                if (result.LicenseID && result.LicenseID > -1) {
                                    licenseIDSelect.val(result.LicenseID);
                                    licenseIDSelect.attr("disabled", true);
                                }

                                var unmappedSuburbSelector = $("#unmappedSuburbSelector");
                                unmappedSuburbSelector.empty().append("<option value='' selected='selected'></option>");
                                application.services.serviceControllers.retrieveUnmappedSuburbs(function (results) {
                                    if (results.Successful) {
                                        $.each(results.Suburbs, function (idx, sub) {
                                            var option = $('<option/>', { value: sub.SeeffAreaID }).text(sub.AreaName + ' (' + sub.SeeffAreaID + ')');
                                            option.data('areaName', sub.AreaName);
                                            unmappedSuburbSelector.append(option);
                                        });
                                    }
                                });
                            } else {                                
                                errorTick.removeClass('step-nonactive').addClass('step-active');
                                step2Tick.removeClass('step-active').addClass('step-nonactive');
                                step3.removeClass('step-active').addClass('step-nonactive');
                                errorContainer.removeClass('step-nonactive').addClass('step-active');
                                errorContainer.empty();
                                var validationMsg = 'Validation failed - Reason: ' + application.utilities.defaultIfNullOrUndef(result.ValidationMessage, 'unspecified') + ' <br />';
                                errorContainer.html(validationMsg);
                                if (result.ConflictingPolys) {
                                    $.each(result.ConflictingPolys, function (idx, area) {
                                        var targetSuburb = application.user.SeeffAreaCollectionLookup[area.PolyID];
                                        if (targetSuburb) {
                                            var radioItem = application.panel.navItemAreaSelection.constructSuburbRadioItem(targetSuburb);
                                            errorContainer.append(radioItem).append('<br />');
                                        } else {
                                            errorContainer.append(area.AreaName + " - restart the application to access.").append('<br />');
                                        }
                                    });
                                }
                            }
                        });
                    }
                },
                resetCreateNewAreaForm: function () {
                    // must obliterate polygon on map unless it's saved, reset form
                    if (application.panel.navItemCreateNewArea.newAreaPolygon) {
                        application.panel.navItemCreateNewArea.newAreaPolygon.setMap(null);
                        application.panel.navItemCreateNewArea.newAreaPolygon = null;
                    }
                    application.stateManager.handleExitEditPolyMode();
                    application.stateManager.handleExitCreateAreaMode();
                    application.stateManager.handleEnterCreateAreaMode();
                    application.panel.navItemCreateNewArea.buildContent();
                },
                saveNewPolygon: function () {                  
                    var unmappedAreaID = $("#unmappedSuburbSelector").val();
                    if (!unmappedAreaID) {
                        alert("Please select an existing unmapped Seeff suburb against which this data will be saved.");
                        return;
                    }
                    var selectedOption = $("#unmappedSuburbSelector option:selected");
                    var areaName = selectedOption.data('areaName');
                    var licenseID = $("#licenseIDSelect").val();
                    //if (!licenseID) {
                    //    alert("Must specify a Seeff License ID for the new suburb");
                    //    return;
                    //}

                    var suburbModel = application.services.serviceModels.buildSuburbModel();
                    suburbModel.PolyWKT = application.Google.getPolyAsGeographyString(application.panel.navItemCreateNewArea.newAreaPolygon);
                    suburbModel.AreaName = areaName;
                    suburbModel.LicenseID = licenseID;
                    suburbModel.SeeffAreaID = unmappedAreaID;
                    application.services.serviceControllers.saveSuburb(suburbModel, application.panel.navItemCreateNewArea.controllers.saveSuburbCallback);
                },
                saveSuburbCallback: function (result) {
                    var step3Tick = $("#createPolyStep3CompleteTick");
                    var errorTick = $("#createPolyStep3ErrorTick");
                    var resultContainer = $("#saveNewAreaResults");
                    var step3 = $("#createAreaStep3");
                    var saveBtn = $("#saveNewAreaBtn");
                    if (result.Successful) {
                        application.user.SeeffAreaCollection.push(result.SuburbResult);
                        application.user.SeeffAreaCollection.sort(function (a, b) {
                            if (a.AreaName > b.AreaName) return 1;
                            if (a.AreaName < b.AreaName) return -1;
                            return 0;
                        });
                        application.user.SeeffAreaCollectionLookup[result.SuburbResult.SeeffAreaID] = result.SuburbResult;
                        application.panel.navItemCreateNewArea.newAreaPolygon.setMap(null);
                        application.panel.navItemCreateNewArea.newAreaPolygon = null;
                        application.stateManager.handleExitEditPolyMode();
                        application.Google.createSuburbPoly(result.SuburbResult, {render: true});
                        application.panel.navItemAreaSelection.addSuburbToCache(result.SuburbResult);
                        application.stateManager.setActiveSuburb(result.SuburbResult);
                        result.SuburbResult.PolygonInstance.setOptions({ fillOpacity: 0.5 });
                        application.Google.showSuburbInfoWindow(result.SuburbResult);
                        
                        step3Tick.removeClass('step-nonactive').addClass('step-active');
                        errorTick.removeClass('step-active').addClass('step-nonactive');
                        saveBtn.prop('disabled', true);
                        resultContainer.append("Area saved successfully!");
                    } else {
                        step3Tick.removeClass('step-active').addClass('step-nonactive');
                        errorTick.removeClass('step-nonactive').addClass('step-active');
                        saveBtn.prop('disabled', true);
                        resultContainer.append("Save failed, please restart the application to access updated information - " + result.SaveMessage);
                    }
                }
            }
        }
    })
});