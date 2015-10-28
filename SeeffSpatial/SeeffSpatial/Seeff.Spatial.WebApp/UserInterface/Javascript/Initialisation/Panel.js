
$(function () {
    $.extend(application, {
        panel: {
            initPanel: function () {
                var panelContainer = $("#panelContainer");
                panelContainer.load("Panel.html", function () {
                    panelContainer.resizable();
                    panelContainer.draggable({ containment: 'parent' });
                    $("#panelContentContainer").css('height', '100%');
                });
            },
            dimensions: {
                width: 0,
                height: 0,
                recalculate: function () {
                    var panelContainer = $("#panelContainer");
                    this.width = panelContainer.css('width');
                    this.height = panelContainer.css('height');
                }
            },
            controllers: {
                collapsePanel: function () {
                    var panelContainer = $("#panelContainer");
                    var panelContentContainer = $("#panelContentContainer");

                    var expandedElements = panelContentContainer.find('.paneldisplayexpanded');
                    var expandBtn = panelContentContainer.find('.paneldisplaycollapsed');

                    expandedElements.removeClass('paneldisplayexpanded').addClass('paneldisplaycollapsed');
                    expandBtn.removeClass('paneldisplaycollapsed').addClass('paneldisplayexpanded');

                    application.panel.dimensions.recalculate();
                    panelContainer.css({ 'width': 100, 'height': 30 });
                    panelContainer.resizable('disable');
                },
                expandPanel: function () {
                    var panelContainer = $("#panelContainer");
                    var panelContentContainer = $("#panelContentContainer");

                    var collapsedElements = panelContentContainer.find('.paneldisplaycollapsed');
                    var expandBtn = panelContentContainer.find('.paneldisplayexpanded');

                    collapsedElements.removeClass('paneldisplaycollapsed').addClass('paneldisplayexpanded');
                    expandBtn.removeClass('paneldisplayexpanded').addClass('paneldisplaycollapsed');

                    var width = application.panel.dimensions.width;
                    var height = application.panel.dimensions.height;
                    panelContainer.css({ 'height': height, 'width': width });
                    panelContainer.resizable('enable');
                }
            },
            navMenu: {
                updateNavItemButtonOnClick: function () {
                    $('.navitem').removeClass('navitem-clicked');
                    var container = $(window.event.target);
                    container.addClass('navitem-clicked');
                },
                controllers: {
                    Item1Click: function () {
                        application.panel.navMenu.updateNavItemButtonOnClick();
                    },
                    Item2Click: function () {
                        application.panel.navMenu.updateNavItemButtonOnClick();
                    },
                    Item3Click: function () {
                        application.panel.navMenu.updateNavItemButtonOnClick();
                    }
                }
            }
        }
    });
});