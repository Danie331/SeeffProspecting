
function detectIE() {
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf('MSIE ');
    var trident = ua.indexOf('Trident/');

    if (msie > 0) {
        // IE 10 or older => return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
    }

    if (trident > 0) {
        // IE 11 (or newer) => return version number
        var rv = ua.indexOf('rv:');
        return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
    }

    // other browser
    return false;
}

function generateUniqueID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function validPhoneNumber(testNumber) {
    if (!testNumber || testNumber.length == 0) return false;
    if (testNumber.length == 10) {
        var reg = /^\d+$/;
        return reg.test(testNumber);
    }

    return false;
}

function validEmailAddress(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

// For now this function does a superficial check of the ID number ie 13 digits...replace this by an actual validation algorithm
function validIDNumber(testNumber) {
    if (!testNumber || testNumber.length == 0) return false;
    if (testNumber.length == 13) {
        var reg = /^\d+$/;
        return reg.test(testNumber);
    }

    return false;
}

function toTitleCase(str) {
    return str.replace(/\w\S*/g, function (txt) {
        if (!txt || txt.length == 0) return '';
        var personTitle = txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
        return personTitle;
    });
}

function getSADialingCodeId() {
    var sa = $.grep(prospectingContext.IntlDialingCodes, function (idc) {
        return idc.Value.indexOf("South Africa") > -1;
    })[0];

    return sa.Key; //should be 1
}

function valueIsSingleDigit(value) {
    if (value.length == 1) {
        var reg = /^\d+$/;
        return reg.test(value);
    }

    return false;
}

function calcMapCenterWithOffset(lat, lng, offset_x, offset_y) {
    var proj = new google.maps.OverlayView();
    proj.draw = function () { };
    proj.setMap(map);

    var currentProj = proj.getProjection();
    if (currentProj) {
        var point = currentProj.fromLatLngToDivPixel(new google.maps.LatLng(lat, lng));
    }
    else {
        return null;
    }

    point.x = point.x + offset_x;
    point.y = point.y + offset_y;

    var newLatLng = proj.getProjection().fromDivPixelToLatLng(point);
    return newLatLng;
}

function isCKNumber(number) {
    // Currently to validate a CK number all we do is test whether there are any non-numeric digits present
    return number.indexOf("/") > -1 || number.indexOf("\\") > -1;
}

function errorHandler(ex) {
    $('#errorDialog').dialog({
        modal: true,
        open: function () { $('#errorDialogText').text('Error: ' + ex.message); },
        buttons: {
            "OK": function () {
                $(this).dialog("close");
            }
        },
        position: ['right', 'center']
    });
}