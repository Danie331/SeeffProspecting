var currentPropertyForPropertyInformation = null;

var propertyConditionDescription = {
    item1: 'High quality finishings ready to move in',
    item2: 'Standard finishings ready to move in',
    item3: 'Spec finishings ready to move in',
    item4: 'Needs TLC however one could move in',
    item5: 'Needs serious attention however one could move in',
    item6: 'Not habitable'
};

function buildPropertyInformationMenu() {

    var div = $("<div class='contentdiv' id='propertyInformationDiv' style='display:none' />");
    div.empty();
    return div;
}

function togglePropertyInformationMenu(hide) {
    var container = $("#propertyInformationDiv");
    if (currentProperty == null || hide) {
        container.empty();
        return;
    }
    if (currentPropertyForPropertyInformation != currentProperty) { 
        currentPropertyForPropertyInformation = currentProperty;
        container.empty();

        var expander = buildContentExpanderForPropertyInfo();
        container.append(expander.construct());
        expander.open('hedonicDataTab');
        // hedonic data
        populateHedonicData();
        $("#saveHedonicDataBtn").click(function () {
            if (validateHedonicInputs()) {
                currentProperty.ErfSize = $("#erfSizeInput").val();
                currentProperty.DwellingSize = $("#dwellingSizeInput").val();
                currentProperty.Beds = $("#noBedsInput").val();
                currentProperty.Baths = $("#noBathsInput").val();
                currentProperty.Receptions = $("#receptionsInput").val();
                currentProperty.Studies = $("#studiesInput").val();
                currentProperty.Garages = $("#garagesInput").val();
                currentProperty.ParkingBays = $("#parkingbaysInput").val();
                currentProperty.Condition = $("#conditionInput>option:selected").html();
                currentProperty.Pool = $("#poolInput").prop('checked');
                currentProperty.StaffAccomodation = $("#staffAccomInput").prop('checked');

                currentProperty.IsShortTermRental = $("#shortTermRentalInput").prop('checked');
                currentProperty.IsLongTermRental = $("#longTermRentalInput").prop('checked');
                currentProperty.IsCommercial = $("#commercialInput").prop('checked');
                currentProperty.IsAgricultural = $("#agriculturalInput").prop('checked');
                currentProperty.IsInvestment = $("#investmentInput").prop('checked');

                updateProspectingRecord(currentProperty, null, null);
            }
        });

        // New valuation
        $("#dateValuedInput").datepicker({
            dateFormat: 'DD, d MM yy',
            maxDate: 0,
            changeMonth: true,
            changeYear: true,
            beforeShow: function (i) { if ($('#isCurrentValueInput').is(':checked')) { return false; } }
        });
        $('#saveNewValuationBtn').click(function () {
            if (validateNewValuationInputs()) {
                saveValuation();
            } else {
                alert("Some of your inputs are not valid.");
            }
        });
        $('#isCurrentValueInput').change(function () {
            var checked = $(this).is(':checked');
            if (checked) {
                $('#valueEstimateInput').val(currentPropertyForPropertyInformation.LastPurchPrice).attr('readonly', true);
                $("#dateValuedInput").datepicker('setDate', new Date());
            } else {
                $('#valueEstimateInput').val('').attr('readonly', false);
                $("#dateValuedInput").datepicker('setDate', '');
            }
        });

        // Valuations history
        loadValuationsHistory();
    }

    container.css('display', 'block');
}

function loadValuationsHistory() {
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: 'load_valuations', ProspectingPropertyID: currentProperty.ProspectingPropertyId }),
        dataType: "json"
    }).done(function (data) {
        if (!handleResponseIfServerError(data)) {
            return;
        }
        
        var tbl = $("#valuationsHistoryTbl");
        tbl.find("tr:gt(0)").remove();
        if (data) {
            $.each(data, function (idx, val) {
                var tr = $("<tr />");
                tr.append("<td>" + formatRandValue(val.Value) + "</td>");
                tr.append("<td>" + val.ValuationDate.substring(0,10) + "</td>");
                tr.append("<td>" + val.CreatedByUsername + "</td>");
                var deleteRecordBtn = $("<input type='button' value='Delete' id='valuation_row_" + val.ValuationRecordId + "' />");
                tr.append(deleteRecordBtn);                
                tbl.append(tr);

                deleteRecordBtn.click(function () {
                    var valuationRecordId = $(this).attr('id').replace('valuation_row_', '');
                    deleteValuation(valuationRecordId);
                });
            });
        }
    });
}

function deleteValuation(valuationRecordId) {
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: 'delete_valuation', ValuationRecordId: valuationRecordId }),
        dataType: "json"
    }).done(function (data) {
        if (!handleResponseIfServerError(data)) {
            return;
        }
        loadValuationsHistory();
    });
}

function saveValuation() {
    var value = $("#valueEstimateInput").val();
    var date = $("#dateValuedInput").val();
    var isCurrentValue = $("#isCurrentValueInput").prop('checked');
    var createActitivity = $("#createValuationActivityInput").prop('checked');

    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: 'create_valuation', ProspectingPropertyId:currentProperty.ProspectingPropertyId, Value: value, ValuationDate: date, IsCurrentValue: isCurrentValue, CreateActivity: createActitivity }),
        dataType: "json"
    }).done(function (data) {
        if (!handleResponseIfServerError(data)) {
            return;
        }
        showSavedSplashDialog('Valuation Saved');
        loadValuationsHistory();
    });
}

function validateNewValuationInputs() {
    var value = $("#valueEstimateInput").val();
    if (value && $.isNumeric(value)) {
        var date = $("#dateValuedInput").val();
        if (date) {
            return true;
        }
    }
    return false;
}

function populateHedonicData() {
    $("#erfSizeInput").val(currentPropertyForPropertyInformation.ErfSize);
    $("#dwellingSizeInput").val(currentPropertyForPropertyInformation.DwellingSize);
    $("#noBedsInput").val(currentPropertyForPropertyInformation.Beds);
    $("#noBathsInput").val(currentPropertyForPropertyInformation.Baths);
    $("#receptionsInput").val(currentPropertyForPropertyInformation.Receptions);
    $("#studiesInput").val(currentPropertyForPropertyInformation.Studies);
    $("#garagesInput").val(currentPropertyForPropertyInformation.Garages);
    $("#parkingbaysInput").val(currentPropertyForPropertyInformation.ParkingBays);
    $("#conditionInput").val(currentPropertyForPropertyInformation.Condition);
    $("#poolInput").prop('checked', currentPropertyForPropertyInformation.Pool);
    $("#staffAccomInput").prop('checked', currentPropertyForPropertyInformation.StaffAccomodation);

    $("#shortTermRentalInput").prop('checked', currentPropertyForPropertyInformation.IsShortTermRental);
    $("#longTermRentalInput").prop('checked', currentPropertyForPropertyInformation.IsLongTermRental);
    $("#commercialInput").prop('checked', currentPropertyForPropertyInformation.IsCommercial);
    $("#agriculturalInput").prop('checked', currentPropertyForPropertyInformation.IsAgricultural);
    $("#investmentInput").prop('checked', currentPropertyForPropertyInformation.IsInvestment);
}

function validateHedonicInputs() {
    if (parseInt($("#erfSizeInput").val()) == NaN) return false;
    if (parseInt($("#dwellingSizeInput").val()) == NaN) return false;
    if (parseInt($("#noBedsInput").val()) == NaN) return false;
    if (parseInt($("#noBathsInput").val()) == NaN) return false;
    if (parseInt($("#receptionsInput").val()) == NaN) return false;
    if (parseInt($("#studiesInput").val()) == NaN) return false;
    if (parseInt($("#garagesInput").val()) == NaN) return false;
    if (parseInt($("#parkingbaysInput").val()) == NaN) return false;

    return true;
}

function buildContentExpanderForPropertyInfo() {
    $('#newValuationTab').empty();
    var newValuationTab = buildContentExpanderItem('newValuationTab', 'Assets/new_valuation.png', "New Valuation", buildNewValuationTab());
    $('#valuationsHistoryTab').empty();
    var valuationsHistoryTab = buildContentExpanderItem('valuationsHistoryTab', 'Assets/valuations_history.png', "Valuations History", buildValuationsHistoryTab());
    $('#hedonicDataTab').empty();
    var basicDataTab = buildContentExpanderItem('hedonicDataTab', 'Assets/hedonic_data.png', "Basic Information", buildBasicDataTab());

    return new ContentExpanderWidget('#contentarea', [basicDataTab, newValuationTab, valuationsHistoryTab], "propertyInfoExpander");
}

function buildNewValuationTab() {
    var container = $("<div />")
                .empty()
                .append("<label class='fieldAlignment' style='width:185px'>Value estimate (R) </label><input type='number' id='valueEstimateInput' size='6' />")
                .append("<br />")
                .append("<label class='fieldAlignment' style='width:185px'>Date valued </label><input type='text' id='dateValuedInput' readonly='true' />")
                .append("<br />")
                .append("<label class='fieldAlignment' style='width:185px'>Use current Lightstone value </label><input type='checkbox' id='isCurrentValueInput' />")
                .append("<br />")
                .append("<label class='fieldAlignment' style='width:185px'>Create a 'Valuation Done' activity? </label><input type='checkbox' id='createValuationActivityInput' checked />")
                .append("<p />")
                .append("<input type='button' id='saveNewValuationBtn' value='Save' />");

    return container;
}

function buildValuationsHistoryTab() {
    var container = $("<div />");
    var tableHeader = $("<table id='valuationsHistoryTbl' style='width:100%' />");
    tableHeader.append("<tr><td id='th_value'>Value Estimate</td><td id='th_datevalued'>Valuation Date</td><td id='th_createdby'>Created By</td></tr>");
    container.append(tableHeader);

    return container;
}

function buildBasicDataTab() {
    var container = $("<div />")
    .empty()
    .append("<label class='fieldAlignment'>ERF size(m&sup2;) </label><input type='number' id='erfSizeInput' size='3' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Dwelling size(m&sup2;) </label><input type='number' id='dwellingSizeInput' size='3' style='margin-top:3px' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Beds </label><input type='number' id='noBedsInput' size='3' style='margin-top:3px' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Baths </label><input type='number' id='noBathsInput' size='3' style='margin-top:3px' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Receptions </label><input type='number' id='receptionsInput' size='3' style='margin-top:3px' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Studies </label><input type='number' id='studiesInput' size='3' style='margin-top:3px' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Garages </label><input type='number' id='garagesInput' size='3' style='margin-top:3px' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Parking bays </label><input type='number' id='parkingbaysInput' size='3' style='margin-top:3px' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Condition </label><select id='conditionInput' style='margin-top:3px' >\
                                                                <option></option>\
                                                                  <option>" + propertyConditionDescription["item1"] + "</option>\
                                                                  <option>" + propertyConditionDescription["item2"] + "</option>\
                                                                  <option>" + propertyConditionDescription["item3"] + "</option>\
                                                                <option>" + propertyConditionDescription["item4"] + "</option>\
                                                                  <option>" + propertyConditionDescription["item5"] + "</option>\
                                                                  <option>" + propertyConditionDescription["item6"] + "</option>\
                                                             </select>")
    .append("<br />")
    .append("<label class='fieldAlignment'>Pool </label><input type='checkbox' id='poolInput' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Staff accomodation </label><input type='checkbox' id='staffAccomInput' />")
    .append("<p />")
        .append("<hr />")
        .append("<label class='fieldAlignment'>Short-term rental </label><input type='checkbox' id='shortTermRentalInput' />")
        .append("<br />")
        .append("<label class='fieldAlignment'>Long-term rental </label><input type='checkbox' id='longTermRentalInput' />")
        .append("<br />")
        .append("<label class='fieldAlignment'>Commercial </label><input type='checkbox' id='commercialInput' />")
        .append("<br />")
        .append("<label class='fieldAlignment'>Agricultural </label><input type='checkbox' id='agriculturalInput' />")
        .append("<br />")
        .append("<label class='fieldAlignment'>Investment </label><input type='checkbox' id='investmentInput' />")
        .append("<hr />")
        .append("<p />")
    .append("<input type='button' id='saveHedonicDataBtn' value='Save' />");

    return container; // validation, saving + loading. update in memory record.
}