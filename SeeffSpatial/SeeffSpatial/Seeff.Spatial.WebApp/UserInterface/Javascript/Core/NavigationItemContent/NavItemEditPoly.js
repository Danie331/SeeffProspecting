
$(function () {
    $.extend(application.panel, {
        navItemEditPoly: {
            buildContent: function () {
                var container = $("#contentContainerContent");
                if (application.panel.navItemEditPoly.contentCache) {
                    container.html(application.panel.navItemEditPoly.contentCache);
                } else {
                    container.html('').load("../HTML/EditPoly.html", function (content) {
                        application.panel.navItemEditPoly.contentCache = content;
                        container.html(application.panel.navItemEditPoly.contentCache);
                    });
                }                
            }
        }
    });
});