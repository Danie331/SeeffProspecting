
$(function () {
    $.extend(application, {
        stateManager: {
            activeNavItem: null,
            activeSuburb: null,
            activeLicense: null,
            activeTerritory: null,
            activeSuburbInEditMode: false,
            activeLicenseInEditMode: false,
            activeTerritoryInEditMode: false,
            createAreaMode: false,
            allSuburbsShown: false,
            allLicensesShown: false,
            allTerritoriesShown: false,
            fileUploadCounter: [],
            handleExitEditPolyMode: function () {
                if (application.stateManager.activeSuburb) {
                    var suburb = application.stateManager.activeSuburb;
                    var polygon = suburb.PolygonInstance;
                    application.Google.resetPolygonSelection();
                    polygon.setMap(null);
                    polygon = null;

                    application.Google.createSuburbPoly(suburb, { render: true });

                    application.stateManager.activeSuburbInEditMode = false;
                    application.stateManager.activeLicenseInEditMode = false;
                }
                if (application.stateManager.activeLicense) {
                    if (application.panel.navItemEditLicense.newLicensePolygon) {
                        application.panel.navItemEditLicense.newLicensePolygon.setMap(null);
                        application.panel.navItemEditLicense.newLicensePolygon = null;
                    }
                    var license = application.stateManager.activeLicense;
                    var polygon = license.PolygonInstance;
                    application.Google.resetPolygonSelection();
                    polygon.setMap(null);
                    polygon = null;

                    application.Google.createLicensePoly(license, { render: true});

                    application.stateManager.activeSuburbInEditMode = false;
                    application.stateManager.activeLicenseInEditMode = false;
                }
                if (application.stateManager.activeTerritory) {
                    var territory = application.stateManager.activeTerritory;
                    var polygon = territory.PolygonInstance;
                    application.Google.resetPolygonSelection();
                    polygon.setMap(null);
                    polygon = null;

                    application.Google.createTerritoryPoly(territory, { render: true });

                    application.stateManager.activeSuburbInEditMode = false;
                    application.stateManager.activeLicenseInEditMode = false;
                    application.stateManager.activeTerritoryInEditMode = false;
                }
            },
            handleEnterPolyEditMode: function () {
                if (application.stateManager.activeSuburb) {
                    var suburb = application.stateManager.activeSuburb;
                    suburb.PolygonInstance.setOptions({ editable: true });
                    suburb.PolygonInstance.setOptions({ fillOpacity: 0.5 });
                    suburb.PolygonInstance.selected = true;
                    application.stateManager.activeSuburbInEditMode = true;
                }
                if (application.stateManager.activeLicense) {
                    var license = application.stateManager.activeLicense;
                    license.PolygonInstance.setOptions({ editable: true });
                    license.PolygonInstance.setOptions({ fillOpacity: 0.5 });
                    license.PolygonInstance.selected = true;
                    application.stateManager.activeLicenseInEditMode = true;
                }
                if (application.stateManager.activeTerritory) {
                    var territory = application.stateManager.activeTerritory;
                    territory.PolygonInstance.setOptions({ editable: true });
                    territory.PolygonInstance.setOptions({ fillOpacity: 0.5 });
                    territory.PolygonInstance.selected = true;
                    application.stateManager.activeTerritoryInEditMode = true;
                }
            },
            handleEnterCreateAreaMode: function () {
                application.stateManager.activeSuburb = null;
                application.stateManager.activeSuburbInEditMode = false;
                application.stateManager.activeLicenseInEditMode = false;
                application.stateManager.createAreaMode = true;
            },
            handleExitCreateAreaMode: function () {
                application.stateManager.createAreaMode = false;

                if (application.panel.navItemCreateNewArea.newAreaPolygon) {
                    application.panel.navItemCreateNewArea.newAreaPolygon.setMap(null);
                    application.panel.navItemCreateNewArea.newAreaPolygon = null;
                }

                if (application.Google.drawingManager != null)
                    application.Google.drawingManager.setMap(null);
                // rollback the create steps
            },
            handleExitLicenseInfoScreen: function () {
                //if (application.stateManager.activeLicense) {
                //    application.Google.createLicensePoly(application.stateManager.activeLicense, { render: false });
                //}
                //application.stateManager.activeLicense = null;
                //if (application.user.SeeffAreaCollection) {
                //    $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                //        if (application.stateManager.activeSuburb != suburb) {
                //            application.Google.createSuburbPoly(suburb, { render: false });
                //        }
                //    });
                //}
                var mc = application.Google.markerClusterer;
                if (mc) {
                    mc.clearMarkers();
                }
            },
            setActiveSuburb: function (suburb) {
                if (application.stateManager.activeLicense) {
                    application.Google.createLicensePoly(application.stateManager.activeLicense, { render: true });
                    application.stateManager.activeLicense = null;
                }
                if (application.stateManager.activeTerritory) {
                    application.Google.createTerritoryPoly(application.stateManager.activeTerritory, { render: true });
                    application.stateManager.activeTerritory = null;
                }
                if (!suburb && application.stateManager.activeSuburb != null) {
                    application.stateManager.handleExitEditPolyMode();
                    application.Google.resetPolygonSelection();
                }
                application.stateManager.activeSuburb = suburb;
            },
            setActiveLicense: function (license) {
                if (application.stateManager.activeSuburb) {
                    application.Google.createSuburbPoly(application.stateManager.activeSuburb, { render: true });
                    application.stateManager.activeSuburb = null;
                }
                if (application.stateManager.activeTerritory) {
                    application.Google.createTerritoryPoly(application.stateManager.activeTerritory, { render: true });
                    application.stateManager.activeTerritory = null;
                }
                if (!license && application.stateManager.activeLicense != null) {
                    application.stateManager.handleExitEditPolyMode();
                    application.Google.resetPolygonSelection();
                }
                application.stateManager.activeLicense = license;
            },
            setActiveTerritory: function(territory) {
                if (application.stateManager.activeLicense) {
                    application.Google.createLicensePoly(application.stateManager.activeLicense, { render: true });
                    application.stateManager.activeLicense = null;
                }
                if (application.stateManager.activeSuburb) {
                    application.Google.createSuburbPoly(application.stateManager.activeSuburb, { render: true });
                    application.stateManager.activeSuburb = null;
                }
                if (!territory && application.stateManager.activeTerritory != null) {
                    application.stateManager.handleExitEditPolyMode();
                    application.Google.resetPolygonSelection();
                }
                application.stateManager.activeTerritory = territory;
            },
            updateActiveSuburb: function (suburbData) {
                var activeSuburb = application.stateManager.activeSuburb;
                activeSuburb.PolyWKT = suburbData.PolyWKT;
                activeSuburb.CentroidWKT = suburbData.CentroidWKT;
                activeSuburb.LicenseID = suburbData.LicenseID;
                application.Google.createSuburbPoly(activeSuburb, { render: true });
            },
            updateActiveLicense: function (licenseData) {
                var activeLicense = application.stateManager.activeLicense;
                activeLicense.PolyWKT = licenseData.PolyWKT;
                activeLicense.CentroidWKT = licenseData.CentroidWKT;
                application.Google.createLicensePoly(activeLicense, {render: true});
            },
            updateActiveTerritory: function (territoryData) {
                var activeTerritory = application.stateManager.activeTerritory;
                activeTerritory.PolyWKT = territoryData.PolyWKT;
                activeTerritory.CentroidWKT = territoryData.CentroidWKT;

                application.Google.createTerritoryPoly(activeTerritory, { render: true });
            }
        }
    });
});