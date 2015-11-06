
$(function () {
    $.extend(application.panel, {
        navItemCreateNewArea: {
            buildContent: function () {
                var container = $("#contentContainerContent");
                if (application.panel.navItemCreateNewArea.contentCache) {
                    container.html(application.panel.navItemCreateNewArea.contentCache);
                } else {
                    container.html('').load("../HTML/CreatePoly.html", function (content) {
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
                    application.Google.drawingManager.setMap(application.Google.map); //exit this mode?, green tick when step complete, edit once drawn, highlight steps as they become available
                }
            }
        }
    })
});