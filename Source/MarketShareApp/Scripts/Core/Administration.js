
var msApp = msApp || {};

msApp.administration =  {
    buildAdminMenuHtml: function () {
        var container = $("<div style='padding:15px' />");
        var autoFateAllBtn = $("<input type='button' value='Auto-fate All Unfated Transactions' />");
        container.append(autoFateAllBtn);

        autoFateAllBtn.click(function () {
            var msg = "Use this option to automatically assign a 'RESIDENTIAL' fating to all unfated transactions under your license. \n\Please note that this operation is irreversible.\n\n\Would you like to proceed?";
            var result = confirm(msg);
            if (result) {
                $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Working. Please wait...</p>' });
                $.ajax({
                    type: "POST",
                    url: "RequestHandler.ashx",
                    data: JSON.stringify({ Instruction: "auto_fate_all" })
                }).done(function (successFlag) {
                    $.unblockUI();
                    if (successFlag == "true") {
                        showSavedSplashDialog("Auto-fating completed successfully");
                        $.each(suburbsInfo, function (idx, suburb) {
                            if (suburb.IsInitialised) {
                                $.each(suburb.Listings, function (idx2, tran) {
                                    if (!tran.Fated) {
                                        tran.Fated = true;
                                        tran.MarketShareType = "R";

                                        if (tran.Marker) {
                                            if (tran.SS_FH == "SS") {
                                                var iconPath = getIconForMarker(tran.Marker);
                                                var allMarkersForSS = getAllMarkersThatWillSpiderfy(tran);
                                                $.each(allMarkersForSS, function (idx, el) {
                                                    el.setIcon(iconPath);
                                                });
                                            } else {
                                                tran.Marker.setIcon(getIconForMarker(tran.Marker));
                                            }
                                        }
                                    }
                                });
                                updateSuburbStats(suburb);
                                suburbsInfo.CanFilter = true;
                                showFilteringOptions(true);
                                generateStatisticsMenu(true);
                            } else {
                                // update of stats must still occur
                                updateSuburbStats(suburb);
                            }
                        });
                    } else {
                        alert("An error occurred processing your request. Please notify support for assistance.");
                    }
                });
            }
        });

        return container;
    }
}

function showSavedSplashDialog(text) {
    var container = $("<div class='dialog' style='margin-right:0;font-family:verdana;font-size:12px'/>");
    if (text) {
        container.text(text);
    }

    container.dialog({
        show: 'fade',
        width: '350',
        position: ['center', 'right'],
        hide: { effect: "fadeOut", duration: 2000 },
        buttons: { "OK": function () { container.dialog("close"); } },
        open: function (event, ui) {
            $('.ui-dialog').css('z-index', 10000);
            container.siblings(".ui-dialog-titlebar").hide();
            //setTimeout(function () {
            //    container.dialog('close');
            //}, 2000);
        }
    });
}