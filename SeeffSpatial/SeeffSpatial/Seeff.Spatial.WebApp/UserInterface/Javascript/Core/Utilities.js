
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
            },
            buildHomeURL: function (servicePath) {
                return application.baseURL + servicePath;
            },
            defaultIfNullOrUndef: function (testValue, defaultValue) {
                return testValue && testValue != null ? testValue : defaultValue;
            },
            handleGetPolygonFromFile: function (evt, callback) {
                var files = evt.target.files;
                var file = files[0];
                var reader = new FileReader();
                reader.onload = (function (theFile) {
                    return function (e) {
                        var poly = application.utilities.readFromKMLFile(e, theFile);
                        callback(poly);
                        // Add statistics
                        application.stateManager.fileUploadCounter.push({ FileName: theFile.name, When: new Date() });
                    };
                })(file);
                reader.readAsDataURL(file);
            },
            parseKMLInputString: function(rawXMLString) {
                var polyString = rawXMLString.match(/<coordinates>[\s\S]*?<\/coordinates>/)[0];
                polyString = polyString.replace('<coordinates>','').replace('</coordinates>','');
                var coords = polyString.split(' ');
                var result = [];
                $.each(coords, function (idx, pair) {
                    var lat = pair.split(',')[1];
                    var lng = pair.split(',')[0];
                    if (lat && lng) {
                        result.push({ lat: Number(lat), lng: Number(lng) });
                    }
                });
                return result;
            },
            readFromKMLFile: function (e, file) {
                var dataStringOffset = e.currentTarget.result.indexOf('base64,');
                var data = e.currentTarget.result.substring(dataStringOffset + 7, e.currentTarget.result.length);
                var decodedData = window.atob(data);
                var coords = application.utilities.parseKMLInputString(decodedData);

                var polygon = new google.maps.Polygon({
                    paths: coords,
                    strokeColor: '#FF0000',
                    strokeOpacity: 0.8,
                    strokeWeight: 1.5,
                    fillColor: '#FF0000',
                    fillOpacity: 0.0
                });
                return polygon;
            },
            yieldingLoop: function (count, chunksize, callback, finished) {
                var i = 0;
                (function chunk() {
                    var end = Math.min(i + chunksize, count);
                    for (; i < end; ++i) {
                        callback.call(null, i);
                    }
                    if (i < count) {
                        setTimeout(chunk, 0);
                    } else {
                        finished.call(null);
                    }
                })();
            },
            download: function (filename) {
                var downloadURL = application.baseURL + '/KMLExports/' + filename + '.kml';
                var form = $('<form method="get" action="' + downloadURL + '"><input type="hidden" value="" /></form>');
                $('body').append(form);
                $(form).submit();
                form.remove();
            }
        }
    });
});