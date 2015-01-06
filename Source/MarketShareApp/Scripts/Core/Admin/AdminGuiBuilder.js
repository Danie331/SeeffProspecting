
var selectedArea = null;

function buildOptionsForAdminUser() {
    createAdminMenuHtml();
    buildAdminContextMenu();
}

function createAdminMenuHtml() {

    var menuItem = createMenuItem("Administrator options", "adminoptions", buildAdminOptions());
    appendMenuItemContent(menuItem.MenuItemContent);
    menuItems.push(menuItem);
    showMenu("adminoptions");
    // pre-load area data here for later use 
    loadAreaData(true);
    // Plot kml temp kml if available
    loadAndShowKML();
}

function buildAdminOptions() {
    var html = $('<div id="adminOptionsDiv"></div>');
    html.append("<button type='button' id='selectAreaButton' value='Area selector'>Area selector</button>");
    html.append("<br />");
    html.append("<button type='button' id='gotoLatLngButton' value='Go to location'>Go to location</button>");
    html.append("<br />");
    html.append("<button type='button' id='toggleDrawingButton' value='Toggle drawing tool'>Toggle drawing tool</button>");
    html.append("<br />");
    html.append("<button type='button' id='showBranchAreas' value='Find branch areas'>Find branch areas</button>");

    menu.on('click', '#selectAreaButton', showAreaSelector);
    menu.on('click', '#gotoLatLngButton', initGotoLocationDialog);
    menu.on('click', '#toggleDrawingButton', toggleDrawingFunctionality);
    menu.on('click', '#showBranchAreas', showBranchAreaSelector);

    return html.html();
}

function showBranchAreaSelector() {

    // TODO: implement.
}

function showAreaSelector() {

    $("#areaSearchContainerDiv").dialog(
        {
            modal: true,
            width: "auto",
            height: "auto",
            resize: "auto",
            position: ['right', 'center'],
            buttons: {
                "Load": function ()
                {
                    var areaId = selectedArea.split("_")[0];
                    var resCommAgri = selectedArea.split("_")[1];
                    loadAreaByIdAndType(areaId, resCommAgri);
                }
            }
        });

    var data = [];
    $.map(areaData, function (item) {
        var itemValue = item.LocationID + "_" + item.ResCommAgri;
        data.push({ value: itemValue, label: item.LocationName });
    });
    $("#availableAreas").autocomplete({
        source: data,
        focus: function (event, ui) {
            event.preventDefault();
            $(this).val(ui.item.label);
            selectedArea = ui.item.value;
        },
        select: function (event, ui) {
            event.preventDefault();
            $(this).val(ui.item.label);
            selectedArea = ui.item.value;
        }
    });
}

function initGotoLocationDialog() {
    $("#dialogPlotPointOnMap").dialog(
                    {
                        modal: true,
                        open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                        width: "auto",
                        height: "auto",
                        resize: "auto",
                        buttons: {
                            "Find": function () {
                                var lat = $('#latInput').val();
                                var lng = $('#lngInput').val();

                                gotoLocation(lat, lng);
                                $(this).dialog("close");
                            }
                        },
                        position: ['right', 'center']
                    });
}

function buildAdminContextMenu() {

    google.maps.event.addListener(map, "rightclick", function (event) {

        rightClickedLocation.Lat = event.latLng.lat();
        rightClickedLocation.Lng = event.latLng.lng();
        $('.context-menu-search').contextMenu({ x: event.pixel.x, y: event.pixel.y });
    });

    $.contextMenu({
        selector: '.context-menu-search',
        build: function ($trigger, e) {
            return {
                callback: function (key, options) {
                    switch (key) {
                        case "Find suburb under this location":
                            tryLocateSuburbUnderPoint(rightClickedLocation);
                            break;
                    }
                },
                items: {
                    "Find suburb under this location": { name: "Find suburb under this location", icon: "find_suburb_under_point" }
                }
            };
        }
    });

    $.contextMenu({
        selector: '.context-menu-polyoptions',
        build: function ($trigger, e) {
            return {
                callback: function (key, options) {
                    switch (key) {
                        case "Edit|Save":
                            if (currentActivePoly) {                               
                                showDialogAddOrEditArea(currentActivePoly);
                            }
                            break;
                        case "Remove from map":
                            if (currentActivePoly) {
                                removePolyFromMap(currentActivePoly);
                            }
                            break;
                    }
                },
                items: {
                    "Edit|Save": { name: "Edit|Save", icon: "save_changes" },
                    "Remove from map": { name: "Remove from map", icon: "remove_poly"}
                }
            };
        }
    });
}

function createAreaArrayFromIDs(areaIDs) {

    var areaArray = [];
    $.map(areaData, function (area) {
        if ($.inArray(area.LocationID, areaIDs) > -1) {
            areaArray.push(area);
        }
    });

    // Ensure that only areas with a unique location ID are returned for the purpose of this control.
    var uniqueIds = [];
    var uniqueAreasByLocationID = [];
    $.each(areaArray, function (idx, element) {

        if ($.inArray(element.LocationID, uniqueIds) == -1) {
            uniqueAreasByLocationID.push(element);
        }
        uniqueIds.push(element.LocationID);
    });
    return uniqueAreasByLocationID;
}

function getAreasExcluding(areasToExclude) {

    var netAreas = [];
    $.map(areaData, function (area) {
        var areaId = area.LocationID;
        if ($.inArray(areaId, areasToExclude) == -1) {
            netAreas.push(area);
        }
    });

    // Ensure that only areas with a unique location ID are returned for the purpose of this control.
    var uniqueIds = [];
    var uniqueAreasByLocationID = [];
    $.each(netAreas, function (idx, element) {

        if ($.inArray(element.LocationID, uniqueIds) == -1) {
            uniqueAreasByLocationID.push(element);
        }
        uniqueIds.push(element.LocationID);
    });
    return uniqueAreasByLocationID;
}

function initRelatedAreasForArea(area) {

    $('#relatedAreasMultiSelect').remove();
    $('#relatedAreasButton').unbind('click').bind('click', function () {
        var areasLeftPanel = getAreasExcluding(area.RelatedAreas);
        var areasRightPanel = createAreaArrayFromIDs(area.RelatedAreas);
        var selector = new MultiSelectWidget("relatedAreasMultiSelect", "Edit related areas", "All Areas", "Related Areas", areasLeftPanel, areasRightPanel,
                                  function (item) { return item.LocationID; },
                                   function (item) { return item.LocationName; },
                                   function (leftSelection, rightSelection) {

                                       area.RelatedAreas.length = 0;
                                       $.map(rightSelection, function (val) {
                                           area.RelatedAreas.push(val.LocationID);
                                       });
                                   });

        var html = selector.buildDialogElement();
        $('body').append(html);
        selector.show();
    });
}

function initNeighboringAreasForArea(area) {

    $('#neighboringAreasMultiSelect').remove();
    $('#neighboringAreasButton').unbind('click').bind('click', function () {
        var areasLeftPanel = getAreasExcluding(area.NeighboringAreas);
        var areasRightPanel = createAreaArrayFromIDs(area.NeighboringAreas);
        var selector = new MultiSelectWidget("neighboringAreasMultiSelect", "Edit neighboring areas", "All Areas", "Neighboring", areasLeftPanel, areasRightPanel,
                                  function (item) { return item.LocationID; },
                                   function (item) { return item.LocationName; },
                                   function (leftSelection, rightSelection) {

                                       area.NeighboringAreas.length = 0;
                                       $.map(rightSelection, function (val) {
                                           area.NeighboringAreas.push(val.LocationID);
                                       });
                                   });

        var html = selector.buildDialogElement();
        $('body').append(html);
        selector.show();
    });
}

function populateEditAreaNameTextBox(areaName) {

    // The area name is likely to be in the fully qualified format, ie areaName, AreaParentName (Comm/Res/Agri).
    // Strip out only the area name portion
    var areaNamePortion = areaName ? areaName.split(',')[0] : '';
    $('#areaNameInput').val(areaNamePortion);
}

function createOptionForAdminSelector(area, nameSelector, valueSelector) {

    var name = nameSelector(area);
    var value = valueSelector(area);
    if (name.length >= 28) {
        return '<option value="' + value + '" title="' + name + '">' + name + '</option>';
    }

    return '<option value="' + value + '" >' + name + '</option>';
}

function populateAreaParent(parentId) {

    $('#areaParentSelector').empty();
    $('#areaParentSelector').append('<option value="0" />');
    $.map(areaData, function (val, i) {
        $('#areaParentSelector').append(createOptionForAdminSelector(val, function (a) { return a.LocationName; }, function (a) { return a.LocationID; }));
    });

    if (parentId) {
        $('#areaParentSelector').val(parentId);
    }
    else {
        $('#areaParentSelector').val(0);
    }
}

function populateAreaType(areaTypeId) {

    $('#areaTypeSelector').empty();
    $('#areaTypeSelector').append('<option value="0" />');
    $.map(areaTypes, function (val) {
        $('#areaTypeSelector').append(createOptionForAdminSelector(val, function (a) { return a.AreaTypeName; }, function (a) { return a.AreaTypeId; }));
    });

    if (areaTypeId) {
        $('#areaTypeSelector').val(areaTypeId);
    } else {
        $('#areaTypeSelector').val(0);
    }
}

function populateAreaPolyTypeCheckbox(areaPolyTypes) {

    $('#residentialCheckbox').prop("checked", false);
    $('#commercialCheckbox').prop("checked", false);
    $('#agriCheckbox').prop("checked", false);

    areaPolyTypes = areaPolyTypes.split(',');

    if ($.inArray('R', areaPolyTypes) != -1) {
        $('#residentialCheckbox').prop("checked", true);
    }
    if ($.inArray('C', areaPolyTypes) != -1) {
        $('#commercialCheckbox').prop("checked", true);
    }
    if ($.inArray('A', areaPolyTypes) != -1) {
        $('#agriCheckbox').prop("checked", true);
    }
}

function getResCommAgriTypeSelection() {
    var areaPolyTypes = '';
    if ($('#residentialCheckbox').prop("checked")) {
        areaPolyTypes += 'R,';
    }
    if ($('#commercialCheckbox').prop("checked")) {
        areaPolyTypes += 'C,';
    }
    if ($('#agriCheckbox').prop("checked")) {
        areaPolyTypes += 'A,';
    }

    return areaPolyTypes.substring(0, areaPolyTypes.length - 1);
}

function removePolyFromMap(poly) {
    poly.setMap(null);
    currentActivePoly = null;
}

function showDialogAddOrEditArea(poly) {

    if (!poly.Area) {
        // If its a new area being created then all we have at our disposal here are the poly coordinates
        poly.Area = createGenericArea(null, null, null, null, createPolyPathArray(poly), null, [], []);
        poly.Area.ResCommAgri = 'R'; //default to res for new areas.
    }

    initRelatedAreasForArea(poly.Area);
    initNeighboringAreasForArea(poly.Area);
    populateEditAreaNameTextBox(poly.Area.LocationName);
    populateAreaParent(poly.Area.ParentLocationId);
    populateAreaType(poly.Area.AreaTypeId);
    populateAreaPolyTypeCheckbox(poly.Area.ResCommAgri); 

    $("#createOrEditAreaDialog").dialog(
        {
            modal: true,
            width: '550',
            height: '450',
            open: function (event, ui) {
                if (slidePanelOpen) {
                    $('#closepanelbutton').trigger('click');
                }
            },
            buttons: {
                "Save All Changes": function () {
                    poly.Area.LocationName = $('#areaNameInput').val();
                    poly.Area.ParentLocationId = $('#areaParentSelector').val();
                    poly.Area.AreaTypeId = $('#areaTypeSelector').val();
                    poly.Area.PolyCoords = createPolyPathArray(poly);                                        
                    poly.Area.ResCommAgri = getResCommAgriTypeSelection();

                    saveArea(poly.Area);                    
                },
                "Close": function () { $(this).dialog("close"); }
            }
        });
}