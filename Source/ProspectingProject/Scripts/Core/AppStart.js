var availableCredit = 0;

var suburbsInfo, allAgencies;
var map, menu, infowindow;
var menuItems = [];
var slidePanelOpen = true;

// Stores *all* domain-level information required by various features. This object maps to the C# type 'ProspectingContext'. 
var prospectingContext = null;

// Flag that indicates whether this is a Prospecting Manager - only PMs can flag contacts as POPI restricted etc.
var userIsProspectingManager;

function initialize() {
    // Make ajax call to load from server 
    var inputPacket = { Instruction: "load_application" };
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify(inputPacket),
        success: function (data, textStatus, jqXHR) {
            if (textStatus == "success") {
                $('#loadingDiv').remove();

                if (!data.Authenticated) {
                    window.location = "/NotAuthorised.aspx";
                }
                if (!handleResponseIfServerError(data)) {
                    return;
                }

                $('#mainpanel').css('display', 'block');

                prospectingContext = JSON.parse(data.StaticProspectingData);
                prospectingContext.LoggedInUser = data.UserGuid;

                userIsProspectingManager = data.IsProspectingManager;
                availableCredit = data.AvailableCredit;
                initializeMenuHtml();
                loadSuburbsInfo(data);
                initializeMap();

                createProspectingMenu(data);
                initEventHandlers();
            }
        },
        error: function (textStatus, errorThrown) { debugger; },
        dataType: "json"
    });

    // Handle the browser close event - cleanup
    $(window).bind("beforeunload", function () {
        unlockCurrentProperty();
    });

    $(document).mouseup(function (e) {
        var commMenu = $("#communicationMenu");
        if (!commMenu.is(e.target)) {
            commMenu.hide();
        }
    });
}

function initializeMap(defaultZoomAndLocation) {
    map = new google.maps.Map(document.getElementById("googleMap"),
        {
            zoom: 13,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            zoomControlOptions: {
                style: google.maps.ZoomControlStyle.DEFAULT,
                position: google.maps.ControlPosition.RIGHT_TOP
            },
            panControlOptions: {
                position: google.maps.ControlPosition.RIGHT_TOP
            }
        });

    map.setZoom(6);
    var center = new google.maps.LatLng(-29.687345, 22.669017);
    map.setCenter(center);
}

function loadSuburbsInfo(data) {
    suburbsInfo = data.AvailableSuburbs;
    if (suburbsInfo) {
        suburbsInfo = JSON.parse(suburbsInfo);

        for (var i = 0; i < suburbsInfo.length; i++) {
            suburbsInfo[i].IsInitialised = false;
        }
    }
}