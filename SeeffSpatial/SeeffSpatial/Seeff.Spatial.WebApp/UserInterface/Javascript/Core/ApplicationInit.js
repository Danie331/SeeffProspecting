
var application = application || {};

application.init = function (userResult) {
    $(function () {
        //application.baseURL = '';
        application.user = userResult;

        application.Google.initMap();

        application.user.SeeffAreaCollectionLookup = {};
        $.each(application.user.SeeffAreaCollection, function (index, area) {
            var areaID = '' + area.SeeffAreaID;
            application.user.SeeffAreaCollectionLookup[areaID] = area;
            application.Google.drawPoly(area);
        });
        application.panel.initPanel();
    });
}