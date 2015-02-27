////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// ContentExpanderWidget - User control that allows onw to create a list of divs with content stacked vertically on top of
// each other, and expand/collapse each DIV separately. User defines the content of each DIV.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Constructor function - pass this function an array of items built using buildContentExpanderItem()
function ContentExpanderWidget(containerElementId, contentExpanderItemArray, elementId) {

    this.construct = function () {

        var mainDiv = elementId ? $("<div class='contentExpander' id='" + elementId + "' />") : $("<div class='contentExpander' />");
        $.map(contentExpanderItemArray, function (item, i) {
            mainDiv.append(item);

            var itemHeader = item.find('.expanderItemHeader');
            $(containerElementId).on('click', '#' + itemHeader.attr('id'), function () {
                // Animate this item
                var contentItem = item.find('.expanderItemContent').first();
                contentItem.slideToggle();
            });
        });        

        mainDiv.css('display', 'block');
        return mainDiv;
    };

    this.open = function (itemId) {
        var targetDiv = $.grep(contentExpanderItemArray, function (div) {
            return div.find('.expanderItemHeader').attr('id') == itemId;
        })[0];

        if (targetDiv.find('.expanderItemContent').css('display') == 'none') {
            targetDiv.find('.expanderItemHeader').trigger('click');
        }
    };

    return this;
}

// Use this function to build an item consumed by ContentExpanderWidget
function buildContentExpanderItem(itemId, pathToIcon, itemHeading, itemContent) {
    var itemContainerDiv = $("<div class='expanderItem' />");
    
    var itemHeaderDiv = $("<div id='" + itemId + "' class='expanderItemHeader' />");
    itemHeaderDiv.append("<img class='expanderItemIcon' src='" + pathToIcon + "'  />");
    itemHeaderDiv.append("<label class='expanderItemText'>" + itemHeading + "</label>");

    var itemContentDiv = $("<div id='content" + itemId + "' class='expanderItemContent' />");
    itemContentDiv.append(itemContent);

    itemContainerDiv.append(itemHeaderDiv);
    itemContainerDiv.append(itemContentDiv);

    return itemContainerDiv;
}