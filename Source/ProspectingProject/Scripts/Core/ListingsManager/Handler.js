
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

    $("#addDevelopmentPropertyTypeBtn").unbind('click').bind('click', app.addDevelopmentPropertyTypeRow);

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

    $("#createListingBtn").unbind('click').bind('click', function() {
        if (app.parsleyInstance.validate()) {
            if (app.validateInputs()) {
                app.showSummaryDialog();
            }
            else {
                app.showInvalidInputs();
            }
        }
    });
}

app.validateInputs = function () {
    if ($("#listingCategorySelector").val() != 'developments') {
        return true;
    }

    var rows = app.getDevelopmentPropertyRows();
    if (!rows.length) {
        return false;
    }

    var isValid = true;
    rows.forEach(function (row) {
        if (!row.PropertyType || !row.Price || !row.Number) {
            isValid = false;
        }
    });

    return isValid;
}

app.showInvalidInputs = function () {
    var dialog = $("<div title='Validation Error' style='font-family: Verdana;font-size: 12px;' />")
        .append("<span>Please fix the following errors before proceeding:</span>")
        .append("<p />");

    var rows = app.getDevelopmentPropertyRows();
    if (!rows.length) {
        dialog.append("<span>- Please add at least one property type (using the \"Add Types\" button)</span>");
    }

    var isValid = true;
    if (rows.length) {
        rows.forEach(function (row) {
            if (!row.PropertyType || !row.Price || !row.Number) {
                isValid = false;
            }
        });
    }
    if (!isValid) {
        dialog.append("<span>- One or more inputs under the Types is empty or incomplete</span>");
    }

    dialog.dialog({
        modal: true,
        closeOnEscape: false,
        width: '600',
        buttons: {
            "OK": function () { $(this).dialog("close"); }
        },
        position: ['middle', 'middle']
    });
}