var currentPropertyForPropertyInformation = null;

function buildPropertyInformationMenu() {

    var div = $("<div class='contentdiv' id='propertyInformationDiv' style='display:none' />");
    div.empty();
    return div;
}

function togglePropertyInformationMenu() {
    var container = $("#propertyInformationDiv");
    if (currentProperty == null) {
        container.empty();
        return;
    }
    if (currentPropertyForPropertyInformation != currentProperty) {
        currentPropertyForPropertyInformation = currentProperty;
        container.empty();

        var expander = buildContentExpanderForPropertyInfo();
        container.append(expander.construct());

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
                currentProperty.Condition = $("#conditionInput").val();
                currentProperty.Pool = $("#poolInput").prop('checked');
                currentProperty.StaffAccomodation = $("#staffAccomInput").prop('checked');

                updateProspectingRecord(currentProperty, null, null);
            }
        });

        // New valuation
        $("#dateValuedInput").datepicker({ dateFormat: 'DD, d MM yy' });
        $('#saveNewValuationBtn').click(function () {
            if (validateNewValuationInputs()) {
                saveValuation();
            } else {
                alert("Some of your inputs are not valid.");
            }
        });
    }

    container.css('display', 'block');
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
    var hedonicDataTab = buildContentExpanderItem('hedonicDataTab', 'Assets/hedonic_data.png', "Hedonic Information", buildHedonicDataTab());

    return new ContentExpanderWidget('#contentarea', [hedonicDataTab, newValuationTab, valuationsHistoryTab], "propertyInfoExpander");
}

function buildNewValuationTab() {
    var container = $("<div />")
                .empty()
                .append("<label class='fieldAlignment'>Value estimate (R) </label><input type='number' id='valueEstimateInput' size='6' />")
                .append("<br />")
                .append("<label class='fieldAlignment'>Date valued </label><input type='text' id='dateValuedInput' />")
                .append("<br />")
                .append("<label class='fieldAlignment'>Same as current value </label><input type='checkbox' id='isCurrentValueInput' />")
                .append("<br />")
                .append("<label class='fieldAlignment'>Create a 'Valuation Done' activity? </label><input type='checkbox' id='createValuationActivityInput' checked />")
                .append("<p />")
                .append("<input type='button' id='saveNewValuationBtn' value='Save' />");

    return container;
}

function buildValuationsHistoryTab() {
    return "";
}

function buildHedonicDataTab() {
    var container = $("<div />")
    .empty()
    .append("<label class='fieldAlignment'>ERF size(m&sup2;) </label><input type='number' id='erfSizeInput' size='3' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Dwelling size(m&sup2;) </label><input type='number' id='dwellingSizeInput' size='3' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Beds </label><input type='number' id='noBedsInput' size='3' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Baths </label><input type='number' id='noBathsInput' size='3' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Receptions </label><input type='number' id='receptionsInput' size='3' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Studies </label><input type='number' id='studiesInput' size='3' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Garages </label><input type='number' id='garagesInput' size='3' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Parking bays </label><input type='number' id='parkingbaysInput' size='3' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Condition </label><input type='text' id='conditionInput' maxlength='16' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Pool </label><input type='checkbox' id='poolInput' />")
    .append("<br />")
    .append("<label class='fieldAlignment'>Staff accomodation </label><input type='checkbox' id='staffAccomInput' />")
    .append("<p />")
    .append("<input type='button' id='saveHedonicDataBtn' value='Save' />");

    return container; // validation, saving + loading. update in memory record.
}