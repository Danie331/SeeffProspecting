
$(function () {
    $.extend(application, {
        stateManager: {
            activeNavItem: null,
            activeSuburb: null,
            activeLicense: null,
            activeSuburbInEditMode: false,
            activeLicenseInEditMode: false,
            createAreaMode: false,
            allSuburbsShown: true,
            allLicensesShown: false,
            allTerritoriesShown: false,
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
                    var license = application.stateManager.activeLicense;
                    var polygon = license.PolygonInstance;
                    application.Google.resetPolygonSelection();
                    polygon.setMap(null);
                    polygon = null;

                    application.Google.createLicensePoly(license, { render: true});

                    application.stateManager.activeSuburbInEditMode = false;
                    application.stateManager.activeLicenseInEditMode = false;
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
            setActiveSuburb: function (suburb) {
                if (application.stateManager.activeLicense) {
                    application.Google.createLicensePoly(application.stateManager.activeLicense, { render: false });
                    application.stateManager.activeLicense = null;
                }
                application.stateManager.activeSuburb = suburb;
            },
            setActiveLicense: function (license) {
                if (application.stateManager.activeSuburb) {
                    application.Google.createSuburbPoly(application.stateManager.activeSuburb, { render: false });
                    application.stateManager.activeSuburb = null;
                }
                application.stateManager.activeLicense = license;
            },
            updateActiveSuburb: function (suburbData) {
                var activeSuburb = application.stateManager.activeSuburb;
                activeSuburb.PolyWKT = suburbData.PolyWKT;
                activeSuburb.CentroidWKT = suburbData.CentroidWKT;
                application.Google.createSuburbPoly(activeSuburb, { render: true });
            },
            updateActiveLicense: function (licenseData) {
                var activeLicense = application.stateManager.activeLicense;
                activeLicense.PolyWKT = licenseData.PolyWKT;
                activeLicense.CentroidWKT = licenseData.CentroidWKT;
                application.Google.createLicensePoly(activeLicense, {render: true});
            }
        }
    });
});