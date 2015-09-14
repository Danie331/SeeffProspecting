///
var commReportsExpander = null;

function buildCommReportingMenu() {
    var div = $("<div class='contentdiv' id='commReportsDiv' style='display:none' />");
    div.empty();
    return div;
}

function toggleCommReportingMenu() {
    var container = $("#commReportsDiv");

    if (container.children('.contentExpander').length) {
        return;
    }

    commReportsExpander = buildContentExpanderForCommReporting();
    container.append(commReportsExpander.construct());
    commReportsExpander.open('commFilterSelectionTab');

    container.css('display', 'block');

    // Mandatory filters:
    // SMS or email?  (mand)
    // Sent by me, or everyone in BU? (mand)
    // Sent all|successfully|failed|pending|reply(sms only) (mand)
    // Optional filters 
    // SEnt between datetime1 datetime2
    // Batch name similar to....
    // Sent to particular email address|cell number
    // Sent to a particular property (prop id?)
    // [Generate Report] | [Reset Form]
}

function buildContentExpanderForCommReporting() {
    $('#commFilterSelectionTab').empty();
    var commFilterSelectionTab = buildContentExpanderItem('commFilterSelectionTab', 'Assets/comm_report_filters.png', "Choose Your Filters", buildCommFilterTab());
    $('#commReportTab').empty();
    var commReportTab = buildContentExpanderItem('commReportTab', 'Assets/comm_message_history.png', "Message History Report", buildCommMessageHistoryTab());

    return new ContentExpanderWidget('#contentarea', [commFilterSelectionTab, commReportTab], "commReportingExpander");
}

function buildCommFilterTab() {
    var containerDiv = $("<div id='commReportFilterContainer' />");

    var filterByWhat = $("<label class='fieldAlignmentShortWidth'>Filter by: </label><label class='fieldAlignmentShortWidth' style='vertical-align:middle;'><input type='radio' name='commFilterWhatOption' id='smsFilterOption' style='vertical-align:middle;margin-top: -1px;' />SMS Messages</label> \
                                                    <label id='emailFilterOptionLabel' style='vertical-align:middle;'><input type='radio' name='commFilterWhatOption' id='emailFilterOption'  style='vertical-align:middle;margin-top: -1px;' />Email Messages</label><p \>");
    //filterByWhat.change(function() {
    //    var target = $(this).attr('id');

    //});

    var sentBy = $("<label class='fieldAlignmentShortWidth'>Sender: </label><label class='fieldAlignmentShortWidth' style='vertical-align:middle;'><input type='radio' name='commFilterSentByOption' id='sentByMe' style='vertical-align:middle;margin-top: -1px;' />Me</label> \
                                                    <label id='sentByBusinessUnitLabel' style='vertical-align:middle;'><input type='radio' name='commFilterSentByOption' id='sentByBusinessUnit'  style='vertical-align:middle;margin-top: -1px;' />Anyone in my business unit</label><p \>");

    var sendingStatus = $("<label class='fieldAlignmentShortWidth'>Message status: </label><label class='fieldAlignmentShortWidth' style='vertical-align:middle;'><input type='radio' name='commFilterStatusOption' id='statusAllMessages' style='vertical-align:middle;margin-top: -1px;' checked  />All statuses</label> \
                                                    <label style='vertical-align:middle;'><input type='radio' name='commFilterStatusOption' id='statusSuccessfulMessages'  style='vertical-align:middle;margin-top: -1px;' />Delivered</label>\
                                                    <label style='vertical-align:middle;'><input type='radio' name='commFilterStatusOption' id='statusPendingMessages'  style='vertical-align:middle;margin-top: -1px;' />Pending</label>\
                                                    <label style='vertical-align:middle;'><input type='radio' name='commFilterStatusOption' id='statusFailedMessages'  style='vertical-align:middle;margin-top: -1px;' />Failed</label><p \>");

    var sentBetweenDatetimes = $("<label class='fieldAlignmentShortWidth'>Sent between: </label><label style='vertical-align:middle;'><input type='text' id='messageSentFromDate' style='vertical-align:middle;margin-top: -1px;' />&nbsp;and</label> \
                                                    <label class='fieldAlignmentExtraShortWidth' style='vertical-align:middle;'><input type='text' id='messageSentToDate'  style='vertical-align:middle;margin-top: -1px;' /></label><p />");

    sentBetweenDatetimes.find('#messageSentFromDate').datepicker({ dateFormat: 'DD, d MM yy', maxDate: '0' });
    sentBetweenDatetimes.find('#messageSentToDate').datepicker({ dateFormat: 'DD, d MM yy', maxDate: '0' });

    var batchNameLike = $("<label class='fieldAlignment'>Batch name similar to: </label><label><input type='text' id='commBatchNameSimilarTo' style='width:55%' /></label><p />");

    var sentToEmailAddress = $("<label class='fieldAlignment'>Sent to email address: </label><label class='fieldAlignmentShortWidth'><input type='text' id='commSentToEmailAddress' /></label><p />");
    var sentToCellNo = $("<label class='fieldAlignment'>Sent to cell no.: </label><label class='fieldAlignmentShortWidth'><input type='text' id='commSentToCellNo' /></label><p />");
    var sentToProperty = $("<label class='fieldAlignment'>Sent to property ID: </label><label class='fieldAlignmentShortWidth'><input type='number' id='commSentToProperty' /></label><p />");

    var generateReportBtn = $("<input type='button' value='Generate Report' style='cursor:pointer' />");
    var resetFormBtn = $("<input type='button' value='Reset Fields' style='display:inline-block;float:right;cursor:pointer' />");

    generateReportBtn.click(function () {
        if (validateFilterInputs()) {
            var type = filterByWhat.find('#smsFilterOption').is(':checked') ? 'SMS' : 'EMAIL';
            var status = $('input[name=commFilterStatusOption]:checked');
            var statusText = status.attr('id');
            var dataPacket = {
                Instruction: "load_comm_report",
                MessageType: type,
                SentByMe: sentBy.find('#sentByMe').is(':checked'),
                SentStatus: statusText,
                FromDate: sentBetweenDatetimes.find('#messageSentFromDate').val(),
                ToDate: sentBetweenDatetimes.find('#messageSentToDate').val(),
                BatchName: batchNameLike.find('#commBatchNameSimilarTo').val(),
                TargetEmailAddress: sentToEmailAddress.find('#commSentToEmailAddress').val(),
                TargetCellNo: sentToCellNo.find('#commSentToCellNo').val(),
                TargetLightstonePropertyId: sentToProperty.find('#commSentToProperty').val()
            };
            loadMessageHistoryReport(dataPacket, renderCommHistoryReport);
        }

        // load, close, open, "showing 250 of 3000 messages - please refine your filter criteria to see more specific results."
    });

    resetFormBtn.click(function () {
        filterByWhat.find('#smsFilterOption').prop('checked', false);
        filterByWhat.find('#emailFilterOption').prop('checked', false);

        sentBy.find('#sentByMe').prop('checked', false);
        sentBy.find('#sentByBusinessUnit').prop('checked', false);

        sendingStatus.find('#statusAllMessages').prop('checked', true);

        sentBetweenDatetimes.find('#messageSentFromDate').datepicker('setDate', '');
        sentBetweenDatetimes.find('#messageSentToDate').datepicker('setDate', '');

        batchNameLike.find('#commBatchNameSimilarTo').val('');
        sentToEmailAddress.find('#commSentToEmailAddress').val('');
        sentToCellNo.find('#commSentToCellNo').val('');
        sentToProperty.find('#commSentToProperty').val('');
    });

    return containerDiv.append(filterByWhat)
                        .append(sentBy)
                        .append(sendingStatus)
                        .append(sentBetweenDatetimes)
                        .append(batchNameLike)
                        .append(sentToEmailAddress)
                        .append(sentToCellNo)
                        .append(sentToProperty)
                        .append(generateReportBtn)
                        .append(resetFormBtn);
}

function loadMessageHistoryReport(dataPacket, callbackFn) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Report...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify(dataPacket),
        dataType: 'json'
    }).done(function (data) {
        $.unblockUI();
        commReportsExpander.close('commFilterSelectionTab');
        commReportsExpander.open('commReportTab');

        callbackFn(data);
    });
}

function handleCommReportRowClick(e) {
    var row = $(this);
    var id = row.attr('id').replace('path_to_contact_','');

    var seeffAreaId = id.split('_')[0];
    var lightstonePropertyId = id.split('_')[1];
    var contactPersonId = id.split('_')[2];

    e.preventDefault();
    // Find the suburb this property belongs to:
    var targetSuburb = $.grep(suburbsInfo, function (sub) {
        return sub.SuburbId == seeffAreaId;
    })[0];

    if (!targetSuburb) {
        alert('This contact belongs to a property that falls outside your available suburbs (' + seeffAreaId + ')');
        return;
    }

    // Load the suburb
    globalZoomLevel = 20;
    $('#suburbLink' + targetSuburb.SuburbId).trigger('click', function () {
        var targetProperty = $.grep(currentSuburb.ProspectingProperties, function (pp) {
            return pp.LightstonePropertyId == lightstonePropertyId;
        })[0];
     
        targetProperty.Whence = 'from_comm_report';
        targetProperty.TargetContactIdForComms = contactPersonId;
        var marker = targetProperty.Marker;
        try {
            centreMap(marker.Suburb, marker, true);
            new google.maps.event.trigger(marker, 'click', function () {
                //debugger;
            });
        } catch (e) {}
    });
}

function renderCommHistoryReport(results) {
    $('#commMessageHistory').empty();
    if (!results.EmailLogItems.length && !results.SMSLogItems.length) {
        $('#commMessageHistory').append("No matching results found.");
        return;
    }
    var options = {
        forceFitColumns: true,
        enableCellNavigation: true,
        enableColumnReorder: false,
        multiColumnSort: true
    };

    function commReportBtnFormatter(row, cell, value, columnDef, dataContext) {
        var id = 'path_to_contact_' + dataContext.SeeffAreaId + '_' + dataContext.TargetLightstonePropertyId + '_' + dataContext.ContactPersonId;
        var viewContactBtn = $("<img src='Assets/view_btn.png' style='cursor:pointer;float:right' id='" + id + "' class='commReportViewContactRow' />");
        return viewContactBtn[0].outerHTML;
    }

    var grid;
    var columns;
    var sortcol;
    var sortdir = 1;
    var dataView = new Slick.Data.DataView(/*{ inlineFilters: true }*/);

    columns = [
                 { id: "col_SentTo", name: "Sent To", field: "SentTo", sortable: true },
                 { id: "col_DateSent", name: "Date Sent", field: "DateSent", sortable: true },
                 { id: "col_SentBy", name: "Sent By", field: "SentBy", sortable: true },
                 { id: "col_DeliveryStatus", name: "Delivery Status", field: "DeliveryStatus", sortable: true },
                 //{ id: "col_TargetLightstonePropertyId", name: "Property Id", field: "TargetLightstonePropertyId", sortable: false },
                 { id: "col_FriendlyNameOfBatch", name: "Batch Name", field: "FriendlyNameOfBatch", sortable: true },
                 { id: "col_ViewContactRecord", name: "", width: 12, formatter: commReportBtnFormatter }
    ];
    grid = new Slick.Grid("#commMessageHistory", dataView, columns, options);
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

    if (results.EmailLogItems.length) {
        dataView.beginUpdate();
        dataView.setItems(results.EmailLogItems);
        dataView.endUpdate();
        dataView.syncGridSelection(grid, true);
    }
    else {
        dataView.beginUpdate();
        dataView.setItems(results.SMSLogItems);
        dataView.endUpdate();
        dataView.syncGridSelection(grid, true);
    }

    $('#commMessageHistory').off('click', '.commReportViewContactRow');
    $('#commMessageHistory').on('click', '.commReportViewContactRow', handleCommReportRowClick);
}

function validateFilterInputs() {
    if (!$('#smsFilterOption').is(':checked') && !$('#emailFilterOption').is(':checked')) {
        var target = document.getElementById('emailFilterOptionLabel');
        tooltip.pop(target, 'Please select an option', { showDelay: 1, hideDelay: 100, calloutPosition: 0.5 });
        return false;
    }

    if (!$('#sentByMe').is(':checked') && !$('#sentByBusinessUnit').is(':checked')) {
        var target = document.getElementById('sentByBusinessUnitLabel');
        tooltip.pop(target, 'Please select an option', { showDelay: 1, hideDelay: 100, calloutPosition: 0.5 });
        return false;
    }

    return true;
}

function buildCommMessageHistoryTab() {
    var container = $("<div id='commMessageHistory' class='contentdiv' style='height:480px' />");
    return container;
}