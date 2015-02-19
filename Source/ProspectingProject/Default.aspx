<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prospecting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
     <title>Seeff Prospecting</title>

    <%-- Jquery --%>
    <script src="http://code.jquery.com/jquery-1.10.1.min.js"></script>
    <script src="http://code.jquery.com/jquery-migrate-1.2.1.min.js"></script>
    <script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js"></script>
    <script src="http://malsup.github.io/jquery.blockUI.js"></script>
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />

        <%-- Stylesheets --%>
    <link href="Scripts/StyleSheets/Default.css" rel="stylesheet" />
    <link href="Scripts/StyleSheets/Panel.css" rel="stylesheet" />
    <link href="Scripts/StyleSheets/jquery.contextMenu.css" rel="stylesheet" />
    <link href="Scripts/StyleSheets/ProspectingDefault.css" rel="stylesheet" />
    <link href="Scripts/StyleSheets/ContentExpanderWidgetStyles.css" rel="stylesheet" />
    <link href="Scripts/StyleSheets/ContactDetailsEditorWidgetStyles.css" rel="stylesheet" />
    <link href="Scripts/3rdParty/tooltipster.css" rel="stylesheet" />
    <link href="Scripts/3rdParty/tooltip.css" rel="stylesheet" />
    <link href="Scripts/3rdParty/OpenTip.css" rel="stylesheet" />

    <%-- Google maps API access (note server key) --%>
    <script src="http://maps.googleapis.com/maps/api/js?key=AIzaSyDWHlk3fmGm0oDsqVaoBM3_YocW5xPKtwA&libraries=drawing,geometry,places&sensor=true"></script>

    <%-- 3rd party --%>
    <script src="Scripts/3rdParty/MarkerClustererPlus.js"></script>
    <script src="Scripts/3rdParty/OverlappingMarkerSpiderfier.js"></script>
    <script src="Scripts/3rdParty/jquery.contextMenu.js"></script>
    <script src="Scripts/3rdParty/jquery.ui.position.js"></script>
    <script src="Scripts/3rdParty/jquery.tooltipster.min.js"></script>
    <script src="Scripts/3rdParty/tooltip.js"></script>
    <script src="Scripts/3rdParty/OpenTip.js"></script>


    <%-- Core code --%>
    <script src="Scripts/Core/Default.js"></script>
    <script src="Scripts/Core/MenuBuilder.js"></script>
    <script src="Scripts/Core/ObjectBuilder.js"></script>
    <script src="Scripts/Core/Utils.js"></script>
    <script src="Scripts/Core/Search.js"></script>
    <script src="Scripts/Core/Filtering.js"></script>
    <script src="Scripts/Core/StatisticsGenerator.js"></script>
    <script src="Scripts/Core/ProspectingDefault.js"></script>
    <script src="Scripts/Core/ProspectingMenuBuilder.js"></script>
    <script src="Scripts/Core/ContentExpanderWidget.js"></script>
    <script src="Scripts/Core/ProspectingContactMenuBuilder.js"></script>
    <script src="Scripts/Core/ContactDetailsEditorWidget.js"></script>
    <script src="Scripts/Core/ProspectingObjectBuilder.js"></script>

    <script>

        var availableCredit = 0;

        var suburbsInfo, allAgencies;
        var map, menu, infowindow;
        var menuItems = [];
        var slidePanelOpen = true;

        // Stores *all* domain-level information required by various features. This object maps to the C# type 'ProspectingContext'. 
        var prospectingContext = null;

        // Entry point from google maps
        google.maps.event.addDomListener(window, 'load', initialize);

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

                        $('#mainpanel').css('display', 'block');

                        availableCredit = data.AvailableCredit;
                        initializeMenuHtml();
                        loadSuburbsInfo(data);
                        initializeMap();
           
                        createProspectingMenu();
                        initEventHandlers();

                        prospectingContext = JSON.parse(data.StaticProspectingData);
                    }
                },
                error: function (textStatus, errorThrown) { debugger;},
                dataType: "json"
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

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id='mainpanel' class="ui-widget-content" style="min-width: 45%; display: none;">
            <div id="panelheader">
                <label style="font-family: Verdana; font-size: 25px; padding: 10px; margin-top: 10px; display: inline-block; color: #000070">Seeff Prospecting</label>
                <img src="Assets/seeff_logo.png" style="float: left;" />
                <img id="closepanelbutton" src="Assets/double-arrow-left.png" style="float: right; padding-right: 5px; cursor: pointer;" />
                <%--<img id='snappanelbutton' src="Assets/snap.png" style="float:right;cursor:pointer;" />--%><br />
                <label style="font-family: Verdana; font-size: 12px; padding: 20px; color: #000070"><i>Works best in Google Chrome&#0153;</i></label>
            </div>
            <div id="menuitempanel" class="ui-widget-content">
                <!-- Create menu items -->
            </div>
            <div id="contentarea" class="scrollable">
                <!-- Create menu item content here -->
            </div>
        </div>
        <div id="openpanelbutton" style="display: none;">
            <img id="closepanelbutton2" src="Assets/double-arrow-right.png" style="float: left;" />
        </div>
        <div id="loadingDiv" style="display:inline-block;text-align:center;margin-left:45%;margin-top:25%;">
            <img src="Assets/loading.gif" />
        </div>        
        <div class="context-menu-map box menu-1"></div>
        <div class="context-menu-prospect box menu-1"></div>
        <div id="itemSavedDialogSplash" style="display: none;">
            <span id="itemSavedSplashText">Changes saved!</span>
        </div>
        <div id="searchResultsDialog" style="display:none;">
            <div id="searchResultsContent"></div>
        </div>
        <div id="existingContactFoundDialog" style="display:none;font-family:Verdana;font-size:12px;">
            <div id="existingContactContent"></div>
        </div>
        <div id="errorDialog" title="Error Message" class="errorDialog"> 
            <label id="errorDialogText"></label>
        </div>
    </form>
        <div id="googleMap"></div>
</body>
</html>
