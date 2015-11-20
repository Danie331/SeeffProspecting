
$(function () {
    $.extend(application, {
        stateManager: {
            activeNavItem: null,
            activeSuburb: null,
            activeSuburbInEditMode: false,
            createAreaMode: false,
            handleExitEditPolyMode: function () {
                if (application.stateManager.activeSuburb) {
                    var suburb = application.stateManager.activeSuburb;
                    var polygon = suburb.PolygonInstance;

                    if (polygon.infoWindow) {
                        polygon.infoWindow.close();
                        polygon.infoWindow = null;
                    }
                    polygon.setMap(null);
                    polygon = null;

                    application.Google.drawPoly(suburb);

                    application.stateManager.activeSuburbInEditMode = false;
                }
            },
            handleEnterPolyEditMode: function () {
                var suburb = application.stateManager.activeSuburb;
                suburb.PolygonInstance.setOptions({ editable: true });
                suburb.PolygonInstance.setOptions({ fillOpacity: 0.5 });
                suburb.PolygonInstance.selected = true;
                application.stateManager.activeSuburbInEditMode = true;
            },
            handleEnterCreateAreaMode: function () {
                application.stateManager.activeSuburb = null;
                application.stateManager.activeSuburbInEditMode = false;
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
            setActiveSuburb: function (activeSuburb) {
                application.stateManager.activeSuburb = activeSuburb;
                if (activeSuburb != null) {
                }
                else {
                }
            },
            updateActiveSuburb: function (suburbData) {
                var activeSuburb = application.stateManager.activeSuburb;
                activeSuburb.PolyWKT = suburbData.PolyWKT;
                activeSuburb.CentroidWKT = suburbData.CentroidWKT;
                application.Google.drawPoly(activeSuburb);
            }
        }
    });
});