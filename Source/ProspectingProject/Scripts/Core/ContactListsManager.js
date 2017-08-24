

var contactListsManager = contactListsManager || {};

contactListsManager.retrieveListsForBranch = function (callback) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: 'retrieve_lists_for_branch'}),
        dataType: "json"
    }).done(function (data) {
        $.unblockUI();
        callback(data);
    });
}

contactListsManager.showListManagerForContactPerson = function () {
    var container = $("<div title='List Manager' style='font-family:Verdana;font-size:12px;overflow: hidden;' />").empty();
    container.append("Lists provide a way to group contacts together. Use this feature to toggle the list(s) this contact is a member of.<p style='margin-top:10px'/>");
    container.append("<p />");
    var selectAllCheckbox = $("<label style='display:inline-block;float:right;'><input type='checkbox' id='selectAllListsForContact' style='vertical-align:middle' />Select All</label>");
    container.append(selectAllCheckbox);
    var listsContainer = $("<div id='listsContainer' style='height:300px;display:inline-block;width:100%;overflow-y:scroll;' />");
    container.append(listsContainer);
    container.dialog({
        modal: true,
        closeOnEscape: false,
        width: '600',
        height: '500',
        open: function (event, ui) { $(".ui-dialog-titlebar-close", $(this).parent()).hide(); },
        buttons: {
            "Save And Close": function () {
                contactListsManager.saveListsForContact(function (data) {
                    if (!handleResponseIfServerError(data)) {
                        return;
                    }

                    showSavedSplashDialog("Selection Saved");
                    container.dialog("close");
                    $(".memberOfListCheckbox").remove();
                    container.remove();
                });
            },
            "Cancel": function () {
                container.dialog("close");
                $(".memberOfListCheckbox").remove();
                container.remove();
            }
        },
        position: ['center', 'center']
    });
    selectAllCheckbox.unbind('change').bind('change', function (e) {
        var target = $(this).find('input');
        var checked = target.is(':checked');
        if (checked) {
            $('.memberOfListCheckbox').prop('checked', true);
        }
        else {
            $('.memberOfListCheckbox').prop('checked', false);
        }
        e.preventDefault();
    });
    contactListsManager.retrieveListsForBranch(function (data) {
        if (!data.length) {
            selectAllCheckbox.remove();
            listsContainer.remove();
            $(".memberOfListCheckbox").remove();
            container.append("<p /><span>No lists have been added for your branch. Use the SmartAdmin interface to add lists.</span>");
            container.dialog("option", "buttons", { "Close": function () { $(this).dialog("close"); } });
            return;
        }
        contactListsManager.showLists(listsContainer, data);        
    });
}

contactListsManager.showLists = function (container, listsData) {
    container.empty();

    var grid;
    var columns;
    var sortcol;
    var sortdir = 1;
    var dataView = new Slick.Data.DataView();

    var options = {
        forceFitColumns: true,
        enableCellNavigation: false,
        enableColumnReorder: false,
        multiColumnSort: false
    };

    columns = [
                 { id: "col_ListName", name: "List", field: "ListName", width: 370, sortable: false },
                 { id: "col_ListTypeDescription", name: "List Type", field: "ListTypeDescription", sortable: false, cssClass: 'center-cell' },
                  { id: "col_MemberCount", name: "List Size", field: "MemberCount", sortable: false, cssClass: 'center-cell' },
                 { id: "col_IsMember", name: "Member Of", formatter: addListMemberFormatter, sortable: false, cssClass: 'center-cell' }
    ];
    grid = new Slick.Grid(container, dataView, columns, options);
    grid.setSelectionModel(new Slick.RowSelectionModel());
    grid.registerPlugin(new Slick.AutoTooltips());

    function comparer(a, b) {
        var x = a[sortcol].toLowerCase(), y = b[sortcol].toLowerCase();
        return (x == y ? 0 : (x > y ? 1 : -1));
    }

    grid.onSort.subscribe(function (e, args) {
        sortcol = args.sortCols[0].sortCol.field;
        sortdir = args.sortCols[0].sortAsc ? true : false;
        dataView.sort(comparer, sortdir);
    });

    dataView.onRowCountChanged.subscribe(function (e, args) {
        grid.updateRowCount();
        grid.render();
    });

    dataView.onRowsChanged.subscribe(function (e, args) {
        grid.invalidateRows(args.rows);
        grid.render();
    });

    dataView.beginUpdate();
    dataView.setItems(listsData);
    dataView.endUpdate();
    dataView.syncGridSelection(grid, true);
    container.find(".slick-viewport").css('overflow-x', 'hidden');

    function addListMemberFormatter(row, cell, value, columnDef, dataContext) {
        var id = 'listid_' + dataContext.ListId;
        var checked = '';
        if (currentPersonContact) {
            var contactsInList = dataContext.Members;
            var isMember = $.grep(contactsInList, function (c) {
                return c.ContactPersonId == currentPersonContact.ContactPersonId;
            })[0];
            if (isMember) {
                checked = 'checked';
            }
        }
        var viewContactBtn = $("<input type='checkbox' name='memberOfListCheckbox' class='memberOfListCheckbox' id='" + id + "'" + checked + " />");
        return viewContactBtn[0].outerHTML;
    }
}

contactListsManager.saveListsForContact = function (callback) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Saving...</p>' });
    var listArray = [];
    $(".memberOfListCheckbox").each(function (idx, listItem) {
        var id = $(listItem).attr('id');
        id = id.replace('listid_', '');
        var isChecked = $(listItem).is(':checked');
        var list = { ListId: id, Selected: isChecked };
        listArray.push(list);
    });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: 'save_lists_for_contact', ContactPersonId: currentPersonContact.ContactPersonId, TargetPropertyId: currentProperty.ProspectingPropertyId, ListAllocation: listArray }),
        dataType: "json"
    }).done(function (data) {
        $.unblockUI();
        callback(data);
    });
}

contactListsManager.saveListsForSelection = function (useVisibleProperties, callback) {
    var visibleProperties = null;
    var recipients = null;
    if (useVisibleProperties) {
        // Determine if properties are filtered -or- if targeting the suburb as-is
        visibleProperties = [];
        $.each(currentSuburb.VisibleMarkers, function(idx, property) {
            visibleProperties.push(property.ProspectingProperty.ProspectingPropertyId);
        });
    } else {
        // Targeting a selection 
        var commSelectedRows = $('#commContactsTable tr.rowSelected');
        recipients = [];
        $.each(commSelectedRows, function (idx, row) {
            var contactId = $(row).attr("id").replace('comm_row_', '');
            var contact = getContactFromId(contactId);
            recipients.push(contact);
        });
    }

    var listArray = [];
    $(".memberOfListCheckbox").each(function (idx, listItem) {
        var id = $(listItem).attr('id');
        id = id.replace('listid_', '');
        var isChecked = $(listItem).is(':checked');
        if (isChecked) {
            var list = { ListId: id, Selected: isChecked };
            listArray.push(list);
        }
    });

    var saveObject = {
        Instruction: 'save_lists_for_selection',
        SelectedLists: listArray,
        VisibleProperties: visibleProperties,
        TargetContactsList: recipients
    };

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Saving...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify(saveObject),
        dataType: "json"
    }).done(function (data) {
        $.unblockUI();
        callback(data);
    });
}

contactListsManager.showListManagerForSelection = function (useVisiblePropertiesForSuburb) {
    var headerMessage = '';
    if (useVisiblePropertiesForSuburb) {
        // use properties visible in the current suburb (NB might come from filtering)
        headerMessage = "All contacts from the currently visible properties will be added to the selected lists below.";
    } else {
        // use the selection
        headerMessage = "Your selection of contacts will be added to the selected lists below.";
    }

    var container = $("<div title='List Manager' style='font-family:Verdana;font-size:12px;overflow: hidden;' />").empty();
    container.append("Lists provide a way to group contacts together. " + headerMessage + "<p style='margin-top:10px'/>");
    container.append("<p />");
    var selectAllCheckbox = $("<label style='display:inline-block;float:right;'><input type='checkbox' id='selectAllListsForSelection' style='vertical-align:middle' />Select All</label>");
    container.append(selectAllCheckbox);
    var listsContainer = $("<div id='listsContainer' style='height:300px;display:inline-block;width:100%;overflow-y:scroll;' />");
    container.append(listsContainer);
    container.dialog({
        modal: true,
        closeOnEscape: false,
        width: '600',
        height: '500',
        open: function (event, ui) { $(".ui-dialog-titlebar-close", $(this).parent()).hide(); },
        buttons: {
            "Save And Close": function () {
                if (!$(".memberOfListCheckbox").is(":checked")) {
                    alert("In order to save you need to select at least one list");
                    return;
                }
                contactListsManager.saveListsForSelection(useVisiblePropertiesForSuburb, function (data) {
                    if (!handleResponseIfServerError(data)) {
                        return;
                    }

                    showSavedSplashDialog("Selection Saved");
                    container.dialog("close");
                    $(".memberOfListCheckbox").remove();
                    container.remove();
                });
            },
            "Cancel": function () {
                container.dialog("close");
                $(".memberOfListCheckbox").remove();
                container.remove();
            }
        },
        position: ['center', 'center']
    });
    selectAllCheckbox.unbind('change').bind('change', function (e) {
        var target = $(this).find('input');
        var checked = target.is(':checked');
        if (checked) {
            $('.memberOfListCheckbox').prop('checked', true);
        }
        else {
            $('.memberOfListCheckbox').prop('checked', false);
        }
        e.preventDefault();
    });
    contactListsManager.retrieveListsForBranch(function (data) {
        if (!data.length) {
            selectAllCheckbox.remove();
            listsContainer.remove();
            $(".memberOfListCheckbox").remove();
            container.append("<p /><span>No lists have been added for your branch. Use the SmartAdmin interface to add lists.</span>");
            container.dialog("option", "buttons", { "Close": function () { $(this).dialog("close"); } });
            return;
        }
        contactListsManager.showLists(listsContainer, data);
    });
}