var availableCredit = 0;

var suburbsInfo, allAgencies;
var map, menu, infowindow;
var menuItems = [];
var slidePanelOpen = true;

// Stores *all* domain-level information required by various features. This object maps to the C# type 'ProspectingContext'. 
var prospectingContext = null;

// Flag that indicates whether this is a Prospecting Manager - only PMs can flag contacts as POPI restricted etc.
var userIsProspectingManager;

function initialize() {
    // Make ajax call to load from server 
    var inputPacket = { Instruction: "load_application" };
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify(inputPacket),
        success: function (data, textStatus, jqXHR) {
            if (textStatus == "success") {
                $('#loadingDiv').remove();

                if (!data.Authenticated) {
                    window.location = "/NotAuthorised.aspx";
                }
                if (!handleResponseIfServerError(data)) {
                    return;
                }

                $('#mainpanel').css('display', 'block');

                prospectingContext = JSON.parse(data.StaticProspectingData);
                prospectingContext.LoggedInUser = data.UserGuid;
                prospectingContext.LoggedInUsername = data.Fullname;
                prospectingContext.UserHasCommAccess = data.HasCommAccess;
                prospectingContext.TrustLookupsEnabled = data.TrustLookupsEnabled;
                prospectingContext.UserActivityTypes = data.ActivityTypes;
                prospectingContext.UserFollowupTypes = data.ActivityFollowupTypes;
                prospectingContext.BusinessUnitUsers = data.BusinessUnitUsers;
                prospectingContext.IsTrainingMode = data.IsTrainingMode;
                prospectingContext.UserHasExportPermission = data.ExportPermission;

                userIsProspectingManager = data.IsProspectingManager;
                availableCredit = data.AvailableCredit;
                initializeMenuHtml();
                loadSuburbsInfo(data);
                initializeMap(function () {
                    createProspectingMenu(data);
                    initEventHandlers();
                    if (prospectingContext.IsTrainingMode) {
                        showDialogTrainingMode();
                    }
                });
            }
        },
        error: function (textStatus, errorThrown) { debugger; },
        dataType: "json"
    });

    // Handle the browser close event - cleanup
    $(window).bind("beforeunload", function () {
        unlockCurrentProperty();
    });

    $(document).mouseup(function (e) {
        var commMenu = $("#communicationMenu");
        if (!commMenu.is(e.target)) {
            commMenu.hide();
        }

        var templatesMenu = $("#templatesMenu");
        if (!templatesMenu.is(e.target)) {
            templatesMenu.hide();
        }

        //var activityFilterMenu = document.getElementsByClassName('filterMenuCombo')[0];
        //if (activityFilterMenu && !$.contains(activityFilterMenu, e.target)) {
        //    $(activityFilterMenu).find('#checkboxes').hide();
        //}
    });
}

function initializeMap(callback) {
    map = new google.maps.Map(document.getElementById("googleMap"),
        {
            zoom: 13,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            zoomControlOptions: {
                style: google.maps.ZoomControlStyle.DEFAULT,
                position: google.maps.ControlPosition.RIGHT_TOP
            },
            panControlOptions: {
                position: google.maps.ControlPosition.RIGHT_TOP
            }
        });

    map.setZoom(6);
    var center = new google.maps.LatLng(-29.687345, 22.669017);
    map.setCenter(center);

    google.maps.event.addListenerOnce(map, 'idle', function () {
        // Leave the 2 statements below [Google Maps initialization quirks]
        var infWindow = new google.maps.InfoWindow({
            content: ""
        });
        calcMapCenterWithOffset(-29.687345, 22.669017);

        callback();
    });
}

function loadSuburbsInfo(data) {
    suburbsInfo = data.AvailableSuburbs;
    if (suburbsInfo) {
        suburbsInfo = JSON.parse(suburbsInfo);
        for (var i = 0; i < suburbsInfo.length; i++) {
            suburbsInfo[i].IsInitialised = false;
            suburbsInfo[i].RequiresStatsUpdate = true;
        }
    }
}

function showDialogTrainingMode() {
    var contents = $("<div title='Prospecting Training Mode' style='font-family:Verdana;font-size:12px;' />");
    contents.append("<p />");
    contents.append("Welcome to the training version of Seeff Prospecting!");
    contents.append("<p />");
    contents.append("You are now in a training environment that emulates most of the features available in the live Prospecting system. Using this mode will not affect the live system and has no monetary impact.");
    contents.append("<p />");
    contents.append("While in training mode:");
    contents.append("<br />");
    contents.append(" - live (production) data will not be affected by changes made here");
    contents.append("<br />");
    contents.append(" - transactions will not be billed (your live balance will not be affected)");
    contents.append("<br />");
    contents.append(" - lookups to service providers will return static test data only");
    contents.append("<br />");
    contents.append(" - communications (email and SMS) will not be sent to any recipients");

    contents.dialog(
                  {
                      modal: true,
                      closeOnEscape: false,
                      width: '550',
                      buttons: {
                          "OK": function () { $(this).dialog("close"); }
                      },
                      position: ['center', 'center']
                  });
}