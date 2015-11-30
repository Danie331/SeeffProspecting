
$(function () {
    $.extend(application, {
        panel: {
            initPanel: function () {
                var panelContainer = $("#panelContainer");
                panelContainer.load("../HTML/Panel.html", function (state) {
                    panelContainer.resizable();
                    panelContainer.draggable({ containment: 'parent' });
                    $("#panelContentContainer").css('height', '100%');

                    // Build content for the first menu item
                    application.panel.navItemAreaSelection.buildContent(true);
                    application.stateManager.activeNavItem = application.panel.navItemAreaSelection;
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
                    suburbSelectionClick: function () {
                        application.panel.navMenu.updateNavItemButtonOnClick();
                        application.stateManager.handleExitEditPolyMode();
                        application.stateManager.handleExitCreateAreaMode();
                        application.panel.navItemAreaSelection.buildContent(true); //clicking to a new poly must also exit edit mode., delete polypoint, undo create new line when creating new poly.
                        application.stateManager.activeNavItem = application.panel.navItemAreaSelection;
                    },
                    createAreaClick: function () {
                        application.panel.navMenu.updateNavItemButtonOnClick();
                        application.stateManager.handleExitEditPolyMode();
                        application.stateManager.handleEnterCreateAreaMode(); // area layering
                        application.panel.navItemCreateNewArea.buildContent();
                        application.stateManager.activeNavItem = application.panel.navItemCreateNewArea;
                    },
                    areaInformationClick: function () {
                        application.panel.navMenu.updateNavItemButtonOnClick();
                        application.stateManager.handleExitEditPolyMode();
                        application.stateManager.handleExitCreateAreaMode();
                        application.panel.navItemAreaInformation.buildContent();
                        application.stateManager.activeNavItem = application.panel.navItemAreaInformation;
                    },
                    enterPolyEditModeClick: function () {
                        if (application.stateManager.activeSuburb || application.stateManager.activeLicense) {
                            application.panel.navMenu.updateNavItemButtonOnClick();
                            application.panel.navItemEditPoly.buildContent();
                            application.stateManager.handleExitCreateAreaMode();
                            application.stateManager.handleEnterPolyEditMode();
                            application.stateManager.activeNavItem = application.panel.navItemEditPoly;
                        }
                    }
                }
            }
        }
    });
});