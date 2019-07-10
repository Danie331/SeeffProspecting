
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
    //$('#listingHistoryTab').empty();
    //var listingHistoryTab = buildContentExpanderItem('listingHistoryTab', 'Assets/valuations_history.png', "Listing History & Event Log", app.buildListingHistoryTab());

    var ce = new ContentExpanderWidget('#contentarea', [listingDetailsTab], "listingExpander");
    container.append(ce.construct());
    ce.open('listingDetailsTab');

    container.css('display', 'block');
}

app.handleNewListingClick = function () {
    if (currentProperty.PropertyListingId) {
        var dialog = $("<div title='Warning' style='font-family: Verdana;font-size: 12px;' />")
            .append("<span>An existing listing is associated with this property. Creating a new listing will overwrite the existing listing on Prospecting. \
                     You will still have access to the listing on the service provider's portal.</span>")
            .append("<p />");
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

    if (selectedCat != '') {
        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading...</p>' });
        $.ajax({
            type: "GET",
            url: `api/Listings/GetLookupData?seeffAreaId=${currentSuburb.SuburbId}`,
            dataType: "json"
        }).done(function (result) {
            app.populateLookupControls(result);
        }).fail(function (error) {
            app.handleApiError(error); // ..here..
        })
            .always(function () {
                $.unblockUI();
            });
    }
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

    $("#createListingBtn").unbind('click').bind('click', function () {
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
        if (!row.PropertyType || !row.Price || !row.Number || !row.SizeFrom || !row.SizeTo) {
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
            if (!row.PropertyType || !row.Price || !row.Number || !row.SizeFrom || !row.SizeTo) {
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

app.handleApiError = function (error) {
    var dialog = $("<div title='Server Error' style='font-family: Verdana;font-size: 12px; width:600px;height:400px' />")
        .append("<span>The following error occurred, please contact support:</span>")
        .append("<p />")
        .append(`<span  style='color:red'>${error.responseJSON.ExceptionMessage}</span>`);

    dialog.dialog({
        modal: true,
        closeOnEscape: false,
        width: '700',
        buttons: {
            "Close": function () { $(this).dialog("close"); }
        },
        position: ['middle', 'middle']
    });
}

app.getListingInfo = function (activeListingId, handler) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading listing...</p>' });
    $.ajax({
        type: "GET",
        url: `api/Listings/GetListing?listingId=${activeListingId}`,
        dataType: "json",
        contentType: "application/json"
    }).done(function (result) {
        handler(result);
    }).fail(function (error) {
        app.handleApiError(error);
    })
        .always(function () {
            $.unblockUI();
        });
}

app.postListingToApi = function (model, category) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Posting listing...</p>' });
    $.ajax({
        type: "POST",
        url: `api/Listings/Create${category}Listing`,
        data: JSON.stringify(model),
        dataType: "json",
        contentType: "application/json"
    }).done(function (result) {
        showSavedSplashDialog('Listing successfully created');
        currentProperty.PropertyListingId = result.ActiveListingId;
        app.togglePropertyListingMenu(true);
        app.togglePropertyListingMenu();
    }).fail(function (error) {
        app.handleApiError(error);
    })
        .always(function () {
            $.unblockUI();
        });
}