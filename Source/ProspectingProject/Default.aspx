<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prospecting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
     <title>Seeff Prospecting</title>

    <%-- Jquery --%>
  <script src="http://code.jquery.com/jquery-1.10.1.min.js"></script>
    <script src="http://code.jquery.com/jquery-migrate-1.2.1.min.js"></script>
    <script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js"></script>
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />

        <%-- Google maps API access (note server key) --%>
    <script src="http://maps.googleapis.com/maps/api/js?key=AIzaSyBzPnnvalo0QCtGgQn17e5nbRS8yyDpcuM&libraries=drawing,geometry,places&sensor=true&v=3.30"></script>
    <script src="https://apis.google.com/js/client.js"></script>

      <%-- UNMINIFIED --%> 
    <script src="Scripts/3rdParty/ckeditor/ckeditor.js"></script>
    <script src="Scripts/3rdParty/ckeditor/adapters/jquery.js"></script>
    <script src="Scripts/Core/Extensions.js"></script>
    <script src="Scripts/3rdParty/tooltip.js"></script>

    <%-- BEGIN_SECTION:STYLESHEETS --%> 
     <%-- BEGIN_SECTION:3RD_PARTY_STYLESHEETS --%>
     <link href="Scripts/StyleSheets/jquery.contextMenu.css" rel="stylesheet" />
     <link href="Scripts/StyleSheets/tooltip.css" rel="stylesheet" />
     <link href="Scripts/StyleSheets/OpenTip.css" rel="stylesheet" />
    <link href="Scripts/3rdParty/SlickGrid/slick.grid.css" rel="stylesheet" />
     <%-- END_SECTION:3RD_PARTY_STYLESHEETS --%>
     <%-- BEGIN_SECTION:CORE_STYLESHEETS --%>
     <link href="Scripts/StyleSheets/Default.css" rel="stylesheet" />
     <link href="Scripts/StyleSheets/Panel.css" rel="stylesheet" />   
     <link href="Scripts/StyleSheets/ProspectingDefault.css" rel="stylesheet" />
     <link href="Scripts/StyleSheets/ContentExpanderWidgetStyles.css" rel="stylesheet" />
     <link href="Scripts/StyleSheets/ContactDetailsEditorWidgetStyles.css" rel="stylesheet" />
    <link href="Scripts/StyleSheets/ComboSelect.css" rel="stylesheet" />    
     <%-- END_SECTION:CORE_STYLESHEETS --%>     
    <%-- END_SECTION:STYLESHEETS --%>

    <%-- BEGIN_SECTION:JAVASCRIPT --%>
    <%-- BEGIN_SECTION:3RD_PARTY_SCRIPTS --%>
    <script src="Scripts/3rdParty/MarkerClustererPlus.js"></script>
    <script src="Scripts/3rdParty/OverlappingMarkerSpiderfier.js"></script>
    <script src="Scripts/3rdParty/jquery.contextMenu.js"></script>
    <script src="Scripts/3rdParty/jquery.ui.position.js"></script>
    <script src="Scripts/3rdParty/OpenTip.js"></script>
    <script src="Scripts/3rdParty/waitForImage.js"></script>
    <script src="Scripts/3rdParty/jqueryBlockUI.js"></script>
    <script src="Scripts/3rdParty/SlickGrid/lib/jquery.event.drag-2.2.js"></script>
    <script src="Scripts/3rdParty/SlickGrid/lib/jquery.event.drop-2.2.js"></script>
    <script src="Scripts/3rdParty/SlickGrid/slick.core.js"></script>
    <script src="Scripts/3rdParty/SlickGrid/slick.grid.js"></script>
    <script src="Scripts/3rdParty/SlickGrid/slick.dataview.js"></script>
    <script src="Scripts/3rdParty/SlickGrid/plugins/slick.rowselectionmodel.js"></script>
    <script src="Scripts/3rdParty/SlickGrid/plugins/slick.rowmovemanager.js"></script>
    <script src="Scripts/3rdParty/SlickGrid/slick.editors.js"></script>
    <script src="Scripts/3rdParty/SlickGrid/plugins/slick.autotooltips.js"></script>
    <script src="Scripts/3rdParty/Parsely/parsley.min.js"></script>
    <%-- END_SECTION:3RD_PARTY_SCRIPTS --%>
    <%-- BEGIN_SECTION:CORE_SCRIPTS --%>
    <script src="Scripts/Core/Utils.js"></script>
    <script src="Scripts/Core/ProspectingDefault.js"></script>
    <script src="Scripts/Core/ProspectingMenuBuilder.js"></script>
    <script src="Scripts/Core/ContentExpanderWidget.js"></script>
    <script src="Scripts/Core/ProspectingContactMenuBuilder.js"></script>
    <script src="Scripts/Core/ContactDetailsEditorWidget.js"></script>
    <script src="Scripts/Core/ProspectingObjectBuilder.js"></script>
    <script src="Scripts/Core/CommunicationTemplates.js"></script>
    <script src="Scripts/Core/Communication.js"></script>
    <script src="Scripts/Core/PropertyInformation.js"></script>
    <script src="Scripts/Core/Filtering.js"></script>
    <script src="Scripts/Core/DocumentVault.js"></script>
    <script src="Scripts/Core/CommReports.js"></script>
    <script src="Scripts/Core/DrawingMultiSelect.js"></script>
    <script src="Scripts/Core/AppStart.js"></script>
    <script src="Scripts/Core/ComboSelect.js"></script>
    <script src="Scripts/Core/ContactListsManager.js"></script>
    <script src="Scripts/Core/ListingsManager/Content/Common.js"></script>
    <script src="Scripts/Core/ListingsManager/Content/Commercial.js"></script>
    <script src="Scripts/Core/ListingsManager/Content/Developments.js"></script>
    <script src="Scripts/Core/ListingsManager/Content/Holiday.js"></script>
    <script src="Scripts/Core/ListingsManager/Content/Residential.js"></script>
    <script src="Scripts/Core/ListingsManager/Handler.js"></script>
    <script src="Scripts/Core/ListingsManager/Models/NewListingModelBase.js"></script>
    <script src="Scripts/Core/ListingsManager/Models/NewResidentialListingModel.js"></script>
    <script src="Scripts/Core/ListingsManager/Models/NewCommercialListingModel.js"></script>
    <script src="Scripts/Core/ListingsManager/Models/NewDevelopmentsListingModel.js"></script>
    <script src="Scripts/Core/ListingsManager/ListingModel.js"></script>
     <%-- END_SECTION:CORE_SCRIPTS --%>
    <%-- END_SECTION:JAVASCRIPT --%>

    <script>

        // Entry point from google maps
        google.maps.event.addDomListener(window, 'load', initialize);

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id='mainpanel' class="ui-widget-content" style="min-width: 45%; display: none;">
            <div id="panelheader">
                <label id="PageHeaderLabel" style="font-family: Verdana; font-size: 25px; padding: 10px; margin-top: 10px; display: inline-block; color: #000070" runat="server">Seeff Prospecting</label>
                <img src="Assets/seeff_logo.png" style="float: left;" />
                <img id="closepanelbutton" src="Assets/double-arrow-left.png" style="float: right; padding-right: 5px; padding-left:5px; cursor: pointer;" title="Collapse" />
                <button id="logoffBtn" type="button" style="display: inline-block; float: right; margin-top:2px; background-color:white;cursor:pointer;border-radius:15px" title="Back to BOSS">
                    <span style="vertical-align:middle; padding-right:2px; font-style:italic; font-weight:100">Log off</span>
                </button>
                <%--<img id='snappanelbutton' src="Assets/snap.png" style="float:right;cursor:pointer;" />--%><br />
                <label style="font-family: Verdana; font-size: 12px; padding: 20px; color: #000070"><i>Works best in Google Chrome</i></label>
            </div>
            <div id="menuitempanel" class="ui-widget-content">
                <!-- Create menu items -->
            </div>
            <div id="contentarea" class="scrollable">
                <!-- Create menu item content here -->
            </div>
        </div>
        <div id="openpanelbutton" style="display: none;">
            <img id="closepanelbutton2" src="Assets/double-arrow-right.png" style="float: left;" title="Expand" />
        </div>
        <div id="loadingDiv">
            <img src="Assets/loading.gif" />
        </div>        
        <div class="context-menu-map box menu-1"></div>
        <div class="context-menu-prospect box menu-1"></div>
        <div class="context-menu-rightclick-property box menu-1"></div>
        <div id="itemSavedDialogSplash" style="display: none;">
            <span id="itemSavedSplashText">Changes saved!</span>
        </div>
        <div id="searchResultsDialog" style="display:none;">
            <div id="searchResultsContent"></div>
        </div>
        <div id="existingContactFoundDialog" style="display:none;font-family:Verdana;font-size:12px;">
            <div id="existingContactContent"></div>
        </div>
        <div id="userIsCompromisedDialog" title="System warning" style="display:none;font-family:Verdana;font-size:12px;">
            <div id="userIsCompromisedContent"></div>
        </div>
        <div id="addActivityDialog" title="Add Activity To Property" style="display:none;font-family:Verdana;font-size:12px;"></div>
        <div id="errorDialog" title="Error Message" class="errorDialog"> 
            <label id="errorDialogText"></label>
        </div>
        <div id="selectPrimaryContactDetailDialog" title="Select Primary Contact Detail" style="display:none;font-family:Verdana;font-size:12px;"></div>
        <div id="multiSelectMode" style="display: none; font-family: Verdana; font-size: xx-large; color: red">
            You are in communications mode
        </div>
        <div id="filterMode" style="display: none; font-family: Verdana; font-size: xx-large; color: red">
            You are in filter mode&nbsp;&nbsp;&nbsp;&nbsp;
        </div>
        <div id="commSendMessageDialog" title="Preview Message" class="errorDialog"> 
        </div>
        <div id="referralsDialog" title="Create Referral" style="font-family: Verdana; font-size: 12px;"></div>
    </form>
        <div id="googleMap"></div>
</body>
</html>
