
$(function () {
    $.extend(application.panel, {
        navItemAreaSelection: {
            buildContent: function (enableContent) {
                var container = $("#contentContainerContent");
                if (application.panel.navItemAreaSelection.contentCache) {
                    container.html(application.panel.navItemAreaSelection.contentCache);
                    application.panel.navItemAreaSelection.selectActiveSuburb();

                    application.panel.navItemAreaSelection.toggleCheckboxes();
                    if (enableContent) {
                        $("#areaSelectionDiv").css('display', 'block');
                    } else {
                        $("#areaSelectionDiv").css('display', 'none');
                    }
                } else {
                    container.load("UserInterface/HTML/AreaSelection.html", function (content) {
                        container.html(content);

                        var header = $("#areaSelectorHeader");
                        var suburbsSelector = $("#areaSelectionDiv");

                        var userSuburbs = application.user.SeeffAreaCollection;
                        $.each(userSuburbs, function (idx, suburb) {
                            var radioItem = application.panel.navItemAreaSelection.constructSuburbRadioItem(suburb);
                            suburbsSelector.append(radioItem);
                            suburbsSelector.append("<br />");
                        });

                        header = header[0].outerHTML;
                        suburbsSelector = suburbsSelector[0].outerHTML;
                        application.panel.navItemAreaSelection.contentCache = header + suburbsSelector;

                        application.panel.navItemAreaSelection.selectActiveSuburb();
                        application.panel.navItemAreaSelection.toggleCheckboxes();
                    });                    
                }                
            },
        selectActiveSuburb: function() {
            if (application.stateManager.activeSuburb) {
                application.panel.navItemAreaSelection.selectSuburbRadioLink(application.stateManager.activeSuburb);
                application.Google.resetPolygonSelection();
                application.Google.showSuburbInfoWindow(application.stateManager.activeSuburb);
                application.stateManager.activeSuburb.PolygonInstance.selected = true;
                application.stateManager.activeSuburb.PolygonInstance.setOptions({ fillOpacity: 0.5 });
            }
        },
        toggleCheckboxes: function() {
            $("#suburbsCheckbox").prop('checked', application.stateManager.allSuburbsShown);
            $("#licensesCheckbox").prop('checked', application.stateManager.allLicensesShown);
            $("#territoriesCheckbox").prop('checked', application.stateManager.allTerritoriesShown);
        },
            addSuburbToCache: function (suburb) {
               application.panel.navItemAreaSelection.contentCache = '';
            },
            constructSuburbRadioItem: function (suburb) {
                var radioItem = $("<input type='radio' name='suburbSelection' class='suburbLinkItem'><a class='suburbLinkItem' href='javascript:void(0);'>" + suburb.AreaName + "</a></input>");
                radioItem.attr('id', 'suburb' + suburb.SeeffAreaID);
                radioItem.attr('onclick', 'application.panel.navItemAreaSelection.handleSuburbLinkItemClick(event)');
                suburb.radioItemLink = radioItem;

                return radioItem;
            },
            selectSuburbRadioLink: function (suburb) {
                var suburbLinkID = 'suburb' + suburb.SeeffAreaID;
                var target = $('#' + suburbLinkID);
                target.prop("checked", true);
                var parentDiv = $('#areaSelectionDiv');
                parentDiv.scrollTop(parentDiv.scrollTop() + target.position().top - parentDiv.height() / 2 + target.height() / 2);
            },
            selectSuburbFromPolyClick: function (suburb) {
                application.stateManager.setActiveSuburb(suburb);
                if (application.stateManager.activeNavItem == application.panel.navItemAreaSelection) {
                    application.panel.navItemAreaSelection.selectSuburbRadioLink(suburb);
                }
                if (application.stateManager.activeNavItem == application.panel.navItemEditPoly) {
                    application.panel.navItemEditPoly.buildContent();
                    application.stateManager.handleEnterPolyEditMode();
                }
            },
            selectLicenseFromPolyClick: function(license) {
                application.stateManager.setActiveLicense(license);
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
            },
            controllers: {
                handleSuburbsCheckbox: function () {
                    var checkbox = $("#suburbsCheckbox");
                    if (checkbox.is(":checked")) {
                        application.stateManager.allSuburbsShown = true;
                        $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                            application.Google.createSuburbPoly(suburb, { render: true });
                        });
                        application.panel.navItemAreaSelection.buildContent(true);
                    } else {
                        application.stateManager.allSuburbsShown = false;
                        $.each(application.user.SeeffAreaCollection, function (index, suburb) {
                            application.Google.createSuburbPoly(suburb, { render: false });
                        });
                        application.panel.navItemAreaSelection.buildContent(false);
                    }
                },
                handleLicensesCheckbox: function () {
                    var checkbox = $("#licensesCheckbox");
                    if (checkbox.is(':checked')) {
                        application.stateManager.allLicensesShown = true;
                        $.each(application.user.SeeffLicenses, function (index, lic) {
                            application.Google.createLicensePoly(lic, {render: true});
                        });
                    } else {
                        application.stateManager.allLicensesShown = false;
                        $.each(application.user.SeeffLicenses, function (index, lic) {
                            application.Google.createLicensePoly(lic, { render: false });
                        });
                    }
                },
                handleTerritoriesCheckbox: function () {
                    var checkbox = $("#territoriesCheckbox");
                    if (checkbox.is(':checked')) {
                        application.stateManager.allTerritoriesShown = true;
                        $.each(application.user.SeeffTerritories, function (index, territory) {
                            application.Google.createTerritoryPoly(territory, { render: true });
                        });
                    } else {
                        application.stateManager.allTerritoriesShown = false;
                        $.each(application.user.SeeffTerritories, function (index, territory) {
                            application.Google.createTerritoryPoly(territory, { render: false });
                        });
                    }
                }
            }
        }
    })
});