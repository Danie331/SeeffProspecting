
var app = app || {};

app.buildPropertyListingMenu = function () {
    var container = $("<div id='propertyListingContainer' class='contentdiv' style='display:none' />");
    container.empty();
    return container;   
}

app.formatAddress = function () {
    if (currentProperty.SS_FH == "FH") {
        return `${currentProperty.StreetOrUnitNo} ${currentProperty.PropertyAddress}`;
    }
    if (currentProperty.SS_FH == "SS") {
        return `Unit ${currentProperty.Unit} ${currentProperty.SSDoorNo ? "(Door no. " + `${currentProperty.SSDoorNo})` : ""} ${currentProperty.SSName}`;
    }

    return `${currentProperty.StreetOrUnitNo} ${currentProperty.PropertyAddress}`;
}

app.buildListingDetailsTab = function () {
    var container = $("<div id='listingDetailsContainer' class='contentdiv' />");
    if (currentProperty.PropertyListingId) {
        // Property has an active listing
        app.getListingInfo(currentProperty.PropertyListingId, function (listingModel) {
            // may need to access container by ID here
          
        });
    } else {
        // No active listing, option to create and post a ListingModel
        container.append($(`<span style='display:block;'>No active listings found for ${app.formatAddress()}</span>`));
        var createListingBtn = $("<input type='button' id='newlistingbtn' class='bigRedButton' value='New Listing' />");
        container.append("<p />").append(createListingBtn).append("<p />");
        createListingBtn.click(app.handleNewListingClick);

        var listingCategorySelectorContainer = $("<div id='listingCategorySelectorContainer' />");
        var listingFieldsContainer = $("<div id='listingFieldsContainer' style='display:none'>\
                                            <hr>\
                                                <div id='listingFieldsContent'>\
                                                    \
                                                </div>\
                                            <hr>\
                                            <input type='button' id='createListingBtn' class='bigRedButton' value='Next >>' />\
                                        </div>");

        container.append(listingCategorySelectorContainer);
        container.append(listingFieldsContainer);
    }

    return container;
}

app.buildListingHistoryTab = function () {
    return '';
}

app.buildListingCategories = function () {
    app.clearListingSelection();
    var container = $("#listingCategorySelectorContainer");
    container.empty();
    var categorySelector = $("<label for='listingCategorySelector' class='fieldAlignment'>Listing Category:</label>\
                                <select id='listingCategorySelector' class='centered-aligned'>\
                                            <option value=''></option>\
                                            <option value='residential'>Residential</option>\
                                            <option value='commercial'>Commercial</option>\
                                            <option value='developments'>Developments</option>\
                                            <option value='holiday'>Holiday</option>\
                                </select>");
    container.append(categorySelector);
    categorySelector.change(app.handleSelectListingCategory);
}

app.clearListingSelection = function () {
    $("#listingFieldsContent").empty();
    $("#listingFieldsContainer").hide();
}

app.buildResidentialListing = function () {
    app.clearListingSelection();
    $("#listingFieldsContainer").show();

    var content = $("#listingFieldsContent");
    content.append('here here!');
}

// here..