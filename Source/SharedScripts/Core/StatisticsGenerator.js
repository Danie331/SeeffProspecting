
function buildListingsByAgencyStatistics() {
    
    var statistics = buildAgencyStats(function () { return 1; });

    // Ensure that the "Unlisted" are the last element in array
    var unlistedIndex = -1;
    var unlisted = $.grep(statistics, function (stat, index) {
        if (stat.AgencyName == "Unlisted") {
            unlistedIndex = index;
            return true;
        }

        return false;
    });

    if (unlistedIndex > -1 && (unlistedIndex != statistics.length-1)) {
        var tmp = statistics[unlistedIndex];
        statistics[unlistedIndex] = statistics[statistics.length - 1];
        statistics[statistics.length - 1] = tmp;
    }

    return sort(statistics);
}

function getTotalMarketValueByAgency() {

    var totVal = 0;
    var statistics = buildAgencyStats(function (listing) { return listing.PurchPrice; });

    $.each(statistics, function (index, stat) {
        totVal += stat.Value;
    });

    return totVal;
}

function buildListingsByMarketShareTypes() {

    var statistics = [];
    for (var idx = 0; idx < suburbsInfo.length; idx++) {
        var suburb = suburbsInfo[idx];

        if (suburb.VisibleMarkers && suburb.VisibleMarkers.length > 0) {

            $.each(suburb.VisibleMarkers, function (index, marker) {

                // At this stage the suburb.VisibleMarkers contains markers with *any* listings that meet the 
                // currently selected filter criteria. For purposes of this method, we should determine 
                // only the listings that are relevant to the current filter and process only those. 
                var relevantListings = getRelevantListingsForPropFilter(marker.Listings);

                $.each(relevantListings, function (index, listing) {

                    var msType = listing.MarketShareType;
                    if (!msType) {
                        msType = "Unfated";
                    }

                    if (listing.IsCurrentSeeffListing) {
                        msType = 'Seeff on market';
                    }

                    switch (msType) {
                        case 'R':
                            msType = 'Residential';
                            break;
                        case 'C':
                            msType = 'Commercial';
                            break;
                        case 'A':
                            msType = 'Agri';
                            break;
                        case 'D':
                            msType = 'Development';
                            break;
                        case 'O':
                            msType = 'Other';
                            break;
                        case 'P':
                            msType = 'Pending';
                            break;
                    }
                    createOrUpdateStatistic(statistics, msType, 1, function (stat) { return stat.PropertyType;}, createPropertyTypeStatistic);
                });
            });
        }
    }

    return sort(statistics);
}

function getRelevantListingsForPropFilter(listings) {

    var selectedMarketShareTypes = [];
    if (menu.find('#R_filter').is(':checked')) {
        selectedMarketShareTypes.push('R');
    }

    if (menu.find('#C_filter').is(':checked')) {
        selectedMarketShareTypes.push('C');
    }

    if (menu.find('#A_filter').is(':checked')) {
        selectedMarketShareTypes.push('A');
    }

    if (menu.find('#D_filter').is(':checked')) {
        selectedMarketShareTypes.push('D');
    }

    if (menu.find('#O_filter').is(':checked')) {
        selectedMarketShareTypes.push('O');
    }

    if (menu.find('#P_filter').is(':checked')) {
        selectedMarketShareTypes.push('P');
    }

    var selectedYears = [];
    //if (menu.find("#2012_filter").is(':checked')) {
    //    selectedYears.push('2012');
    //}
    //if (menu.find("#2013_filter").is(':checked')) {
    //    selectedYears.push('2013');
    //}
    if (menu.find("#2014_filter").is(':checked')) {
        selectedYears.push('2014');
    }
    if (menu.find("#2015_filter").is(':checked')) {
        selectedYears.push('2015');
    }
    if (menu.find("#2016_filter").is(':checked')) {
        selectedYears.push('2016');
    }
    if (menu.find("#2017_filter").is(':checked')) {
        selectedYears.push('2017');
    }

    return $.grep(listings, function (listing, idx) {
        var isRelevant = $.inArray(listing.MarketShareType, selectedMarketShareTypes) > -1 || listing.IsCurrentSeeffListing == true
                                                || !listing.MarketShareType || listing.MarketShareType == null;
        if (listing.RegDate) {
            var yearPortion = listing.RegDate.substring(0, 4);
            isRelevant = isRelevant && $.inArray(yearPortion, selectedYears) > -1;
        }
        return isRelevant;
    });
}

function buildTotalValueStatistics() {

    var statistics = buildAgencyStats(function (listing) { return listing.PurchPrice; });

    // Ensure that the "Unlisted" are the last element in array
    var unlistedIndex = -1;
    var unlisted = $.grep(statistics, function (stat, index) {
        if (stat.AgencyName == "Unlisted") {
            unlistedIndex = index;
            return true;
        }

        return false;
    });

    if (unlistedIndex > -1 && (unlistedIndex != statistics.length - 1)) {
        var tmp = statistics[unlistedIndex];
        statistics[unlistedIndex] = statistics[statistics.length - 1];
        statistics[statistics.length - 1] = tmp;
    }

    return sort(statistics);
}

function sort(stats) {

    stats.sort(function (x, y) {
        return y.Value - x.Value;
    });

    return stats;
}

function getTotalVisibleListings(residentialOnly) {

    var totalFilteredListings = 0;
    for (var idx = 0; idx < suburbsInfo.length; idx++) {
        var suburb = suburbsInfo[idx];
        if (suburb.VisibleMarkers && suburb.VisibleMarkers.length > 0) {
            $.each(suburb.VisibleMarkers, function (index, marker) {
                var relevantListings = getRelevantListingsForPropFilter(marker.Listings);
                $.each(relevantListings, function (idxm, listing) {

                    if (residentialOnly) {
                        // Only include the listing IFF it's a residential listing AND it is not a child property (ie not linked to a parent)
                        if (listing.MarketShareType == 'R' && listing.ParentPropertyId == null) {
                            totalFilteredListings++;
                        } 
                    }
                    else {
                        totalFilteredListings++;
                    }
                });
            });
        }
    }

    return totalFilteredListings;
}

function buildAgencyStats(valueSelector) {

    var statistics = [];
    for (var idx = 0; idx < suburbsInfo.length; idx++) {
        var suburb = suburbsInfo[idx];

        if (suburb.VisibleMarkers && suburb.VisibleMarkers.length > 0) {

            $.each(suburb.VisibleMarkers, function (index, marker) {

                // At this stage the suburb.VisibleMarkers contains markers with *any* listings that meet the 
                // currently selected filter criteria. For purposes of this method, we should determine 
                // only the listings that are relevant to the current filter and process only those. 
                var relevantListings = getRelevantListingsForPropFilter(marker.Listings);
                $.each(relevantListings, function (index, listing) {
                    // Only include the listing IFF it's a residential listing AND it is not a child property (ie not linked to a parent)
                    if (listing.MarketShareType == 'R' && listing.ParentPropertyId == null) {
                        // Try to find an agency with this name in the list
                        var agencyName = getAgencyName(listing.Agency);
                        if (!listing.Agency || listing.Agency == -1) {
                            agencyName = "Unlisted";
                        }
                        createOrUpdateStatistic(statistics,
                                            agencyName,
                                            valueSelector(listing),
                                            function (stat) { return stat.AgencyName; },
                                            createAgencyStatistic);
                    }
                });
            });
        }
    }

    return statistics;
}

function createOrUpdateStatistic(statistics, name, value, keySelector, statBuilder) {

    var existingStat = $.grep(statistics, function (stat, index) {
        return keySelector(stat) == name;
    });

    if (existingStat.length > 0) {
        // If stat exists then update it
        existingStat[0].Value += value;
    }
    else {
        // Create and add it
        var statistic = statBuilder(name, value);
        statistics.push(statistic);
    }
}