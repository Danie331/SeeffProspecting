var currentPropertyForPropertyInformation = null;

var mandateLookupData = null;

var selectedAgency = -1;

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
        //expander.open('hedonicDataTab');
        if (currentPropertyForPropertyInformation.HasMandate) {
            expander.open('mandateInformationTab');
        } else {
            expander.open('hedonicDataTab');
        }
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
                $(this).val('Saved').prop('disabled', true);
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

            var followupComment = buildCommentForValuationFollowup($("#dateValuedInput").val());
            $("#newValuationFollowupCommentInput").empty().append(followupComment);
        });

        $("#dateValuedInput").change(function () {
            var followupComment = buildCommentForValuationFollowup($(this).val());
            $("#newValuationFollowupCommentInput").empty().append(followupComment);
        });

        // Valuations history
        loadValuationsHistory();

        // Mandate History
        //loadMandateHistory();
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
    value = value.replace(/\s+/g, "");
    var date = $("#dateValuedInput").val();
    var isCurrentValue = $("#isCurrentValueInput").prop('checked');
    var createActitivity = $("#createValuationActivityInput").prop('checked');

    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({
            Instruction: 'create_valuation',
            ProspectingPropertyId: currentProperty.ProspectingPropertyId,
            Value: value,
            ValuationDate: date,
            IsCurrentValue: isCurrentValue,
            CreateActivity: createActitivity,
            CreateFollowup: $("#createValuationFollowupInput").is(":checked"),
            FollowupDate: $("#newValuationFollowupDateInput").val(),
            RelatedTo: $("#newValuationRelatedToInput").val(),
            Comment: $("#newValuationFollowupCommentInput").val()
        }),
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
    value = value.replace(/\s+/g, "");
    if (value && /^\d+$/.test(value)) {
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
    //$('#mandateInformationTab').empty();
    //var mandateInformationTab = buildContentExpanderItem('mandateInformationTab', 'Assets/mandate_info.png', "Mandate Information", buildMandateInformationTab());

    return new ContentExpanderWidget('#contentarea', [basicDataTab, newValuationTab, valuationsHistoryTab/*, mandateInformationTab*/], "propertyInfoExpander");
}

function buildCommentForValuationFollowup(dateString) {
    if (!dateString) {
        return "";
    }
    var address = currentPropertyForPropertyInformation.StreetOrUnitNo + ' ' + currentPropertyForPropertyInformation.PropertyAddress;
    if (currentPropertyForPropertyInformation.SS_FH == 'FH') {
        address = address;
    } else {
        address = "Unit " + currentPropertyForPropertyInformation.Unit + ' ' + currentPropertyForPropertyInformation.SSName + ', ' + address;
    }
    
    var commentText = 'Please follow up on a validation done on ' + dateString + ' for ' + address;
    return commentText;
}

function buildNewValuationTab() {

    var comment = buildCommentForValuationFollowup(null);
    //rem restrictions on follow up date + validation on follow up itself + test on Units in SS + test chrome

    var createFollowupForNewValInput = $("<input type='checkbox' id='createValuationFollowupInput' checked />");
    createFollowupForNewValInput.change(function () {
        var checked = $(this).is(':checked');
        $("#createFollowupForNewValuationFieldset").css('display', checked ? 'block' : 'none');
    });

    var relatedToSelect = $("<select id='newValuationRelatedToInput' style='width:185px;margin-top:3px' />");
    relatedToSelect.append($("<option />").val(-1).text(''));
    $.each(currentPropertyForPropertyInformation.Contacts, function (idx, el) {
        relatedToSelect.append($("<option />").val(el.ContactPersonId).text(el.Firstname + ' ' + el.Surname));
    });

    var newValuationFollowupDateInput = $("<input type='text' id='newValuationFollowupDateInput' readonly='true' style='width:185px;margin-top:3px' />");
    newValuationFollowupDateInput.datepicker({
        dateFormat: 'DD, d MM yy', minDate: 0, changeMonth: true,
        changeYear: true        
    }).datepicker('setDate', "+7");

    var followupContainer = $("<div id='valuationFollowupContainer' />")
                             .append("<label class='fieldAlignmentExtendedWidth' style='width:280px'>Create a follow-up for the activity? </label>")
                             .append(createFollowupForNewValInput)
                             .append('<p />')
                             .append($("<fieldset id='createFollowupForNewValuationFieldset' style='border:1px solid gray'></fieldset>")
                                    .append("<legend style='padding: 0.2em 0.5em; border:1px solid gray; color:gray;font-size:90%;'>Follow up Details</legend>")
                                    .append("<label class='fieldAlignmentShortWidth' style='width:165px'>Follow-up Type: </label><span style='color:red'>Valuation Complete - Follow Up</span>")
                                    .append("<br />")
                                    .append("<label class='fieldAlignmentShortWidth' style='width:165px;margin-top:3px'>Allocated To: </label><span style='color:red;margin-top:3px'>Me</span>")
                                    .append("<br />")
                                    .append("<label class='fieldAlignmentShortWidth' style='width:165px;margin-top:3px'>Related To: </label>")
                                    .append(relatedToSelect)
                                    .append("<br />")
                                    .append("<label class='fieldAlignmentShortWidth' style='width:165px'>Follow-up Date: </label>")
                                    .append(newValuationFollowupDateInput)
                                    .append("<p />")
                                    .append("<label style='width:165px'>Comment (edit as desired): </label>")
                                    .append("<br />")
                                    .append($("<textarea id='newValuationFollowupCommentInput' style='width:98%;border:1px solid gray' rows='8' />").append(comment))
                                    );

    var createValuationActivityInput = $("<input type='checkbox' id='createValuationActivityInput' checked />"); // toggle display, uncheck & disable - undo
    createValuationActivityInput.change(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            createFollowupForNewValInput.prop('disabled', false);
            createFollowupForNewValInput.attr('checked', true);
            $("#createFollowupForNewValuationFieldset").css('display', 'block');
        } else {
            createFollowupForNewValInput.prop('disabled', true);
            createFollowupForNewValInput.attr('checked', false);
            $("#createFollowupForNewValuationFieldset").css('display', 'none');
        }
    });

    var container = $("<div />")
                .empty()
                .append("<label class='fieldAlignmentExtendedWidth' style='width:280px'>Value estimate (R) </label><input type='number' id='valueEstimateInput' style='width:185px;' />")
                .append("<br />")
                .append("<label class='fieldAlignmentExtendedWidth' style='width:280px'>Date valued </label><input type='text' id='dateValuedInput' readonly='true' style='width:185px;margin-top:3px' />")
                .append("<br />")
                .append("<label class='fieldAlignmentExtendedWidth' style='width:280px'>Use current Lightstone value </label><input type='checkbox' id='isCurrentValueInput' />")
                .append("<br />")
                .append("<label class='fieldAlignmentExtendedWidth' style='width:280px'>Create a 'Valuation Done' activity? </label>")
                .append(createValuationActivityInput)
                .append("<br />")
                // hidden by default, hinged on checkbox above
                .append(followupContainer)
                .append("<p />")
                .append("<input type='button' id='saveNewValuationBtn' value='Create' style='cursor:pointer' />");

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

function buildMandateInformationTab() {

    var mandatesHistoryContainer = $("<div id='mandatesHistoryContainer' class='contentdiv' style='height:100px;' />"); // class='contentdiv' style='height:480px'
    var createNewMandateBtn = $("<button type='text' id='addNewMandateBtn' style='cursor:pointer;display:block;vertical-align:middle;'><img src='Assets/add_mandate.png' style='height:16px;display:inline-block;vertical-align:middle;margin-right:5px' /><label style='vertical-align:middle'>Add Mandate With Agency</label></button>");
    var loadingLabel = $("<span id='mandateLoadingLabel' style='display:none'>Loading...</span>");

    var container = $("<div id='mandatesContainer' />")
    .empty()
    .append(loadingLabel)
    .append("<p />")
    .append(mandatesHistoryContainer)
    .append("<p />")
    .append(createNewMandateBtn);

    createNewMandateBtn.click(handleAddNewMandate);

    return container;
}

function handleAddNewMandate(e) {
    e.preventDefault();
    openCreateNewMandateDialog();
}

function loadMandateHistory() {
    // The last mandate created for this property: (No mandates created for this property)
    // No existing mandates found for this property
    // Edit THIS mandate
    // Create New Mandate with a green "plus" icon top right-hand corner.
    // Account for case in which mandate created by user(s) not in license.
    // Mandate icon on property
    // "deactivate" option
    // test with change-of-ownership UNITs, test with comm mode.
    // test with red and purple highlighted units as well.
    // update HasMandate property.
    // when adding new mandate on top of previous, replace UI etc.
    //1 table = 1 mandate: always use last created mandate
    // test followups 
    // create new: delete previous mandate (with foollowups) and replace.
    // when overriding mandate set rem to delete all FUTURE-DATED followups
    // when deactivating/deleting rem to delete follow-ups
    // rem changes to web.config
    // inputs validation
    // ensure that seeff_agents pointing to live seeff.
    // test activities and followups!

    //Hi Danie

    //Please change the mandate follow up type, needs to be selectable and not hard coded, you can set the type as a default however. 

    //Change Follow up type "Mandate Expiry" to "Mandate Set To Expire"

    //Set default follow up date for "Mandate Set to Expire" - check with debbie how far in advance this needs to be. 

    //Also Add Mandate Types
    //Mandate Expired
    //Lease Expired 

    //Add Calendar date picker to select the start and end date of the mandate period

    //Please remove follow-up column from Mandate table. 

    // *********************************************************************

    //Hi Danie

    //Please limit the seeff agents in the drop down menu to agents which work for that specific license;

    //Join Agent table with branch table where seeff.dbo.branch.fk_license_id = license id.



    var mandatesHistoryContainer = $("#mandatesHistoryContainer");
    mandatesHistoryContainer.empty();

    if (mandateLookupData == null) {
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({ Instruction: 'load_mandate_lookup_data' }),
            dataType: "json"
        }).done(function (data) {
            if (!handleResponseIfServerError(data)) {
                return;
            }
            mandateLookupData = data;
        });
    }

    if (!currentPropertyForPropertyInformation.HasMandate) {
        mandatesHistoryContainer.append('<label style="vertical-align:middle">No existing mandates found for this property.</label>');
    } else {
        loadMandateResults();
    }
}

function loadMandateResults() {
    $("#mandatesHistoryContainer").empty();
    $("#mandateLoadingLabel").show();
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: 'load_current_mandate_set', LightstonePropertyId: currentProperty.LightstonePropertyId }),
        dataType: "json"
    }).done(function (data) {
        if (!handleResponseIfServerError(data)) {
            return;
        }
        $("#mandateLoadingLabel").hide();

        renderMandates(data.ListOfMandates);
    });
}

function renderMandates(listOfMandates) {

    var options = {
        forceFitColumns: true,
        enableCellNavigation: true,
        enableColumnReorder: false,
        multiColumnSort: true
    };

    function editBtnFormatter(row, cell, value, columnDef, dataContext) {
        var id = 'mandate_item_' + dataContext.PropertyMandateAgencyID;
        var editBtn = $("<a href='javascript:void(0);'  style='cursor:pointer;text-decoration: underline;' class='mandateItemEditor' >Delete</a>");
        return editBtn[0].outerHTML;
    }

    var grid;
    var columns;
    var sortcol;
    var sortdir = 1;
    var dataView = new Slick.Data.DataView(/*{ inlineFilters: true }*/);

    columns = [
                 { id: "col_Agency", name: "Agency", field: "Agency", sortable: true },
                 { id: "col_MandateType", name: "Mandate Type", field: "MandateType", sortable: true },
                 { id: "col_Status", name: "Status", field: "Status", sortable: true },
                 { id: "col_ListingPrice", name: "Listing Price", field: "ListingPrice", sortable: true },
                 { id: "col_FollowupDate", name: "Follow-up Date", field: "FollowupDate", sortable: true },
                 { id: "col_Agents", name: "Agents", field: "Agents", sortable: true },
                 { id: "col_EditRecord", name: "", width: 12, formatter: editBtnFormatter }
    ];
    grid = new Slick.Grid("#mandatesHistoryContainer", dataView, columns, options);
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
    dataView.setItems(listOfMandates);
    dataView.endUpdate();
    dataView.syncGridSelection(grid, true);
}

function openCreateNewMandateDialog() {

    var titleText = "Create Mandate With Agency";

    var dialog = $("<div id='mandateCreateDialog' title='" + titleText + "' style='font-family: Verdana; font-size: 12px;' />");
    dialog.empty();
    dialog.append('<p />');
    dialog.append("<label class='fieldAlignmentShortWidth' style='margin-top:3px'>Agency: </label>");
    var mandateAgencyInput = $("<input type='text' size='30' id='mandateAgencyInput' style='margin-top:3px' />");
    dialog.append(mandateAgencyInput);
    dialog.append('<br />');
    var agentNamesContainerSeeffAgents = $("<label class='fieldAlignmentShortWidth' style='margin-top:3px;vertical-align:middle'>Agents: </label><div id='agentsContainer' class='autoCompleteBlock autocompleteInputFields' style='margin-top:3px;display:none !important'><input type='text' id='mandateAgentsInput' class='agentNamesAutoCompleteSearchBox' style='margin-top:3px' /><input type='hidden' id='AgentNamesHidden' class='agentNames' /><input type='hidden' id='AgentIDsHidden' class='agentIds' /></div>");
    dialog.append(agentNamesContainerSeeffAgents);
    var agentNamesContainerOtherAgents = $("<textarea id='mandateAgentsInputOtherAgents' style='margin-top:3px;display:none;height:80px;width:350px;vertical-align:middle' />");
    dialog.append(agentNamesContainerOtherAgents);
    dialog.append('<p /><hr />');
    dialog.append("<label class='fieldAlignmentShortWidth' style='margin-top:3px'>Mandate Type: </label>");
    var mandateTypesInput = $("<select id='mandateTypesInput' style='margin-top:3px;width:210px' />");
    dialog.append(mandateTypesInput);
    $.each(mandateLookupData.MandateTypes, function (idx, item) {
        mandateTypesInput.append($('<option>', { value: item.MandateTypeID, text: item.TypeDescription }));
    });
    dialog.append('<br />');
    dialog.append("<label class='fieldAlignmentShortWidth' style='margin-top:3px'>Mandate Status: </label>");
    var mandateStatusInput = $("<select id='mandateStatusInput' style='margin-top:3px;width:210px' />");
    dialog.append(mandateStatusInput);
    $.each(mandateLookupData.MandateStatuses, function (idx, item) {
        mandateStatusInput.append($('<option>', { value: item.MandateStatusID, text: item.StatusDescription }));
    });
    dialog.append('<br />');
    dialog.append("<label class='fieldAlignmentShortWidth' style='margin-top:3px'>Listing Price (R): </label>");
    var listingPrice = $("<input id='mandateListingPrice' type='text' style='margin-top:3px;width:205px;' />");
    dialog.append(listingPrice);

    function setFollowupType() {
        var followupTypeText = 'n/a';
        var mandateStatus = mandateStatusInput.find("option:selected").text();
        switch (mandateStatus) {
            case 'On Market': followupTypeText = 'Mandate Expiry';
                break;
            case 'Rented': followupTypeText = 'Lease Expiry';
                break;
            case 'Sold': {
                if (selectedAgency == 1)
                    followupTypeText = 'After Sales Service';
                break;
            }
            case 'Short Term Rental': followupTypeText = 'Short-term Rental Client Care';
                break;
        }
        followupTypes.text(followupTypeText);
        if (followupTypeText == 'n/a') {
            // clear and disable date selector
            $('#mandateFollowupDate').val('');
            $('#mandateFollowupDate').datepicker('disable');
            $('#mandateFollowupComment').attr("disabled", true);
            $('#mandateFollowupAllocatedToInput').attr("disabled", true);
        } else {
            $('#mandateFollowupDate').datepicker('enable');
            $('#mandateFollowupComment').attr("disabled", false);
            $('#mandateFollowupAllocatedToInput').attr("disabled", false);
        }
    }

    mandateStatusInput.change(function () {
        setFollowupType();
    });
    dialog.append('<p /><hr />');
    dialog.append("<label class='fieldAlignmentShortWidth' style='margin-top:3px'>Follow-up Type: </label>");
    var followupTypes = $("<label id='followupTypeLabel' style='margin-top:3px;width:205px;color:red' /> ");
    dialog.append(followupTypes);
    setFollowupType();
    dialog.append('<br />');
    dialog.append("<label class='fieldAlignmentShortWidth' style='margin-top:3px'>Follow-up Date: </label>");
    var followupDate = $("<input id='mandateFollowupDate' type='text' style='margin-top:3px;width:205px;' readonly='true' />");
    dialog.append(followupDate);
    followupDate.datepicker({ dateFormat: 'DD, d MM yy', minDate: 0, changeMonth: true, changeYear: true });
    dialog.append('<br />');
    dialog.append("<label class='fieldAlignmentShortWidth' style='margin-top:3px'>Allocated To: </label>");
    var allocatedTo = $("<select id='mandateFollowupAllocatedToInput' style='margin-top:3px;width:210px' />");
    dialog.append(allocatedTo);

    allocatedTo.append($("<option />").val(-1).text(''));
    $.each(prospectingContext.BusinessUnitUsers, function (idx, el) {
        allocatedTo.append($("<option />").val(el.UserGuid).text(el.UserName + " " + el.UserSurname));
    });

    dialog.append('<br />');
    dialog.append("<label class='fieldAlignmentShortWidth' style='margin-top:3px'>Comment: </label>");
    var comment = $("<textarea id='mandateFollowupComment' style='margin-top:3px;height:80px;width:350px;vertical-align:middle' />");
    dialog.append(comment);

    // error section
    var errorSection = $("<div id='mandateErrors' style='color:red' />")
    dialog.append("<p />");
    dialog.append(errorSection);
    dialog.dialog(
                            {
                                modal: true,
                                closeOnEscape: false,
                                width: '600',
                                buttons: {
                                    "Save": function () {
                                        saveMandate();
                                        //$(this).dialog("close");
                                    },
                                    "Cancel": function () { dialog.empty(); $(this).dialog("close"); }
                                },
                                position: ['top', 'top']
                            });

    function enableFreeText() {
        $("#agentsContainer").hide();
        agentNamesContainerOtherAgents.show();
    }

    function hideAgentInputs() {
        $("#agentsContainer").hide();
        agentNamesContainerOtherAgents.hide();
    }

    $('#mandateAgencyInput').autocomplete({
        source: mandateLookupData.MarketshareAgencies,
        select: function (event, ui) {
            $('#mandateAgencyInput').val(ui.item.label);
            selectedAgency = ui.item.value;
            if (selectedAgency == 1) {
                createSeeffAgentsAutocomplete();
            } else {
                enableFreeText();
            }
            setFollowupType();
            return false; // Prevent the widget from inserting the value.
        },
        focus: function (event, ui) {
            $('#mandateAgencyInput').val(ui.item.label);
            return false; // Prevent the widget from inserting the value.
        },
        close: function (event, ui) {
            if (selectedAgency == -1) {
                $('#mandateAgencyInput').val('');
                hideAgentInputs();
            }
            setFollowupType();
        }
    });

    $('#mandateAgencyInput').blur(function () {
        if (selectedAgency == -1) {
            $('#mandateAgencyInput').val('');
            hideAgentInputs();
        }
        setFollowupType();
    });

    $('#mandateAgencyInput').attr('autocomplete', 'on');

    $('#mandateAgencyInput').on('keydown', function (e) {
        selectedAgency = -1;
        if (e.which == 8) { //bkspce
            $('#mandateAgencyInput').val('');
            hideAgentInputs();
        }
        setFollowupType();
    });

  
    function createSeeffAgentsAutocomplete() {       

        $("#agentsContainer").show();
        agentNamesContainerOtherAgents.hide();

        function autoCompleteBlock(message, idValue, name) {
            var span = $('<span value="' + name + '">').text(message).prependTo('.autoCompleteBlock');
            var a = $("<a></a>", { "class": "remove", href: "javascript:void(0)", value: idValue, title: "remove", text: "x" }).appendTo(span);
            $('.autoCompleteBlock').scrollTop(0);
        }

        $('#mandateAgentsInput').autocomplete({
            source: mandateLookupData.SeeffAgents,
            select: function (event, ui) {

                if (ui.item.id < 0)
                    return false;

                var agentID = ui.item.value;
                var agentName = ui.item.label;
                autoCompleteBlock(ui.item ? ui.item.label : this.label, agentID, agentName);

                var idList = $(".agentIds");
                var ids = idList.val().split(",");
                ids.push(agentID);
                idList.val(ids.join(","));

                $(this).val('');
                return false;

            }
        });
        //$("#mandateAgentsInput").attr('autocomplete', 'on');
        $(".autoCompleteBlock").click(function () {
            $("#mandateAgentsInput").focus();
        });
        $(".autoCompleteBlock").on("click", "a", removeItemBlock);
    }


    function removeItemBlock(id) {

        var target = $(this);
        if ($.isNumeric(id)) {
            target = $('a.remove').filter(function () {
                return $(this).attr('value') == id;
            });
        }

        var agentId = String(target.attr('value'));
        var agentIds = $(".agentIds").val().split(",");

        target.parent().remove();

        $.each(agentIds, function (i, val) {
            if (val === agentId) {
                agentIds.splice(i, 1);
            }
        });
        $(".agentIds").val(agentIds);
    }
}

function saveMandate() {
    var valid = true;
    var message = '';
    // Validate Agency present:
    if (selectedAgency < 1) {
        valid = false;
        message = '- You must select an agency';
    }

    var listingPrice = $('#mandateListingPrice').val();
    if (listingPrice.length > 0 && isNaN(listingPrice)) {
        valid = false;
        message += '<br />- The listing price entered is not a valid number';
    }

    var followupType = $("#followupTypeLabel").text();
    if (followupType != 'n/a' && $("#mandateFollowupDate").val() == '') {
        valid = false;
        message += '<br />- A follow up date must be selected';
    }

    var mandateAgencyID = selectedAgency;
    var mandateAgents = '';
    if (mandateAgencyID == 1) {
        mandateAgents = $(".agentIds").val();
    } else {
        mandateAgents = $("#mandateAgentsInputOtherAgents").val();
    }
    var mandateType = $("#mandateTypesInput").val();
    var mandateStatus = $("#mandateStatusInput").val();
    // listing price
    var mandateFollowupDate = $("#mandateFollowupDate").val();
    var mandateFollowupType = $("#followupTypeLabel").text();
    var followupAllocatedTo = $("#mandateFollowupAllocatedToInput").val();
    var followupComment = $("#mandateFollowupComment").val();

    if (!valid) {
        $("#mandateErrors").empty().append(message);
    } else {
        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Saving Mandate...</p>' });
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({
                Instruction: 'save_mandate',
                LightstonePropertyId: currentProperty.LightstonePropertyId,
                MandateAgencyID: mandateAgencyID,
                MandateAgents: mandateAgents,
                MandateType: mandateType,
                MandateStatus: mandateStatus,
                ListingPrice: listingPrice ? listingPrice : null,
                MandateFollowupDate: mandateFollowupDate ? mandateFollowupDate : null,
                MandateFollowupTypeText: mandateFollowupType != 'n/a' ? mandateFollowupType : null,
                FollowupAllocatedTo: followupAllocatedTo != '-1' ? followupAllocatedTo : null,
                FollowupComment: followupComment ? followupComment : null
            }),
            dataType: "json"
        }).done(function (data) {
            $.unblockUI();
            $("#mandateCreateDialog").dialog("close");
            $("#mandateCreateDialog").remove();
            selectedAgency = -1;
            if (!handleResponseIfServerError(data)) {
                return;
            }

            loadMandateResults();
            //$("#mandateLoadingLabel").hide();
            //$("#mandatesContainer").children().not("#mandateLoadingLabel").show();
            //renderMandates(data.ListOfMandates);
        });
    }
}