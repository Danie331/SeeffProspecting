

var contactListsManager = contactListsManager || {};

contactListsManager.retrieveListsForBranch = function (callback) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: 'retrieve_lists_for_branch', ContactPersonId: currentPersonContact ? currentPersonContact.ContactPersonId : null }),
        dataType: "json"
    }).done(function (data) {
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
        $.unblockUI();
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
                 { id: "col_ListName", name: "List", field: "ListName", width: 300, sortable: false },
                 { id: "col_ListTypeDescription", name: "Export Filter", width: 150, field: "ListTypeDescription", sortable: false },
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
            if (dataContext.CurrentContactIsMember) {
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

    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Saving...Please wait.</p>' });
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
        $.unblockUI();
    });
}

contactListsManager.toggleListsMainMenu = function() {
    var container = $("#listsDiv");
    if (container.children('.contentExpander').length) {
        contactListsManager.retrieveListsForBranch(function (data) {
            var target = $("#selectListToExport").empty().append($("<option value='-1' />"));
            $.each(data, function (idx, list) {
                target.append($("<option value='" + list.ListId + "'>" + list.ListName + "</option>"));
            });
            $.unblockUI();
        });
        return;
    }

    var createListTabInit = false, createExportTabInit = false;
    $('#createNewListTab').empty();
    var createNewListTab = buildContentExpanderItem('createNewListTab', 'Assets/new_list.png', "Create New List", buildCreateNewListTab());
    $('#listExportTab').empty();
    var listExportTab = buildContentExpanderItem('listExportTab', 'Assets/export_list.png', "Export List", buildlistExportTab());

    var listsExpander = new ContentExpanderWidget('#contentarea', [createNewListTab, listExportTab], "listExpander");
    container.append(listsExpander.construct());
    listsExpander.open('listExportTab');
    container.css('display', 'block');

    function buildlistExportTab() {

        var grid; // slickgrid containing columns

        var div = $("<div />").empty().css('display', 'block');      
        var selectListDesc = $("<span class='fieldAlignment' style='display:inline-block'>Select a list to export:</span>");
        var selectListMenu = $("<select id='selectListToExport' style='display:inline-block;width:70%;max-width:60%;' />").append($("<option value='-1' />"));
        div.append(selectListDesc).append(selectListMenu).append("<p />");
        var outputFormatDesc = $("<span class='fieldAlignment' style='display:inline-block'>Select an output format:</span>");
        var selectFormatMenu = $("<select style='display:inline-block' />").append($("<option value='1'>.xlsx</option>")).append($("<option value='2'>.csv</option>"));
        div.append(outputFormatDesc).append(selectFormatMenu).append("<p />").append("<p />");

        var fieldSelectionAndOrderingDesc = $("<span style='display:inline-block;width:90%'>Select the fields (columns) to be included in the export. The columns are ordered from top to bottom, to re-order the columns drag the handle in the left-most column to the appropriate position:</span>");
        div.append(fieldSelectionAndOrderingDesc);

        var gridContainer = $('<div style="width:90%"> \
                                <div class="grid-header" style="width:100%"> \
                                </div> \
                                <div id="columnExportGrid" style="width:100%;height:260px;"></div> \
                              </div>');
        div.append(gridContainer);

        // export options
        var optionsFieldset = $("<fieldset style='border:1px solid gray;width:90%'></fieldset>");
        optionsFieldset.append("<legend style='padding: 0.2em 0.5em; border:1px solid gray; color:gray;font-size:90%;'>Export Options</legend>");
        optionsFieldset.append('<span>When a field is empty or null:</span><br />');
        var emptyFieldRadioGroup = $('<input id="leaveFieldIfEmptyRadio" style="vertical-align:middle" type="radio" name="emptyFieldGroup" value="true" checked="checked" /> <span style="vertical-align:middle;margin-right:20px">Leave as empty</span> \
                                      <input id="omitFieldIfEmptyRadio" style="vertical-align:middle" type="radio" name="emptyFieldGroup" value="false" /> <span style="vertical-align:middle">Exclude the record</span> <br />');
        optionsFieldset.append(emptyFieldRadioGroup).append("<p />");
        optionsFieldset.append('<span>Contact details:</span><br />');
        var contactDetailSelector = $('<input style="vertical-align:middle" type="radio" name="contactDetailsGroup" value="true" checked="checked" /> <span style="vertical-align:middle">Use any contact details but prefer primary contact detail if present</span> <br /> \
                                       <input id="usePrimaryDetailOnly" style="vertical-align:middle" type="radio" name="contactDetailsGroup" value="false"  /> <span style="vertical-align:middle;margin-right:20px">Use <b>only</b> primary contact details</span><br />');
        optionsFieldset.append(contactDetailSelector).append("<p />");
        optionsFieldset.append('<span>Exclude contacts with the following statuses:</span><br />');
        var optOutGroup = $("<input id='excludeRecordIfPopiChecked' style='vertical-align:middle' type='checkbox' checked /> POPI opt out \
                            <input id='excludeRecordIfEmailOptOut' style='vertical-align:middle;margin-left:20px' type='checkbox' checked /> Email opt out  \
                             <input id='excludeRecordIfSmsOptOut' style='vertical-align:middle;margin-left:20px' type='checkbox' /> SMS opt out  \
                            <input id='excludeRecordIfDoNotContactChecked' style='vertical-align:middle;margin-left:20px' type='checkbox' /> Do not contact <br />");
        optionsFieldset.append(optOutGroup).append("<p />");
        optionsFieldset.append('<span>Duplicates:</span><br />');
        var duplicatesGroup = $("<input id='excludeDuplicateContacts' style='vertical-align:middle' type='checkbox' disabled='disabled' checked /> Exclude duplicate contacts \
                                <input id='excludeDuplicateEmailAddresses' style='vertical-align:middle;margin-left:20px' type='checkbox' checked /> Exclude records with a duplicate email address <br />");
        optionsFieldset.append(duplicatesGroup).append("<p />");
        div.append("<br />").append(optionsFieldset).append("<p />");
        var exportBtn = $("<input type='button' style='display:inline-block;' value='Export..' />");
        div.append(exportBtn);
        exportBtn.click(function () {

            if (!prospectingContext.UserHasExportPermission) {
                alert("You do not have access (permission) to use this feature.");
                return;
            }

            var listSelected = selectListMenu.val() > -1;
            var atLeastOneColumnSelected = $('.fieldSelectableCheckbox').is(':checked');
            if (listSelected && atLeastOneColumnSelected) {
                var gridSelection = grid.getData();
                var columns = [];
                $.each(gridSelection, function (idx, column) {
                    if (column.selected) {
                        columns.push(column.fieldname);
                    }
                });
                var exportObject =
                    {
                        Instruction: 'export_list',
                        ListId: selectListMenu.val(),
                        OutputFormat: selectFormatMenu.find("option:selected").text(),
                        Columns: columns,
                        OmitRecordIfEmptyField: $("#omitFieldIfEmptyRadio").is(":checked"),
                        UsePrimaryContactDetailOnly: $("#usePrimaryDetailOnly").is(":checked"),
                        ExcludeRecordIfPopiChecked: $("#excludeRecordIfPopiChecked").is(":checked"),
                        ExcludeRecordIfEmailOptOut: $("#excludeRecordIfEmailOptOut").is(":checked"),
                        ExcludeRecordIfSmsOptOut: $("#excludeRecordIfSmsOptOut").is(":checked"),
                        ExcludeRecordIfDoNotContactChecked: $("#excludeRecordIfDoNotContactChecked").is(":checked"),
                        ExcludeDuplicateContacts: true,
                        ExcludeDuplicateEmailAddress: $("#excludeDuplicateEmailAddresses").is(":checked")
                    };
                contactListsManager.exportList(exportObject, contactListsManager.exportListCallback);
            } else {
                alert("Please select a list to export and ensure that at least one field/column is selected.");
            }
        });

        function fieldSelectableFormatter(row, cell, value, columnDef, dataContext) {
            var id = 'fieldid' + dataContext.id;
            var checked = dataContext.selected ? 'checked' : '';
            var selected = $("<input id='" + id + "' type='checkbox' class='fieldSelectableCheckbox' " + checked + " />");
            return selected[0].outerHTML;
        }

        contactListsManager.retrieveListsForBranch(function (data) {
            $.each(data, function (idx, list) {
                selectListMenu.append($("<option value='" + list.ListId + "'>" + list.ListName + "</option>"));
            });
            
            var data = [];
            var columns = [
              {
                  id: "#",
                  name: "",
                  width: 40,
                  behavior: "selectAndMove",
                  selectable: false,
                  resizable: false,
                  cssClass: "cell-reorder dnd"
              },
              {
                  id: "fieldname",
                  name: "Column Name",
                  field: "fieldname",
                  width: 300,
                  cssClass: "cell-title"
              },
              {
                  id: "selected",
                  name: "Selected",
                  width: 60,
                  field: "selected",
                  cssClass: "slick-selected-column",
                  formatter: fieldSelectableFormatter
              }
            ];

            var options = { forceFitColumns: true };
            data = [
        { fieldname: "[Title]", id: 1, selected: true },
        { fieldname: "[First Name]", id: 2, selected: true },
        { fieldname: "[Surname]", id: 3, selected: true },
        { fieldname: "[Email Address]", id: 4, selected: true },
        { fieldname: "[Home Landline]", id: 5, selected: true },
        { fieldname: "[Work Landline]", id: 6, selected: true },
        { fieldname: "[Cellphone]", id: 7, selected: true },
        { fieldname: "[ID number]", id: 8, selected: true },
        { fieldname: "[Property Address]", id: 9, selected: true }
            ];
            grid = new Slick.Grid("#columnExportGrid", data, columns, options);
            grid.setSelectionModel(new Slick.RowSelectionModel());
            var moveRowsPlugin = new Slick.RowMoveManager();
            moveRowsPlugin.onBeforeMoveRows.subscribe(function (e, data) {
                for (var i = 0; i < data.rows.length; i++) {
                    // no point in moving before or after itself
                    if (data.rows[i] == data.insertBefore || data.rows[i] == data.insertBefore - 1) {
                        e.stopPropagation();
                        return false;
                    }
                }
                return true;
            });
            moveRowsPlugin.onMoveRows.subscribe(function (e, args) {
                var extractedRows = [], left, right;
                var rows = args.rows;
                var insertBefore = args.insertBefore;
                left = data.slice(0, insertBefore);
                right = data.slice(insertBefore, data.length);
                rows.sort(function (a, b) { return a - b; });
                for (var i = 0; i < rows.length; i++) {
                    extractedRows.push(data[rows[i]]);
                }
                rows.reverse();
                for (var i = 0; i < rows.length; i++) {
                    var row = rows[i];
                    if (row < insertBefore) {
                        left.splice(row, 1);
                    } else {
                        right.splice(row - insertBefore, 1);
                    }
                }
                data = left.concat(extractedRows.concat(right));
                var selectedRows = [];
                for (var i = 0; i < rows.length; i++)
                    selectedRows.push(left.length + i);
                grid.resetActiveCell();
                grid.setData(data);
                grid.setSelectedRows(selectedRows);
                grid.render();
            });
            grid.registerPlugin(moveRowsPlugin);
            grid.onDragInit.subscribe(function (e, dd) {
                // prevent the grid from cancelling drag'n'drop by default
                e.stopImmediatePropagation();
            });

            $.drop({ mode: "mouse" });
            $("#columnExportGrid .slick-viewport").css('overflow-x', 'hidden');

            div.unbind('change').bind('change', '.fieldSelectableCheckbox', function (e) {
                var target = $(e.target);
                if (target.hasClass('fieldSelectableCheckbox')) {
                    var targetItem = $.grep(data, function (item) {
                        var id = target.attr('id').replace('fieldid', '');
                        return item.id == id;
                    })[0];
                    targetItem.selected = target.is(':checked');
                }
            });
            createExportTabInit = true;
            if (createListTabInit && createExportTabInit) {
                $.unblockUI();
            }
        });
        
        return div;
    }

    function buildCreateNewListTab() {        
        var container = $("<div />").empty().css('display', 'block');   
        var newListDesc = $("<span class='fieldAlignment' style='display:inline-block'>List Name:</span>");
        var newListNameInput = $("<input type='text' style='display:inline-block;width:70%;max-width:60%;' />");
        container.append(newListDesc).append(newListNameInput).append("<p />");

        var selectListTypeDesc = $("<span class='fieldAlignment' style='display:inline-block'>Export Filter:</span>");
        var selectListTypeMenu = $("<select style='display:inline-block;width:40%;max-width:40%;' />");
        container.append(selectListTypeDesc).append(selectListTypeMenu).append("<p />");

        var createBtn = $("<input type='button' style='display:inline-block;' value='Create..' />");
        container.append(createBtn);

        createBtn.click(function () {
            var listType = selectListTypeMenu.val();
            var listName = newListNameInput.val();
            if (listName == '') {
                alert("You must provide a name for the list");
                return;
            }
            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Adding List...</p>' });
            $.ajax({
                type: "POST",
                url: "RequestHandler.ashx",
                data: JSON.stringify({ Instruction: 'create_list', ListName: listName, ListType: {Key: listType, Value: ''} }),
                dataType: "json"
            }).done(function (data) {
                $.unblockUI();
                if (data == true) {
                    $("<div title='List Created Successfully' style='font-family:Verdana;font-size:12px;' />")
                        .append("Your list is ready to be populated and available to all users within your branch. To populate the list you may use any of the methods listed below:")
                        .append("<p />")
                        .append("1. To add/remove individual contacts to lists, navigate to the contact record and click 'List Manager'")
                        .append("<br />")
                        .append("2. To add contacts in bulk, select a suburb, navigate to the Communication menu and click 'Add Contacts To List'")
                        .append("<br />")
                        .append("3. To filter properties first and then add contacts, perform the filtering and then click 'Add To List'")
                        .dialog({
                        modal: true,
                        closeOnEscape: true,
                        width: '730',
                        height: '250',
                        buttons: { "Ok": function () { $(this).dialog("close");} },
                        position: ['center', 'center']
                    });
                    newListNameInput.val("");
                } else {
                    alert("Error occurred saving list. Please contact support.");
                }
            });
        });

        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading...</p>' });
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({ Instruction: 'retrieve_list_types' }),
            dataType: "json"
        }).done(function (data) {
            $.each(data, function (idx, listType) {
                selectListTypeMenu.append($("<option value='" + listType.ListTypeId + "'>" + listType.ListTypeDescription + "</option>"));
            });
            createListTabInit = true;
            if (createListTabInit && createExportTabInit) {
                $.unblockUI();
            }
        });

        return container;
    }
}

contactListsManager.exportList = function (exportObject, callback) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Generating Export. Please wait...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify(exportObject),
        dataType: "json"
    }).done(function (data) {
        $.unblockUI();
        callback(data);
    });
}

contactListsManager.exportListCallback = function (fileData) {
    if (fileData.Success == true) {
        var downloadURL = fileData.Filepath;
        var form = $('<form method="get" action="' + downloadURL + '"><input type="hidden" value="" /></form>');
        $('body').append(form);
        $(form).submit();
        form.remove();
    } else {
        alert("An error occurred during the export. Please contact support if the error persists: " + fileData.Error);
    }
}