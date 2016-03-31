

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// MultiSelectWidget: jquery dialog that allows you to move elements between two panels, and save current selection in both panels.
// Pre-conditions: the items used must have a unique identifier per item, and a display value - the item/value selector functions are used to get these.
// Usage: create a new object passing in the required parameters. The element needn't exist on the DOM - will be created and added internally.
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function MultiSelectWidget(anchorElementId,   // The id of the element that will be created to hold the dialog
                            titleDialog,       // Dialog title
                            captionLeftPanel,  // Caption on the left
                            captionRightPanel, // Caption on the right
                            itemsLeftPanel,    // Item array to populate left panel with
                            itemsRightPanel,   // Item array to populate right panel with
                            itemValueSelector, // Function that determines the "value" or ID property of an item. Format: function(item) { return unique_id; }
                            itemNameSelector,  // Function that determines the display text of an item. Format: function(item) { return display value; }
                            saveAction) {      // Action to perform with items on RIGHT panel when save is clicked. Format: function(itemsLeft, itemsRight) {}

    var leftSelect = null;
    var rightSelect = null;
    var moveItemRight = null;
    var moveItemLeft = null;

    var allItems = $.merge($.merge([], itemsLeftPanel), itemsRightPanel);

    // Call this function from the client script to retrieve the html for the dialog
    this.buildDialogElement = function buildDialogHtml() {
        var dialogHtml =
                    '<div id="' + anchorElementId + '" class="multiSelectDialogDiv" title="' + titleDialog + '">' +
                      '<label class="multiSelectCaptionsDiv">' + 'Move agencies from the left panel to the right panel to make them available to the suburbs under your license.' + '</label>' + '<p />' +
                      '<div class="multiSelectCaptionsDiv">' +
                      '<label class="multiSelectLeftPanelCaption">' + captionLeftPanel + '</label>'+
                      '<label class="multiSelectRightPanelCaption">' + captionRightPanel + '</label>' +
                      '</div>'+
                      '<br />'+
                      ' <div class="multiSelectLeftSelectorDiv">'+
                      '<select class="multiSelectLeftSelect" id="leftSelect_' + anchorElementId + '" size="20" multiple></select>' +
                      '</div>'+
                      '<div class="multiSelectControlsDiv">' +
                     '<input id="moveitemright_' + anchorElementId + '" type="button" value=">" disabled />' +
                     '<br />'+
                     '<br />'+
                     '<input id="moveitemleft_' + anchorElementId + '" type="button" value="<" disabled />' +
                     '</div>'+
                     '<div class="multiSelectRightSelectorDiv">' +
                     '<select class="multiSelectRightSelect" id="rightSelect_' + anchorElementId + '" size="20" multiple></select>' +
                     '</div>'+
                     '</div>';

        return dialogHtml;
    }

    // Call this function to show the dialog after you have added the element's html to the dom.
    // NB.: It is assumed that you have already added the html to the dom *before* calling this function.
    this.show = function () {

        leftSelect = $("#leftSelect_" + anchorElementId);
        rightSelect = $("#rightSelect_" + anchorElementId);
        moveItemRight = $("#moveitemright_" + anchorElementId);
        moveItemLeft = $("#moveitemleft_" + anchorElementId);

        // Populate the panels with items
        populateLeftPanel();
        populateRightPanel();

        // Attach event handlers
        moveItemRight.click(moveRight);
        moveItemLeft.click(moveLeft);

        leftSelect.change(leftItemSelected);
        rightSelect.change(rightItemSelected);

        // We assume that the html for the widget has already been added to the dom at this point using buildDialogElement()
        $('#' + anchorElementId).dialog({
            modal: true,
            closeOnEscape: true,
            open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
            width: 700,
            height: 540,
            resizable: false,
            buttons: { "Save changes": executeSave, "Close": function () { $(this).dialog("close"); } },
            //position: ['center', 'center']
        });
    }

    function executeSave() {

        // Get items currently on the left and right panels and pass them to users save function
        var currentItemsLeft = [];
        var currentItemsRight = [];

        leftSelect.children().each(function () {
            var itemId = $(this).attr("value");
            var originalItem = $.grep(allItems, function (item) { return itemValueSelector(item) == itemId; })[0];

            currentItemsLeft.push(originalItem);
        });

        rightSelect.children().each(function () {
            var itemId = $(this).attr("value");
            var originalItem = $.grep(allItems, function (item) { return itemValueSelector(item) == itemId; })[0];

            currentItemsRight.push(originalItem);
        });

        saveAction(currentItemsLeft, currentItemsRight);
    }

    function populateLeftPanel() {

        leftSelect.empty();

        $.map(itemsLeftPanel, function (val, i) {
            leftSelect.append(createOption(val));
        });
    }

    function populateRightPanel() {

        rightSelect.empty();

        $.map(itemsRightPanel, function (val, i) {
            rightSelect.append(createOption(val));
        });
    }

    function moveLeft() {

        var items = rightSelect.val();
        if (items) {
            rightSelect.find('option:selected').each(function () { $(this).remove(); });
            $.map(items, function (val, i)
            {
                var originalItem = findItemById(val);
                leftSelect.append(createOption(originalItem));
            });
        }
    }

    function moveRight() {

        var items = leftSelect.val();
        if (items) {
            leftSelect.find('option:selected').each(function () { $(this).remove(); });
            $.map(items, function (val, i)
            {
                var originalItem = findItemById(val);
                rightSelect.append(createOption(originalItem));
            });
        }
    }

    function createOption(item) {

        var itemValue = itemValueSelector(item);
        var itemDisplayText = itemNameSelector(item);

        if (itemDisplayText.length >= 28) {
            return '<option value="' + itemValue + '" title="' + itemDisplayText + '">' + itemDisplayText + '</option>';
        }

        return '<option value="' + itemValue + '" >' + itemDisplayText + '</option>';
    }

    function leftItemSelected() {

        moveItemRight.removeAttr("disabled");
        moveItemLeft.attr("disabled", "disabled");

        rightSelect.val([]);
    }

    function rightItemSelected() {

        moveItemLeft.removeAttr("disabled");
        moveItemRight.attr("disabled", "disabled");

        leftSelect.val([]);
    }

    function findItemById(id) {

        return $.grep(allItems, function (i) {
            return itemValueSelector(i) == id;
        })[0];
    }

    return this;
}