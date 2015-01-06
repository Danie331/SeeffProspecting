

var areaData = [];
var areaTypes = [];

function loadAreaData(showLoadingDialog) {

    areaData.length = 0;
    areaTypes.length = 0;

    if (showLoadingDialog) {
        $.blockUI({ message: "<h1>Loading areas....</h1>" });
    }
    dataRequest("load_areas_data", null, function (data) {
        areaData = data;
        dataRequest("get_area_types", null, function (data) {
            areaTypes = data;

            if (showLoadingDialog) {
                $.unblockUI();
            }
        });
    });   
}

function loadAndShowKML() {

    var ctaLayer = new google.maps.KmlLayer({
        url: 'http://boss.seeff.com/mapping/template.kml'
    });
    ctaLayer.setMap(map);
}

function dataRequest(instruction, inputData, actionAfterRequest) {

    var inputPacket = $.extend({Instruction: instruction}, inputData);
    $.ajax({
        type: "POST",
        url: "AdminDataManager.ashx",
        data: JSON.stringify(inputPacket),
        success: function (data, textStatus, jqXHR) {
            if (textStatus == "success") {
                actionAfterRequest(data);
            } else {
                alert('Error occurred: ' + data);
            }
        },
        error: function (textStatus, errorThrown) {
            alert(textStatus.responseText);
            alert(errorThrown);
        },
        dataType: "json"
    });
}

//function updateCache() {

//    $.blockUI({ message: "<h1>Updating the cache...</h1>" });
//    dataRequest("update_cache", null, function () {

//        dataRequest("load_areas_data", null, function (data) {
//            areaData = data;
//            $.unblockUI();
//        });
//    });
//}

function toggleDrawingFunctionality() {

    if (drawingManager == null) {
        drawingManager = new google.maps.drawing.DrawingManager({
            drawingMode: google.maps.drawing.OverlayType.POLYGON,
            drawingControl: true,
            drawingControlOptions: {
                position: google.maps.ControlPosition.TOP_CENTER,
                drawingModes: [google.maps.drawing.OverlayType.POLYGON]
            }
        });

        google.maps.event.addListener(drawingManager, 'polygoncomplete', function (poly) {

            poly.Area = null; // No area associated with this poly yet.
            poly.setEditable(true);
            google.maps.event.addListener(poly, 'rightclick', function (event) {              

                if (event.vertex != null) {
                    poly.getPath().removeAt(event.vertex);
                } else {
                    $('.context-menu-polyoptions').contextMenu({ x: currentMousePos.x, y: currentMousePos.y });
                    currentActivePoly = poly;
                }
            });
            google.maps.event.addListener(poly.getPath(), 'insert_at', function (index, obj) {});
            google.maps.event.addListener(poly.getPath(), 'set_at', function (index, obj) {});
        });
    }

    if (drawingManager.map) {
        drawingManager.setMap(null);
    }
    else {
        drawingManager.setMap(map);
    }
}

function loadAreaByIdAndType(areaId, resCommAgri) {

    dataRequest("load_area", { LocationID: areaId, ResCommAgri: resCommAgri }, function (data) {
        
        if (data.PolyCoords.length > 0) {
            var area = data;
            drawPolygonForArea(area);
            map.setCenter(new google.maps.LatLng(area.PolyCoords[0].Lat, area.PolyCoords[0].Lng));
            map.setZoom(13);
        }
        else {
            alert('No kml present for this area.');
        }
    });
}

function gotoLocation(lat, lng) {
    var loc = new google.maps.LatLng(lat, lng);
    map.setCenter(loc);

    var address = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + lat + "," + lng + "&sensor=true";
    $.ajax({
        type: "POST",
        url: address,        
        success: function (data, textStatus, jqXHR) {
            if (textStatus == "success") {
                var formattedAddress = data.results[0].formatted_address;
                var infWindow = createInfoWindow(lat, lng, "Lat: " + lat + "<br />Lng: " + lng + "<br/>" + formattedAddress);
                infWindow.open(map);
            }
        }
    });
}

function tryLocateSuburbUnderPoint(rightClickedLocation) {

    var infWindow = createInfoWindow(rightClickedLocation.Lat, rightClickedLocation.Lng, "Attempting to find area/suburb here...");
    infWindow.open(map);

    function updateInfoWindow(infWindow, suburbName) {
        var content = $("<div style='line-height:1.35;overflow:hidden;white-space:nowrap;'></div>");
        content.append(suburbName ? "Suburb found: " + suburbName : "Could not find an area/suburb under this point.");
        infWindow.setContent(content.html());
    }

    dataRequest("find_area_under_location", { LatLng: { Lat: rightClickedLocation.Lat, Lng: rightClickedLocation.Lng } }, function (data) {

        if (data && data.PolyCoords.length > 0) {
            var area = data;
            drawPolygonForArea(area);
            updateInfoWindow(infWindow, area.LocationName);
        }
        else {
            updateInfoWindow(infWindow, null);
        }
    });
}

function drawPolygonForArea(area) {

    var coords = [];
    for (var j = 0; j < area.PolyCoords.length; j++) {
        var lat = area.PolyCoords[j].Lat;
        var lng = area.PolyCoords[j].Lng;
        coords.push(new google.maps.LatLng(lat, lng));
    }

    // The last coordinate set to add will be the starting point 
    coords.push(new google.maps.LatLng(area.PolyCoords[0].Lat, area.PolyCoords[0].Lng));
    var poly = new google.maps.Polygon({
        paths: coords,
        editable: false
    });

    poly.setMap(map);
    poly.Area = area;

    google.maps.event.addListener(poly, 'click', function (event) {
        var infoWindow = createInfoWindow(event.latLng.lat(), event.latLng.lng(), area.LocationName);
        infoWindow.setMap(map);
        poly.setEditable(true);
    });

    google.maps.event.addListener(poly, 'rightclick', function (event) {

        if (event.vertex != null) {
            poly.getPath().removeAt(event.vertex);
        }
        else {
            $('.context-menu-polyoptions').contextMenu({ x: currentMousePos.x, y: currentMousePos.y });
            currentActivePoly = poly;
        }
    });
    google.maps.event.addListener(poly.getPath(), 'insert_at', function (index, obj) {
        poly.Area.PolyCoords = createPolyPathArray(poly);
    });
    google.maps.event.addListener(poly.getPath(), 'set_at', function (index, obj) {
        poly.Area.PolyCoords = createPolyPathArray(poly);
    });
}

function createPolyPathArray(poly) {

    var coords = [];
    var vertices = poly.getPath();
    for (var i = 0; i < vertices.getLength() ; i++) {
        var lat = vertices.getAt(i).lat();
        var lng = vertices.getAt(i).lng();
        coords.push({ Lat: lat, Lng: lng});
    }

    return coords;
}

// This function does an UPSERT depending on whether the the area exists or is new.
function saveArea(area) {

    // Validate user input
    var invalidInput = null;
    if (area.LocationName == null || area.LocationName == '') {
        invalidInput = "Area Name";
    }
    if (area.ParentLocationId == null || area.ParentLocationId == 0) {
        invalidInput = "Area parent";
    }
    if (area.AreaTypeId == null || area.AreaTypeId == 0) {
        invalidInput = "Area type";
    }
    if (area.ResCommAgri == null || area.ResCommAgri == '') {
        invalidInput = "Area polygon type [Res/Comm/Agri]";
    }
    if (invalidInput) {
        alert('One or more fields are not populated:  ' + invalidInput);
    }
    else { // Perform actual save
        $.blockUI({ message: "<h1>Saving area....</h1>" });
        dataRequest("add_or_update_area", area, function (data) {

            if (data.Status == "success") {
                area.LocationID = data.LocationId;
                loadAreaData(false);

                $.unblockUI();
            }
            else {
                alert('Error occurred ' + data);
            }
        });
    }
}