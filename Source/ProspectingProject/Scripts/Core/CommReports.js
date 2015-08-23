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

function renderCommHistoryReport(results) {
    var headerText = '';
    if (results.TotalResultsPerFilterCriteria > 500) {
        headerText = 'Showing 500 of ' + results.TotalResultsPerFilterCriteria + ' transactions. Refine your search criteria to see more specific results.';
    } else {
        headerText = 'Showing ' + results.TotalResultsPerFilterCriteria + ' transactions.';
    }
    var resultsDiv1 = $("<div id='commResultsHeaderDiv' style='display:block;width:100%' />")
                        .empty()
                        .append(headerText);

    var tbl = $("<div style='overflow:auto;max-height:450px;'><table id='commReportTableBody' class='commTable' ></table></div>");
    var resultsDiv2 = $("<div id='commResultsBodyDiv' />")
                        .empty()
                        .append($("<table id='commReportTableHeader' class='commTable' ></table>")
                         .append("<thead><tr class='commTableHeader'><th class='commTableRow120'>Sent To</th><th class='commTableRow70'>Date</th><th class='commTableRow'>Sender</th><th class='commTableRow'>Status</th><th class='commTableRow'>Prop ID</th><th class='commTableRow120' style='padding-right:20px'>Batch Name</th></tr></thead>"))
                         .append(tbl);

    var tbody = $("<tbody></tbody>");
    if (results.EmailLogItems) {
        $.each(results.EmailLogItems, function (idx, record) {
            var tr = $("<tr />");
            var sentTo = $("<td class='commTableRow120' title='" + record.SentTo + "'></td>").append(record.SentTo);
            var dateSent = $("<td class='commTableRow70' title='" + record.DateSent + "'></td>").append(record.DateSent.substring(0,10));
            var sentBy = $("<td class='commTableRow' title='" + record.SentBy + "'></td>").append(record.SentBy);
            var status = $("<td class='commTableRow' title='" + record.DeliveryStatus + "'></td>").append(record.DeliveryStatus);
            var propID = $("<td class='commTableRow' title='" + record.TargetLightstonePropertyId + "'></td>").append(record.TargetLightstonePropertyId);
            var batchName = $("<td class='commTableRow120' title='" + record.FriendlyNameOfBatch + "'></td>").append(record.FriendlyNameOfBatch);

            tr.append(sentTo).append(dateSent).append(sentBy).append(status).append(propID).append(batchName);
            tbody.append(tr);
        });
        tbl.find('table').append(tbody);
    }
    if (results.SMSLogItems) {
        $.each(results.SMSLogItems, function (idx, record) {
            var tr = $("<tr />");
            var sentTo = $("<td class='commTableRow120' title='" + record.SentTo + "'></td>").append(record.SentTo);
            var dateSent = $("<td class='commTableRow70' title='" + record.DateSent + "'></td>").append(record.DateSent.substring(0, 10));
            var sentBy = $("<td class='commTableRow' title='" + record.SentBy + "'></td>").append(record.SentBy);
            var status = $("<td class='commTableRow' title='" + record.DeliveryStatus + "'></td>").append(record.DeliveryStatus);
            var propID = $("<td class='commTableRow' title='" + record.TargetLightstonePropertyId + "'></td>").append(record.TargetLightstonePropertyId);
            var batchName = $("<td class='commTableRow120' title='" + record.FriendlyNameOfBatch + "'></td>").append(record.FriendlyNameOfBatch);

            tr.append(sentTo).append(dateSent).append(sentBy).append(status).append(propID).append(batchName);
            tbody.append(tr);
        });
        tbl.find('table').append(tbody);
    }

    resultsDiv2.append(tbl);
    $('#commMessageHistory').empty().append(resultsDiv1).append("<p />").append(resultsDiv2);
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
    var container = $("<div id='commMessageHistory' class='contentdiv' />");
    return container;
}