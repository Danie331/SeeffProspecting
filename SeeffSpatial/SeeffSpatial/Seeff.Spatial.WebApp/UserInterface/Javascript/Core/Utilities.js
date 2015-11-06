
$(function () {
    $.extend(application, {
        utilities: {
            getUrlParameter: function (paramName) {
                var url = window.location.search;
                var querystring = url.substring(url.indexOf('?') + 1, url.length);
                var params = querystring.split('&');
                var kvps = [];
                $.each(params, function (idx, param) {
                    var key = param.indexOf('=') > -1 ? param.split('=')[0] : param;
                    kvps[key] = param.indexOf('=') > -1 ? param.split('=')[1] : param;
                });

                return kvps[paramName];
            }
        }
    });
});