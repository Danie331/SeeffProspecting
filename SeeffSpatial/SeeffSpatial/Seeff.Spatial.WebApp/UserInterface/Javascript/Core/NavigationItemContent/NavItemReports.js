$(function () {
    $.extend(application.panel, {
        navItemReports: {
            buildContent: function () {
                var container = $("#contentContainerContent");
                if (application.panel.navItemReports.contentCache) {
                    container.html(application.panel.navItemReports.contentCache);
                    application.panel.navItemReports.refresh();
                } else {
                    container.html('').load("UserInterface/HTML/Reporting.html", function (content) {
                        application.panel.navItemReports.contentCache = content;
                        container.html(application.panel.navItemReports.contentCache);
                        application.panel.navItemReports.refresh();
                    });
                }
            },
            refresh: function () {
                var datetime = new Date().toLocaleString();
                $("#reportLastUpdatedAt").text(datetime);

                application.services.serviceControllers.getSuburbsUnderMaintenance(function (result) {
                    if (result.Successful) {
                        var contentContainer = $("#suburbsUnderMaintenanceContainer");
                        contentContainer.empty();

                        var suburbsUnderMaint = result.Suburbs;
                        if (!suburbsUnderMaint.length) {
                            contentContainer.append('- No suburbs under maintenance -');
                        } else {
                            var ul = $('<ul />');
                            $.each(suburbsUnderMaint, function (idx, sub) {
                                var itemValue = "<li>" + sub.AreaName + ' (' + sub.SeeffAreaID + ')' + "</li>";
                                ul.append(itemValue);
                            });
                            contentContainer.append(ul);
                        }
                    } else {
                        alert(result.Message);
                    }
                });

                // Update recent file list..
                var recentFilesContainer = $("#recentUploadsContainer");
                recentFilesContainer.empty();
                if (!application.stateManager.fileUploadCounter.length) {
                    recentFilesContainer.append('- No files uploaded during this session -');
                } else {
                    application.stateManager.fileUploadCounter.sort(function (a, b) {
                        return b.When - a.When;
                    });

                    var ul = $('<ul />');
                    $.each(application.stateManager.fileUploadCounter, function (idx, file) {
                        var itemValue = "<li>" + file.FileName + ' uploaded at ' + file.When.toLocaleString() + "</li>";
                        ul.append(itemValue);
                    });
                    recentFilesContainer.append(ul);
                }
            }
        }
    });
})