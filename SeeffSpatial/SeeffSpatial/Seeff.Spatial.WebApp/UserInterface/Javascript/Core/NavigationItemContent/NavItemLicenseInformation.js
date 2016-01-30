
$(function () {
    $.extend(application.panel, {
        navItemLicenseInformation: {
            contentCache: null,
            currentLicense: null,
            buildContent: function () {
                var container = $("#contentContainerContent");
                container.empty();

                if (application.panel.navItemLicenseInformation.contentCache) {
                    container.html(application.panel.navItemLicenseInformation.contentCache);
                } else {
           
                    container.html('').load("UserInterface/HTML/LicenseInformation.html", function (content) {                        
                        container.html(content);

                        var licenseSelector = $("#licenseSelector").append("<option value='' />");
                        $.each(application.user.SeeffLicenses, function (idx, lic) {
                            var option = $('<option />', { value: lic.LicenseID }).text(lic.LicenseID + " - " + lic.LicenseName);
                            licenseSelector.append(option);
                        });

                        application.panel.navItemLicenseInformation.contentCache = container.html();
                    });
                }
            },
            handleLicenseItemSelect: function () {
                application.stateManager.handleExitLicenseInfoScreen();
                $("#showAssociatedSuburbsCheckbox").prop("checked", false);
                $("#showOrphansForLicense").prop("checked", false);
                if (application.user.SeeffAreaCollection.length) {
                    // clear everything first
                    $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                        application.Google.createSuburbPoly(suburb, { render: false });
                    });
                }
                var mc = application.Google.markerClusterer;
                if (mc) {
                    mc.clearMarkers();
                }

                var selectedValue = $("#licenseSelector").val();
                if (selectedValue) {
                    var targetLicense = $.grep(application.user.SeeffLicenses, function (lic) {
                        return lic.LicenseID == selectedValue;
                    })[0];
                    application.panel.navItemLicenseInformation.currentLicense = targetLicense;
                    application.Google.createLicensePoly(targetLicense, { render: true });
                    application.Google.showLicenseInfoWindow(targetLicense);
                    targetLicense.PolygonInstance.selected = true;
                    targetLicense.PolygonInstance.setOptions({ fillOpacity: 0.5 });
                    application.Google.centreMapAndZoom(targetLicense.CentroidInstance, 10);
                    application.stateManager.setActiveLicense(targetLicense);

                    $("#showAssociatedSuburbsCheckbox").prop("disabled", false);
                    $("#showOrphansForLicense").prop("disabled", false);
                }
                else {
                    $("#showAssociatedSuburbsCheckbox").prop("disabled", true);
                    $("#showOrphansForLicense").prop("disabled", true);
                }
            },
            handleShowAssociatedSuburbsClick: function () {
                application.stateManager.allSuburbsShown = true;
                if (application.user.SeeffAreaCollection.length) {
                    $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                        application.Google.createSuburbPoly(suburb, { render: false });
                    });
                }
                if ($("#showAssociatedSuburbsCheckbox").is(":checked")) {
                    var license = application.stateManager.activeLicense;
                    if (!application.user.SeeffAreaCollection.length) {
                        application.panel.navItemAreaSelection.getSuburbs(function () {
                            $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                                application.Google.createSuburbPoly(suburb, { render: false });
                            });

                            $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                                if (license.LicenseID == suburb.LicenseID) {
                                    application.Google.createSuburbPoly(suburb, { render: true });
                                }
                            });
                        });
                    } else {                        
                        $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                            if (license.LicenseID == suburb.LicenseID) {
                                application.Google.createSuburbPoly(suburb, { render: true });
                            }
                        });
                    }
                } else {
                    if (application.user.SeeffAreaCollection.length) {
                        $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                            application.Google.createSuburbPoly(suburb, { render: false });
                        });
                    }
                }
            },
            handleShowOrphansForLicenseClick: function () {
                // clear here + cache per license.
                var license = application.panel.navItemLicenseInformation.currentLicense;
                if ($("#showOrphansForLicense").is(":checked")) {
                    if (license) {
                        var licenseModel = application.services.serviceModels.buildLicenseModel(license);
                        application.services.serviceControllers.retrieveOrphanRecords(licenseModel, function (result) {
                            application.utilities.yieldingLoop(result.Orphans.length, 1, function (i) {
                                var orphan = result.Orphans[i];
                                var pos = { lat: orphan.LatLng.Lat, lng: orphan.LatLng.Lng };
                                var marker = new google.maps.Marker({
                                    position: pos,
                                    map: application.Google.map,
                                    title: '' + orphan.LightstonePropertyID
                                });
                                orphan.Marker = marker;
                                application.Google.markerClusterer.addMarker(marker);
                            }, function () { });
                        });
                    }
                } else {
                    var mc = application.Google.markerClusterer;
                    if (mc) {
                        mc.clearMarkers();
                    }
                }
            },
            handleExportToKMLClick: function () {
                var selectedValue = $("#licenseSelector").val();
                if (selectedValue) {
                    var includeSuburbs = $("#showAssociatedSuburbsCheckbox").is(":checked");
                    var includeOrphanedRecords = $("#showOrphansForLicense").is(":checked");
                    application.services.serviceControllers.exportLicenseToKML(
                        {   LicenseID: selectedValue, 
                            IncludeSuburbs: includeSuburbs, 
                            IncludeOrphanedRecords: includeOrphanedRecords
                        }, application.panel.navItemLicenseInformation.handleExportToKMLCallback);
                } else {
                    alert('No license selected');
                }
            },
            handleExportToKMLCallback: function (fileData) {
                if (fileData.Successful) {
                    application.utilities.download(fileData.SeeffLicense.LicenseName);
                } else {
                    alert(fileData.Message);
                }
            }
        }
    });
})