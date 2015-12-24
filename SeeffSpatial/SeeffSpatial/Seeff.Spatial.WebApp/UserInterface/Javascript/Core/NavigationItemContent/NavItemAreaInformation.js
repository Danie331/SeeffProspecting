
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

                var showSuburbsUnderLicenseBtn = $("<input type='button' value='Show Suburbs For License' />");

                container.append(pointPlotterLbl)
                         .append("<br />")
                         .append(latInput)
                .append("<br />")
                .append(lngInput)
                    .append("<br />")
                .append(plotpointBtn)
                .append("<p />")
                .append(showSuburbsUnderLicenseBtn);

                plotpointBtn.click(function () {
                    var lat = Number($("#latInput").val());
                    var lng = Number($("#lngInput").val());

                    var infoWindow = new google.maps.InfoWindow();
                    infoWindow.setContent("lat: " + lat + "<br />" + "lng: " + lng);
                    infoWindow.setPosition(new google.maps.LatLng(lat, lng));
                    infoWindow.open(application.Google.map);
                });

                showSuburbsUnderLicenseBtn.click(function () {
                    var license = application.stateManager.activeLicense;
                    if (license) {
                        if (!application.user.SeeffAreaCollection.length) {
                            application.panel.navItemAreaSelection.getSuburbs(function () {
                                // Reset suburb selection
                                application.stateManager.allSuburbsShown = false;
                                $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                                    application.Google.createSuburbPoly(suburb, { render: false });
                                });

                                $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                                    if (license.LicenseID == suburb.LicenseID) {
                                        application.Google.createSuburbPoly(suburb, { render: true });
                                    }
                                });
                            });
                        } else {
                            // Reset suburb selection
                            application.stateManager.allSuburbsShown = false;
                            $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                                application.Google.createSuburbPoly(suburb, { render: false });
                            });

                            $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                                if (license.LicenseID == suburb.LicenseID) {
                                    application.Google.createSuburbPoly(suburb, { render: true });
                                }
                            });
                        }
                    }
                    else {
                        alert("No license selected.");
                    }
                });
            }
        }
    })
});