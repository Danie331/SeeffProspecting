<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Seeff.Spatial.WebApp.UserInterface.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Seeff Area Mapping Toolkit</title>
    <style>
          .loadingGif {
            position: absolute;
            top: 50%;
            left: 48%;
            margin-top: -50px;
            margin-left: -50px;
            width: 100px;
            height: 100px;
        }

           .mainpanel {
            /*position: absolute;*/
            height: 97%;
            width: 45%;
            background: white;
            float: left;
            z-index: 999;
            border-radius: 15px;
            border-right: 1px solid grey;
            overflow: hidden;
        }

        .panelcontentcontainer {
            font-family: Verdana;
            font-size: 12px;
            margin: 10px 10px;
        }

            .panelcontentcontainer > br {
                line-height: 10px;
                content: " ";
            }

        html, body {
            height: 100%;
            margin: 0;
            padding: 0;
        }

        #map {
            height: 100%;
            position: absolute;
            height: 100%;
            width: 100%;
            float: right;
        }

    </style>

         <!-- Jquery -->
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css" />
    <script src="https://code.jquery.com/jquery-2.1.4.min.js"></script>
    <script src="https://code.jquery.com/ui/1.11.4/jquery-ui.min.js"></script>

    <script src="http://maps.googleapis.com/maps/api/js?v=3&key=AIzaSyDWHlk3fmGm0oDsqVaoBM3_YocW5xPKtwA&libraries=drawing,geometry,places&sensor=true"></script>

    <!-- Core functionality -->
    <script src="../Javascript/Core/Google.js"></script>
    <script src="../Javascript/Core/Panel.js"></script>
    <script src="../Javascript/Core/Utilities.js"></script>
    <script src="../Javascript/Core/ApplicationInit.js"></script>
    <script src="../Javascript/Core/StateManager.js"></script>
    <script src="../Javascript/Core/NavigationItemContent/NavItemAreaInformation.js"></script>
    <script src="../Javascript/Core/NavigationItemContent/NavItemCreateNewArea.js"></script>
    <script src="../Javascript/Core/NavigationItemContent/NavItemSuburbSelection.js"></script>
    <script src="../Javascript/Core/NavigationItemContent/NavItemEditPoly.js"></script>

    <!-- Bootstrap -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap-theme.min.css" />
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>

    <script type="text/javascript">
        $(function () {
            $.ajax({
                url: "/api/Home/Login",
                type: 'POST',
                contentType: "application/json"
            }).done(function (userResult) {
                if (userResult.LoginSuccess) {
                    //google.maps.event.addDomListener(window, 'load', application.init);
                    application.init(userResult);
                    $('#loadingDiv').remove();
                    $('#map').css('display', 'block');
                    $('#panelContainer').css('display', 'block');
                }
                else {
                    alert("An error occurred logging in. Please contact support. Error message: " + userResult.LoginMessage);
                }
            });
        });
    </script>
</head>
<body>
    <div id="loadingDiv" class="loadingGif">
        <img src="../Graphics/loading.gif" />
    </div> 
    <div id="map" style="display:none"></div>
    <div id="panelContainer" class="mainpanel ui-widget-content" style="display:none" />
</body>
</html>
