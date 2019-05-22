
var app = app || {};

app.togglePropertyListingMenu = function (hide) {
    var container = $("#propertyListingContainer");
    if (currentProperty == null || hide) {
        container.empty();
        return;
    }

    if (container.children('.contentExpander').length) {
        return;
    }

    $('#listingDetailsTab').empty();
    var listingDetailsTab = buildContentExpanderItem('listingDetailsTab', 'Assets/prpdata.png', "Listing Details", app.buildListingDetailsTab());
    $('#listingHistoryTab').empty();
    var listingHistoryTab = buildContentExpanderItem('listingHistoryTab', 'Assets/valuations_history.png', "Listing History & Event Log", app.buildListingHistoryTab());

    var ce = new ContentExpanderWidget('#contentarea', [listingDetailsTab, listingHistoryTab], "listingExpander");
    container.append(ce.construct());
    ce.open('listingDetailsTab');

    container.css('display', 'block');
}

app.handleNewListingClick = function () {
    app.buildListingCategories();
}

app.handleSelectListingCategory = function () {
    var selectedCat = $(this).val();
    switch (selectedCat) {
        case 'residential':
            app.buildResidentialListing();
            break;
        case 'commercial':
            app.buildCommercialListing();
            break;
        case 'developments':
            app.buildDevelopmentsListing();
            break;
        case 'holiday':
            app.buildHolidayListing();
            break;
        case '':
            app.clearListingSelection();
            break;
    }
    app.parsleyInstance = $("#listingFieldsContent").parsley();
    app.attachEventHandlers();
}

app.attachEventHandlers = function () {
    $("#priceInput").unbind('keyup').bind('keyup', function () {
        var val = $(this).val().replace(/\s/g, '');
        if (val == '' || isNaN(val)) {
            $(this).val('');
            return;
        }
        val = parseFloat(val);
        val = val.toLocaleString().replace(/,/g, " ");
        $(this).val(val);
    });

    $("#addDevelopmentPropertyTypeBtn").click(app.addDevelopmentPropertyTypeRow);

    var container = $("#propertyListingContainer");
    container.on('keyup', '#pricedFromInput', function () {
        var val = $(this).val().replace(/\s/g, '');
        if (val == '' || isNaN(val)) {
            $(this).val('');
            return;
        }
        val = parseFloat(val);
        val = val.toLocaleString().replace(/,/g, " ");
        $(this).val(val);
    });

    $("#createListingBtn").click(function () {
        app.parsleyInstance.validate();
    });
}