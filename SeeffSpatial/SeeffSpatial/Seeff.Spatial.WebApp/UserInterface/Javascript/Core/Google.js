
$(function () {
    $.extend(application, {
        Google: {
            map: null,
            drawingManager: null,
            initMap: function () {
                application.Google.map = new google.maps.Map(document.getElementById('map'), {
                    zoomControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP },
                    panControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP }
                });

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
                //application.Google.drawingManager.setMap(application.Google.map);
                application.Google.map.setZoom(6);
                var center = new google.maps.LatLng( -29.258248, 15.358887 );
                application.Google.map.setCenter(center);
                google.maps.event.addListenerOnce(application.Google.map, 'idle', function () {
                    var infWindow = new google.maps.InfoWindow({ content: "" });
                    application.Google.findOffsetPoint(-29.687345, 22.669017, 0, 0); // Initialisation hack.
                });

                google.maps.event.addListener(application.Google.map, "rightclick", function () {

                    if (!application.stateManager.createAreaMode)
                        return;

                    if (application.Google.drawingManager != null)
                        application.Google.drawingManager.setMap(null);                   
                });
            },
            findOffsetPoint: function calcMapCenterWithOffset(lat, lng, offset_x, offset_y) {
                var proj = new google.maps.OverlayView();
                proj.draw = function () { };
                proj.setMap(application.Google.map);

                var currentProj = proj.getProjection();
                if (currentProj) {
                    var point = currentProj.fromLatLngToDivPixel(new google.maps.LatLng(lat, lng));
                }
                else {
                    return null;
                }

                point.x = point.x + offset_x;
                point.y = point.y + offset_y;

                var newLatLng = proj.getProjection().fromDivPixelToLatLng(point);
                return newLatLng;
            },
            centreMapAndZoom: function (centrePoint, zoomLevel) {
                application.Google.map.setZoom(zoomLevel);
                var offsetPoint = application.Google.findOffsetPoint(centrePoint.lat, centrePoint.lng, -250, 0);
                application.Google.map.setCenter(offsetPoint);
            },
            createPolyFromWKT: function(polyWKT) {                                
                var coordPairs = polyWKT.replace("POLYGON ((", "").replace("))", "").split(',');
                var outputPairs = [];
                $.each(coordPairs, function (idx, pairObject) {
                    var coordPair = pairObject.trim().split(' ');
                    outputPairs.push({ lat: Number(coordPair[1]), lng: Number(coordPair[0]) });
                });
                return outputPairs;
            },
            createPointFromWKT: function(centroidWKT) {
                var coordPair = centroidWKT.replace("POINT (", "").replace(")", "").split(' ');
                return { lat: Number(coordPair[1]), lng: Number(coordPair[0]) };
            },
            createSuburbPoly: function (suburb, drawOptions) {
                if (!drawOptions) {
                    drawOptions = {};
                }
                drawOptions.outline = drawOptions.outline ? drawOptions.outline : '#FF0000';
                drawOptions.fillcolour = drawOptions.fillcolour ? drawOptions.fillcolour : '#FF0000';

                if (suburb.PolygonInstance) {
                    if (suburb.PolygonInstance.infoWindow) {
                        suburb.PolygonInstance.infoWindow.close();
                        suburb.PolygonInstance.infoWindow = null;
                    }
                    suburb.PolygonInstance.setMap(null);
                    suburb.PolygonInstance = null;
                }
                var polygon = new google.maps.Polygon({
                    paths: application.Google.createPolyFromWKT(suburb.PolyWKT),
                    strokeColor: drawOptions.outline,
                    strokeOpacity: 0.8,
                    strokeWeight: 1.5,
                    fillColor: drawOptions.fillcolour,
                    fillOpacity: 0.0
                });
                if (drawOptions.render) {
                    polygon.setMap(application.Google.map);
                }
                suburb.PolygonInstance = polygon;
                suburb.CentroidInstance = application.Google.createPointFromWKT(suburb.CentroidWKT);
                google.maps.event.addListener(polygon, "mouseover", function () {
                    if (application.stateManager.activeSuburbInEditMode)
                        return;
                    if (application.stateManager.createAreaMode)
                        return;
                    if (this.selected)
                        return;
                    this.setOptions({ fillOpacity: 0.5 });
                });

                google.maps.event.addListener(polygon, "mouseout", function () {
                    if (this.selected)
                        return;
                    this.setOptions({ fillOpacity: 0.0 });
                });

                google.maps.event.addListener(polygon, "click", function () {
                    if (application.stateManager.allLicensesShown || application.stateManager.allTerritoriesShown)
                        return;
                    if (application.stateManager.createAreaMode)
                        return;
                    if (application.stateManager.activeSuburb != suburb) {
                        application.stateManager.handleExitEditPolyMode();
                        application.Google.resetPolygonSelection();
                        application.Google.showSuburbInfoWindow(suburb);
                    }
                    this.selected = true;
                    this.setOptions({ fillOpacity: 0.5 });

                    application.panel.navItemAreaSelection.selectSuburbFromPolyClick(suburb);
                });
            },
            createLicensePoly: function(license, drawOptions) {
                if (!drawOptions) {
                    drawOptions = {};
                }
                drawOptions.outline = drawOptions.outline ? drawOptions.outline : '#0000FF';
                if (license.PolygonInstance) {
                    if (license.PolygonInstance.infoWindow) {
                        license.PolygonInstance.infoWindow.close();
                        license.PolygonInstance.infoWindow = null;
                    }
                    license.PolygonInstance.setMap(null);
                    license.PolygonInstance = null;
                }
                var polygon = new google.maps.Polygon({
                    paths: application.Google.createPolyFromWKT(license.PolyWKT),
                    strokeColor: drawOptions.outline,
                    strokeOpacity: 0.8,
                    strokeWeight: 1.5,
                    fillOpacity: 0.0
                });
                if (drawOptions.render) {
                    polygon.setMap(application.Google.map);
                }
                license.PolygonInstance = polygon;
                license.CentroidInstance = application.Google.createPointFromWKT(license.CentroidWKT);

                google.maps.event.addListener(polygon, "click", function () {
                    if (application.stateManager.allSuburbsShown || application.stateManager.allTerritoriesShown)
                        return;
                    if (application.stateManager.createAreaMode)
                        return;
                    if (application.stateManager.activeLicense != license) {
                        application.stateManager.handleExitEditPolyMode();
                        application.Google.resetPolygonSelection();
                        application.Google.showLicenseInfoWindow(license);
                    }
                    this.selected = true;
                    this.setOptions({ fillOpacity: 0.5 });

                    application.panel.navItemAreaSelection.selectLicenseFromPolyClick(license);
                });
            },
            createTerritoryPoly: function(territory, drawOptions) {
                if (!drawOptions) {
                    drawOptions = {};
                }
                drawOptions.outline = drawOptions.outline ? drawOptions.outline : '#FF00FF';
                if (territory.PolygonInstance) {
                    territory.PolygonInstance.setMap(null);
                    territory.PolygonInstance = null;
                }
                var polygon = new google.maps.Polygon({
                    paths: application.Google.createPolyFromWKT(territory.PolyWKT),
                    strokeColor: drawOptions.outline,
                    strokeOpacity: 0.8,
                    strokeWeight: 1.5,
                    fillOpacity: 0.0
                });
                if (drawOptions.render) {
                    polygon.setMap(application.Google.map);
                }
                territory.PolygonInstance = polygon;
                territory.CentroidInstance = application.Google.createPointFromWKT(territory.CentroidWKT);
            },
            resetPolygonSelection: function () {
                if (application.stateManager.activeSuburb) {
                    var suburb = application.stateManager.activeSuburb;
                    if (suburb && suburb.PolygonInstance.selected) {
                        suburb.PolygonInstance.selected = false;
                        suburb.PolygonInstance.setOptions({ fillOpacity: 0.0 });
                        if (suburb.PolygonInstance.infoWindow) {
                            suburb.PolygonInstance.infoWindow.close();
                            suburb.PolygonInstance.infoWindow = null;
                        }
                    }
                }
                if (application.stateManager.activeLicense) {
                    var license = application.stateManager.activeLicense;
                    if (license && license.PolygonInstance.selected) {
                        license.PolygonInstance.selected = false;
                        license.PolygonInstance.setOptions({ fillOpacity: 0.0 });
                        if (license.PolygonInstance.infoWindow) {
                            license.PolygonInstance.infoWindow.close();
                            license.PolygonInstance.infoWindow = null;
                        }
                    }
                }
            },
            showSuburbInfoWindow: function (suburb) {
                var contentString = $("<div />")
                                    .append("Area name: " + suburb.AreaName)
                                    .append("<br />")
                                    .append("Seeff Area ID: " + suburb.SeeffAreaID)
                                    .append("<br />")
                                    .append("License ID: " + suburb.LicenseID)
                                    .append("<br />")
                                    .append("Territory ID: " + suburb.TerritoryID);

                var infoWindow = new google.maps.InfoWindow();
                infoWindow.setContent(contentString.html());
                infoWindow.setPosition(suburb.CentroidInstance);
                infoWindow.open(application.Google.map);

                suburb.PolygonInstance.infoWindow = infoWindow;
            },
            showLicenseInfoWindow: function(license) {
                var contentString = $("<div />").append("License ID: " + license.LicenseID);
                var infoWindow = new google.maps.InfoWindow();
                infoWindow.setContent(contentString.html());
                infoWindow.setPosition(license.CentroidInstance);
                infoWindow.open(application.Google.map);

                license.PolygonInstance.infoWindow = infoWindow;
            },
            getPolyAsGeographyString: function (polygon) {
                var path = polygon.getPath().getArray();
                var resultString = 'POLYGON ((';
                $.each(path, function (idx, latLngPair) {
                    resultString += latLngPair.lng() + ' ' + latLngPair.lat() + ','
                });
                resultString = resultString + '))';
                resultString = resultString.replace(',))', '))');

                return resultString;
            },
            getPointAsGeographyString: function (point) {
                var resultString = "POINT (";
                resultString += point.lng() + " " + point.lat() + ")";
                return resultString;
            },
            addRightClickHook: function () {
                google.maps.event.addListener(application.Google.map, "rightclick", function (event) {
                    var lat = event.latLng.lat();
                    var lng = event.latLng.lng();
                    application.services.post('/api/Home/GetSuburbFromPoint', { Lat: lat, Lng: lng } , '', function(result) {
                        debugger;
                    });
                });
            }
        }
    });
});