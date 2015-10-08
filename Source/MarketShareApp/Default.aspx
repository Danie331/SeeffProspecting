<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Buffer="false" Inherits="MarketShare" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Market Share Indicator</title>

    <%-- Jquery --%>
    <script src="http://code.jquery.com/jquery-1.10.1.min.js"></script>
    <script src="http://code.jquery.com/jquery-migrate-1.2.1.min.js"></script>
    <script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js"></script>
    <script src="http://malsup.github.io/jquery.blockUI.js"></script>
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />

    <%-- Google maps API access (note server key) --%>
    <script src="http://maps.googleapis.com/maps/api/js?key=AIzaSyDWHlk3fmGm0oDsqVaoBM3_YocW5xPKtwA&libraries=drawing,geometry,places&sensor=true"></script>

    <%-- 3rd party libs --%>
    <script src="Scripts/3rdParty/MarkerClustererPlus.js"></script>
    <script src="Scripts/3rdParty/OverlappingMarkerSpiderfier.js"></script>
    <script src="Scripts/3rdParty/Purl.js"></script>
    <script src="Scripts/3rdParty/jquery.contextMenu.js"></script>
    <script src="Scripts/3rdParty/jquery.ui.position.js"></script>
    <script src="Scripts/3rdParty/jquery.blockUI.js"></script>

    <%-- Style sheets --%>
    <link href="Scripts/StyleSheets/Default.css" rel="stylesheet" />
    <link href="Scripts/StyleSheets/jquery.contextMenu.css" rel="stylesheet" />
    <link href="Scripts/StyleSheets/Panel.css" rel="stylesheet" />
    <link href="Scripts/StyleSheets/MultiSelectWidget.css" rel="stylesheet" />
    <link href="Scripts/StyleSheets/AdminInterface.css" rel="stylesheet" />
    <link href="Scripts/StyleSheets/ContentExpanderWidgetStyles.css" rel="stylesheet" />

    <%-- Core functionality / engine --%>
    <script src="Scripts/Core/Default.js"></script>
    <script src="Scripts/Core/MenuBuilder.js"></script>
    <script src="Scripts/Core/Admin/AdminGuiBuilder.js"></script>
    <script src="Scripts/Core/Admin/AdminCore.js"></script>
    <script src="Scripts/Core/Filtering.js"></script>
    <script src="Scripts/Core/ObjectBuilder.js"></script>
    <script src="Scripts/Core/StatisticsGenerator.js"></script>
    <script src="Scripts/Core/Utils.js"></script>
    <script src="Scripts/Core/MultiSelectWidget.js"></script>
    <script src="Scripts/Core/Search.js"></script>
    <script src="Scripts/Core/ContentExpanderWidget.js"></script>

    <script>

        //////////////////////////////////////////////////////////////////////////////////////////////
        // *******************************************************************************************
        // Global variables
        // *******************************************************************************************
        //////////////////////////////////////////////////////////////////////////////////////////////
        var initializationData = null;

        var suburbsInfo, allAgencies;
        var map, menu, infowindow;
        var rightClickedLocation = {};
        var currentActivePoly = null;
        var slidePanelOpen = true;
        var menuItems = [];
        var idleTime = 0;
        var timeoutElapsed;
        var currentMousePos = { x: -1, y: -1 };
        var drawingManager;

        // Entry point from google maps
        google.maps.event.addDomListener(window, 'load', initialize);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // ********************************************************************************************
        // SECTION Initialisation
        // ********************************************************************************************
        ////////////////////////////////////////////////////////////////////////////////////////////////
        function initialize() {
            $(function () {
                $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading. Please wait...</p>' });
                $.ajax({
                    type: "POST",
                    url: "ApplicationLoad.ashx",
                    dataType:"json"
                }).done(function (data) {
                    initializationData = data;

                    loadApplication();
                    adjustHtml();

                    // For timeout
                    setInterval(timerIncrement, 60000);
                    $(document).mousemove(function (e) {
                        // when the mouse is moved reset the idle time, ie the time that determines user inactivity
                        idleTime = 0;

                        // update the current position of the mouse
                        currentMousePos.x = e.clientX;
                        currentMousePos.y = e.clientY;
                    });
                    $.unblockUI();
                });
            });
        }

        function adjustHtml() {
            $('#legend').width($('#menuitempanel').width());
            $('#menuitempanel').resize(function () {
                $('#legend').width($(this).width());
            });            
        }

        $(function () {
            $('#legendContents').empty();
            var legendContents = buildContentExpanderItem('legendContents', 'Assets/legend.png', "Legend", buildLegend());
            var expanderWidget = new ContentExpanderWidget('#mainpanel', [legendContents], "legendExpander");

            function buildLegend() {
                return '<span class="legendCaptionTextStyle">Unfated entities</span>\
                    <div><img src="Assets/legend/FH_unfated.png" class="legendIconStyle" /><span class="legendTextStyle">Unfated property</span></div>\
                    <div><img src="Assets/legend/SS_unfated.PNG" class="legendIconStyle" /><span class="legendTextStyle">Complex with unfated units</span></div>\
                    <span class="legendCaptionTextStyle">Seeff deals</span>\
                    <div><img src="Assets/legend/seeff_FH_key.png" class="legendIconStyle" /><span class="legendTextStyle">Free-hold sold by Seeff</span></div>\
                    <div><img src="Assets/legend/seeff_SS_key.PNG" class="legendIconStyle" /><span class="legendTextStyle">Complex with only Seeff deals</span></div>\
                    <div><img src="Assets/legend/SS_seeff_and_non_seeff_sales.png" class="legendIconStyle" /><span class="legendTextStyle">Complex with some Seeff deals</span></div>\
                    <div><img src="Assets/legend/seeff_comm_key.png" class="legendIconStyle" /><span class="legendTextStyle">Commercial Seeff deal</span></div>\
                    <div><img src="Assets/legend/seeff_agri_key.png" class="legendIconStyle" /><span class="legendTextStyle">Agri Seeff deal</span></div>\
                    <span class="legendCaptionTextStyle">Fated entities</span>\
                    <div><img src="Assets/legend/FH_key.png" class="legendIconStyle" /><span class="legendTextStyle">Residential property</span></div>\
                    <div><img src="Assets/legend/SS_key.PNG" class="legendIconStyle" /><span class="legendTextStyle">Complex (fully fated)</span></div>\
                    <div><img src="Assets/legend/comm_key.png" class="legendIconStyle" /><span class="legendTextStyle">Commercial property</span></div>\
                    <div><img src="Assets/legend/agri_key.png" class="legendIconStyle" /><span class="legendTextStyle">Agri property</span></div>\
                    <div><img src="Assets/legend/dev_key.png" class="legendIconStyle" /><span class="legendTextStyle">Development</span></div>\
                    <div><img src="Assets/legend/FH_pending.png" class="legendIconStyle" /><span class="legendTextStyle">Pending further research</span></div>\
                    <div><img src="Assets/legend/other_key.png" class="legendIconStyle" /><span class="legendTextStyle">Other type of property</span></div>\
                    <span class="legendCaptionTextStyle">Seeff currently on market</span>\
                    <div><img src="Assets/legend/seeff_FH_buy.png" class="legendIconStyle" /><span class="legendTextStyle">Free-hold to buy</span></div>\
                    <div><img src="Assets/legend/seeff_FH_rent.png" class="legendIconStyle" /><span class="legendTextStyle">Free-hold to rent</span></div>\
                    <div><img src="Assets/legend/SS_to_buy.png" class="legendIconStyle" /><span class="legendTextStyle">Unit to buy</span></div>\
                    <div><img src="Assets/legend/SS_to_rent.png" class="legendIconStyle" /><span class="legendTextStyle">Unit to rent</span></div>\
                    <div><img src="Assets/legend/SS_to_rent_and_buy.png" class="legendIconStyle" /><span class="legendTextStyle">Units to rent and buy</span></div>\
                    <div><img src="Assets/legend/seeff_comm_buy.png" class="legendIconStyle" /><span class="legendTextStyle">Commercial property to buy</span></div>\
                    <div><img src="Assets/legend/seeff_comm_rent.png" class="legendIconStyle" /><span class="legendTextStyle">Commercial property to rent</span></div>\
                    <div><img src="Assets/legend/seeff_agri_buy.png" class="legendIconStyle" /><span class="legendTextStyle">Agri property to buy</span></div>\
                    <div><img src="Assets/legend/seeff_agri_rent.png" class="legendIconStyle" /><span class="legendTextStyle">Agri property to rent</span></div>';
            }

            var legendDiv = $('#legend');
            legendDiv.empty();
            legendDiv.append(expanderWidget.construct());
        });

    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div id='mainpanel' class="ui-widget-content" style="min-width: 45%;">
            <div id="panelheader">
                <label style="font-family: Verdana; font-size: 25px; padding: 10px; margin-top: 10px; display: inline-block; color: #000070">Seeff Market Share</label>
                <img src="Assets/seeff_logo.png" style="float: left;" />
                <img id="closepanelbutton" src="Assets/double-arrow-left.png" style="float: right; padding-right: 5px; cursor: pointer;" />
                <%--<img id='snappanelbutton' src="Assets/snap.png" style="float:right;cursor:pointer;" />--%><br />
                <label style="font-family: Verdana; font-size: 12px; padding: 20px; color: #000070"><i>Works best in Google Chrome&#0153;</i></label>
            </div>
            
                <div id="menuitempanel" style="width:240px;" class="ui-widget-content">
                    <!-- Create menu items -->
                </div>
                <div id="legend" style="position:absolute;bottom:5px;left:0px;"></div>
            
            <div id="contentarea" class="scrollable">
                <!-- Create menu item content here -->
            </div>
        </div>
        <div id="openpanelbutton" style="display: none;">
            <img id="closepanelbutton2" src="Assets/double-arrow-right.png" style="float: left;" />
        </div>
        
        <%--<div id="menu" style="width: 37%; top: 2%; right: 2%; position: absolute;"></div>--%>
        <div class="context-menu-search box menu-1"></div>
        <div class="context-menu-polyoptions box menu-1"></div>
        <div id="popupUnfatedAreas" class="dialog" title="Unfated transactions" style="display: none;">
            <p>
                One or more of your suburbs have unfated transactions.
                <br />
                Please fate all listings or request that an administrator do so.
            </p>
        </div>
        <div id="popupSuburbUnfated" class="dialog" title="Unfated transactions in suburb" style="display: none;">
            <p>This suburb has unfated transactions. Please fate all outstanding listings.</p>
        </div>
        <div id="popupAdminRequired" class="dialog" title="Admininstrator required" style="display: none;">
            <p>Please request that an administrator fate transactions for this licence before you can proceed.</p>
        </div>
        <div id="popupAllListingsFated" class="dialog" title="Fating complete" style="display: none;">
            <p>Thank you for fating all your listings!</p>
        </div>
        <div id="popupAllListingsFatedForSuburbButNotOthers" class="dialog" title="Suburb Fated" style="display: none;">
            <p>
                All listings have been fated for this suburb.<br />
                However, there are suburb(s) that have unfated listings.
            </p>
        </div>
        <div id="warningAgencyAlreadyOnListing" class="dialog" style="display: none;">
            <p>
                Warning: Cannot remove selected agency(ies).
                <br />
                One or more listings contain an agency you are trying to remove.
            </p>
        </div>
        <div id="agenciesSavedSplashScreen" class="dialog" style="display: none;">
            <p>Item(s) saved!</p>
        </div>

        <%--Admin menu builder content--%>
        <div id="areaSearchContainerDiv" class="dialog" title="Load an area" style="display: none;">
            <label for="availableAreas" style="font-family:Verdana;font-size:12px;">Areas: </label>
            <input id="availableAreas" size="40"/>
        </div>

        <div id="dialogPlotPointOnMap" class="dialog" title="Go to location" style="display: none;">
            <label>Lat: </label>
            <input type="text" id="latInput" value="" />
            <br />
            <label>Long: </label>
            <input type="text" id="lngInput" value="" />
            <br />
        </div>

        <div id="timeoutDialog" title="Session expiration" style="display: none;">
            <p>
                Your session is about to expire due to inactivity.
                <br />
                Continue using the application?
               <label id="timerLabel"></label>
            </p>
        </div>

        <div id="createOrEditAreaDialog" class="dialog" title="Add|Edit Area" style="display: none;">
            <p><label for="areaNameInput">Name of this area:</label><input type="text" id="areaNameInput" style="width: 200px; float: right; display: inline-block;" /><br /></p>
            <p><label for="areaParentSelector">Area parent:</label><select id="areaParentSelector" style="width: 200px; float: right; display: inline-block;"></select><br /></p>
            <p><label for="areaTypeSelector">Area type:</label><select id="areaTypeSelector" style="width: 200px; float: right; display: inline-block;"></select><br /></p>
            <p><input type="button" id="relatedAreasButton" value="Edit Related Areas" /></p>
            <p><input type="button" id="neighboringAreasButton" value="Edit Neighboring Areas" /></p>
            <p><label>Area Polygon Type:</label></p>
            <input type="checkbox" id="residentialCheckbox" name="residentialCheckbox" /><label for="residentialCheckbox">Residential</label><br />
            <input type="checkbox" id="commercialCheckbox" name="commercialCheckbox" /><label for="commercialCheckbox">Commercial</label><br />
            <input type="checkbox" id="agriCheckbox" name="agriCheckbox" /><label for="agriCheckbox">Agri</label>
        </div>

    </form>
    <div id="googleMap"></div>
</body>
</html>
