
$(function () {
    $.extend(application.panel, {
        navItemSuburbSelection: {
            buildContent: function () {
                var container = $("#contentContainerContent");
                container.html('');
                if (application.panel.navItemSuburbSelection.contentCache) {
                    var html = application.panel.navItemSuburbSelection.contentCache.html();
                    container.html(html);
                } else {
                    application.panel.navItemSuburbSelection.contentCache = $("<div />");
                    var userSuburbs = application.user.SeeffAreaCollection;
                    $.each(userSuburbs, function (idx, suburb) {
                        var radioItem = application.panel.navItemSuburbSelection.constructSuburbRadioItem(suburb);
                        application.panel.navItemSuburbSelection.contentCache.append(radioItem);

                        application.panel.navItemSuburbSelection.contentCache.append("<br />");
                    });
                    var html = application.panel.navItemSuburbSelection.contentCache.html();
                    container.html(html);
                }
                if (application.stateManager.activeSuburb) {
                    application.panel.navItemSuburbSelection.selectSuburbRadioLink(application.stateManager.activeSuburb);

                    application.Google.resetPolygonSelection();
                    application.Google.showSuburbInfoWindow(application.stateManager.activeSuburb);
                    application.stateManager.activeSuburb.PolygonInstance.selected = true;
                    application.stateManager.activeSuburb.PolygonInstance.setOptions({ fillOpacity: 0.5 });
                }
            },
            rebuildContentCache: function() {
                application.panel.navItemSuburbSelection.contentCache = null;
                application.panel.navItemSuburbSelection.contentCache = $("<div />");
                var userSuburbs = application.user.SeeffAreaCollection;
                $.each(userSuburbs, function (idx, suburb) {
                    var radioItem = application.panel.navItemSuburbSelection.constructSuburbRadioItem(suburb);
                    application.panel.navItemSuburbSelection.contentCache.append(radioItem);

                    application.panel.navItemSuburbSelection.contentCache.append("<br />");
                });
            },
            constructSuburbRadioItem: function (suburb) {
                var radioItem = $("<input type='radio' name='suburbSelection' class='suburbLinkItem'><a href='javascript:void(0);'>" + suburb.AreaName + "</a></input>");
                radioItem.attr('id', 'suburb' + suburb.SeeffAreaID);
                radioItem.attr('onclick', 'application.panel.navItemSuburbSelection.handleSuburbLinkItemClick(event)');
                suburb.radioItemLink = radioItem;

                return radioItem;
            },
            selectSuburbRadioLink: function (suburb) {
                var suburbLinkID = 'suburb' + suburb.SeeffAreaID;
                var target = $('#' + suburbLinkID);
                target.prop("checked", true);
                var parentDiv = $('#contentContainerContent');
                parentDiv.scrollTop(parentDiv.scrollTop() + target.position().top - parentDiv.height() / 2 + target.height() / 2);
            },
            selectSuburbFromPolyClick: function (suburb) {
                application.stateManager.setActiveSuburb(suburb);
                if (application.stateManager.activeNavItem == application.panel.navItemSuburbSelection) {
                    application.panel.navItemSuburbSelection.selectSuburbRadioLink(suburb);
                }
                if (application.stateManager.activeNavItem == application.panel.navItemEditPoly) {
                    application.panel.navItemEditPoly.buildContent();
                    application.stateManager.handleEnterPolyEditMode();
                }
            },
            handleSuburbLinkItemClick: function (e) {
                e.stopPropagation();
                var id = $(e.target).attr('id');
                $('#' + id).prop('checked', true);

                var targetSuburbID = id.replace('suburb', '');
                var targetSuburb = application.user.SeeffAreaCollectionLookup[targetSuburbID];

                application.stateManager.handleExitEditPolyMode();
                application.Google.resetPolygonSelection();
                application.Google.showSuburbInfoWindow(targetSuburb);
                targetSuburb.PolygonInstance.selected = true;
                targetSuburb.PolygonInstance.setOptions({ fillOpacity: 0.5 });

                application.Google.centreMapAndZoom(targetSuburb.CentroidInstance, 10);

                application.stateManager.setActiveSuburb(targetSuburb);
            }
        }
    })
});