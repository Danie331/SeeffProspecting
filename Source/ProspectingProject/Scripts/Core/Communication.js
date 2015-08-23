
var communicationsMode = null;
var cellphoneItemId;
var costOfBatch = 0;

var defaultSubjectText = 'Subject';
var defaultBodyText = "[Email body]<p />Use the 'Symbols' drop-down to insert placeholder information.";
var emailBodyInitialised = false;

var uploadedFiles = [];

var userEmailSignature = '';

var smsOptOut = " STOP to opt out";

var specialActivityTemplates = ['Birthday', '5 Year Anniversary', '7 Year Anniversary'];

var selectedTemplateActivityTypeId = null;

var commCustomSelectionEnabled = true;

function buildCommunicationMenu() {

    var contentDiv = $("<div class='contentdiv' id='communicationsDiv' style='padding-right:10px' />");
    var communicationBtn = $("<input type='button' id='communicationBtn' value='New' />");
    communicationBtn.click(handleCommBtnClick);
    var menu = $("<div id='communicationMenu' />");

    var remainingCreditLabel = $("<div id='commRemainingCredit' style='display:inline-block;float:right'>Available Credit: R <span id='commCreditValue'>" + availableCredit + "</span></div>");
    var messageContentContainer = $("<div id='messageContentContainer' />");
    var contactablesContentContainer = buildContactablesTable();
    var getInfoLabel = $("<label id='commGetInfoLabel' style='display:none'></label>");

    var bottomContainer = $("<div id='commBottomDiv' style='display:none' />");
    var sendButton = $("<div id='commSendMessage' style='display:inline-block;float:right'><input type='button' id='commSendMessageBtn' value='Preview'  /></div>");
    sendButton.click(handleCommSendBtnClick);
    bottomContainer.append(sendButton);

    var templatesDiv = $("<div id='templateOptionsDiv' style='display:none;width:100%' />");
    var templatesBtn = $("<input type='button' id='templatesBtn' value='Templates' />");
    var templatesMenu = $("<div id='templatesMenu' />");
    templatesBtn.click(handleTemplatesBtnClick);


    //var relationshipSelect = $("<select id='relTypesSelect' />");
    //relationshipSelect.append("<option value=''></option>");
    //$.each(prospectingContext.PersonPropertyRelationshipTypes, function (idx, rel) {
    //    var option = $("<option data-type='propertyRelationship' value='pr" + rel.Key + "'>" + rel.Value + "</option>");
    //    relationshipSelect.append(option);
    //});
    var templateItemOptionsDiv = buildTemplateItemOptionsDiv();
    templatesDiv.append(templatesBtn).append(templatesMenu).append(templateItemOptionsDiv);

    contentDiv.append(communicationBtn).append(menu).append(remainingCreditLabel).append("<p />").append(templatesDiv).append("<p />").append(messageContentContainer).append("<p />").append(contactablesContentContainer).append(getInfoLabel).append("<br />").append(bottomContainer).append("<p style='padding-bottom:10px' />");

    var multiSelectSticker = $("#multiSelectMode");
    var mapControl = map.controls[google.maps.ControlPosition.TOP_RIGHT];
    mapControl.push(multiSelectSticker[0]);

    function buildContactablesTable() {
        var container = $("<div id='contactablesContentContainer' />");
        container.append($("<table id='commContactsTableHeader' class='commTable' ></table>")
                         .append("<thead><tr class='commTableHeader'><th class='commTableRow30'>&nbsp;</th><th class='commTableRow'>Fullname</th><th class='commTableRow120'>Address</th><th class='commTableRow'>Contact</th><th class='commTableRow'>Action</th><th class='commTableRow'>&nbsp;</th></tr></thead>"))
                         .append($("<div style='overflow:auto;max-height:310px;'><table id='commContactsTable' class='commTable' ></table></div>"));
        return container;
    }

    cellphoneItemId = getProspectingPhoneTypeId('cell');
    return contentDiv; 
}

function buildTemplateItemOptionsDiv(option) {
    var templateItemOptionsDiv = $("#templateItemOptionsDiv");
    if (templateItemOptionsDiv.length) {
        templateItemOptionsDiv.empty();
        templateItemOptionsDiv.css('display','inline-block');
    }
    else {
        templateItemOptionsDiv = $("<div id='templateItemOptionsDiv' style='display:none;' />");
    }

    var templateSelectionDropdown = $("<select id='templateSelector' style='margin-left:5px;width:200px' />");
    var deleteTemplateBtn = '';
    if (option == 'my_template') {
        deleteTemplateBtn = $("<input type='button' id='deleteTemplateBtn' value='Delete' style='margin-left:2px;margin-right:5px;display:none' />");
    }
    var suburbSelectorDiv = $("<div style='display:inline-block' />");
    var currentSuburbSelector = $("<input type='radio' name='templateSuburbOption' id='commCurrentSuburbRadioBtn' value='unchecked'>Current Suburb</input>");
    var commCurrentSuburbRadioBtnContainer = $("<div id='commCurrentSuburbRadioBtnContainer' style='margin-left:5px;display:inline-block' />").append(currentSuburbSelector);
    var allSuburbSelector = $("<input type='radio' name='templateSuburbOption' id='commAllSuburbsRadioBtn' value='unchecked' ><label>All Suburbs</label></input>");
    var allSuburbsOptionContainer = $("<div id='allSuburbsOptionContainer' style='margin-left:5px;display:inline-block' />").append(allSuburbSelector);
    suburbSelectorDiv.append(commCurrentSuburbRadioBtnContainer).append(allSuburbsOptionContainer);
    currentSuburbSelector.click(function () {
        allSuburbSelector.attr('value', 'unchecked');
        if ($(this).attr('value') == 'unchecked' && currentSuburb != null) {
            selectCurrentSuburbRadioBtn();
        } else {
            deselectCurrentSuburbRadioBtn();
        }
    });
    allSuburbSelector.click(function () {
        currentSuburbSelector.attr('value', 'unchecked');
        if ($(this).attr('value') == 'unchecked') {
            selectAllSuburbsRadioBtn();
        } else {
            deselectAllSuburbsRadioBtn();
        }
    });

    templateItemOptionsDiv.append(templateSelectionDropdown).append(deleteTemplateBtn).append(suburbSelectorDiv);
   
    return templateItemOptionsDiv;
}

function selectAllSuburbsRadioBtn() {
    $('#commAllSuburbsRadioBtn').attr('value', 'checked');
    $('#commAllSuburbsRadioBtn').prop('checked', true);
    commCustomSelectionEnabled = false;
    removeMarkersFromSelection();
    $("#commGetInfoLabel").css('display', 'block').text("The communication will be sent to all contacts across all your available suburbs, who have a default contact value");
    $("#commBottomDiv").css('display', 'block');
}

function deselectAllSuburbsRadioBtn() {
    $('#commAllSuburbsRadioBtn').attr('value', 'unchecked');
    $('#commAllSuburbsRadioBtn').prop('checked', false);
    commCustomSelectionEnabled = true;
    $("#commGetInfoLabel").text("");
    $("#commBottomDiv").css('display', 'none');
}

function selectCurrentSuburbRadioBtn() {
    $('#commCurrentSuburbRadioBtn').attr('value', 'checked');
    $('#commCurrentSuburbRadioBtn').prop('checked', true);
    commCustomSelectionEnabled = false;
    removeMarkersFromSelection();
    $("#commGetInfoLabel").css('display', 'block').text("The communication will be sent to all contacts in your current suburb, who have a default contact value");
    $("#commBottomDiv").css('display', 'block');
}

function deselectCurrentSuburbRadioBtn() {
    $('#commCurrentSuburbRadioBtn').attr('value', 'unchecked');
    $('#commCurrentSuburbRadioBtn').prop('checked', false);
    commCustomSelectionEnabled = true;
    $("#commGetInfoLabel").text("");
    $("#commBottomDiv").css('display', 'none');
}

function buildTemplateItems() {
    var templatesMenu = $("#templatesMenu");
    templatesMenu.empty();

    var myTemplates = buildCommMenuItem("template_menu_mytemplates", buildMyTemplatesItemContent(), handleMyTemplatesItemClick);
    templatesMenu.append(myTemplates);
    var standardTemplates = buildCommMenuItem("template_menu_standardtemplates", buildStandardTemplatesItemContent(), handleStandardTemplatesItemClick);
    templatesMenu.append(standardTemplates);
    if (communicationsMode == 'EMAIL') {
        var newsletterTemplate = buildCommMenuItem("template_menu_newslettertemplate", buildNewsletterTemplateItemContent(), handleNewsletterTemplateItemClick);
        templatesMenu.append(newsletterTemplate);
    }
}

function buildCommunicationMenuItems() {
    var menu = $("#communicationMenu");
    menu.empty();

    if (currentSuburb) {
        var polygonMenuItem = buildCommMenuItem("comm_menu_polygon", buildPolyMenuItemContent(), handlePolyMenuItemClick);
        menu.append(polygonMenuItem);

        if (userPolygons.length) {
            var removeAllPolysMenuItem = buildCommMenuItem("comm_menu_remove_polygons", buildRemovePolyMenuItemContent(), handleRemovePolyMenuItemClick);
            menu.append(removeAllPolysMenuItem);
        }

        var separator = buildCommMenuItem("separator", "-----------------------------------------", null);
        menu.append(separator);
    }

    var smsMessage = buildCommMenuItem("comm_menu_sms", buildSMSMenuItemContent(), handleSMSMessageClick);
    menu.append(smsMessage);

    var emailMessage = buildCommMenuItem("comm_menu_email", buildEmailMenuItemContent(), handleEmailMessageClick);
    menu.append(emailMessage);


    //if (currentSuburb) {
    //    var currentSubMenuItem = buildCommMenuItem("separator", currentSuburb.SuburbName, null);
    //    menu.append(currentSubMenuItem);

    //    var todaysBdaysSmsForSuburb = buildCommMenuItem("comm_menu_current_suburb_todays_birthdays_sms", buildTodaysBdaysSMS(), null);
    //    menu.append(todaysBdaysSmsForSuburb);

    //    var todaysBdaysEmailForSuburb = buildCommMenuItem("comm_menu_current_suburb_todays_birthdays_email", buildTodaysBdaysEmail(), null);
    //    menu.append(todaysBdaysEmailForSuburb);

    //    var todaysAnniversarySMSForSuburb = buildCommMenuItem("comm_menu_current_suburb_todays_anniversary_sms", buildTodaysAnniversarySMS(), null);
    //    menu.append(todaysAnniversarySMSForSuburb);

    //    var todaysAnniversaryEmailForSuburb = buildCommMenuItem("comm_menu_current_suburb_todays_anniversary_email", buildTodaysAnniversaryEmail(), null);
    //    menu.append(todaysAnniversaryEmailForSuburb);

    //    var separator = buildCommMenuItem("separator", "-----------------------------------------", null);
    //    menu.append(separator);
    //}

    //var allSuburbsMenuItem = buildCommMenuItem("separator", "All Suburbs", null);
    //menu.append(allSuburbsMenuItem);

    //var todaysBdaysSms = buildCommMenuItem("comm_menu_todays_birthdays_sms", buildTodaysBdaysSMS(), null);
    //menu.append(todaysBdaysSms);

    //var todaysBdaysEmail = buildCommMenuItem("comm_menu_todays_birthdays_email", buildTodaysBdaysEmail(), null);
    //menu.append(todaysBdaysEmail);

    //var todaysAnniversarySMS = buildCommMenuItem("comm_menu_todays_anniversary_sms", buildTodaysAnniversarySMS(), null);
    //menu.append(todaysAnniversarySMS);

    //var todaysAnniversaryEmail = buildCommMenuItem("comm_menu_todays_anniversary_email", buildTodaysAnniversaryEmail(), null);
    //menu.append(todaysAnniversaryEmail);
    
    if (communicationsMode == "SMS") {
        var separator = buildCommMenuItem("separator", "-----------------------------------------", null);
        menu.append(separator);

        var smsTemplate = buildCommMenuItem("comm_menu_sms_template", buildSMSTemplate(), handleSMSTemplateBtnClick);
        menu.append(smsTemplate);
    }

    if (communicationsMode == "EMAIL") {
        var separator = buildCommMenuItem("separator", "-----------------------------------------", null);
        menu.append(separator);

        var emailTemplate = buildCommMenuItem("comm_menu_email_template", buildEmailTemplate(), handleEmailTemplateBtnClick);
        menu.append(emailTemplate);
    }
}

function buildCommMenuItem(identifier, itemContent, onClickFunction) {
    var class_ = identifier != 'separator' ? "class='commMenuItem'" : "class='commMenuSeparator'";
    var containerDiv = $("<div id='" + identifier + "' " + class_ + " />");
    containerDiv.append(itemContent);
    containerDiv.click(function () { if (onClickFunction) { onClickFunction(); } });
    return containerDiv;
}

// Menu item content builders
function handleCommBtnClick() {
    var commBtn = $('#communicationBtn');
    var width = parseInt(commBtn.css('width').replace('px', ''));
    var position = commBtn.position();

    var menu = $('#communicationMenu');
    menu.append(buildCommunicationMenuItems());
    menu.css({'display': 'block', 'top': position.top, 'left': (position.left + width)});
}

function handleTemplatesBtnClick() {
    var templatesBtn = $("#templatesBtn");
    var width = parseInt(templatesBtn.css('width').replace('px', ''));
    var position = templatesBtn.position();

    var templatesMenu = $('#templatesMenu');
    templatesMenu.append(buildTemplateItems());
    templatesMenu.css({ 'display': 'block', 'top': position.top, 'left': (position.left + width) });
}

function buildMyTemplatesItemContent() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/my_templates.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px' />").append("My Templates");

    return container.append(iconDiv).append(textDiv);
}

function buildStandardTemplatesItemContent() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/standard_templates.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px' />").append("Standard Templates");

    return container.append(iconDiv).append(textDiv);
}

function buildNewsletterTemplateItemContent() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/newsletter_template.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px;padding-right:10px' />").append("Newsletter Template");

    return container.append(iconDiv).append(textDiv);
}

function buildPolyMenuItemContent() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/poly_menu_item.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px' />").append("Shape");

    return container.append(iconDiv).append(textDiv);
}

function buildRemovePolyMenuItemContent() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/poly_menu_remove.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px' />").append("Remove Shapes");

    return container.append(iconDiv).append(textDiv);
}

function buildSMSTemplate() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/comm_sms.png' />");
    var iconDiv2 = $("<div style='display:inline-block;float:left;padding-left:5px'/>");
    iconDiv2.append("<img src='Assets/item_template.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px' />").append("SMS Templates");

    return container.append(iconDiv).append(iconDiv2).append(textDiv);
}

function buildEmailTemplate() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/comm_email.png' />");
    var iconDiv2 = $("<div style='display:inline-block;float:left;padding-left:5px'/>");
    iconDiv2.append("<img src='Assets/item_template.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px' />").append("Email Templates");

    return container.append(iconDiv).append(iconDiv2).append(textDiv);
}

function buildSMSMenuItemContent() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/comm_sms.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px' />").append("SMS Message");

    return container.append(iconDiv).append(textDiv);
}

function buildEmailMenuItemContent() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/comm_email.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px' />").append("Email Message");

    return container.append(iconDiv).append(textDiv);
}

function buildTodaysBdaysSMS() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/comm_bday.png' />");
    var iconDiv2 = $("<div style='display:inline-block;float:left;padding-left:5px'/>");
    iconDiv2.append("<img src='Assets/comm_sms.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px' />").append("Today's Birthdays (SMS)");

    return container.append(iconDiv).append(iconDiv2).append(textDiv);
}

function buildTodaysBdaysEmail() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/comm_bday.png' />");
    var iconDiv2 = $("<div style='display:inline-block;float:left;padding-left:5px'/>");
    iconDiv2.append("<img src='Assets/comm_email.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px' />").append("Today's Birthdays (Email)");

    return container.append(iconDiv).append(iconDiv2).append(textDiv);
}

function buildTodaysAnniversarySMS() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/comm_anniversary.png' />");
    var iconDiv2 = $("<div style='display:inline-block;float:left;padding-left:5px'/>");
    iconDiv2.append("<img src='Assets/comm_sms.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px' />").append("Today's Anniversaries (SMS)");

    return container.append(iconDiv).append(iconDiv2).append(textDiv);
}

function buildTodaysAnniversaryEmail() {
    var container = $("<div />");
    var iconDiv = $("<div style='display:inline-block;float:left'/>");
    iconDiv.append("<img src='Assets/comm_anniversary.png' />");
    var iconDiv2 = $("<div style='display:inline-block;float:left;padding-left:5px'/>");
    iconDiv2.append("<img src='Assets/comm_email.png' />");
    var textDiv = $("<div style='display:inline-block;padding-left:22px' />").append("Today's Anniversaries (Email)");

    return container.append(iconDiv).append(iconDiv2).append(textDiv);
}

function buildSMSContentContainer() {
    var contentContainer = $("<div />");

    var symbolsDropdown = $("<select id='smsSymbolsDropdown' />")
                            .append($("<option value='-1'>--- Symbols ---</option>"))
                            .append($("<option value='*title*'>Person title</option>"))
                            .append($("<option value='*name*'>Person name</option>"))
                            .append($("<option value='*surname*'>Person surname</option>"))
                            .append($("<option value='*address*'>Property address</option>"));
   
    var msgDiv = $("<textarea id='smsMessageContainer' style='border: 1px solid gray;width:100%;height:80px;color:lightgray;' />");
    var introText = "Use the 'Symbols' drop-down to insert placeholder information.";
    msgDiv.append(introText);

    msgDiv.focus(function () {
        if (msgDiv.val() != introText)
            return;

        msgDiv.empty();
        msgDiv.css('color', 'black');
    });

    msgDiv.keyup(updateCostOfBatchSMS);
    //msgDiv.change(updateCostOfBatchSMS);
    symbolsDropdown.change(function () {
        if (msgDiv.val() == introText) {
            msgDiv.empty();
            msgDiv.css('color', 'black');
        }
        var option = $(this).val();
        if (option != '-1') {
            var position = msgDiv.getCursorPosition();
            var content = msgDiv.val();
            var newContent = content.substr(0, position) + option + content.substr(position);
            msgDiv.val(newContent);
        }
    });

    var saveAsTemplateBtn = $("<input type='button' id='saveAsTemplateBtn' style='margin-right:5px;' value='Save As Template' />");

    contentContainer.append(symbolsDropdown).append("<br />").append(msgDiv).append("<br />").append(saveAsTemplateBtn);

    saveAsTemplateBtn.click(function () {
        showSaveAsTemplateDialog();
    });

    return contentContainer;
}

function buildEmailContentContainer() {
    var emailContainer = $("<div />");

    var subjectLine = $("<input type='text' id='emailSubject' style='width:100%;border:1px solid gray' />");
    subjectLine.val(defaultSubjectText);
    subjectLine.focus(function () {
        if (subjectLine.val() != defaultSubjectText) {
            return;
        }
        subjectLine.val('');
    });

    subjectLine.keyup(function () {
        var val = $(this).val();
        if (val == '') {
            subjectLine.css('border', '1px solid red');
        }
        else {
            subjectLine.css('border', '1px solid gray');
            tooltip.hide();
        }
    });

    var body = $("<textarea id='emailMessageBody' name='emailMessageBody' style='width:100%;height:120px;padding-bottom:1px' />");
    emailContainer.append(subjectLine).append("<p />").append(body);
    emailBodyInitialised = false;
    //var myToolbar = [
    //    //{ name: 'document', items: ['Source', '-', 'Save', 'NewPage', 'DocProps', 'Preview', 'Print', '-', 'Templates'] },
    //    { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
    //    { name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker', 'Scayt'] },
    //    //{ name: 'forms', items: ['Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton', 'HiddenField'] },
    //    //'/',
    //    { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline'/*, 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'*/] },
    //    { name: 'paragraph', items: ['NumberedList', /*'BulletedList', '-', */'Outdent', 'Indent', '-', /*'Blockquote', 'CreateDiv', '-', */'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'/*, '-', 'BidiLtr', 'BidiRtl'*/]},
    //    { name: 'links', items: ['Link'/*, 'Unlink', 'Anchor'*/] },
    //    { name: 'insert', items: ['Image', /*'Flash', */'Table'/*, 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', 'Iframe'*/] },
    //    //'/',
    //    { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
    //    //{ name: 'colors', items: ['TextColor'/*, 'BGColor'*/] },
    //    { name: 'tools', items: ['Maximize', 'ShowBlocks'/*, '-', 'About'*/, 'custom'] },
    //];
    //var config = { toolbar_defaultToolbar: myToolbar, toolbar: 'defaultToolbar', allowedContent: true };
    var editor = body.ckeditor(/*config,*/ function (textarea) {
        $(body).val(defaultBodyText);
        //attachFilesDiv.css('display', 'inline-block');
        //saveAsTemplateBtn.css('display', 'block');
    });
           
    //var focusManager = new CKEDITOR.focusManager(CKEDITOR.instances.emailMessageBody);
    CKEDITOR.instances.emailMessageBody.on('focus', function (evt) {

        if (!emailBodyInitialised) {
            emailBodyInitialised = true;
            $(body).val('');
        }

        if (subjectLine.val() == '' || subjectLine.val() == defaultSubjectText) {
            subjectLine.css('border', '1px solid red');
            //CKEDITOR.instances.emailMessageBody.focusManager.blur();
            evt.editor.focusManager.blur(true);
            subjectLine.focus();
            var subjectLineElement = document.getElementById('emailSubject');
            try {
                tooltip.pop(subjectLineElement, 'Please enter a subject...', { showDelay: 1, hideDelay: 100, calloutPosition: 0.5 });
            } catch (e) { }
        }
    });

    var saveAsTemplateBtn = $("<input type='button' id='saveAsTemplateBtn' style='margin-right:5px;' value='Save As Template' />");

    var attachFilesDiv = $("<div style='display:inline-block;padding-top:0' />");
    var attachFileBtn = $("<input type='button' id='attachFilesBtn' style='display:inline-block;margin-right:10px' value='Attach..' />");
    var attachFileList = $("<div id='attachFilesList' style='display:inline-block;'  />");
    var attachFileOperation = $("<input id='attachFilesOp' type='file' name='attachFilesOp' style='display: none;' />");
    attachFilesDiv.append(attachFileBtn).append(attachFileList).append(attachFileOperation);
    attachFileBtn.click(handleShowFileUploadDialog);

    emailContainer.append("<br />").append(saveAsTemplateBtn).append(attachFilesDiv);

    saveAsTemplateBtn.click(function () {
        showSaveAsTemplateDialog();
    });

    return emailContainer;
}

function showSaveAsTemplateDialog() {
    var validationErrorMsg = '';
    if (communicationsMode == "EMAIL") {
        var emailSubject = $('#emailSubject').val().trim();
        var emailBody = $('#emailMessageBody').val().trim();
        if (emailBody == '' || emailSubject == '' || emailSubject == 'Subject') {
            validationErrorMsg = "Please supply a valid subject and body for the email before saving the template.";
        }
    }
    if (communicationsMode == "SMS") {
        var smsBody = $("#smsMessageContainer").val().trim();
        if (smsBody == '') {
            validationErrorMsg = 'Please supply message content before saving the template.';
        }
    }

    if (validationErrorMsg != '') {
        alert(validationErrorMsg);
        return;
    }
    var dialogHandle = $("<div title='Save Content As Template' style='display:none;font-family:Verdana;font-size:12px;'></div>");
    dialogHandle.empty();
    var dialogContent = '';
    if (communicationsMode == "EMAIL") {
        dialogContent = "<div>This message will be saved as a template message for future re-use.<br /> \
                                 The subject text will be used as the template name.<br /><br /> Please note that any existing template with the same name will be overwritten.</div>";
    }
    var smsTemplateName = $("<input type='text' id='smsTemplateName' style='width:200px' value='' />");
    if (communicationsMode == "SMS") {
        dialogContent = $("<div />")
                        .append("This message will be saved as a template message for future re-use.")
                        .append("<p />")
                        .append("Please enter a name for this template: ")
                        .append(smsTemplateName)
                        .append("<p />")
                        .append("Note: Any existing template with the same name will be overwritten.");
    }
    dialogHandle.append(dialogContent);

    dialogHandle.dialog(
      {
          modal: true,
          closeOnEscape: true,
          width: 'auto',
          open: function () {
              smsTemplateName.val('');
          },
          buttons: {
              "Save and close": function () {
                  if (communicationsMode == "SMS") {
                      var templateName = smsTemplateName.val();
                      if (!templateName) {
                          alert('You must specify a name for the template.');
                          return;
                      }
                  }
                  saveUserTemplate(smsTemplateName.val());
                  $(this).dialog("close");
              }
          },
          position: ['center', 'center']
      });
}

function handleShowFileUploadDialog() {
    
    if (uploadedFiles.length == 1) {
        alert('You currently restricted to only one attachment per message.');
        return;
    }

    $('#attachFilesOp').unbind('change').bind('change', handleGetFiles);
    $('#attachFilesOp').trigger('click');

    function handleGetFiles(evt) {
        var files = evt.target.files;
        $.each(files, function (idx, file) {
            var reader = new FileReader();
            reader.onload = (function (theFile) {
                return function (e) {
                    //debugger;
                    createUploadFile(e, theFile);
                };
            })(file);

            // Read in the image file as a data URL.
            reader.readAsDataURL(file);
        });
    }

    function createUploadFile(e, file) {
        // creates an object, adds it to the array, adds a UI item to the list
        var attachmentId = generateUniqueID();
        var dataStringOffset = e.currentTarget.result.indexOf('base64,');
        var data = e.currentTarget.result.substring(dataStringOffset+7, e.currentTarget.result.length);
        var uploadFile = { fileId: attachmentId, type: file.type, name: file.name, base64: data };
        uploadedFiles.push(uploadFile);

        var cancelIconBtn = $("<img src='Assets/remove_poly.png' style='cursor:pointer;display:inline-block;vertical-align:middle;padding-left: 5px' />");
        var fileUploadItem = $("<div id='" + attachmentId + "' style='border:1px solid black;background-color:#CCCCFF;padding:2px;display:inline-block;margin-right:5px;border-radius:3px' />")
            .append('<span style="display:inline-block;vertical-align:middle">"' + file.name + '"</span>').append(cancelIconBtn);

        cancelIconBtn.click(function () {
            var itemId = fileUploadItem.attr("id");
            // Remove item from array
            var index = -1;
            $.each(uploadedFiles, function (idx, el) {
                if (el.fileId == itemId) {
                    index = idx;
                }
            });
            uploadedFiles.splice(index, 1);
            fileUploadItem.remove();
        });

        $('#attachFilesList').append(fileUploadItem);
    }
}

function handleMyTemplatesItemClick() {
    removeMarkersFromSelection();
    commCustomSelectionEnabled = true;
    buildTemplateItemOptionsDiv('my_template');
    $('#allSuburbsOptionContainer').hide();
    loadTemplates('get_user_template_list', null, function (results) {
        $('#templateSelector').append("<option value='-1'>--- My Templates ---</option>");
        $.each(results, function (idx, result) {
            $('#templateSelector').append("<option value='" + idx + "'>" + result.TemplateName + "</option>");
        });
    });
    $('#templateSelector').unbind('change').bind('change', function () {
        var selectedIndex = $('#templateSelector').val();
        if (selectedIndex > -1) {
            var templateName = $('#templateSelector option:selected').text();
            loadTemplates('get_user_template', templateName, function (result) {
                populateTemplateContent(result);
            });

            $('#deleteTemplateBtn').css('display', 'inline-block');
        } else {
            populateTemplateContent({ TemplateName: "", TemplateContent: "", TemplateActivityTypeId: null });

            $('#deleteTemplateBtn').css('display', 'none');
        }
    });

    $('#deleteTemplateBtn').unbind('click').bind('click', function () {
        var templateName = $('#templateSelector option:selected').text();
        deleteTemplate(templateName, function () {
            // splash screen and update UI
            populateTemplateContent({ TemplateName: "", TemplateContent: "", TemplateActivityTypeId: null });
            //buildTemplateItemOptionsDiv('my_template');
            handleMyTemplatesItemClick();
        });
    });
}

function handleStandardTemplatesItemClick() {
    removeMarkersFromSelection();
    commCustomSelectionEnabled = true;
    buildTemplateItemOptionsDiv();
    $('#allSuburbsOptionContainer').hide();
    loadTemplates('get_system_template_list', null, function (results) {
        $('#templateSelector').append("<option value='-1'>--- Standard Templates ---</option>");
        $.each(results, function (idx, result) {
            $('#templateSelector').append("<option value='" + idx + "'>" + result.TemplateName + "</option>");
        });
    });
    $('#templateSelector').unbind('change').bind('change', function () {
        var selectedIndex = $('#templateSelector').val();
        if (selectedIndex > -1) {
            var templateName = $('#templateSelector option:selected').text();
            loadTemplates('get_system_template', templateName, function (result) {
                populateTemplateContent(result);
                if (specialActivityTemplates.indexOf(result.ActivityName) > -1) {
                    $('#allSuburbsOptionContainer').show();
                    selectAllSuburbsRadioBtn();
                    $('#commCurrentSuburbRadioBtnContainer').hide();
                    showDialogSpecialTemplateSelected();
                } else {
                    var checkedVal = $('#commAllSuburbsRadioBtn').attr('value');
                    if (checkedVal == 'checked') {
                        deselectAllSuburbsRadioBtn();
                    }
                    $('#allSuburbsOptionContainer').hide();
                    $('#commCurrentSuburbRadioBtnContainer').show();
                }
            });
        } else {
            var checkedVal = $('#commAllSuburbsRadioBtn').attr('value');
            if (checkedVal == 'checked') {
                deselectAllSuburbsRadioBtn();
            }
            checkedVal = $('#commCurrentSuburbRadioBtn').attr('value');
            if (checkedVal == 'checked') {
                deselectCurrentSuburbRadioBtn();
            }
            $('#allSuburbsOptionContainer').hide();
            $('#commCurrentSuburbRadioBtnContainer').hide();
            populateTemplateContent({ TemplateName: "", TemplateContent: "", TemplateActivityTypeId: null });
        }
    });
}

function currentOrAllSuburbsSelected() {
    var currentSub = $('#commCurrentSuburbRadioBtn').is(':checked');
    var allSubs =  $('#commAllSuburbsRadioBtn').is(':checked');
    return currentSub == true || allSubs == true;
}

function showDialogSpecialTemplateSelected() {
    //if (currentOrAllSuburbsSelected()) {
    //    return; // Either option is selected.
    //}

    var specialActivityTemplateDiv = $("<div title='Special Template' style='font-family:Verdana;font-size:12px;' />");
    specialActivityTemplateDiv.empty().append("<span>Please note that this type of template applies specifically to the 'All Suburbs' option.</span>");

    specialActivityTemplateDiv.dialog(
  {
      modal: true,
      closeOnEscape: true,
      width: '400',
      buttons: { "Ok": function () { $(this).dialog("close"); removeMarkersFromSelection(); } },
      position: ['center', 'center']
  });
}

function handleNewsletterTemplateItemClick() {
    removeMarkersFromSelection();
    commCustomSelectionEnabled = true;
    buildTemplateItemOptionsDiv();
    $('#allSuburbsOptionContainer').hide();
    var date = toTitleCase(getMonthName()) + ' ' + new Date().getFullYear();
    $('#templateSelector').empty().append("<option value='-1'>Newsletter - " + date + "</option>");

    loadTemplates('get_newsletter', null, function (result) {
        populateTemplateContent(result);
    });
}

function populateTemplateContent(templateItem) {
    selectedTemplateActivityTypeId = templateItem.TemplateActivityTypeId;
    if (communicationsMode == "EMAIL") {
        $('#emailSubject').val(templateItem.TemplateName);
        var txt = document.createElement("textarea");
        txt.innerHTML = templateItem.TemplateContent;

        CKEDITOR.instances.emailMessageBody.setData(txt.value);
        emailBodyInitialised = true;
    }
    if (communicationsMode == "SMS") {
        var txt = document.createElement("textarea");
        txt.innerHTML = templateItem.TemplateContent;
        $("#smsMessageContainer").css('color', 'black');
        $("#smsMessageContainer").val(txt.value);
    }
}

// Menu item click handlers
function handlePolyMenuItemClick() {
    if (!commCustomSelectionEnabled) {
        showDialogCommCustomSelectionDisabled();
        return;
    }

    toggleDrawingMode();
}

function handleRemovePolyMenuItemClick() {
    removePolygonsWithMarkers();
}

function handleEmailTemplateBtnClick() {
    $("#templateOptionsDiv").css('display', 'block');
}

function handleSMSTemplateBtnClick() {
    $("#templateOptionsDiv").css('display', 'block');
}

function handleSMSMessageClick() {
    uploadedFiles.length = 0;
    communicationsMode = "SMS";
    var container = $("#messageContentContainer");
    container.empty();
    var templateContainer = $("#templateItemOptionsDiv");
    templateContainer.empty();
    container.append(buildSMSContentContainer());

    updateCommunicationsContacts();

    commCustomSelectionEnabled = true;
}

function handleEmailMessageClick() {
    uploadedFiles.length = 0;
    communicationsMode = "EMAIL";
    var container = $("#messageContentContainer");
    container.empty();
    var templateContainer = $("#templateItemOptionsDiv");
    templateContainer.empty();
    container.append(buildEmailContentContainer());

    updateCommunicationsContacts();

    commCustomSelectionEnabled = true;
}

function handleEnableComms() {
    // you are about to enter comms..restart if made changes/edits. + when re-prospecting warn existing contact will be removed (create acticvity with primary contact details).
    // And of course the suburb selection screen.
    var commWarningDialog = $("<div id='commWarningDialog' title='Communications Mode' style='font-family:Verdana;font-size:12px;' />");
    commWarningDialog.empty().append('You have entered communications mode. Please note that if you have made changes to property and/or contact information during this session, \
                                     it is recommended that you restart Prospecting to ensure that the communications system picks up the latest information.');
    commWarningDialog.dialog(
  {
      modal: true,
      closeOnEscape: true,
      width: '450',
      buttons: { "Ok": function () {  $(this).dialog("close"); } },
      position: ['center', 'center']
  });

    toggleMultiSelectMode(true);
}

function updateCommunicationsContacts() {
    var commGetInfoLabel = $("#commGetInfoLabel");
    commGetInfoLabel.css('display', 'none');
    var commBottomDiv = $("#commBottomDiv");
    commBottomDiv.css('display', 'none');
    var commCostOfBatch = $('#commCostOfBatch');
    commCostOfBatch.css('display', 'none');

    var contactsContainer = $("#contactablesContentContainer");
    if (currentSuburb && communicationsMode != null && selectedMarkers.length) {
        getContactsFromSelectedMarkers(function (contacts) {
            if (contacts.length) {
                contactsContainer.css('display', 'block');
                buildContactsBody(contacts, true);

                if (communicationsMode == "SMS") {
                    updateCostOfBatchSMS();
                }
                commBottomDiv.css('display', 'block');
            } else {
                contactsContainer.css('display', 'none');
                if (communicationsMode == "SMS") {
                    commGetInfoLabel.text("Select properties with contacts who have a cellphone number.");
                }
                if (communicationsMode == "EMAIL") {
                    commGetInfoLabel.text("Select properties with contacts who have an email address.");
                }
                commGetInfoLabel.css('display', 'block');
            }
        }); //display a message saying "please select contacts with an email" or  "mobile number". NB also CHECK for AND USE only cell numbers
    }
    else {
        if (currentOrAllSuburbsSelected()) {
            commBottomDiv.css('display', 'block');
        }
        var commContactsTable = $("#commContactsTable");
        commContactsTable.find("tr").remove();
        contactsContainer.css('display', 'none');
    }
}

function updateCostOfBatchSMS() {

    var smsCost = prospectingContext.SMSCost * 100;
    var smsLen = prospectingContext.SMSLength;

    var commSelectedRows = $('#commContactsTable tr.rowSelected').length;
    var numChars = $("#smsMessageContainer").val().trim().length + smsOptOut.length;
    var c1 = numChars / smsLen;
    var c2 = Math.ceil(c1);
    var calc = c2 * smsCost * commSelectedRows;

    var labelContainer = $("<div style='font-size:12px' />").append("Cost of batch (R " + prospectingContext.SMSCost + " per " + smsLen + " characters): R " + (calc / 100));
    
    var commCostOfBatch = $('#commCostOfBatch');
    commCostOfBatch.empty();
    commCostOfBatch.append(labelContainer);
    commCostOfBatch.css('display', 'block');
}

function getContactFromId(contactPersonId) {
    var result = null;
    $.each(currentSuburb.ProspectingProperties, function (idx1, pp) {
        if (pp.Contacts != null) {
            $.each(pp.Contacts, function (idx2, con) {
                if (con.ContactPersonId == contactPersonId) result = con;
            });
        }
    });

    return result;
}

function buildContactsBody(contacts, selectCells) {
    var commContactsTable = $("#commContactsTable");
    commContactsTable.find("tr").remove();
    var body = $("<tbody></tbody>");

    var uniqueItems = [];
    $.each(contacts, function (idx, c) {
        var rowId = c.ContactPersonId; // TODO: This actually causes a bug when the same person owns multiple selected properties. Fix by creating a key using ContactPersonId + LightstoneId
        var tr = $("<tr id='comm_row_" + rowId + "' ></tr>");

        var contactDetailContent, contactDetailTitle = '', actionStatus = 'Ready';
        var rowIsSelected = 'checked';
        if (c.EmailSubmitted) {
            actionStatus = 'Submitted';
        }
        if (c.SmsSubmitted) {
            actionStatus = 'Submitted';
        }
        if (c.SendError) {
            actionStatus = 'Error';
        }
        if (c.IsPOPIrestricted) {
            actionStatus = 'Opt-out';
        }
        if (c.SMSOptout && communicationsMode == "SMS") {
            actionStatus = 'Opt-out';
        }
        if (c.EmailOptout && communicationsMode == "EMAIL") {
            actionStatus = 'Opt-out';
        }
        if (communicationsMode == "SMS") {
            // Find a cell nr marked as the default
            var defaultCell;
            var allCellNos = $.grep(c.PhoneNumbers, function (cell) {
                return cell.ItemType == cellphoneItemId;
            });
            if (allCellNos.length == 1) {
                defaultCell = allCellNos[0];
            } else {
                defaultCell = $.grep(allCellNos, function (cell) {
                    return cell.IsPrimary == true;
                })[0];
            }
            if (defaultCell) {
                contactDetailContent = defaultCell.ItemContent;
                contactDetailTitle = contactDetailContent;

                rowIsSelected = uniqueItems.indexOf(defaultCell.ItemContent) > -1 ? '' : 'checked';
                uniqueItems.push(defaultCell.ItemContent);

                if (c.SmsSubmitted) {
                    rowIsSelected = '';
                    tr.css('background-color', 'lightgreen');
                }
                if (c.SendError) {
                    tr.css('background-color', '#CC0000');
                }
            } else {
                tr.addClass('noDefault');
                var cellSelectCombo = $("<select id='comm_default_cell_select_" + rowId + "' style='width:95%' />");
                cellSelectCombo.append("<option value='-1'></option>");
                var cellNumbers = $.grep(c.PhoneNumbers, function (ph) {
                    return ph.ItemType == cellphoneItemId;
                });
                $.each(cellNumbers, function (idx2, cell) {
                    cellSelectCombo.append("<option value='comm_default_cell_no_" + cell.ItemId + "' >" + cell.ItemContent + "</option>");
                });
                cellSelectCombo.change(function (e) {
                    var item = $(this).val();
                    var titleText = $(this).find('option:selected').text();
                    var itemId = item.replace('comm_default_cell_no_', '');
                    var actionStatusTd = $('#comm_action_' + rowId);
                    if (itemId != '-1') {
                        handleMakeDefaultCellNo(itemId, function () {
                            cellSelectCombo.attr('title', titleText);

                            $.each(allCellNos, function (idx3, cell) {
                                cell.IsPrimary = false;
                            });
                            // Find and set the primary detail on the contact
                            var primaryCell = $.grep(c.PhoneNumbers, function (cell) {
                                return cell.ItemId == itemId;
                            })[0];
                            primaryCell.IsPrimary = true;
                            actionStatusTd.html("Ready");
                            tr.removeClass('noDefault');
                            tr.css('background-color', 'white');
                        });
                    } else {
                        cellSelectCombo.attr('title', '');
                        actionStatusTd.html("No default");
                        tr.css('background-color', '#FFEB99');
                    }
                });

                contactDetailContent = cellSelectCombo;
                actionStatus = 'No default';
                tr.css('background-color', '#FFEB99');
            }
        } else {
            var defaultEmail;
            var allEmails = c.EmailAddresses;
            if (allEmails.length == 1) {
                defaultEmail = allEmails[0];
            } else {
                defaultEmail = $.grep(allEmails, function (email) {
                    return email.IsPrimary == true;
                })[0];
            }
            if (defaultEmail) {
                contactDetailContent = defaultEmail.ItemContent;
                contactDetailTitle = contactDetailContent;

                rowIsSelected = uniqueItems.indexOf(defaultEmail.ItemContent) > -1 ? '' : 'checked';
                uniqueItems.push(defaultEmail.ItemContent);

                if (c.EmailSubmitted) {
                    rowIsSelected = '';
                    tr.css('background-color', 'lightgreen');
                }
                if (c.SendError) {
                    tr.css('background-color', '#CC0000');
                }
            } else {
                tr.addClass('noDefault');
                var emailSelectCombo = $("<select id='comm_default_email_select_" + rowId + "' style='width:95%' />");
                emailSelectCombo.append("<option value='-1'></option>");
                $.each(allEmails, function (idx2, email) {
                    emailSelectCombo.append("<option value='comm_default_email_address_" + email.ItemId + "' >" + email.ItemContent + "</option>");
                });
                emailSelectCombo.change(function (e) {
                    var item = $(this).val();
                    var titleText = $(this).find('option:selected').text();
                    var itemId = item.replace('comm_default_email_address_', '');
                    var actionStatusTd = $('#comm_action_' + rowId);
                    if (itemId != '-1') {
                        handleMakeDefaultEmailAddress(itemId, function () {
                            emailSelectCombo.attr('title', titleText);

                            $.each(allEmails, function (idx3, email) {
                                email.IsPrimary = false;
                            });
                            // Find and set the primary detail on the contact
                            var primaryEmail = $.grep(c.EmailAddresses, function (email) {
                                return email.ItemId == itemId;
                            })[0];
                            primaryEmail.IsPrimary = true;
                            actionStatusTd.html("Ready");
                            tr.removeClass('noDefault');
                            tr.css('background-color', 'white');
                        });
                    } else {
                        emailSelectCombo.attr('title', '');
                        actionStatusTd.html("No default");
                        tr.css('background-color', '#FFEB99');
                    }
                });

                contactDetailContent = emailSelectCombo;
                actionStatus = 'No default';
                tr.css('background-color', '#FFEB99');
            }
        }
        if (c.SMSOptout && communicationsMode == "SMS") {
            rowIsSelected = '';
        }
        if (c.EmailOptout && communicationsMode == "EMAIL") {
            rowIsSelected = '';
        }
        if (c.IsPOPIrestricted) {
            rowIsSelected = '';
        }
        if (!selectCells) {
            rowIsSelected = '';
        }

        var checkboxDisabled = '';
        if (c.SMSOptout && communicationsMode == "SMS") {
            checkboxDisabled = 'disabled';
        }
        if (c.EmailOptout && communicationsMode == "EMAIL") {
            checkboxDisabled = 'disabled';
        }

        var isSelectedCell = $("<td class='commTableRow30' id='comm_selected_" + rowId + "' ></td>").append($("<input type='checkbox' id='comm_selected_checkbox_" + rowId + "' name='comm_selected_checkbox_" + rowId + "' value='' " + rowIsSelected + " " + checkboxDisabled + " />"));
        var name = $("<td class='commTableRow' id='comm_fullname_" + rowId + "' title='" + c.Firstname + ' ' + c.Surname + "' ></td>").append(toTitleCase(c.Firstname) + ' ' + toTitleCase(c.Surname));
        var address = $("<td class='commTableRow120' id='comm_address_" + rowId + "' title='" + c.PropertyAddress + "' ></td>").append(c.PropertyAddress); // change for SS (unit no) + ordering
        var contact = $("<td class='commTableRow' id='comm_contactdetail_" + rowId + "' title='" + contactDetailTitle + "' ></td>").append(contactDetailContent); // change
        var action = $("<td class='commTableRow' id='comm_action_" + rowId + "' ></td>").append(actionStatus);
        var editBtn = $("<a href='' id='comm_edit_contact_" + rowId + "' style='text-decoration:underline!important;'>Edit</a>");
        var edit = $("<td class='commTableRow' id='comm_edit_" + rowId + "' ></td>").append(editBtn);

        tr.append(isSelectedCell).append(name).append(address).append(contact).append(action).append(edit);
        body.append(tr);

        if (rowIsSelected == 'checked') {
            tr.addClass('rowSelected');
        }

        // Event handlers 
        isSelectedCell.change(function () {
            if (!$(this).find('input').is(':checked')) {
                tr.css('background-color', '#E8E8E8');
                tr.removeClass('rowSelected');
            } else {
                if (tr.hasClass('noDefault')) {
                    tr.css('background-color', '#FFEB99');
                } else {
                    tr.css('background-color', 'white');
                }

                tr.addClass('rowSelected');
            }

            if (communicationsMode == "SMS") {
                updateCostOfBatchSMS();
            }
        });

        editBtn.click(function (e) {
            e.preventDefault();
            handleCommEditBtnClick(c, tr);
        });
    });
    commContactsTable.append(body);
}

function handleMakeDefaultCellNo(itemId, callbackFn) {
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({
            Instruction: 'make_default_contact_detail',
            ItemId: itemId
        }),
        dataType: "json",
    }).done(function (data) {
        if (!handleResponseIfServerError(data)) {
            return;
        }

        callbackFn();
    });
}

function handleMakeDefaultEmailAddress(itemId, callbackFn) {
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({
            Instruction: 'make_default_contact_detail',
            ItemId: itemId
        }),
        dataType: "json",
    }).done(function (data) {
        if (!handleResponseIfServerError(data)) {
            return;
        }

        callbackFn();
    });
}

function handleCommEditBtnClick(contact, row) {
    var dialogHandle = $("<div title='Edit Contact|Property Details' style='display:none;font-family:Verdana;font-size:12px;'></div>");
    dialogHandle.empty();
    var title = $("<label class='fieldAlignmentExtraShortWidth'>Title:</label>");
    var titleCombo = buildContactTitle();
    var firstname = $("<label class='fieldAlignmentExtraShortWidth'>First name:</label><input type='text' id='commContactFirstnameEdit' name='commContactFirstnameEdit' size='35' value='" + contact.Firstname + "' />");
    var surname = $("<label class='fieldAlignmentExtraShortWidth'>Surname:</label><input type='text' id='commContactSurnameEdit' name='commContactSurnameEdit' size='35' value='" + contact.Surname + "' />");
    var property = $.grep(currentSuburb.ProspectingProperties, function (pp) {
        return pp.LightstonePropertyId == contact.TargetLightstonePropertyId;
    })[0];
    var streetNo = $("<label class='fieldAlignmentExtraShortWidth'>Street no.:</label><input type='text' id='commContactStreetNoEdit' name='commContactStreetNoEdit' size='5' value='" + property.StreetOrUnitNo + "' />");
    var streetAddressLbl = $("<label class='fieldAlignmentExtraShortWidth'>Street Address:</label>");
    var streetAddress = $("<input type='text' id='commContactStreetAddressEdit' name='commContactStreetAddressEdit' value='" + property.PropertyAddress + "' size='35' />");
    var convertToTitleCase = $("<input type='button' value='Convert names to Title Case' />");

    var ssDoor = null;
    if (property.SS_FH == 'SS' || property.SS_FH == 'FS') {
        var ssNameLbl = $("<label> (" + formatPropertyAddressTitleCase(property.SSName) + ")</label>");
        var ssDoorLbl = $("<label class='fieldAlignmentExtraShortWidth'>SS Door no.:</label>");
        var doorNo = property.SSDoorNo != null ? property.SSDoorNo : '';
        ssDoor = $("<input type='text' id='commContactSSDoorEdit' name='commContactSSDoorEdit' size='5' value='" + doorNo + "' />");

        dialogHandle.append(title)
            .append(titleCombo)
            .append("<br />")
            .append(firstname)
            .append("<br />")
            .append(surname)
            .append("<p />")
            .append(ssDoorLbl)
            .append(ssDoor)
            .append(ssNameLbl)
            .append("<p />")
            .append(convertToTitleCase);
    } else {
        dialogHandle.append(title)
            .append(titleCombo)
            .append("<br />")
            .append(firstname)
            .append("<br />")
            .append(surname)
            .append("<p />")
            .append(streetNo)
            .append("<br />")
            .append(streetAddressLbl)
            .append(streetAddress)
            .append("<p />")
            .append(convertToTitleCase);
    }

    function buildContactTitle() {
        //<input type='text' id='commContactTitleEdit' name='commContactTitleEdit' /> test SS, test address updates in grid
        var comboBox = $("<select id='commContactTitleEdit' name='commContactTitleEdit' />");
        if (!contact.Title) {
            comboBox.append("<option value='' />");
        }
        $.each(prospectingContext.ContactPersonTitle, function (idx, item) {
            comboBox.append("<option value='" + item.Key + "'>" + item.Value + "</option>");
        });
        //select rigth one + change event.
        if (contact.Title) {
            comboBox.val(contact.Title);
        }

        return comboBox;
    }

    convertToTitleCase.click(function () {
        var firstnameInput = firstname.next();
        var firstnameText = firstnameInput.val();
        firstnameText = toTitleCase(firstnameText);
        firstnameInput.val(firstnameText);

        var surnameInput = surname.next();
        var surnameText = surnameInput.val();
        surnameText = toTitleCase(surnameText);
        surnameInput.val(surnameText);

        var streetAddressNew = streetAddress.val();
        streetAddressNew = toTitleCase(streetAddressNew);
        streetAddress.val(streetAddressNew);
    });
    dialogHandle.dialog(
  {
      modal: true,
      closeOnEscape: true,
      //open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
      width: 'auto',
      buttons: { "Save and close": function () { handleSaveChanges(); $(this).dialog("close"); } },
      position: ['center', 'center']
  });

    function handleSaveChanges() {
        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Saving Changes...</p>' });
        // Name + surname
        var newFirstnameText = firstname.next().val();
        var newSurnameText = surname.next().val();
        // Title
        var newTitle = titleCombo.val();
        // Property address
        var newStreetNo = streetNo.next().val();
        var newStreetAddress = streetAddress.val();
        // SS door no
        var newSSDoorNo = ssDoor != null ? ssDoor.val() : null;
    
        if (newFirstnameText.trim() && newSurnameText.trim() && newStreetNo.trim() && newStreetAddress.trim()) {

            var allPersonContacts = [];
            $.each(currentSuburb.ProspectingProperties, function (idx1, pp) {
                if (pp.Contacts) {
                    $.each(pp.Contacts, function (idx2, pc) {
                        if (pc.ContactPersonId == contact.ContactPersonId) {
                            allPersonContacts.push(pc);
                        }
                    });
                }
            });

            $.each(allPersonContacts, function (idx, cc) {
                cc.Firstname = newFirstnameText.trim();
                cc.Surname = newSurnameText.trim();
                cc.Title = newTitle;
            });

            var prospectingPropId = contact.TargetCommPropertyId;
            var propertyDetailsInputPacket = {
                PropertyAddress: newStreetAddress,
                StreetOrUnitNo: newStreetNo,
                SSDoorNo: newSSDoorNo,
                SS_FH: property.SS_FH,
                ProspectingPropertyId: prospectingPropId
            };

            saveContact(contact, { ProspectingPropertyId: prospectingPropId }, function () {
                updateProspectingRecord(propertyDetailsInputPacket, property, function () {

                    contact.PropertyAddress = getFormattedAddress(property);

                    var addressTD = row.find('#comm_address_' + contact.ContactPersonId);
                    addressTD.empty().append(contact.PropertyAddress);
                });
            });
        }
    }
}

function handleSendMessage(callbackFn) {
    if (communicationsMode == "EMAIL") {
        submitEmails(callbackFn);
    }
    if (communicationsMode == "SMS") {
        submitSMS(callbackFn);
    }
}

function submitSMS(callbackFn) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Processing. Please wait...</p>' });
    var commSelectedRows = $('#commContactsTable tr.rowSelected');
    var recipients = [];
    $.each(commSelectedRows, function (idx, row) {
        var contactId = $(row).attr("id").replace('comm_row_', '');
        var contact = getContactFromId(contactId);
        var cell = getContactDetailFromContactRow(contactId);
        contact.TargetContactCellphoneNumber = cell;
        recipients.push(contact);
    });

    var batchNameRaw = encodeURIComponent(b64EncodeUnicode($('#batchFriendlyName').val()));
    var smsRequestPacket =
        {
            Instruction: 'submit_sms',
            Recipients: recipients,
            TargetCurrentSuburb: $('#commCurrentSuburbRadioBtn').is(':checked'),
            TargetAllMySuburbs: $('#commAllSuburbsRadioBtn').is(':checked'),
            CurrentSuburbId: currentSuburb != null ? currentSuburb.SuburbId : null,
            NameOfBatch: batchNameRaw,
            TemplateActivityTypeId: selectedTemplateActivityTypeId,
            UserSuburbIds: getUserSuburbsList(suburbsInfo),
            SmsBodyRaw: encodeURIComponent(b64EncodeUnicode($("#smsMessageContainer").val()))
        };

    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify(smsRequestPacket),
        dataType: "json"
    }).done(function (response) {
        $.unblockUI();
        if (response.SuccessfullySubmitted) {
            var submissionSuccessDialog = $("<div id='submissionSuccessDialog' title='Communication Batch Received' style='font-family:Verdana;font-size:12px;' />");
            submissionSuccessDialog.empty().append("Thank you, your request has been successfully received and enqueued for processing.");
            submissionSuccessDialog.dialog(
                                {
                                    modal: true,
                                    closeOnEscape: true,
                                    width: '400',
                                    height: '200',
                                    buttons: { "Ok": function () { $(this).dialog("close"); } },
                                    position: ['center', 'center']
                                });
            if (!currentOrAllSuburbsSelected() && recipients.length) {
                var contacts = [];
                var commAllRows = $('#commContactsTable > tbody > tr');
                $.each(commAllRows, function (idx, row) {
                    var contactId = $(row).attr("id").replace('comm_row_', '');
                    var contact = getContactFromId(contactId);
                    if ($(row).hasClass('rowSelected')) {
                        contact.SmsSubmitted = true;
                    }
                    contacts.push(contact);
                });
                buildContactsBody(contacts, false);
            }
            if (callbackFn) {
                callbackFn();
            }
        } else {
            alert("An error occurred submitting your request. Please contact support. Details of the error: " + response.ErrorMessage);
            return;
        }
    });
}

function getDefaultCellNo(contact) {
    var defaultCell;
    var allCellNos = $.grep(contact.PhoneNumbers, function (cell) {
        return cell.ItemType == cellphoneItemId;
    });
    if (allCellNos.length == 1) {
        defaultCell = allCellNos[0];
    } else {
        defaultCell = $.grep(allCellNos, function (cell) {
            return cell.IsPrimary == true;
        })[0];
    }

    return defaultCell.ItemContent;
}

function validateMessage() {
    // Validate person title (only if used in the message)
    function validateTitleIfPresent(msgBody) {
        var containsRecordsWithoutTitle = false;
        if (msgBody.indexOf('*title*') > -1) {
            var commSelectedRows = $('#commContactsTable tr.rowSelected');
            $.each(commSelectedRows, function (idx, row) {
                var contactId = $(row).attr("id").replace('comm_row_', '');
                var contact = getContactFromId(contactId);
                if (!contact.Title) {
                    containsRecordsWithoutTitle = true;
                }
            });
            if (containsRecordsWithoutTitle) {
                alert("One or more of your selected rows do not have a person title set. Please set a title for each of the selected contact people. (Hint: You can use the 'Edit' link in the last column to do this)");
                return false;
            }
        }
        return true;
    }

    function validateAddressIsValid(msgBody) {
        var containsRecordsWithInvalidAddress = false;
        if (msgBody.indexOf('*address*') > -1) {
            var commSelectedRows = $('#commContactsTable tr.rowSelected');
            $.each(commSelectedRows, function (idx, row) {
                var contactId = $(row).attr("id").replace('comm_row_', '');
                var contact = getContactFromId(contactId);
                if (contact.PropertyAddress.indexOf('n/a') == 0) {
                    containsRecordsWithInvalidAddress = true;
                }
            });
            if (containsRecordsWithInvalidAddress) {
                alert("One or more of your selected rows do not have a valid street number for the address. Please provide a street number for each address. (Hint: You can use the 'Edit' link in the last column to do this)");
                return false;
            }
        }
        return true;
    }

    function validateNoRowsSelected() {
        var commSelectedRows = $('#commContactsTable tr.rowSelected');
        if (!commSelectedRows.length) {
            alert('No rows selected.');
            return false;
        }
        return true;
    }

    function validateRequiresDefaultContactDetails() {
        var commSelectedRows = $('#commContactsTable tr.rowSelected');
        if (commSelectedRows.length) {
            var hasRowsWithoutDefault = false;
            commSelectedRows.each(function (idx, row) {
                if ($(row).hasClass('noDefault')) hasRowsWithoutDefault = true;
            });
            if (hasRowsWithoutDefault) {
                var emailOrSms = communicationsMode == "SMS" ? ' cellphone number' : ' email address';
                alert('One or more selected rows require a default contact ' + emailOrSms + '. Please ensure each contact row has a default assigned.');
                return false;
            }
        }

        return true;
    }

    function validateInsufficientCredit() {
        if (costOfBatch > availableCredit) {
            alert('The total cost of the items selected (R ' + costOfBatch + ') exceeds your available Prospecting credit (R ' + availableCredit + '). Either refine your selection or request additional Prospecting credit.');
            return false;
        }

        return true;
    }

    if (communicationsMode == "EMAIL") {
        var emailSubject = $('#emailSubject').val().trim();
        var emailBody = $('#emailMessageBody').val().trim();

        if (emailSubject == '' || emailSubject == defaultSubjectText) {
            alert('Please add a subject line.');
            return false;
        }

        if (emailBody == '' || emailBody == defaultBodyText) {
            alert('Please add a message body.');
            return false;
        }

        if (currentOrAllSuburbsSelected()) {
            return true;
        }

        if (!validateNoRowsSelected()) {
            return false;
        }

        if (!validateRequiresDefaultContactDetails()) {
            return false;
        }

        if (!validateTitleIfPresent(emailBody)) {
            return false;
        }

        if (!validateAddressIsValid(emailBody)) {
            return false;
        }

        return true;
    }
    if (communicationsMode == "SMS") {
        var smsBody = $("#smsMessageContainer").val().trim();

        if (!smsBody) {
            alert('Please add the message content');
            return false;
        }

        if (currentOrAllSuburbsSelected()) {
            return true;
        }

        if (!validateNoRowsSelected()) {
            return false;
        }

        if (!validateRequiresDefaultContactDetails()) {
            return false;
        }

        if (!validateInsufficientCredit()) {
            return false;
        }

        if (!validateTitleIfPresent(smsBody)) {
            return false;
        }

        if (!validateAddressIsValid(smsBody)) {
            return false;
        }

        return true;
    }

    return false;
}

function getContactDetailFromContactRow(contactId) {
    var value = '';
    var itemContent = $('#comm_contactdetail_' + contactId);
    if (itemContent.children(":first").is('select')) {
        value = itemContent.children(":first").find(":selected").text();
    } else {
        value = itemContent.text();
    }
    return value;
}

function submitEmails(callbackFn) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Processing. Please wait...</p>' });

    var commSelectedRows = $('#commContactsTable tr.rowSelected');
    var recipients = [];
    $.each(commSelectedRows, function (idx, row) {
        var contactId = $(row).attr("id").replace('comm_row_', '');
        var contact = getContactFromId(contactId);
        var email = getContactDetailFromContactRow(contactId);
        contact.TargetContactEmailAddress = email;
        if (contact.TargetLightstonePropertyIdForComms) {
            recipients.push(contact);
        }
    });

    var emailBodyRaw = encodeURIComponent(b64EncodeUnicode($('#emailMessageBody').val() + '<br />' + userEmailSignature));
    var emailSubjectRaw = encodeURIComponent(b64EncodeUnicode($('#emailSubject').val()));
    var batchNameRaw = encodeURIComponent(b64EncodeUnicode($('#batchFriendlyName').val()));
    var attachments = [];
    if (uploadedFiles.length) {
        attachments.push({ name: uploadedFiles[0].name, type: uploadedFiles[0].type, base64: encodeURIComponent(uploadedFiles[0].base64) });
    }
    var emailRequestPacket =
        {
            Instruction: 'submit_emails',
            Recipients: recipients,
            EmailBodyHTMLRaw: emailBodyRaw,
            EmailSubjectRaw: emailSubjectRaw,
            TargetCurrentSuburb: $('#commCurrentSuburbRadioBtn').is(':checked'),
            TargetAllMySuburbs: $('#commAllSuburbsRadioBtn').is(':checked'),
            CurrentSuburbId: currentSuburb != null ? currentSuburb.SuburbId : null,
            NameOfBatch: batchNameRaw,
            TemplateActivityTypeId: selectedTemplateActivityTypeId,
            UserSuburbIds: getUserSuburbsList(suburbsInfo),
            Attachments: attachments
        };

    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify(emailRequestPacket),
        dataType: "json"
    }).done(function (response) {
        $.unblockUI();
        if (response.SuccessfullySubmitted) {
            var submissionSuccessDialog = $("<div id='submissionSuccessDialog' title='Communication Batch Received' style='font-family:Verdana;font-size:12px;' />");
                submissionSuccessDialog.empty().append("Thank you, your request has been successfully received and enqueued for processing.");
                submissionSuccessDialog.dialog(
                                    {
                                        modal: true,
                                        closeOnEscape: true,
                                        width: '400',
                                        height: '200',
                                        buttons: { "Ok": function () { $(this).dialog("close"); } },
                                        position: ['center', 'center']
                                    });
                if (!currentOrAllSuburbsSelected() && recipients.length) {
                    var contacts = [];
                    var commAllRows = $('#commContactsTable > tbody > tr');
                    $.each(commAllRows, function (idx, row) {
                        var contactId = $(row).attr("id").replace('comm_row_', '');
                        var contact = getContactFromId(contactId);
                        if ($(row).hasClass('rowSelected')) {
                            contact.EmailSubmitted = true;
                        }
                        contacts.push(contact);
                    });
                    buildContactsBody(contacts, false);
                }
                if (callbackFn) {
                    callbackFn();
                }
        } else {
            alert("An error occurred submitting your request. Please contact support. Details of the error: " + response.ErrorMessage);
            return;
        }
    });
}

function getDefaulEmailAddress(contact) {
    var emails = contact.EmailAddresses;
    var primaryEmail = $.grep(emails, function (em) {
        return em.IsPrimary == true;
    })[0];
    
    return primaryEmail ? primaryEmail.ItemContent : emails[0].ItemContent;
}

function getContactsFromSelectedMarkers(actionWhenDone) {
    if (selectedMarkers.length) {
        var selectedProperties = getPropertiesFromSelectedMarkers();
        var propertiesWithoutContactInfo = getPropertiesWithoutContactInfo(selectedProperties);
        if (propertiesWithoutContactInfo.length) { // NB draw poly over same area twice to make sure data persisted on front-end
            var propIds = $.map(propertiesWithoutContactInfo, function (pp) { return pp.LightstonePropertyId; });
            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Contacts...</p>' });
            $.ajax({
                type: "POST",
                url: "RequestHandler.ashx",
                data: JSON.stringify({ Instruction: "load_properties", LightstonePropertyIDs: propIds }),
                success: function (data, textStatus, jqXHR) {
                    $.unblockUI();
                    if (textStatus == "success" && data) {
                        if (!handleResponseIfServerError(data)) {
                            return;
                        }

                        if (data.ErrorMsg && data.ErrorMsg.length > 0) {
                            alert(data.ErrorMsg);
                        }

                        $.each(propertiesWithoutContactInfo, function (idx, targetProp) {
                            // Find the property with new data
                            var propWithContacts = $.grep(data.Properties, function (pp) {
                                return targetProp.LightstonePropertyId == pp.LightstonePropertyId;
                            })[0];
                            updateExistingPropertyFromProperty(targetProp, propWithContacts);
                        });

                        var contacts = getContactsFromSelection(selectedProperties);
                        actionWhenDone(contacts);
                    } else {
                        alert('Could not complete request.');
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(jqXHR.status);
                    alert(jqXHR.responseText);
                },
                dataType: "json"
            });
        } else {
            var contacts = getContactsFromSelection(selectedProperties);
            actionWhenDone(contacts);
        }
    }

    function getPropertiesFromSelectedMarkers() {
        var selectedProperties = [];
        $.each(selectedMarkers, function (idx, marker) {
            var prop = marker.ProspectingProperty;
            if (selectedProperties.indexOf(prop) == -1) {
                selectedProperties.push(prop);
            }
        });
        return selectedProperties;
    }

    function getPropertiesWithoutContactInfo(selectedProperties) {
        var propsWithoutContactInfo = [];
        $.each(selectedProperties, function (idx, pp) {
            if (!pp.Contacts) {
                propsWithoutContactInfo.push(pp);
            }
        });
        return propsWithoutContactInfo;
    }

    function getContactsFromSelection(selectedProperties) {

        function hasCellphoneNumber(contact) {
            var hasCell = false;
            if (contact.PhoneNumbers && contact.PhoneNumbers.length) {
                $.each(contact.PhoneNumbers, function (idx, ph) {
                    if (ph.ItemType == cellphoneItemId) {
                        hasCell = true;
                    }
                });
            }
            return hasCell;
        }
        var selectedContacts = [];
        if (communicationsMode == "SMS") {
            $.each(selectedProperties, function (idx, pp) {
                if (pp.Contacts) {
                    $.each(pp.Contacts, function (idx2, contact) {
                        if (hasCellphoneNumber(contact)) {
                            contact.TargetCommPropertyId = pp.ProspectingPropertyId;
                            contact.PropertyAddress = getFormattedAddress(pp);
                            contact.TargetLightstonePropertyId = pp.LightstonePropertyId;
                            contact.TargetLightstonePropertyIdForComms = pp.LightstonePropertyId;

                            selectedContacts.push(contact);
                        }
                    });
                }
            });
        }

        if (communicationsMode == "EMAIL") {
            $.each(selectedProperties, function (idx, pp) {
                if (pp.Contacts) {
                    $.each(pp.Contacts, function (idx2, contact) {
                        if (contact.EmailAddresses && contact.EmailAddresses.length) {
                            contact.TargetCommPropertyId = pp.ProspectingPropertyId;
                            contact.PropertyAddress = getFormattedAddress(pp);
                            contact.TargetLightstonePropertyId = pp.LightstonePropertyId;
                            contact.TargetLightstonePropertyIdForComms = pp.LightstonePropertyId;

                            selectedContacts.push(contact);
                        }
                    });
                }
            });
        }

        // What about duplicate contacts
        return selectedContacts;
    }
}

function getFormattedAddress(property) {
    if (property.SS_FH == "SS" || property.SS_FH == "FS") {
        var doorNr = '';
        if (property.SSDoorNo) doorNr = "(Door no.: " + property.SSDoorNo + ")";
        return "Unit " + property.Unit + " " + doorNr + " " + formatPropertyAddressTitleCase(property.SSName);
    } else {
        return property.StreetOrUnitNo + " " + toTitleCase(property.PropertyAddress);
    }
}

function handleCommSendBtnClick() {
    costOfBatch = 0;

    if (!validateMessage()) {
        return;
    }

    var readyStatus = '';
    if (currentOrAllSuburbsSelected()) {
        if ($('#commCurrentSuburbRadioBtn').is(':checked')) {
            readyStatus = '\nReady to send communication to the relevant contact persons in ' + currentSuburb.SuburbName + '.' +
                          '\nPlease note that only contacts with a default email address or cellphone number will be targeted.';
        }
        if ($('#commAllSuburbsRadioBtn').is(':checked')) {
            readyStatus = '\nReady to send communication to the relevant contact persons in all your available suburbs.' +
                          '\nPlease note that only contacts with a default email address or cellphone number will be targeted.';
        }
    } else {
        var commSelectedRows = $('#commContactsTable tr.rowSelected');
        readyStatus = '\nReady to send communication to ' + commSelectedRows.length + ' contact rows.';
    }

    var batchNameDiv = $("<div style='display:inline-block' />").append("<span>Specify a name for this batch (optional):</span>")
                        .append("<input type='text' style='width:300px' id='batchFriendlyName' />");
    
        var dialog = $("#commSendMessageDialog");
        dialog.empty();
        dialog.append(readyStatus).append('<p />').append(batchNameDiv).append("<p />");
        
            var previewMsgLabel = $("<p>Sample message:</p>");
            dialog.append(previewMsgLabel);            
            handleShowMessagePreview(function (canProceed, signatureData) {
                var previewDialog = createPreviewMessage(signatureData, dialog);
                dialog.append(previewDialog);
                if (canProceed) {
                    dialog.dialog(
                          {
                              modal: true,
                              closeOnEscape: true,
                              //open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                              width: '800',
                              height: '600',
                              buttons: {
                                  "Calculate Cost": function() {
                                      calculateCostOfBatch();
                                  },
                                  "Send Message": function () {
                                      // calc cost here anyway to determine whether they have enough credit
                                      if (!costOfBatch) {
                                          calculateCostOfBatch(function () {
                                              dialog.dialog("close");
                                              userEmailSignature = signatureData;
                                              handleSendMessage(function () {
                                                  availableCredit -= costOfBatch;
                                                  $('#commCreditValue').text(availableCredit.toFixed(2));
                                              });
                                          });
                                      } else {
                                          if (availableCredit > costOfBatch) {
                                              $(this).dialog("close");
                                              userEmailSignature = signatureData;
                                              handleSendMessage(function () {
                                                  availableCredit -= costOfBatch;
                                                  $('#commCreditValue').text(availableCredit.toFixed(2));
                                              });
                                          } else {
                                              alert('You have insufficient credit to perform this operation');
                                          }
                                      }                                          
                                  }
                              },
                              position: ['center', 'center']
                          });
                }
                else {
                    dialog.dialog(
                                    {
                                        modal: true,
                                        closeOnEscape: true,
                                        //open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                                        width: '600',
                                        height: '450',
                                        buttons: { "OK": function () { $(this).dialog("close"); } },
                                        position: ['center', 'center']
                                    });
                }
            });                   
}

function calculateCostOfBatch(callbackFn) {
        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Calculating Cost. Please wait...</p>' });
        var recipients = [];
        var currentSuburbId = null;
        var targetAllUserSuburbs = false;
        var smsBody = null;
        if (currentOrAllSuburbsSelected()) {
            // we are targetting the current or all user suburbs
            if ($('#commCurrentSuburbRadioBtn').is(':checked')) {
                currentSuburbId = currentSuburb.SuburbId;
            }
            if ($('#commAllSuburbsRadioBtn').is(':checked')) {
                targetAllUserSuburbs = true;
            }
        } else {
            // we are targetting a batch of recipients
            var commSelectedRows = $('#commContactsTable tr.rowSelected');
            if (communicationsMode == "EMAIL") {
                $.each(commSelectedRows, function (idx, row) {
                    var contactId = $(row).attr("id").replace('comm_row_', '');
                    var contact = getContactFromId(contactId);
                    var email = getContactDetailFromContactRow(contactId);
                    contact.TargetContactEmailAddress = email;  // TODO: make this right at some point.
                    recipients.push(contact);
                });
            }
            if (communicationsMode == "SMS") {
                $.each(commSelectedRows, function (idx, row) {
                    var contactId = $(row).attr("id").replace('comm_row_', '');
                    var contact = getContactFromId(contactId);
                    var cell = getContactDetailFromContactRow(contactId);
                    contact.TargetContactCellphoneNumber = cell;
                    recipients.push(contact);
                });
            }            
        }
        var instruction = null;
        if (communicationsMode == "EMAIL") {
            instruction = "calculate_cost_email_batch";
        }
        if (communicationsMode == "SMS") {
            instruction = "calculate_cost_sms_batch";
            smsBody = $("#smsMessageContainer").val();
        }
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({
                Instruction: instruction,
                Recipients: recipients,
                TargetCurrentSuburb: currentSuburbId != null ? true : false,
                TargetAllMySuburbs: targetAllUserSuburbs,
                CurrentSuburbId: currentSuburbId,
                TemplateActivityTypeId: selectedTemplateActivityTypeId,
                UserSuburbIds: getUserSuburbsList(suburbsInfo),
                SmsBodyRaw: smsBody
            }),
            dataType: "json"
        }).done(function (result) {
            $.unblockUI();
            costOfBatch = result.TotalCost;
            var costResultsDialog = null;
            if (communicationsMode == "EMAIL") {
             costResultsDialog = $("<div title='Calculation Results' style='font-family:Verdana;font-size:12px;' />").empty()
                                        .append("Available Prospecting credit: R " + availableCredit.toFixed(2))
                                        .append("<br />")
                                        .append("Unit cost per email: R " + result.UnitCost)
                                        .append("<br />")
                                        .append("Number of emails: " + result.NumberOfUnits)
                                        .append("<br />")
                                        .append("Cost of batch: R " + result.TotalCost.toFixed(2));
            }
            if (communicationsMode == "SMS") {
                costResultsDialog = $("<div title='Calculation Results' style='font-family:Verdana;font-size:12px;' />").empty()
                                        .append("Available Prospecting credit: R " + availableCredit.toFixed(2))
                                        .append("<br />")
                                        .append("Unit cost per 160 characters: R " + result.UnitCost)
                                        .append("<br />")
                                        .append("Number of messages (160 characters per message): " + result.NumberOfUnits)
                                        .append("<br />")
                                        .append("Cost of batch: R " + result.TotalCost.toFixed(2));
            }

            var buttonText = "Ok";
            if (callbackFn) {
                buttonText = "Ok - Send Message";
            }
            costResultsDialog.dialog(
              {
                  modal: true,
                  closeOnEscape: true,
                  width: '400',
                  buttons: [{
                      text: buttonText,
                      click: function () {
                          $(this).dialog("close");
                          if (callbackFn) {
                              if (availableCredit > result.TotalCost) {
                                  callbackFn();
                              } else {
                                  alert('You have insufficient credit to perform this operation');
                              }
                          }
                      }
                  }],
                  position: ['center', 'center']
              });
        });
}

function createUnSubscribeOption(contact) {
    if (communicationsMode == "SMS") {
        return smsOptOut;
    }
}

function createPreviewMessage(signatureData, dialog) {
    var div = $("<div id='previewMsgDiv' />");
    var textarea = $("<textarea id='previewTextarea' style='width:100%;height:160px' />");
    div.append(textarea);

    var firstRecord = null;
    if (currentOrAllSuburbsSelected()) {
        firstRecord = { ContactPersonId: 1, Title: 1, Firstname: 'John', Surname: 'Doe', PropertyAddress: '22 Smith Street, Constantia', EmailAddresses: [{IsPrimary: true, ItemContent:'john.doe@somedomain.com'}] };
    } else {
        var commSelectedRows = $('#commContactsTable tr.rowSelected');
        var contactId = $(commSelectedRows[0]).attr("id").replace('comm_row_', '');
        firstRecord = getContactFromId(contactId);
    }

    if (communicationsMode == "EMAIL") {
        var preview = generateMessageForRecord($("#emailMessageBody").val(), firstRecord);

        var myToolbar = [{ name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] }, { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] }, { name: 'paragraph', items: ['NumberedList', 'BulletedList'] }, { name: 'links', items: ['Link', 'Unlink', 'Anchor'] }, { name: 'insert', items: ['Image', 'Flash', 'Table'] } ];
        var config = { toolbar_mySimpleToolbar: myToolbar, toolbar: 'mySimpleToolbar', allowedContent: true };
        $(textarea).ckeditor(config, function () {
            CKEDITOR.instances.previewTextarea.setReadOnly(true);
        });

        textarea.ckeditor(function (txtarea) {
            if (!signatureData) {
                signatureData = "<span>Error: Your signature has not yet been set on BOSS</span>" +
                                        "<br /><span>Please set your signature under the administration panel in BOSS:</span>" +
                                        "<br /><span style='margin-left: 0 auto;margin-right: 0 auto'><img src='Assets/set_signature_boss.png' /></span>";
                CKEDITOR.instances.previewTextarea.setData(signatureData);
            }
            else {
                CKEDITOR.instances.previewTextarea.setData(preview + '<br />' + signatureData);
            }
            dialog.dialog({ position: ['center', 'center'] });
            CKEDITOR.instances.previewTextarea.setReadOnly(true);
            CKEDITOR.instances.previewTextarea.resize('100%', '800');
        });
    }
    if (communicationsMode == "SMS") {
        var previewText = generateMessageForRecord($("#smsMessageContainer").val(), firstRecord);
        textarea.val(previewText + createUnSubscribeOption());
        textarea.attr('readonly', 'readonly');
    }

    return div;
}

function generateMessageForRecord(templateMsg, record) {
    var titleText = $.grep(prospectingContext.ContactPersonTitle, function (i) {
        return i.Key == record.Title;
    })[0];
    titleText = !titleText ? '' : titleText.Value;
    templateMsg = replaceAll(templateMsg, '*title*', titleText);
    templateMsg = replaceAll(templateMsg, '*name*', toTitleCase(record.Firstname));
    templateMsg = replaceAll(templateMsg, '*surname*', toTitleCase(record.Surname));

    var propertyAddress = formatPropertyAddressTitleCase(record.PropertyAddress);
    templateMsg = replaceAll(templateMsg, '*address*', propertyAddress);
    return templateMsg;
}

function handleShowMessagePreview(callback) {
    if (communicationsMode == "EMAIL") {
        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Retrieving your signature from BOSS...</p>' });
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({ Instruction: 'retrieve_user_signature' }),
            async: false,
            dataType: "json",
        }).done(function (data) {
            $.unblockUI();
            var canSend = true;
            var signatureData = null;
            if (!data) {
                canSend = false;
            } else {
                signatureData = data;
            }
            callback(canSend, signatureData);
        });
    }
    if (communicationsMode == "SMS") {
        callback(true, null);
    }
}

function showDialogCommCustomSelectionDisabled() {
    var customSelectionDisabledDiv = $("<div title='Custom Selection Disabled' style='font-family:Verdana;font-size:12px;' />");
    customSelectionDisabledDiv.empty().append("<span>Custom selection mode is disabled because you have selected either the \
                                                    'Current Suburb' or 'All Suburbs' option.</span>");

    customSelectionDisabledDiv.dialog(
  {
      modal: true,
      closeOnEscape: true,
      width: '400',
      buttons: { "Ok": function () { $(this).dialog("close"); } },
      position: ['center', 'center']
  });
}