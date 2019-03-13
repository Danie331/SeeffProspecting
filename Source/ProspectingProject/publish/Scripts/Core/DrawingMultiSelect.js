var drawingManager = null;

var drawingMode = false;

var multiSelectMode = false;

var selectedMarkers = []; // must reset when exiting multi-select mode

var userPolygons = [];

function toggleMultiSelectMode(value) {
    if (value != null) {
        multiSelectMode = value;
    }
    else {
        multiSelectMode = !multiSelectMode;
    }

    if (!multiSelectMode) {
        removeMarkersFromSelection();
    }

    var multiSelectSticker = $("#multiSelectMode");
    if (multiSelectMode) {
        multiSelectSticker.css('display', 'block');
        closeInfoWindow();
    } else {
        multiSelectSticker.css('display', 'none');
    }
}

function addMarkerToSelection(marker, mustTriggerUpdate, addOnlyThisUnit, fromFilter) {
    // Must include SS hey.
    // check exitsing
    // reset all markers too when resetting array
    // check how LS search and ""view property" behave in multi-select mode
    // clicking marker again removes from selection
    // all units of selected ss must be same
    // polygon selection
    // sms logging (put code into separate folders)
    // calculate cost of the batch and display.

    var property = marker.ProspectingProperty;
    if (property.SS_FH == 'SS' || property.SS_FH == 'FS') {
        if (addOnlyThisUnit) {
            flagMarkerSelected(marker, true);
            //marker.setIcon(getIconForMarker(marker));
            if (selectedMarkers.indexOf(marker) == -1) {
                selectedMarkers.push(marker);
            }
        } else {
            var ssUnits = $.grep(currentSuburb.ProspectingProperties, function (pp) {
                if (!pp.SS_UNIQUE_IDENTIFIER) return false;
                return pp.SS_UNIQUE_IDENTIFIER == property.SS_UNIQUE_IDENTIFIER;
            });

            if (fromFilter) {
                ssUnits = $.grep(ssUnits, function (u) {
                    return u.Whence == 'from_filter';
                });
            }

            var requiresOwnerUpdates = false;
            $.each(ssUnits, function (idx, unit) {
                if (unit.LatestRegDateForUpdate) {
                    requiresOwnerUpdates = true;
                }
            });
            if (!requiresOwnerUpdates) {
                $.each(ssUnits, function (idx, u) {
                    flagMarkerSelected(u.Marker, true);
                    u.Marker.setIcon(getIconForMarker(u.Marker));
                    if (selectedMarkers.indexOf(u.Marker) == -1) {
                        selectedMarkers.push(u.Marker);
                    }
                });
            }
        }
    }
    else {
        if (/*marker.ProspectingProperty.Prospected && */marker.ProspectingProperty.LatestRegDateForUpdate == null) {
            if (fromFilter == false || (fromFilter == true && marker.ProspectingProperty.Whence == 'from_filter')) {
                flagMarkerSelected(marker, true);
                marker.setIcon(getIconForMarker(marker));
                if (selectedMarkers.indexOf(marker) == -1) {
                    selectedMarkers.push(marker);
                }
            }
        }
    }

    if (mustTriggerUpdate) {
        // fire update of contactables here
        updateCommunicationsContacts();
    }
}

function flagMarkerSelected(marker, selected) {
    marker.IsPartOfSelection = selected;
}

function removeMarkersFromSelection(marker) {
    if (marker) {
        marker.IsPartOfSelection = false;
        var property = marker.ProspectingProperty;
        if (property.SS_FH == 'SS' || property.SS_FH == 'FS') {
            var ssUnits = $.grep(currentSuburb.ProspectingProperties, function (pp) {
                if (!pp.SS_UNIQUE_IDENTIFIER) return false;
                return pp.SS_UNIQUE_IDENTIFIER == property.SS_UNIQUE_IDENTIFIER;
            });

            var unitMarkers = [];
            $.each(ssUnits, function (idx, el) {
                unitMarkers.push(el.Marker);
                el.Marker.IsPartOfSelection = false;
                el.Marker.setIcon(getIconForMarker(el.Marker));
            });

            selectedMarkers = $.grep(selectedMarkers, function (m) {
                return unitMarkers.indexOf(m) == -1;
            });
        } else {
            selectedMarkers = $.grep(selectedMarkers, function (m) {
                return m != marker;
            });
            marker.setIcon(getIconForMarker(marker));
        }
    }
    else {
        // If nothing passed in the remove the whole selection
        $.each(selectedMarkers, function (idx, m) {
            m.IsPartOfSelection = false;
            m.setIcon(getIconForMarker(m));
        });
        selectedMarkers.length = 0;

        if (userPolygons && userPolygons.length) {
            $.each(userPolygons, function (idx, poly) {
                poly.setMap(null);
            });
            userPolygons.length = 0;
        }
    }

    // fire call to update contactables list here
    updateCommunicationsContacts();
}

function selectPropertiesInsidePolygon(polygon) {
    // exit drawing mode, clear selection and empty arrays
    var fromFilter = false;
    $.each(currentSuburb.ProspectingProperties, function (idx, pp) {
        if (pp.Whence == 'from_filter') {
            fromFilter = true;
        }
    });
    $.each(currentSuburb.ProspectingProperties, function (idx, pp) {
        if (google.maps.geometry.poly.containsLocation(pp.Marker.position, polygon)) {
            addMarkerToSelection(pp.Marker, false, false, fromFilter);
        }
    });

    // fire call to update contactables here.
    updateCommunicationsContacts();
}

function getMarkersInsidePolygons() {
    var allMarkersInsidePolygons = [];
    $.each(userPolygons, function (idx, poly) {
        $.each(currentSuburb.ProspectingProperties, function (idx, pp) {
            if (google.maps.geometry.poly.containsLocation(pp.Marker.position, poly)) {
                if (allMarkersInsidePolygons.indexOf(pp.Marker) == -1 && pp.Marker.IsPartOfSelection) {
                    allMarkersInsidePolygons.push(pp.Marker);
                }
            }
        });
    });

    return allMarkersInsidePolygons;
}

function removePolygonsWithMarkers() {
    var markersInPolys = getMarkersInsidePolygons();
    $.each(markersInPolys, function (idx, marker) {
        var targetMarker = $.grep(selectedMarkers, function (sm) {
            return sm == marker;
        })[0];

        targetMarker.IsPartOfSelection = false;
        targetMarker.setIcon(getIconForMarker(targetMarker));

        selectedMarkers = $.grep(selectedMarkers, function (m) {
            return m != marker;
        });
    });

    if (userPolygons && userPolygons.length) {
        $.each(userPolygons, function (idx, poly) {
            poly.setMap(null);
        });
        userPolygons.length = 0;
    }

    updateCommunicationsContacts();
}

function toggleDrawingMode() {
    closeInfoWindow();
    drawingMode = !drawingMode;
    if (!drawingMode) {
        if (drawingManager) {
            drawingManager.setMap(null);
        }
    } else {
        if (!drawingManager) {
            drawingManager = new google.maps.drawing.DrawingManager({
                drawingMode: google.maps.drawing.OverlayType.POLYGON,
                polygonOptions: { strokeColor: 'red' }
            });
            drawingManager.setMap(map);
            google.maps.event.addListener(drawingManager, 'polygoncomplete', function (polygon) {
                userPolygons.push(polygon);
                selectPropertiesInsidePolygon(polygon);
                //google.maps.event.addListener(polygon, 'rightclick', function (event) {
                //    var point = fromLatLngToPoint(event.latLng, map);
                //    $('.context-menu-polygon').contextMenu({ x: point.x, y: point.y });
                //});
                toggleDrawingMode();
            });
        }

        drawingManager.setMap(map);
    }
}