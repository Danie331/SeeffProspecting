
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

                var mcOptions = { maxZoom: 15, minimumClusterSize: 1 };
                var markerClusterer = new MarkerClusterer(application.Google.map, [], mcOptions);

                plotpointBtn.click(function () {
                    var lat = Number($("#latInput").val());
                    var lng = Number($("#lngInput").val());

                    var infoWindow = new google.maps.InfoWindow();
                    infoWindow.setContent("lat: " + lat + "<br />" + "lng: " + lng);
                    infoWindow.setPosition(new google.maps.LatLng(lat, lng));
                    infoWindow.open(application.Google.map);
                });

                //plotOrphanMSRecords.click(function () {
                //    markerClusterer.clearMarkers();
                //    application.services.serviceControllers.retrieveOrphanRecords("marketshare", function (result) {
                //        if (result.Successful) {
                //            msOrphansProgress.css("display", "inline-block");
                //            $("#msOrphansCount").text(result.Orphans.length);
                //            application.utilities.yieldingLoop(result.Orphans.length, 1, function (i) {
                //                var orphan = result.Orphans[i];
                //                var pos = {lat : orphan.LatLng.Lat, lng: orphan.LatLng.Lng};
                //                var marker = new google.maps.Marker({
                //                    position: pos,
                //                    //map: application.Google.map,
                //                    title: orphan.LightstonePropertyID
                //                });
                //                //marker.setMap(application.Google.map);
                //                markerClusterer.addMarker(marker);
                //                $("#msOrphansProgress").text(i);
                //            }, function () { });                            
                //        } else {
                //            alert(result.Message);
                //        }
                //    });
                //});

                //plotOrphanProspectingRecords.click(function () {
                //    markerClusterer.clearMarkers();
                //    application.services.serviceControllers.retrieveOrphanRecords("prospecting", function (result) {
                //        if (result.Successful) {
                //            prospectingOrphansProgress.css("display", "inline-block");
                //            $("#prospectingOrphansCount").text(result.Orphans.length);
                //            application.utilities.yieldingLoop(result.Orphans.length, 1, function (i) {
                //                var orphan = result.Orphans[i];
                //                var pos = { lat: orphan.LatLng.Lat, lng: orphan.LatLng.Lng };
                //                var marker = new google.maps.Marker({
                //                    position: pos,
                //                    //map: application.Google.map,
                //                    title: orphan.LightstonePropertyID
                //                });
                //                //marker.setMap(application.Google.map);
                //                markerClusterer.addMarker(marker);
                //                $("#prospectingOrphansProgress").text(i);
                //            }, function () { });
                //        } else {
                //            alert(result.Message);
                //        }
                //    });
                //});

                //showSuburbsUnderLicenseBtn.click(function () {
                //    var license = application.stateManager.activeLicense;
                //    if (license) {
                //        if (!application.user.SeeffAreaCollection.length) {
                //            application.panel.navItemAreaSelection.getSuburbs(function () {
                //                // Reset suburb selection
                //                application.stateManager.allSuburbsShown = false;
                //                $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                //                    application.Google.createSuburbPoly(suburb, { render: false });
                //                });

                //                $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                //                    if (license.LicenseID == suburb.LicenseID) {
                //                        application.Google.createSuburbPoly(suburb, { render: true });
                //                    }
                //                });
                //            });
                //        } else {
                //            // Reset suburb selection
                //            application.stateManager.allSuburbsShown = false;
                //            $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                //                application.Google.createSuburbPoly(suburb, { render: false });
                //            });

                //            $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                //                if (license.LicenseID == suburb.LicenseID) {
                //                    application.Google.createSuburbPoly(suburb, { render: true });
                //                }
                //            });
                //        }
                //    }
                //    else {
                //        alert("No license selected.");
                //    }
                //});
            }
        }
    });
})