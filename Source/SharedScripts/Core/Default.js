

//
// Entry point for the application.
//
function loadApplication() {

    initializeMenuHtml();
    if (userContext() == "user") {
        loadSuburbsInfo();
        loadAllAgencies();
        initializeMap();
        buildOptionsForOrdinaryUser();
    }

    if (userContext() == "admin") {
        initializeMap({ DefaultZoom: 5, DefaultLat: -29.512027, DefaultLng: 21.578423 });
        buildOptionsForAdminUser();
    }
}

function timerIncrement() {
    idleTime++;

    if (idleTime > 29) { // 30 mins
        idleTime = 0;
        $('#timeoutDialog').dialog({            
            modal: true,
            open: function () {
                $('#timeoutDialog').siblings(".ui-dialog-titlebar").hide();
                timeoutElapsed = true;
                setTimeout(function () {
                    if (timeoutElapsed) {
                        window.location.href = "Logout.aspx";
                    }
                }, 30000); // 30 seconds to react
            },
            buttons: {
                "Continue": function () { $(this).dialog("close"); timeoutElapsed = false; },
                "Logout": function () { window.location.href = "Logout.aspx"; }
            },
            position: ['right', 'center']
        });
    }
}

function loadAllAgencies() {
    allAgencies = $("#all_agencies").val();
    if (allAgencies) {
        allAgencies = JSON.parse(allAgencies);
    }
}

function loadSuburbsInfo() {
    suburbsInfo = $("#user_suburbs").val();
    if (suburbsInfo) {
        suburbsInfo = JSON.parse(suburbsInfo);

        for (var i = 0; i < suburbsInfo.length; i++) {
            suburbsInfo[i].IsInitialised = false;
        }
    }
}

function initializeMap(defaultZoomAndLocation) {
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
    defaultZoomAndLocation = { DefaultZoom: 5, DefaultLat: -29.683976, DefaultLng: 19.900560 }; 
    if (defaultZoomAndLocation) {

        var zoom = defaultZoomAndLocation.DefaultZoom;
        var defaultLat = defaultZoomAndLocation.DefaultLat;
        var defaultLng = defaultZoomAndLocation.DefaultLng;

        // Set zoom here
        map.setZoom(zoom);
        var center = new google.maps.LatLng(defaultLat, defaultLng);
        map.setCenter(center);
    }
}

function initializeMenuHtml() {

    menu = $('#mainpanel');
    $('#openpanelbutton').click(function () {
        $('#openpanelbutton').css('display', 'none');
        $('#closepanelbutton').trigger('click');
    });

    $('#snappanelbutton').click(function () {
        menu.css({ 'left': '0', 'top': '0' });
    });

    togglePanel();

    $("#menuitempanel").resizable();
}

function togglePanel(actionAfterClosing) {

    $('#closepanelbutton').unbind('click').bind('click', function () {
        $('#mainpanel').css('min-width', '');
        $("#mainpanel").animate(
            { width: 'toggle' },
            {
                duration: 500,
                complete: function () {

                    $('#mainpanel').css('min-width', '45%');
                    if (slidePanelOpen) {
                        slidePanelOpen = false;
                        $('#openpanelbutton').css('display', 'block');
                    }
                    else {
                        slidePanelOpen = true;
                        $('#openpanelbutton').css('display', 'none');
                    }

                    if (actionAfterClosing) {
                        actionAfterClosing();
                    }
                }
            })
    });
}

////////////////////////////////////////////////////////////////////////////////////////////////
// ********************************************************************************************
// SECTION Business logic and all domain specific client side functionality
// ********************************************************************************************
////////////////////////////////////////////////////////////////////////////////////////////////

function userContext() {

    var typeOfUser = $("#user_designation").val();
    typeOfUser = typeOfUser.split("_")[0];
    switch (typeOfUser) {
        case "licensee":
            return "user";
        case "admin":
            return "admin";
    }
}

function userHasAtLeastOneSuburb() {
    return suburbsInfo.length > 0;
}

function userCanEditAtLeastOneSuburb() {
    var suburbsCanEdit = $.grep(suburbsInfo, function (s) {
        return s.CanEdit;
    });

    return suburbsCanEdit.length > 0;
}

function getListingByUniqueId(uniqueId) {

    for (var suburbIdx = 0; suburbIdx < suburbsInfo.length; suburbIdx++) {
        if (suburbsInfo[suburbIdx].Listings) {
            for (var listingIdx = 0; listingIdx < suburbsInfo[suburbIdx].Listings.length; listingIdx++) {
                if (suburbsInfo[suburbIdx].Listings[listingIdx].UniqueId === uniqueId)
                    return suburbsInfo[suburbIdx].Listings[listingIdx];
            }
        }
    }
}

function getFatedListingsForMarker(listingsForMarker) {
    var fatedListings = $.grep(listingsForMarker, function (listing) {
        return listing.Fated;
    });

    return fatedListings;
}

function getCountLightstoneListings(listings) {

    var total = $.grep(listings, function (listing, index) {
        return !listing.IsCurrentSeeffListing;
    });

    return total.length;
}

function hasUnfatedListings(listingsForMarker) {

    var fatedListings = getFatedListingsForMarker(listingsForMarker);
    return getCountLightstoneListings(listingsForMarker) > fatedListings.length;
}

function getSeeffDeals(listings) {

    return $.grep(listings, function (listing, index) {
        return listing.SeeffDeal && listing.SeeffDeal == true;
    });
}

function hasSeeffDeal(listings) {

    return getSeeffDeals(listings).length > 0;
}

function noSeeffDeals(listings) {

    return getSeeffDeals(listings).length == 0;
}

function getAllMarkersThatWillSpiderfy(listing) {

    var suburb = listing.Marker.Suburb;
    var allListingsWithSameLatLong = $.grep(suburb.Listings, function (lis, index) {
        return lis.LatLong.Lat == listing.LatLong.Lat && lis.LatLong.Lng == listing.LatLong.Lng;
    });

    if (allListingsWithSameLatLong.length > 0) {

        var markersForListings = [];
        $.each(allListingsWithSameLatLong, function (index, lis) {

            var marker = lis.Marker;
            if (marker) {
                if ($.inArray(marker, markersForListings) == -1) {
                    markersForListings.push(marker);
                }
            }
        });

        return markersForListings;
    }

    return [];
}

function getIconForMarker(marker) {

    var listingsForMarker = marker.Listings;
    if (listingsForMarker[0].IsCurrentSeeffListing) {
        var seeffListing = listingsForMarker[0];
        try {
            return buildPathToIconForCurrentSeeffData(seeffListing);
        }
        catch (e) {
            return 'Assets/unknown.png';
        }
    }
    else {
        // Order by reg date
        listingsForMarker = listingsForMarker.sort(function (x, y) {
            return new Date(formatDate(y.RegDate)) - new Date(formatDate(x.RegDate));
        });

        var isSpiderfied = listingsForMarker[0].Marker.MarkerIsSpiderfied;
        var allListingsForMarkerFated = !hasUnfatedListings(listingsForMarker);
        var listingsForMarkerContainSeeffDeal = hasSeeffDeal(listingsForMarker);

        try {
            var fullPathToIcon = buildPathToIconForLightstoneData(listingsForMarker);
            return fullPathToIcon;
        } catch (e) {
            return 'Assets/unknown.png';
        }
    }

    function buildPathToIconForCurrentSeeffData(seeffListing) {
        var path = 'Assets/marker_icons/seeff/';

        if (seeffListing.CurrentSeeffPropIsSS && !seeffListing.Marker.MarkerIsSpiderfied) {
            var markersThatWillSpiderfy = getAllMarkersThatWillSpiderfy(seeffListing);
            var hasRentals = $.grep(markersThatWillSpiderfy, function(marker) {
                return $.grep(marker.Listings, function(list) {
                    return list.IsCurrentSeeffListing && list.CurrentSeeffPropIsSS && list.CurrentSeeffSaleOrRent == 3;
                }).length > 0;
            }).length > 0;

            var hasSales = $.grep(markersThatWillSpiderfy, function (marker) {
                return $.grep(marker.Listings, function (list) {
                    return list.IsCurrentSeeffListing && list.CurrentSeeffPropIsSS && list.CurrentSeeffSaleOrRent == 2;
                }).length > 0;
            }).length > 0;

            var hasRentalsAndSales = hasRentals && hasSales;
            if (hasRentalsAndSales) {
                return path += "ss_rentandbuy.png";
            }
            if (hasRentals && !hasSales) {
                return path += "ss_rent.png";
            }
            if (hasSales && !hasRentals) {
                return path += "ss_buy.png";
            }
        }

        return path += getPathIdentifierForCurrentSeeffListing(seeffListing);
    }

    function buildPathToIconForLightstoneData(listingsForMarker) {
        var path = 'Assets/marker_icons/';

        // If the listings are part of a "SS" and they aren't spiderfied, then determine which icon to show based on 4 possibilities:
        // 1) GREEN "ss_unfated" icon: If any listing of any marker under this SS is unfated.
        // 2) RED "ss" icon: If there are NO seeff deals for any listings under this SS, OR the agency is unknown/unlisted.
        // 3) BLUE "SS" icon: If ALL markers under this SS contains at least ONE seeff transaction
        // 4) PINK "ss" icon: When there are a mixture of non-seeff and seeff deals under this SS.
        // All other listings will be FH and have the appropriate icon based on their market share type and year, except for listings 
        // of an SS that ARE spiderfied, which are always treated as FH residential for purposes of the icon selection.

        // An assumption is made here that if the first listing under a marker is a "SS", then they all are. 
        // At this stage this is the only way to test for an SS, however this should be tested more thoroughly at a later stage.
        // This section of code deals with UNSPIDERFIED SS icon retrieval
        if (listingsForMarker[0].SS_FH == "SS") {

            var ssAllListingsFatedForAllMarkers = allListingsForMarkerFated;
            var ssContainsAtLeastOneSeeffDeal = listingsForMarkerContainSeeffDeal;
            var ssContainsSeeffDealsForEveryMarker = listingsForMarkerContainSeeffDeal;

            var markersThatWillSpiderfyWithThisOne = getAllMarkersThatWillSpiderfy(listingsForMarker[0]);
            $.each(markersThatWillSpiderfyWithThisOne, function (idx, m) {
                // Check for fated/unfated listings
                if (hasUnfatedListings(m.Listings)) {

                    if (marker.TypeOfFating) {
                        if (marker.TypeOfFating != "fated") {
                            ssAllListingsFatedForAllMarkers = false;
                        }
                    }
                    else {
                        ssAllListingsFatedForAllMarkers = false;
                    }
                }

                // Check for any seeff deals contained in this SS
                var markerHasASeeffDeal = hasSeeffDeal(m.Listings);
                if (markerHasASeeffDeal) {
                    ssContainsAtLeastOneSeeffDeal = true;
                }

                // Update variable to determine if every marker contains *at least* one seeff deal
                ssContainsSeeffDealsForEveryMarker = ssContainsSeeffDealsForEveryMarker && markerHasASeeffDeal;
            });

            // Get the icon
            switch (ssAllListingsFatedForAllMarkers) {
                case true:
                    switch (ssContainsSeeffDealsForEveryMarker) {
                        case true:
                            return path += "seeff/house or apartment/ss.PNG"; // BLUE
                        case false:
                            {
                                if (ssContainsAtLeastOneSeeffDeal) {
                                    return path += "ss.png"; // PINK
                                }
                                return path += "competitor/house or apartment/ss.PNG"; // RED
                            }
                    }
                case false:
                    return path += 'unfated house or apartment/ss_unfated.PNG'; // UNFATED
            }
        }

        // Pending
        if (listingsForMarker[0].MarketShareType == 'P') {
            return path += 'pending.png';
        }

        // Any other FH types
        return path += getPathIdentifierForFH(listingsForMarker);
    }

    function getPathIdentifierForFH(listingsForMarker) {
        switch (allListingsForMarkerFated) {
            case true:
                return getIconPathIdentifierByAgency()
                      + getIconPathIdentifierByMarketShareType(listingsForMarker)
                        + getIconPathIdentifierByYear(listingsForMarker)
                        + ".png";
            case false:
                return 'unfated house or apartment/house_unfated.png';
        }
    }

    function getPathIdentifierForCurrentSeeffListing(seeffListing) {

        var pathIdentifier;
        switch (seeffListing.CurrentSeeffPropCategoryName) {
            case "Residential Property": pathIdentifier = "house or apartment/"; break;
            case "Commercial / Industrial": pathIdentifier = "commercial/"; break;
            case "Agricultural": pathIdentifier = "agricultural/"; break;
        }

        switch (seeffListing.CurrentSeeffSaleOrRent) {
            case 2: pathIdentifier += "current_buy.png"; break;
            case 3: pathIdentifier += "current_rent.png"; break;
        }

        return pathIdentifier;
    }

    function getIconPathIdentifierByAgency() {

        switch (listingsForMarkerContainSeeffDeal) {
            case true:
                return 'seeff/';
            case false:
                return 'competitor/';
        }
    }

    function getIconPathIdentifierByMarketShareType(listingsForMarker) {

        // NB.: In terms of the icon, the market share type is determined by the most recent transaction/listing, 
        // which *should* be the first item in the list
        switch (listingsForMarker[0].MarketShareType) {
            case 'R': return 'house or apartment/';
            case 'A': return 'agricultural/';
            case 'C': return 'commercial/';
            case 'D': return 'development/';
            case 'O': return 'other/';
        }
    }

    function getIconPathIdentifierByYear(listingsForMarker) {
        return formatDate(listingsForMarker[0].RegDate).substring(0, 4);
    }
}

function markerIsSelected() {
    return infowindow && infowindow.map && infowindow.Marker;
}

function getSuburbById(suburbId) {
    if (suburbsInfo) {
        var suburb = $.grep(suburbsInfo, function (s) {
            return s.SuburbId == suburbId;
        });

        if (suburb[0]) {
            return suburb[0];
        }
    }

    return null;
}

function enableMarkerClustering(suburb, visibleMarkers) {
    var mcOptions = { maxZoom: 15, minimumClusterSize: 1 };
    var markerClusterer = new MarkerClusterer(map, visibleMarkers, mcOptions);

    suburb.MarkerClusterer = markerClusterer;
}

function initialiseAndDisplaySuburb(suburb, data, typeOfFating, showSeeffCurrentListings, searching, showPopup) {

    if (data) {
        suburb.PolyCoords = data.PolyCoords;
        suburb.Listings = data.Listings;
        suburb.Name = data.LocationName;
        suburb.SuburbId = data.LocationID;
        suburb.IsInitialised = true;

        if (suburb.Listings) {
            $.each(suburb.Listings, function (i, val) {val.CanEdit = suburb.CanEdit;});
        }
    } 

    if (suburb.IsInitialised) {

        if (infowindow) {
            infowindow.Marker = null;
        }
        suburb.Visible = true;
        suburbsInfo.CanFilter = allVisibleSuburbsAreFated();        
        showFilteringOptions(suburbsInfo.CanFilter);

        updatePropertyInfoMenu([]);
        createMarkersForSuburb(suburb, typeOfFating, showSeeffCurrentListings);
        var visibleMarkers = suburbsInfo.CanFilter ? applyActiveFilters(suburb) : suburb.MarkersForListings;

        if (searching) {
            visibleMarkers = applySearch(visibleMarkers);
        }

        enableMarkerClustering(suburb, visibleMarkers);
        //enableSpiderfier(suburb, visibleMarkers);
        suburb.VisibleMarkers = visibleMarkers;

        // Supress showing dialog popups if we have used the searching parameter - TODO: change this to 'undefined'...
        if (showPopup) {
            if (searching !== true && searching !== false) {
                var userHasUnfatedTransactions = suburbHasUnfatedListings(suburb);
                if (userHasUnfatedTransactions) {
                    if (suburb.CanEdit) {
                        showPopupUnfatedTransactions("popupSuburbUnfated");
                    } else {
                        showPopupAdminRequired();
                    }
                } else {
                    // If this suburb has no unfated transactions BUT there are suburb(s) that are unfated, show a dialog
                    if (!suburbsInfo.CanFilter) {
                        showPopupAllListingsFatedForSuburb();
                    }
                }
            }
        }

        drawPolygonForSuburb(suburb);       
    }
}

function allVisibleSuburbsAreFated() {

    var visibleSuburbs = $.grep(suburbsInfo, function (suburb) { return suburb.Visible; });
    if (visibleSuburbs.length == 0) {
        return false;
    }

    for (var sub = 0; sub < visibleSuburbs.length; sub++) {
        var suburb = visibleSuburbs[sub];
        if (suburbHasUnfatedListings(suburb)) { return false; }
    }

    return true;
}

function centreMap(suburb) {

    if (!suburb.IsInitialised) {
        return;
    }

    var center = null;
    if (suburb.Listings && suburb.Listings.length > 0) {
        center = new google.maps.LatLng(suburb.Listings[0].LatLong.Lat, suburb.Listings[0].LatLong.Lng);
    }
    else {
        center = new google.maps.LatLng(suburb.PolyCoords[0].Lat, suburb.PolyCoords[0].Lng);
    }

    map.setCenter(center);
}

function clearSuburbBySuburbId(suburbId) {
    var suburb = getSuburbById(suburbId);

    if (suburb.IsInitialised) {
        // Close open info window if any
        closeInfoWindow();
        suburb.MarkerClusterer.clearMarkers();
        suburb.MarkersForListings.length = 0;
        suburb.VisibleMarkers = null;
        if (suburb.SeeffCurrentListings) {
            suburb.SeeffCurrentListings.length = 0;
        }
        suburb.Polygon.setMap(null);
        //suburb.Spiderfier.clearMarkers();
        suburb.Visible = false;
    }
}

function clearSuburb(suburb) {
    if (suburb.IsInitialised) {
        // Close open info window if any
        closeInfoWindow();
        suburb.MarkerClusterer.clearMarkers();
        suburb.MarkersForListings.length = 0;
        suburb.VisibleMarkers = null;
        if (suburb.SeeffCurrentListings) {
            suburb.SeeffCurrentListings.length = 0;
        }
        suburb.Polygon.setMap(null);
        //suburb.Spiderfier.clearMarkers();
        suburb.Visible = false;
    }
}

function enableSpiderfier(suburb, visibleMarkers) {
    var spiderfier = new OverlappingMarkerSpiderfier(map, { circleFootSeparation: 70, keepSpiderfied: true, circleSpiralSwitchover: 15, nearbyDistance: 1 });
    for (var i = 0; i < visibleMarkers.length; i++) {
        spiderfier.addMarker(visibleMarkers[i]);
    }

    spiderfier.addListener("spiderfy", function (markers) {
        closeInfoWindow();
        updatePropertyInfoMenu([]);

        $.each(markers, function (idx, m) {
            m.MarkerIsSpiderfied = true;
            m.setIcon(getIconForMarker(m));
        });
    });

    spiderfier.addListener('unspiderfy', function (markers) {
        $.each(markers, function (idx, m) {
            m.MarkerIsSpiderfied = false;
            m.setIcon(getIconForMarker(m));
        });
    });
    suburb.Spiderfier = spiderfier;
}

function getAgencyName(agencyId) {
    var agency = $.grep(allAgencies, function (value, index) {
        return value.agency_id == agencyId;
    });

    return agency.length > 0 ? agency[0].agency_name : "unknown";
}

function getAgencyId(agencyName) {
    var agency = $.grep(allAgencies, function (value, index) {
        return value.agency_name.toLowerCase() == agencyName.toLowerCase();
    });

    return agency.length > 0 ? agency[0].agency_id : -1;
}

function handleSuburbItemSelect() {
    var checkbox = $(this);
    var areaId = checkbox.attr("id").replace('unfated', '').replace('fated', '').replace('seeffcurrentlistings', '');
    loadDataForSuburb(areaId, false, true, true);
}

function getTypeOfFatingToLoad(areaId) {

    var fatedCheckbox = $('#fated' + areaId);
    var unfatedCheckbox = $('#unfated' + areaId);

    var fatedChecked = fatedCheckbox.is(":checked");
    var unfatedChecked = unfatedCheckbox.is(":checked");

    var typeOfFating = null;
    if (fatedChecked && unfatedChecked) {
        typeOfFating = "all";
    }
    else if (fatedChecked) {
        typeOfFating = "fated";
    }
    else if (unfatedChecked) {
        typeOfFating = "unfated";
    }

    return typeOfFating;
}

function mustLoadDataForSeeffCurrentListings(areaId) {

    var seeffCurrentListingsCheckbox = $('#seeffcurrentlistings' + areaId);
    var seeffCurrentListingsChecked = seeffCurrentListingsCheckbox.is(":checked");

    return seeffCurrentListingsChecked;
}

function loadDataForSuburb(areaId, loadEverything, generateStats, showPopup, callbackHandler) {

    // Get the status of the checked states for this particular row.
    // There are 3 possibilities: 1) Fated only, 2) Unfated only, 3) Both    
    var fatedChecked = true, unfatedChecked = true, seeffCurrentListingsChecked = true;

    var fatedCheckbox = $('#fated' + areaId);
    var unfatedCheckbox = $('#unfated' + areaId);
    var seeffCurrentListingsCheckbox = $('#seeffcurrentlistings' + areaId);
    if (loadEverything) {
        fatedCheckbox.off();
        unfatedCheckbox.off();
        seeffCurrentListingsCheckbox.off();

        fatedCheckbox.prop('checked', true);
        unfatedCheckbox.prop('checked', true);
        seeffCurrentListingsCheckbox.prop('checked', true);

        fatedCheckbox.on();
        unfatedCheckbox.on();
        seeffCurrentListingsCheckbox.on();
    }
    else {
        fatedChecked = fatedCheckbox.is(":checked");
        unfatedChecked = unfatedCheckbox.is(":checked");
        seeffCurrentListingsChecked = seeffCurrentListingsCheckbox.is(":checked");
    }

    // If a suburb that was previously checked is unchecked, unload it from the screen
    clearSuburbBySuburbId(areaId);

    if (generateStats) {
        generateStatisticsMenu(true);
    }

    var typeOfFating = getTypeOfFatingToLoad(areaId);
    var mustLoadSeeffCurrentListings = mustLoadDataForSeeffCurrentListings(areaId);

    // Load the suburb with given area id if any one of the checkboxes are selected.
    if (typeOfFating || mustLoadSeeffCurrentListings) {
        loadSuburb(areaId, typeOfFating, mustLoadSeeffCurrentListings, showPopup, callbackHandler);
    }
}

function handleFilterItemClick() {

    $.each(suburbsInfo, function (index, value) {
        var suburb = getSuburbById(value.SuburbId);
        if (suburb.Visible) {
            clearSuburb(suburb);
            var typeOfFating = getTypeOfFatingToLoad(suburb.SuburbId);
            var mustShowSeeffCurrentListings = mustLoadDataForSeeffCurrentListings(suburb.SuburbId);
            initialiseAndDisplaySuburb(suburb, null, typeOfFating, mustShowSeeffCurrentListings);
        }
    });

    generateStatisticsMenu(true);
}

function createMarkersForSuburb(suburb, typeOfFating, showSeeffCurrentListings) {

    var markersForListings = [];
    var uniquePropIds = [];
    for (var i = 0; i < suburb.Listings.length; i++) {

        // If this is a current listing and we must not show current listings, skip
        if (suburb.Listings[i].IsCurrentSeeffListing && !showSeeffCurrentListings) {
            continue;
        }

        // Create a new google maps marker from the lat/long of the listing
        var lat = suburb.Listings[i].LatLong.Lat;
        var lng = suburb.Listings[i].LatLong.Lng;
        var marker = new google.maps.Marker({ position: new google.maps.LatLng(lat, lng)});

        // If this is a current listing from seeff.com...
        if (suburb.Listings[i].IsCurrentSeeffListing) {

            marker.Suburb = suburb;
            suburb.Listings[i].Marker = marker;
            marker.Listings = [suburb.Listings[i]];
            google.maps.event.addListener(marker, 'click', markerClick);
            markersForListings.push(marker);
        }
        else { // Ordinary transaction from lightstone

            // This check is there to ensure that we only add ONE marker per property id
            var propertyId = suburb.Listings[i].PropertyId;
            if ($.inArray(propertyId, uniquePropIds) == -1) {

                marker.Listings = getAllListingsForMarker(suburb.Listings, propertyId);
                // Only add this marker to the list *if* it's relevant based on the user's fating selection
                if (canAddMarkerBasedOnFating(marker, typeOfFating)) {
                    $.each(marker.Listings, function (idx, val) { val.Marker = marker;});
                    marker.Suburb = suburb;
                    marker.TypeOfFating = typeOfFating;
                    google.maps.event.addListener(marker, 'click', markerClick);
                    markersForListings.push(marker);
                }

                uniquePropIds.push(propertyId);
            }
        }
    }

    $.each(markersForListings, function (idx, m) {m.setIcon(getIconForMarker(m));});
    suburb.MarkersForListings = markersForListings;
}

function canAddMarkerBasedOnFating(marker, typeOfFating) {

    switch (typeOfFating) {
        case "fated": return !hasUnfatedListings(marker.Listings);
        case "unfated": return hasUnfatedListings(marker.Listings);
        case "all": return true;
        default: return false;
    }
}

function getAllListingsForMarker(listings, propertyId) {
    var listingsWithPropertyId = $.grep(listings, function (value, index) {
        return value.PropertyId == propertyId;
    });

    // Order by reg date
    listingsWithPropertyId = listingsWithPropertyId.sort(function (x, y) {
        return new Date(formatDate(y.RegDate)) - new Date(formatDate(x.RegDate));
    });

    return listingsWithPropertyId;
}

function markerClick() {
    var marker = $(this)[0];

    // Determine here whether the marker being clicked is spiderfiable 
    // If so then do not display the prop info menu etc, its not relevant
    //if (markerNotSpiderfiable(marker)) {
    //}
    openInfoWindow(marker);
    if (marker.Listings[0].SS_FH == "FH" && markerIsSelected()) {
        if (!marker.Listings[0].IsCurrentSeeffListing) {
            updatePropertyInfoMenu(marker.Listings);
            showMenu("propertyinfo");
        }
    }
}

//function markerNotSpiderfiable(marker) {
//    var spiderfier = marker.Suburb.Spiderfier;

//    return spiderfier.markersNearMarker(marker, true).length == 0;
//}

function suburbHasUnfatedListings(suburb) {

    if (!suburb.IsInitialised) {
        return !suburb.Fated;
    }

    var unfated = $.grep(suburb.Listings, function (s) {

        if (s.IsCurrentSeeffListing) {
            return false; // Do not include current seeff listings in the calculation of unfated listings
        }

        return !s.Fated;
    });

    return unfated.length > 0;
}

function allListingsFated() {

    for (var s = 0; s < suburbsInfo.length; s++) {
        if (suburbHasUnfatedListings(suburbsInfo[s])) {
            return false;
        }
    }

    return true;
}

function drawPolygonForSuburb(suburb) {
    var coords = [];
    for (var j = 0; j < suburb.PolyCoords.length; j++) {
        var lat = suburb.PolyCoords[j].Lat;
        var lng = suburb.PolyCoords[j].Lng;
        coords.push(new google.maps.LatLng(lat, lng));
    }

    // The last coordinate set to add will be the starting point 
    coords.push(new google.maps.LatLng(suburb.PolyCoords[0].Lat, suburb.PolyCoords[0].Lng));
    var poly = new google.maps.Polygon({
        paths: coords,
        editable: false
    });

    poly.setMap(map);
    suburb.Polygon = poly;
    poly.Suburb = suburb;
}

function loadMarketShareTypes() {
    var marketShareTypes = ["Residential", "Commercial", "Agri", "Development", "Other", "Pending"];
    return marketShareTypes;
}

function closeInfoWindow() {
    if (infowindow) {
        infowindow.close();
        infowindow = null;
    }
}

function openInfoWindow(marker) {
    closeInfoWindow();
    updatePropertyInfoMenu([]);

    function createInfoWindowForMarker(marker) {
        var listing = marker.Listings[0];
        infowindow = new google.maps.InfoWindow();
        infowindow.setContent(buildContentForInfoWindow(listing));
        infowindow.setPosition(marker.getPosition());
        infowindow.open(map);
        infowindow.Marker = marker;

        google.maps.event.addDomListener(infowindow, 'closeclick', function () {
            infowindow.Marker = null;
            updatePropertyInfoMenu([]);
        });
    }

    var listing = marker.Listings[0];
    if (listing.PropertyAddress == 'n/a') {
        findAndSaveGoogleAddressForListing(listing, function () {
            createInfoWindowForMarker(marker);
            updatePropertyInfoMenu(marker.Listings);
            showMenu("propertyinfo");
        });
    }
    else {
        createInfoWindowForMarker(marker);
    }
}

function buildManageAgenciesScreen() {
    loadAgenciesForSuburb(showManageAgenciesDialog);
}

function getListingWithLatestRegDate(listings) {
    // For now return first record because the sorted order *should* be by latest date
    return listings[0];
}

function buildContentForInfoWindowForCurrentSeeffListing(listing) {

    function buildLinkToSeeffWebsite(listing) {
        var rentOrBuy = '';
        switch(listing.CurrentSeeffSaleOrRent) {
            case 2: rentOrBuy = "buy"; break;
            case 3: rentOrBuy = "rent"; break;
        }
        var propertyType = "";
        switch (listing.CurrentSeeffPropCategoryName) {
            case "Residential Property": propertyType = "residential"; break;
            case "Commercial / Industrial": propertyType = "commercial"; break;
            case "Agricultural": propertyType = "agricultural"; break;
        }

        var url = "http://www.seeff.com/" + rentOrBuy + "/" + propertyType + "/details.php?pref=" + listing.CurrentSeeffSearchReference;
        return url;
    }

    var linkToSeeffWebsite = buildLinkToSeeffWebsite(listing);
    function buildPathToImage(listing) {
        var imgUrl = "http://newimages1.seeff.com/images/" + listing.CurrentSeeffSearchReference + "/" + listing.CurrentSeeffSearchImage + "_213.jpg";
        var imgLink = "<a href='" + linkToSeeffWebsite + "' target='_blank'><img src='" + imgUrl + "' width='150' onerror=\"this.src='Assets/img_not_found.png';\" /></a>";
        return imgLink;
    }

    function buildCurrentListingDetails(listing) {
        var actionString = '';
        switch (listing.CurrentSeeffSaleOrRent) {
            case 2: actionString = "for sale"; break;
            case 3:
                actionString = "to rent (" + listing.CurrentSeeffSearchRentalTerm + ")";
                break;
        }
        var typeCatAction = listing.CurrentSeeffPropCategoryName + " " + listing.CurrentSeeffPropTypeName + " " + actionString;
        var price = formatRandValue(listing.CurrentSeeffSearchPrice);
        var area = listing.CurrentSeeffAreaName + ", " + listing.CurrentSeeffAreaParentName;
        var webRef = "<a href='" + linkToSeeffWebsite + "' target='_blank'>Search Ref: " + listing.CurrentSeeffSearchReference + "</a>";

        return "<label>" + typeCatAction + "</label> <br />" +
                   "<label>" + price + "</label> <br />" +
                   "<label>" + area + "</label> <br />" +
                   "<label>" + webRef + "</label> <br />";
    }

    var html = "<div style='overflow: hidden;line-height:1.35;white-space:nowrap;'>" +
                    "<div style='display:inline-block;float:left;'>" +
                           buildPathToImage(listing) +
                    "</div>" +
                    "<div style='display:inline-block;float:left;padding-left:10px;'>" +
                           buildCurrentListingDetails(listing) +
                    "</div>" +
                "</div>";

    return html;
}

function buildContentForInfoWindow(listing) {
    if (listing.IsCurrentSeeffListing) {
        return buildContentForInfoWindowForCurrentSeeffListing(listing);
    }
    else {
        if (listing.SS_FH == "SS") {
            // Get all markers that would previously have been spiderfied with this one:
            var markersForSS = getAllMarkersThatWillSpiderfy(listing);
            // Ensure markers are sorted by unit no
            markersForSS = markersForSS.sort(function (x, y) { return x.Listings[0].StreetOrUnitNo - y.Listings[0].StreetOrUnitNo; });
            var tableOfUnits = $("<table id='ssUnits' style='max-width:100%;width:250px;' />");
            tableOfUnits.empty();

            for (var i = 0; i < markersForSS.length; i++) {
                var unitContent = buildUnitContentRow(markersForSS[i]);
                tableOfUnits.append(unitContent);
            }

            var div = $("<div style='max-height:400px;overflow-x: hidden;' />");
            var ssName = listing.PropertyAddress && listing.PropertyAddress != 'n/a' ? listing.PropertyAddress.split(',')[0] : listing.PropertyAddress;
            div.append(ssName);
            div.append('<br />');
            div.append(tableOfUnits);
            return div[0].outerHTML;
        }
        else {
            function formatPropertyStreetOrUnitNo(streetOrUnitNo) {
                if (streetOrUnitNo == '0' || streetOrUnitNo == '-1') {
                    return "n/a";
                }

                return streetOrUnitNo;
            }

            function getPropertyTypeName(listing) {
                switch (listing.PropertyType) {
                    case "SS": return "Sectional Scheme";
                    case "FH": return "Free-hold";
                    case "FRM": return "Farm";
                    case "EST": return "Estate (" + listing.EstateName + ")";
                }
            }

            var SS_or_FH_erfsize_label = listing.SS_FH == "SS" ? "Unit size: " : "Erf size: ";
            return "<div id='" + listing.UniqueId + "' style='line-height:1.35;overflow:hidden;white-space:nowrap;'>" +
                       "<label>Property Address: </label>" + listing.PropertyAddress + "<br />" +
                       "<label>Street/Unit no.: </label>" + formatPropertyStreetOrUnitNo(listing.StreetOrUnitNo) + "<br />" +
                       "<label>Title deed no.: </label>" + listing.TitleDeedNo + "<br />" +
                       "<label>Erf no.: </label>" + listing.ErfNo + "<br />" +
                       "<label>" + SS_or_FH_erfsize_label + "</label>" + listing.ErfOrUnitSize + "<br />" +
                       "<label>Property ID: </label>" + listing.PropertyId + "<br />" +
                       "<label>Last seller: </label>" + formatDisplayString(listing.SellerName) + "<br />" +
                       "<label>Last buyer: </label>" + formatDisplayString(listing.BuyerName) + "<br />" +
                       "<label>Property type: </label>" + getPropertyTypeName(listing) +
                       "</div>";
        }
    }
}

function buildUnitContentRow(unit) {
    function getBGColourForRow(unit) {
        return 'white';
    }
    // Sort the listings in this unit by last reg date
    unit.Listings = unit.Listings.sort(function (x, y) {
        return new Date(formatDate(y.RegDate)) - new Date(formatDate(x.RegDate));
    });
    var id = "unit" + unit.Listings[0].PropertyId;
    var tr = $("<tr class='unitrow' />");
    var color = getBGColourForRow(unit);
    var bgcolor = "background-color:" + color;

    var unitContent = "Unit " + unit.Listings[0].StreetOrUnitNo + '<br />';
    unitContent += 'Last Reg Date: ' + formatDate(unit.Listings[0].RegDate) + '<br />';
    unitContent += 'Last purchase price: ' + formatRandValue(unit.Listings[0].PurchPrice) + '<br />';
    unitContent += 'Prop ID: ' + unit.Listings[0].PropertyId + '<br />';
    unitContent += 'Owner: ' + unit.Listings[0].BuyerName + '<br />';
    unitContent += 'Seller: ' + unit.Listings[0].SellerName;

    tr.append($("<td id='" + id + "' class='unittd' data-color='" + color + "' style='cursor:pointer;width:150px;text-align:left;" + bgcolor + "' />").append(unitContent));

    $('body').unbind('mouseover.' + id).on('mouseover.' + id, '#' + id, function () {
        var row = $('#' + id);
        //row.css('background-color', 'lightblue');
        if (row.data('color2') != 'lightblue') { row.css('background-color', 'lightyellow'); }
    });

    $('body').unbind('mouseout.' + id).on('mouseout.' + id, '#' + id, function () {
        var row = $('#' + id);
        var color = row.data('color');
        var color2 = row.data('color2');
        if (!color2) {
            // If we mouse out an already clicked row, do not change color
            row.css('background-color', color);
        }
    });

    $('body').unbind('click.' + id).on('click.' + id, '#' + id, function () {
        // Reset all color2 attributes of all other rows
        var row = $('#' + id);
        $('.unittd').not(row).each(function (idx, val) {
            var r = $(val);
            r.data('color2', '');
            var color = r.data('color');
            r.css('background-color', color);
        });
        row.data('color2', 'lightblue');
        row.css('background-color', 'lightblue');
        
        updatePropertyInfoMenu(unit.Listings);
        showMenu("propertyinfo");
    });

    return tr;
}

//function buildContentForInfoWindow(listing) {
//    if (listing.IsCurrentSeeffListing) {
//        return buildContentForInfoWindowForCurrentSeeffListing(listing);
//    }
//    else {
//        function formatPropertyStreetOrUnitNo(streetOrUnitNo) {
//            if (streetOrUnitNo == '0' || streetOrUnitNo == '-1') {
//                return "n/a";
//            }

//            return streetOrUnitNo;
//        }

//        function getPropertyTypeName(listing) {
//            switch (listing.PropertyType) {
//                case "SS": return "Sectional Scheme";
//                case "FH": return "Free-hold";
//                case "FRM": return "Farm";
//                case "EST": return "Estate (" + listing.EstateName + ")";
//            }
//        }

//        var SS_or_FH_erfsize_label = listing.SS_FH == "SS" ? "Unit size: " : "Erf size: ";
//        return "<div id='" + listing.UniqueId + "' style='line-height:1.35;overflow:hidden;white-space:nowrap;'>" +
//                   "<label>Property Address: </label>" + listing.PropertyAddress + "<br />" +
//                   "<label>Street/Unit no.: </label>" + formatPropertyStreetOrUnitNo(listing.StreetOrUnitNo) + "<br />" +
//                   "<label>Title deed no.: </label>" + listing.TitleDeedNo + "<br />" +
//                   "<label>Erf no.: </label>" + listing.ErfNo + "<br />" +
//                   "<label>" + SS_or_FH_erfsize_label + "</label>" + listing.ErfOrUnitSize + "<br />" +
//                   "<label>Property ID: </label>" + listing.PropertyId + "<br />" +
//                   "<label>Last seller: </label>" + formatDisplayString(listing.SellerName) + "<br />" +
//                   "<label>Last buyer: </label>" + formatDisplayString(listing.BuyerName) + "<br />" +
//                   "<label>Property type: </label>" + getPropertyTypeName(listing) +
//                   "</div>";
//    }
//}

function formatDate(date) {
    return date.substring(0, 4) + "-" + date.substring(4, 6) + "-" + date.substring(6, 8);
}

function formatRandValue(value) {
    if (value) {
        return "R " + value.toFixed(0).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
    }

    return "n / a";
}

function formatDisplayString(valueToDisplay) {
    return valueToDisplay != null ? valueToDisplay : "n/a";
}

function getSelectedValuesOfSelectBox(selectBoxId) {

    var selectedValues = [];
    $(selectBoxId + ' :selected').each(function () {
        selectedValues.push($(this).val());
    });

    return selectedValues;
}

function addValuesToSelectBox(selectedValues, selectBoxId, valueSelector) {

    var selectBox = $(selectBoxId);
    $.each(selectedValues, function (index, value) {
        selectBox.append("<option value='" + value + "'>" + valueSelector(value) + "</option>");
    });
}

function removeSelectedValuesFromSelectBox(selectBoxId) {
    $(selectBoxId + ' option:selected').each(function () {
        $(this).remove();
    });
}

////////////////////////////////////////////////////////////////////////////////////////////////
// ********************************************************************************************
// SECTION Database and server interactivity
// ********************************************************************************************
////////////////////////////////////////////////////////////////////////////////////////////////

function updateMarketShareForListing(uniqueId, selectedValue) {

    var listing = getListingByUniqueId(uniqueId);
    var agencyId = listing.Agency ? listing.Agency : -1;
    $.ajax({
        type: "POST",
        url: "ListingUpdater.ashx",
        data: JSON.stringify({ instruction: "update_market_share", uniqueid: uniqueId, marketsharetype: selectedValue, agencyid: agencyId }),
        success: function (data, textStatus, jqXHR) {
            if (textStatus == "success") {

                listing.MarketShareType = selectedValue && selectedValue != '' ? selectedValue.substring(0, 1) : null;
                listing.Fated = selectedValue && selectedValue != '' ? true : false;
                // Update the Ui 
                updateSuburbStats(listing.Marker.Suburb);
                if (listing.Marker) {
                    listing.Marker.setIcon(getIconForMarker(listing.Marker));
                }

                suburbsInfo.CanFilter = false;
                if (allListingsFated() || allVisibleSuburbsAreFated()) {
                    //showPopupAllListingsFated();
                    suburbsInfo.CanFilter = true;
                    showFilteringOptions(true);

                    //clearSuburb(listing.Marker.Suburb);
                    //var typeOfFating = getTypeOfFatingToLoad(listing.Marker.Suburb.SuburbId);
                    //var mustShowSeeffCurrentListings = mustLoadDataForSeeffCurrentListings(listing.Marker.Suburb.SuburbId);
                    //initialiseAndDisplaySuburb(listing.Marker.Suburb, null, typeOfFating, mustShowSeeffCurrentListings);
                }

                generateStatisticsMenu(true);
            } else {
                alert('Could not update listing at this time.');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(jqXHR.status);
            alert(jqXHR.responseText);
        },
        dataType: "json"
    });
}

function updateAgencyForListing(uniqueId, value) {

    var listing = getListingByUniqueId(uniqueId);
    $.ajax({
        type: "POST",
        url: "ListingUpdater.ashx",
        data: JSON.stringify({ instruction: "update_agency", uniqueid: uniqueId, agencyId: value, marketsharetype: listing.MarketShareType }),
        success: function (data, textStatus, jqXHR) {
            if (textStatus == "success") {

                listing.Agency = value;
                if (listing.Marker) {
                    listing.Marker.setIcon(getIconForMarker(listing.Marker));
                }
                generateStatisticsMenu(true);
            } else {
                alert('Could not update listing at this time.');
            }
        },
        dataType: "json"
    });
}

function loadSuburb(suburbId, typeOfFating, showSeeffCurrentListings, showPopup, callbackHandler) {

    var suburb = getSuburbById(suburbId);
    if (suburb == null) {
        suburb = newSuburb();
        suburb.SuburbId = suburbId;
    }
    if (!suburb.IsInitialised) {

        $.ajax({
            type: "POST",
            url: "SuburbDataManager.ashx",
            data: suburbId,
            success: function (data, textStatus, jqXHR) {
                if (textStatus == "success" && data.PolyCoords.length > 0) {
                    initialiseAndDisplaySuburb(suburb, data, typeOfFating, showSeeffCurrentListings, undefined, true);
                    centreMap(suburb);
                    generateStatisticsMenu(true);

                    if (callbackHandler) {
                        callbackHandler();
                    }
                } else {
                    alert('No data found for this area on the map. Please contact support.');
                }
            },
            error: function (textStatus, errorThrown) {
                alert(textStatus.responseText);
                alert(errorThrown);
            },
            dataType: "json"
        });
    } else {
        initialiseAndDisplaySuburb(suburb, null, typeOfFating, showSeeffCurrentListings, undefined, showPopup);
        generateStatisticsMenu(true);

        if (callbackHandler) {
            callbackHandler();
        }
    }
}

function loadAgenciesForSuburb(actionAfterLoading) {

    var currentSuburb = markerIsSelected() ? infowindow.Marker.Suburb : null;
    if (currentSuburb) {
        if (!currentSuburb.SelectedAgencies) {
            $.ajax({
                type: "POST",
                url: "AgenciesDataManager.ashx",
                data: JSON.stringify({ instruction: 'load', suburbID: currentSuburb.SuburbId, userGuid: $('#user_guid').attr('value') }),
                success: function (data, textStatus, jqXHR) {
                    if (textStatus == "success") {
                        currentSuburb.SelectedAgencies = data;
                        if (actionAfterLoading) {
                            actionAfterLoading();
                        }
                    } else {
                        alert('Could not load all agencies at this time.');
                    }
                },
                dataType: "json"
            });
        }
        else {
            if (actionAfterLoading) {
                actionAfterLoading();
            }
        }
    }
}


function saveAgencies(selectedAgencyIds, callbackFunction) {
    $.ajax({
        type: "POST",
        url: "AgenciesDataManager.ashx",
        data: JSON.stringify({
            instruction: 'save',
            userGuid: $('#user_guid').attr('value'),
            selectedAgencies: selectedAgencyIds
        }),
        dataType: "json"
    }).done(callbackFunction);
}

function updateListingLinkedParent(listing, propertyId) {
    $.ajax({
        type: "POST",
        url: "ListingUpdater.ashx",
        data: JSON.stringify({ instruction: 'update_parent_prop_id', uniqueid: listing.UniqueId, propertyid: propertyId }),
        success: function (data, textStatus, jqXHR) {
            if (textStatus == "success") {
                listing.ParentPropertyId = propertyId;
                updateCheckboxItemForLinkedListing(listing, propertyId);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.status + " - " + thrownError); },
        dataType: "json"
    });
}

function findAndSaveGoogleAddressForListing(listing, actionToExecuteAfterwards) {

    function getAddress(data) {
        if (data && data.results.length > 0) {
            var streetName = data.results[0].address_components[1].short_name + ', ' + data.results[0].address_components[2].short_name + ', ' + data.results[0].address_components[3].short_name + ' (from Google)';
            var streetOrUnitNo = data.results[0].address_components[0].short_name; 
            return { StreetName: streetName, StreetOrUnitNo: streetOrUnitNo };
        }

        return null;
    }

    function updatePropertyAddressForListing(listing, address) {
        if (address) {
            listing.PropertyAddress = address.StreetName;
            listing.StreetOrUnitNo = address.StreetOrUnitNo;
        }
    }

    var latLngPair = listing.LatLong.Lat + ',' + listing.LatLong.Lng;
    var url = 'https://maps.googleapis.com/maps/api/geocode/json?latlng=' + latLngPair + '&sensor=true&key=AIzaSyDWHlk3fmGm0oDsqVaoBM3_YocW5xPKtwA';
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json"
    }).done(function (data) {
        var address = getAddress(data);
        updatePropertyAddressForListing(listing, address);
        actionToExecuteAfterwards();

        if (address) {
            $.ajax({
                type: "POST",
                url: "ListingUpdater.ashx",
                data: JSON.stringify({ instruction: 'update_property_address', propertyid: listing.PropertyId, streetName: address.StreetName, streetOrUnitNo: address.StreetOrUnitNo }),
                dataType: "json"
            });
        }
    });
}

////////////////////////////////////////////////////////////////////////////////////////////////
// ********************************************************************************************
// SECTION Dialog popups
// ********************************************************************************************
////////////////////////////////////////////////////////////////////////////////////////////////

function showPopupAllListingsFated() {

    $("#popupAllListingsFated").dialog(
            {
                modal: true,
                closeOnEscape: true,
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                width: 'auto',
                buttons: { "Ok": function () { $(this).dialog("close"); } },
                position: ['right', 'center']
            });
}

function showPopupAllListingsFatedForSuburb() {

    $("#popupAllListingsFatedForSuburbButNotOthers").dialog(
            {
                modal: true,
                closeOnEscape: true,
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                width: 'auto',
                buttons: { "Ok": function () { $(this).dialog("close"); } },
                position: ['right', 'center']
            });
}

function showPopupUnfatedTransactions(popupId) {
    $("#" + popupId).dialog(
            {
                modal: true,
                closeOnEscape: true,
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                width: 'auto',
                buttons: { "Ok": function () { $(this).dialog("close"); } },
                position: ['right', 'center']
            });
}

function showPopupAdminRequired() {
    $("#popupAdminRequired").dialog(
            {
                modal: true,
                closeOnEscape: true,
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                width: 'auto',
                buttons: { "Ok": function () { $(this).dialog("close"); } },
                position: ['right', 'center']
            });
}

function showManageAgenciesDialog() {

    var defaultExclusions = ['Seeff', 'Private sale', 'Auction'];

    function getAgenciesFromIds(arrayOfIds) {
        return $.grep(allAgencies, function (agency) {
            return $.inArray(agency.agency_id, arrayOfIds) != -1;
        });
    }

    function getAgenciesExcluding(agencies, agenciesToExlude) {

        var results = $.merge([], agencies);
        results = $.grep(results, function (agency) {
            return $.inArray(agency.agency_name, defaultExclusions) == -1;
        });

        if (agenciesToExlude.length == 0) {
            return results;
        }
        return $.grep(results, function (agency1) {
            return $.grep(agenciesToExlude, function (agency2) {
                return agency1.agency_id == agency2.agency_id;
            }).length == 0;
        });
    }

    var currentSuburb = markerIsSelected() ? infowindow.Marker.Suburb : null;
    if (currentSuburb) {
        $('#closepanelbutton').trigger('click');
        $('#agenciesMultiSelect').remove();

        var agenciesForCurrentSuburb = getAgenciesFromIds(currentSuburb.SelectedAgencies);
        var agenciesLeftPanel = getAgenciesExcluding(allAgencies, agenciesForCurrentSuburb);
        var agenciesRightPanel = getAgenciesExcluding(agenciesForCurrentSuburb, []);
        var selector = new MultiSelectWidget("agenciesMultiSelect", "Agency Selector", "All Agencies", "Selected Agencies", agenciesLeftPanel, agenciesRightPanel,
                                      function (item) { return item.agency_id; },
                                       function (item) { return item.agency_name; },
                                       function (leftSelection, rightSelection) {
                                           $("body").css("cursor", "progress");
                                           var agencyIds = $.map(rightSelection, function (agencyObject) {
                                               return agencyObject.agency_id;
                                           });
                                           saveAgencies(agencyIds, function (response) { // rem to exclude defaults
                                               $("body").css("cursor", "default");
                                               if (response.Saved) {
                                                   showAgencySavedScreen();
                                                   for (var s = 0; s < suburbsInfo.length; s++) {
                                                       suburbsInfo[s].SelectedAgencies = agencyIds;
                                                   }
                                                   
                                                   updatePropertyInfoMenu(infowindow.Marker.Listings);
                                               } else {
                                                   showDialogRemoveAgencyWarning();
                                               }
                                           });
                                       });

        var html = selector.buildDialogElement();
        $('body').append(html);
        selector.show();
    }

    function showAgencySavedScreen() {
        $('#agenciesSavedSplashScreen').dialog({
            show: 'fade',
            position: ['center', 'center'],
            hide: { effect: "fadeOut", duration: 500 },
            open: function (event, ui) {
                $('#agenciesSavedSplashScreen').siblings(".ui-dialog-titlebar").hide();
                setTimeout(function () {
                    $('#agenciesSavedSplashScreen').dialog('close');
                }, 1000);
            }
        });
    }

    // In order to save the selected agencies for the suburb in question, one may not remove agencies that are currently listed against one or more listings.
    function canSaveSelectedAgencies(selectedAgenciesToSave) {        
        var canSave = true;
        if (currentSuburb.Listings && currentSuburb.Listings.length > 0) {
            for (var index=0;index <currentSuburb.Listings.length;index++) {
                var agencyForListing = currentSuburb.Listings[index].Agency;
                if (agencyForListing && agencyForListing > 0) {

                    // Check also the default exclsuions, these don't count.
                    if ($.inArray(getAgencyName(agencyForListing), defaultExclusions) == -1) {
                        // the agencyForListing must exist in the selectedAgenciesToSave for it to be valid
                        var itemExists = $.grep(selectedAgenciesToSave, function (selectedAgency) { return selectedAgency.agency_id == agencyForListing; }).length > 0;
                        if (!itemExists) {
                            canSave = false;
                        }
                    }
                }
            }
        }

        return canSave;
    }

    function showDialogRemoveAgencyWarning() {
        $("#warningAgencyAlreadyOnListing").dialog(
                {
                    modal: true,
                    open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                    width: 'auto',
                    buttons: { "Ok": function () { $(this).dialog("close"); } },
                    position: ['center', 'center']
                });
    }
}