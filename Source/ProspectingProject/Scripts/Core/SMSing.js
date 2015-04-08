
var multiSelectMode = false;


function buildSMSMenu() {
    var labelHeader = $("<label>Enter the SMS to send below:</label>");
    var smsTextBox = $("<textarea rows='5' style='width:100%;padding:0' ></textarea>");
    var sendBtn = $("<input type='button' id='sendSMS' style='float:right;cursor:pointer;' value='Send' />");
    var toggleMultiSelect = $("<input type='button' id='toggleMultiSelect' style='float:left;cursor:pointer;' value='Toggle Multi-Select' />");

    var contentDiv = $("<div class='contentdiv' style='padding-right:10px' />");
    contentDiv.append(labelHeader).append("<br/>").append(smsTextBox).append("<br/>").append(sendBtn).append('<p/>').append(toggleMultiSelect);

    toggleMultiSelect.click(function () { toggleMultiSelectMode(); });

    var multiSelectSticker = $("#multiSelectMode");
    var mapControl = map.controls[google.maps.ControlPosition.TOP_RIGHT];
    mapControl.push(multiSelectSticker[0]);

    return contentDiv;
}

function handleSMSMenuItemClick() {

}

function toggleMultiSelectMode(value) {
    if (value) {
        multiSelectMode = value
    }
    else {
        multiSelectMode = !multiSelectMode;
    }

    var multiSelectSticker = $("#multiSelectMode");
    if (multiSelectMode) {
        multiSelectSticker.css('display', 'block');
    } else {
        multiSelectSticker.css('display', 'none');
    }
}