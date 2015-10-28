
var application = application || {};

application.Google = {
    map: null,
    initMap: function () {
        map = new google.maps.Map(document.getElementById('map'), {
            center: { lat: -29.258248, lng: 15.358887 },
            zoom: 6,
            zoomControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP },
            panControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP }
        });
    }
};

application.init = function () {
    $(function () {
        // Order important
        application.Google.initMap();
        application.panel.initPanel();
    });                
}