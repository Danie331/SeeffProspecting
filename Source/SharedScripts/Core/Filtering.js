// Filtering

var filterSets = [];

var monthLookup = {};
monthLookup["jan"] = "01";
monthLookup["feb"] = "02";
monthLookup["mar"] = "03";
monthLookup["apr"] = "04";
monthLookup["may"] = "05";
monthLookup["jun"] = "06";
monthLookup["jul"] = "07";
monthLookup["aug"] = "08";
monthLookup["sep"] = "09";
monthLookup["oct"] = "10";
monthLookup["nov"] = "11";
monthLookup["dec"] = "12";

var yearLookup = [];

function applyActiveFilters(suburb) {

    closeInfoWindow();

    var filteredResults = applyDefaultFilters(suburb.MarkersForListings);
    var filteredCurrentListings = applyFiltersSeeffCurrentListings(suburb.MarkersForListings);
    var finalSet = $.merge(filteredResults, filteredCurrentListings);

    return removeDuplicates(finalSet);
}

function isDefaultFilterSet(filterSet) {
    return $.inArray("forsale_filter", filterSet) == -1;
}

function applyDefaultFilters(markersForListings) {
    yearLookup = []; // reset the selection of years
    var results = markersForListings;
    for (var fs = 0; fs < filterSets.length; fs++) {
        if (isDefaultFilterSet(filterSets[fs])) {
            results = applyFilterSet(results, filterSets[fs]);
        }
    }

    results = filterByPrice(results);

    return results;
}

function applyFiltersSeeffCurrentListings(markers) {
    
    var markersCurrentListings = $.grep(markers, function (m) {
        return $.grep(m.Listings, function (list) {
            return list.IsCurrentSeeffListing == true;
        }).length > 0;
    });

    var forSaleChecked = $('#forsale_filter').is(':checked');
    var forRentChecked = $('#forrent_filter').is(':checked');
    return $.grep(markersCurrentListings, function (m) {
        return $.grep(m.Listings, function (list) {            
            return (list.CurrentSeeffSaleOrRent == 2 && forSaleChecked == true) ||
                    (list.CurrentSeeffSaleOrRent == 3 && forRentChecked == true);
        }).length > 0;
    });
}

function applyFilterSet(results, filterSet) {
    var filteredResults = [];

    for (var fsi = 0; fsi < filterSet.length; fsi++) {
        var filter = filterSet[fsi];
        var isChecked = menu.find('#' + filter).is(':checked');

        filteredResults = isChecked ? filteredResults.concat(applyFilter(results, filter, isChecked))
            : filteredResults.concat(applyFilter(filteredResults, filter, isChecked));
    }

    return removeDuplicates(filteredResults);
}

function removeDuplicates(list) {
        
    var uniqueList = [];
    for (var i = 0; i < list.length; i++) {

        if ($.inArray(list[i], uniqueList) == -1) {
            uniqueList.push(list[i]);
        }
    }

    return uniqueList;
}

function applyFilter(markersToFilter, filter, isChecked) {
    filter = filter.replace(/_filter/g, "");
    switch (filter) {
        case "FH":
        case "SS":
            return filterSSFH(markersToFilter, filter, isChecked);
        case "R":
        case "C":
        case "A":
        case "D":
        case "O":
        case "P":
            return filterMarketShareType(markersToFilter, filter, isChecked);
        //case "2011":
        case "2012":
        case "2013":
        case "2014":
        case "2015":
            return filterYear(markersToFilter, filter, isChecked);
        case "Seeff":
        case "PGP":
        case "Dogan":
        case "OtherAgent":
            return filterAgency(markersToFilter, filter, isChecked);
        case "withagencyassigned":
            return filterAgencyAssigned(markersToFilter, filter, isChecked);
        case "withoutagencyassigned":
            return filterAgencyNotAssigned(markersToFilter, filter, isChecked);
        case "jan":
        case "feb":
        case "mar":
        case "apr":
        case "may":
        case "jun":
        case "jul":
        case "aug":
        case "sep":
        case "oct":
        case "nov":
        case "dec":
            return filterByMonth(markersToFilter, filter, isChecked);
        default: break;
    }
}

function filterByPrice(markersToFilter) {
    var fromPrice = $('#priceFrom_filter').val().trim();
    var toPrice = $('#priceTo_filter').val().trim();

    if (fromPrice == '' && toPrice == '') {
        return markersToFilter;
    }
    if (!isNaN(fromPrice) && toPrice == '') {
        toPrice = 1000000000;
    }
    if (fromPrice == '' && !isNaN(toPrice)) {
        fromPrice = 0;
    }

    if (!isNaN(fromPrice) && !isNaN(toPrice)) {

        function matchPriceRange(listings) {
            var matchingListings = $.grep(listings, function (listing) {
                var salePrice = listing.PurchPrice;
                if (salePrice == null) {
                    return false;
                }
                return salePrice >= fromPrice && salePrice <= toPrice;
            });

            return matchingListings.length > 0;
        }

        var filtered = $.grep(markersToFilter, function (m) {
            return matchPriceRange(m.Listings);
        });

        return filtered;
    } else {
        return markersToFilter;
    }
}

function filterByMonth(markersToFilter, filter, isChecked) {
    
    function anyListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {
            var monthPart = listing.RegDate.substring(4, 6);
            var yearPart = listing.RegDate.substring(0, 4);
            return monthLookup[filter] == monthPart && $.inArray(yearPart, yearLookup) > -1;
        });

        return matchingListings.length > 0;
    }

    function noListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {
            var monthPart = listing.RegDate.substring(4, 6);
            var yearPart = listing.RegDate.substring(0, 4);
            return monthLookup[filter] != monthPart || $.inArray(yearPart, yearLookup) == -1;
        });

        return matchingListings.length == listings.length;
    }
    
    var filtered = $.grep(markersToFilter, function (m) {
        return isChecked ? anyListingsMeetFilterRequirement(m.Listings) : noListingsMeetFilterRequirement(m.Listings);
    });

    return filtered;
}

function isCurrentSeeffListing(listing) {
    return listing.IsCurrentSeeffListing == true;
}

function filterAgencyAssigned(markersToFilter, filter, isChecked) {

    function anyListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {
            return listing.Agency != null && listing.Agency > -1;
        });

        return matchingListings.length > 0;
    }

    function noListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {
            return listing.Agency == null || listing.Agency == -1;
        });

        return matchingListings.length == listings.length;
    }

    var filtered = $.grep(markersToFilter, function (m) {
        return isChecked ? anyListingsMeetFilterRequirement(m.Listings) : noListingsMeetFilterRequirement(m.Listings);
    });

    return filtered;
}

function filterAgencyNotAssigned(markersToFilter, filter, isChecked) {

    function anyListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {
            return listing.Agency == null || listing.Agency == -1;
        });

        return matchingListings.length > 0;
    }

    function noListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {
            return listing.Agency != null && listing.Agency > -1;
        });

        return matchingListings.length == listings.length;
    }

    var filtered = $.grep(markersToFilter, function (m) {
        return isChecked ? anyListingsMeetFilterRequirement(m.Listings) : noListingsMeetFilterRequirement(m.Listings);
    });

    return filtered;
}

function filterSSFH(markersToFilter, filter, isChecked) {

    function anyListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {
            return listing.SS_FH == filter;
        });

        return matchingListings.length > 0;
    }

    function noListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {
            return listing.SS_FH != filter;
        });

        return matchingListings.length == listings.length;
    }

    var filtered = $.grep(markersToFilter, function (m) {
        return isChecked ? anyListingsMeetFilterRequirement(m.Listings) : noListingsMeetFilterRequirement(m.Listings);
    });

    return filtered;
}

function filterMarketShareType(markersToFilter, filter, isChecked) {

    function anyListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {          
            return listing.MarketShareType == filter || listing.MarketShareType == null || listing.MarketShareType == 'undefined';
        });

        return matchingListings.length > 0;
    }

    function noListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {          
            return listing.MarketShareType != filter;
        });

        return matchingListings.length == listings.length;
    }

    var filtered = $.grep(markersToFilter, function (m) {
             
        return isChecked ? anyListingsMeetFilterRequirement(m.Listings) : noListingsMeetFilterRequirement(m.Listings);
    });

    return filtered;
}

function filterYear(markersToFilter, filter, isChecked) {

    function anyListingsMeetFilterRequirement(listings) {
        yearLookup.push(filter);
        var matchingListings = $.grep(listings, function (listing) {       
            var yearPart = listing.RegDate.substring(0, 4);
            return yearPart == filter;
        });

        return matchingListings.length > 0;
    }

    function noListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {          
            var yearPart = listing.RegDate.substring(0, 4);
            return yearPart != filter;
        });

        return matchingListings.length == listings.length;
    }

    var filtered = $.grep(markersToFilter, function (m) {        
        return isChecked ? anyListingsMeetFilterRequirement(m.Listings) : noListingsMeetFilterRequirement(m.Listings);
    });

    return filtered;
}

function filterAgency(markersToFilter, filter, isChecked) {

    function anyListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {      
            var listingAgency = getAgencyName(listing.Agency);
            if (filter == "OtherAgent" && $.inArray(listingAgency, ["Seeff", "Dogan", "PGP"]) == -1) {
                return true;
            }
            return listingAgency.toLowerCase() == filter.toLowerCase();
        });

        return matchingListings.length > 0;
    }

    function noListingsMeetFilterRequirement(listings) {
        var matchingListings = $.grep(listings, function (listing) {        
            var listingAgency = getAgencyName(listing.Agency);
            return listingAgency != filter;
        });

        return matchingListings.length == listings.length;
    }

    var filtered = $.grep(markersToFilter, function (m) {

        return isChecked ? anyListingsMeetFilterRequirement(m.Listings) : noListingsMeetFilterRequirement(m.Listings);
    });

    return filtered;
}