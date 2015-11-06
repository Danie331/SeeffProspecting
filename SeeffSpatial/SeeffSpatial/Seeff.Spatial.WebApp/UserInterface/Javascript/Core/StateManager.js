
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

                application.stateManager.activeSuburbInEditMode = true;
            },
            handleEnterCreateAreaMode: function () {
                application.stateManager.activeSuburb = null;
                application.stateManager.activeSuburbInEditMode = false;
                application.stateManager.createAreaMode = true;
            },
            handleExitCreateAreaMode: function () {
                application.stateManager.createAreaMode = false;
                if (application.Google.drawingManager != null)
                    application.Google.drawingManager.setMap(null);
                // rollback the create steps
            }
        }
    });
});