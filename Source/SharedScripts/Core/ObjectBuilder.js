

function newSuburb() {

    return { IsInitialised: false, SelectedAgencies: null, SuburbId: null, Listings: null, PolyCoords: null, Name: null };
}

function createInfoWindow(lat, lng, text) {
    var infowindow = new google.maps.InfoWindow();
    infowindow.setContent("<div style='line-height:1.35;overflow:hidden;white-space:nowrap;'>" + text + "</div>");
    infowindow.setPosition(new google.maps.LatLng(lat, lng));

    return infowindow;
}

function createAgencyStatistic(agencyName, value) {

    return { AgencyName: agencyName, Value: value };
}

function createPropertyTypeStatistic(type, value) {

    return { PropertyType: type, Value: value };
}

function createMenuItem(itemName, itemId, itemContent) {

    var itemContentDiv = $("<div id='" + itemId + "_content" + "' />");
    itemContentDiv.append(itemContent);
    var item = { MenuItemName: itemName, MenuItemId: itemId, MenuItemContent: itemContentDiv, MenuItemDiv: null };

    var menuItemPanel = $('#menuitempanel');

    var div = $("<div id='itemdiv' style='padding:5px 5px 10px 10px;' />");
    var button = $("<a href='' id='" + itemId + "'>" + itemName + "</a>");

    // attach click handler to button
    button.unbind('click').bind('click', function (event) {

        event.preventDefault();
        var btn = $(this);
        clearMenuSelection();

        //btn.append("<img src='Assets/tick.png' />");
        btn.parent().css('background-color', '#E0E0E0');

        showContentForItem($(this).attr("id"));
    });

    div.append(button);
    menuItemPanel.append(div);

    item.MenuItemDiv = div;

    return item;
}

// Maps to the GenericArea.cs type
function createGenericArea(areaId, areaName, areaParentId, areaTypeId, polyData, listings, relatedAreas, neighboringAreas) {

    return {
        LocationID: areaId,
        LocationName: areaName,
        ParentLocationId: areaParentId,
        AreaTypeId: areaTypeId,
        PolyCoords: polyData,
        Listings: listings,
        RelatedAreas: relatedAreas,
        NeighboringAreas: neighboringAreas
    };
}