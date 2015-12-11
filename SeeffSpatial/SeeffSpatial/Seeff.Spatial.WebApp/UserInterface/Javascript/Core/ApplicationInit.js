
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
            application.Google.createSuburbPoly(area, {render: true});
        });
        $.each(application.user.SeeffLicenses, function (index, poly) {
            application.Google.createLicensePoly(poly);
        });
        $.each(application.user.SeeffTerritories, function (index, poly) {
            application.Google.createTerritoryPoly(poly);
        });
        application.panel.initPanel();

        //application.Google.addRightClickHook();
    });
}