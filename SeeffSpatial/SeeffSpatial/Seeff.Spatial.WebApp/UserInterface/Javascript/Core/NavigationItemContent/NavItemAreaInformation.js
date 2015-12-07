
$(function () {
    $.extend(application.panel, {
        navItemAreaInformation: {
            buildContent: function () {
                var container = $("#contentContainerContent");
                container.empty();

                var pointPlotterLbl = $("<label>Plot a point:</label>");
                var latInput = $("<label>Lat:<input type='text' id='latInput' /></label>");
                var lngInput = $("<label>Lng:<input type='text' id='lngInput' /></label>");
                var plotpointBtn = $("<input type='button' value='Plot' />");

                container.append(pointPlotterLbl)
                         .append("<br />")
                         .append(latInput)
                .append("<br />")
                .append(lngInput)
                    .append("<br />")
                .append(plotpointBtn);

                plotpointBtn.click(function () {
                    var lat = Number($("#latInput").val());
                    var lng = Number($("#lngInput").val());

                    var infoWindow = new google.maps.InfoWindow();
                    infoWindow.setContent("lat: " + lat + "<br />" + "lng: " + lng);
                    infoWindow.setPosition(new google.maps.LatLng(lat, lng));
                    infoWindow.open(application.Google.map);
                });
            }
        }
    })
});