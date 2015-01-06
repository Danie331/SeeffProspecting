
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Defines the searching functionality available on the "Search" tab
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

function initSearch() {
    
    // Validate that we have some data to search on..
    if (canSearch()) {
        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Searching</p>' });
        setTimeout(reloadSuburbsWithSearchOption,500);
    }
}

function performResetSearch() {

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Restoring suburbs</p>' });
    setTimeout(resetSearchFieldsAndReloadSuburbs,500);
}

function reloadSuburbsWithSearchOption() {

    for (var i = 0; i < suburbsInfo.length; i++) {
        var suburb = suburbsInfo[i];
        if (suburb.Visible) {
            clearSuburb(suburb);
            var typeOfFating = getTypeOfFatingToLoad(suburb.SuburbId);
            var mustShowSeeffCurrentListings = mustLoadDataForSeeffCurrentListings(suburb.SuburbId);
            initialiseAndDisplaySuburb(suburb, null, typeOfFating, mustShowSeeffCurrentListings, true);
        }
    }

    $.unblockUI(); // This should always be the last statement in this function.
}


function canSearch() {

    if ($('#searchStreetOrSSName').val() != '') return true;
    if ($('#searchStreetOrUnitNo').val() != '') return true;
    if ($('#searchSellerName').val() != '') return true;
    if ($('#searchBuyerName').val() != '') return true;
    if ($('#searchTitleDeed').val() != '') return true;
    if ($('#searchErfNo').val() != '') return true;
    if ($('#searchPropId').val() != '') return true;

    return false;
}

function resetSearchFieldsAndReloadSuburbs() {
    // Clear search fields
    $('#searchStreetOrSSName').val('');
    $('#searchStreetOrUnitNo').val('');
    $('#searchSellerName').val('');
    $('#searchBuyerName').val('');
    $('#searchTitleDeed').val('');
    $('#searchErfNo').val('');
    $('#searchPropId').val('');

    // Reload all suburbs - restore to previous state prior to search
    for (var i = 0; i < suburbsInfo.length; i++) {
        var suburb = suburbsInfo[i];
        if (suburb.Visible) {
            clearSuburb(suburb);
            var typeOfFating = getTypeOfFatingToLoad(suburb.SuburbId);
            var mustShowSeeffCurrentListings = mustLoadDataForSeeffCurrentListings(suburb.SuburbId);
            initialiseAndDisplaySuburb(suburb, null, typeOfFating, mustShowSeeffCurrentListings, false);
        }
    }

    $.unblockUI(); // This should always be the last statement in this function.
}

// For now this does an "inclusive" type search meaning that *any* matching data for any of the search fields
// is brought back. 
function applySearch(visibleMarkers) {

    var netResults = [];

    var streetOrSSName = $('#searchStreetOrSSName').val();
    if (streetOrSSName != '') {
        netResults = netResults.concat(searchByStreetOrSSName(streetOrSSName, visibleMarkers));
    }

    var streetOrUnitNo = $('#searchStreetOrUnitNo').val();
    if (streetOrUnitNo != '') {
        netResults = netResults.concat(searchByStreetOrUnitNo(streetOrUnitNo, visibleMarkers));
    }

    var sellerName = $('#searchSellerName').val();
    if (sellerName != '') {
        netResults = netResults.concat(searchBySearchSellerName(sellerName, visibleMarkers));
    }

    var buyerName = $('#searchBuyerName').val();
    if (buyerName != '') {
        netResults = netResults.concat(searchByBuyerName(buyerName, visibleMarkers));
    }

    var titleDeed = $('#searchTitleDeed').val();
    if (titleDeed != '') {
        netResults = netResults.concat(searchByTitleDeed(titleDeed, visibleMarkers));
    }

    var erfNo = $('#searchErfNo').val();
    if (erfNo != '') {
        netResults = netResults.concat(searchByErfNo(erfNo, visibleMarkers));
    }

    var propId = $('#searchPropId').val();
    if (propId != '') {
        netResults = netResults.concat(searchByPropId(propId, visibleMarkers));
    }

    return removeDuplicates(netResults);
}

function searchByStreetOrSSName(value, visibleMarkers) {

    var results = [];
    for (var i = 0; i < visibleMarkers.length; i++) {
        var marker = visibleMarkers[i];
        for (var j =0;j<marker.Listings.length;j++) {
            var listing = marker.Listings[j];
            if (listing.PropertyAddress) {
                var propAddress = listing.PropertyAddress.toLowerCase();
                value = value.toLowerCase();
                if (propAddress.indexOf(value) !== -1) {
                    results.push(marker);
                    break;
                }
            }
        }
    }

    return results;
}

function searchByStreetOrUnitNo(value, visibleMarkers) {
    var results = [];
    for (var i = 0; i < visibleMarkers.length; i++) {
        var marker = visibleMarkers[i];
        for (var j = 0; j < marker.Listings.length; j++) {
            var listing = marker.Listings[j];
            if (listing.StreetOrUnitNo == value) {
                results.push(marker);
                break;
            }
        }
    }

    return results;
}

function searchBySearchSellerName(value, visibleMarkers) {
    var results = [];
    for (var i = 0; i < visibleMarkers.length; i++) {
        var marker = visibleMarkers[i];
        for (var j = 0; j < marker.Listings.length; j++) {
            var listing = marker.Listings[j];
            if (listing.SellerName) {
                var seller = listing.SellerName.toLowerCase();
                value = value.toLowerCase();
                if (seller.indexOf(value) !== -1) {
                    results.push(marker);
                    break;
                }
            }
        }
    }

    return results;
}

function searchByBuyerName(value, visibleMarkers) {
    var results = [];
    for (var i = 0; i < visibleMarkers.length; i++) {
        var marker = visibleMarkers[i];
        for (var j = 0; j < marker.Listings.length; j++) {
            var listing = marker.Listings[j];
            if (listing.BuyerName) {
                var buyer = listing.BuyerName.toLowerCase();
                value = value.toLowerCase();
                if (buyer.indexOf(value) !== -1) {
                    results.push(marker);
                    break;
                }
            }
        }
    }

    return results;
}

function searchByTitleDeed(value, visibleMarkers) {
    var results = [];
    for (var i = 0; i < visibleMarkers.length; i++) {
        var marker = visibleMarkers[i];
        for (var j = 0; j < marker.Listings.length; j++) {
            var listing = marker.Listings[j];
            if (listing.TitleDeedNo == value) {
                results.push(marker);
                break;
            }
        }
    }

    return results;
}

function searchByErfNo(value, visibleMarkers) {
    var results = [];
    for (var i = 0; i < visibleMarkers.length; i++) {
        var marker = visibleMarkers[i];
        for (var j = 0; j < marker.Listings.length; j++) {
            var listing = marker.Listings[j];
            if (listing.ErfNo == value) {
                results.push(marker);
                break;
            }
        }
    }

    return results;
}

function searchByPropId(value, visibleMarkers) {
    var results = [];
    for (var i = 0; i < visibleMarkers.length; i++) {
        var marker = visibleMarkers[i];
        for (var j = 0; j < marker.Listings.length; j++) {
            var listing = marker.Listings[j];
            if (listing.PropertyId == value) {
                results.push(marker);
                break;
            }
        }
    }

    return results;
}