
var communicationsMode = null;
var cellphoneItemId;
var costOfBatch = 0;

var defaultSubjectText = 'Subject';
var defaultBodyText = "[Email body]<p />Use *title*, *name*, *surname* or *address* as placeholders for contact's information.";

var uploadedFiles = [];

var userEmailSignature = '';

var smsOptOut = " -Reply STOP to opt-out.";

function buildCommunicationMenu() {

    var contentDiv = $("<div class='contentdiv' id='communicationsDiv' style='padding-right:10px' />");
    var communicationBtn = $("<input type='button' id='communicationBtn' value='New' />");
    communicationBtn.click(handleCommBtnClick);
    var menu = $("<div id='communicationMenu' />");

    var remainingCreditLabel = $("<div id='commRemainingCredit' style='display:inline-block;float:right'>Available Credit: R <span>" + availableCredit + "</span></div>");
    var messageContentContainer = $("<div id='messageContentContainer' />");
    var contactablesContentContainer = buildContactablesTable();
    var getInfoLabel = $("<label id='commGetInfoLabel' style='display:none'></label>");

    var bottomContainer = $("<div id='commBottomDiv' style='display:none' />");
    var costOfBatchLabel = $("<div id='commBatchCost' style='display:inline-block;float:left'><span id='commCostOfBatch'></span></div>");
    var sendButton = $("<div id='commSendMessage' style='display:inline-block;float:right'><input type='button' id='commSendMessageBtn' value='Preview'  /></div>");
    sendButton.click(handleCommSendBtnClick);
    bottomContainer.append(costOfBatchLabel).append(sendButton);

    contentDiv.append(communicationBtn).append(menu).append(remainingCreditLabel).append("<p />").append(messageContentContainer).append("<p />").append(contactablesContentContainer).append(getInfoLabel).append("<p style='height:5px' />").append(bottomContainer);

    var multiSelectSticker = $("#multiSelectMode");
    var mapControl = map.controls[google.maps.ControlPosition.TOP_RIGHT];
    mapControl.push(multiSelectSticker[0]);

    function buildContactablesTable() {
        var container = $("<div id='contactablesContentContainer' />");
        container.append($("<table id='commContactsTableHeader' class='commTable' ></table>")
                         .append("<thead><tr class='commTableHeader'><th class='commTableRow30'>&nbsp;</th><th class='commTableRow'>Name</th><th class='commTableRow'>Surname</th><th class='commTableRow120'>Address</th><th class='commTableRow'>Contact</th><th class='commTableRow'>Action</th><th class='commTableRow'>&nbsp;</th></tr></thead>"))
                         .append($("<div style='overflow:auto;max-height:310px;'><table id='commContactsTable' class='commTable' ></table></div>"));
        return container;
    }

    cellphoneItemId = getProspectingPhoneTypeId('cell');
    return contentDiv; 
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

    var separator = buildCommMenuItem("separator", "-----------------------------------------", null);
    menu.append(separator);

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
    
    var smsTemplate = buildCommMenuItem("comm_menu_sms_template", buildSMSTemplate(), null);
    menu.append(smsTemplate);

    var emailTemplate = buildCommMenuItem("comm_menu_email_template", buildEmailTemplate(), null);
    menu.append(emailTemplate);
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
    var msgDiv = $("<textarea id='smsMessageContainer' style='border: 1px solid gray;width:100%;height:80px;color:lightgray' />");
    var introText = "Use *title*, *name*, *surname* or *address* as placeholders for contact's information.";
    msgDiv.append(introText);

    msgDiv.focus(function () {
        if (msgDiv.val() != introText)
            return;

        msgDiv.empty();
        msgDiv.css('color', 'black');
    });

    msgDiv.keyup(updateCostOfBatchSMS);
    //msgDiv.change(updateCostOfBatchSMS);

    return msgDiv;
}

function buildEmailContentContainer() {
    var emailContainer = $("<div />");

    var subjectLine = $("<input type='text' id='emailSubject' style='width:100%;color:lightgray;border:1px solid gray' />");
    subjectLine.val(defaultSubjectText);
    subjectLine.focus(function () {
        if (subjectLine.val() != defaultSubjectText) {
            return;
        }
        subjectLine.val('');
        subjectLine.css('color', 'black');
    });

    subjectLine.keyup(function () {
        var val = $(this).val();
        if (val == '') {
            subjectLine.css('border', '1px solid red');
        }
        else {
            subjectLine.css('border', '1px solid black');
            tooltip.hide();
        }
    });

    var body = $("<textarea id='emailMessageBody' name='emailMessageBody' style='width:100%;height:120px;padding-bottom:1px' />");
    emailContainer.append(subjectLine).append("<p />").append(body);

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
        attachFilesDiv.css('display', 'block');
    });
           
    //var focusManager = new CKEDITOR.focusManager(CKEDITOR.instances.emailMessageBody);
    var firstFocus = false;
    CKEDITOR.instances.emailMessageBody.on('focus', function (evt) {
        if (!firstFocus) {
            firstFocus = true;
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

    var attachFilesDiv = $("<div style='display:none;padding-top:0' />");
    var attachFileBtn = $("<input type='button' id='attachFilesBtn' style='display:inline-block;margin-right:10px' value='Attach..' />");
    var attachFileList = $("<div id='attachFilesList' style='display:inline-block;'  />");
    var attachFileOperation = $("<input id='attachFilesOp' type='file' name='attachFilesOp' style='display: none;' multiple />");
    attachFilesDiv.append(attachFileBtn).append(attachFileList).append(attachFileOperation);
    attachFileBtn.click(handleShowFileUploadDialog);

    return emailContainer.append("<br />").append(attachFilesDiv);
}

function handleShowFileUploadDialog() {
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
            uploadedFiles = $.grep(uploadedFiles, function (f) {
                return f.fileId != itemId;
            });
            fileUploadItem.remove();
        });

        $('#attachFilesList').append(fileUploadItem);
    }
}

// Menu item click handlers
function handlePolyMenuItemClick() {
    toggleDrawingMode();
}

function handleRemovePolyMenuItemClick() {
    removePolygonsWithMarkers();
}

function handleSMSMessageClick() {
    communicationsMode = "SMS";
    var container = $("#messageContentContainer");
    container.empty();
    container.append(buildSMSContentContainer());

    updateCommunicationsContacts();
}

function handleEmailMessageClick() {
    communicationsMode = "EMAIL";
    var container = $("#messageContentContainer");
    container.empty();
    container.append(buildEmailContentContainer());

    updateCommunicationsContacts();
}

function handleSMSMenuItemClick() {
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
                buildContactsBody(contacts);

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

function buildContactsBody(contacts) {
    var commContactsTable = $("#commContactsTable");
    commContactsTable.find("tr").remove();
    var body = $("<tbody></tbody>");

    var uniqueItems = [];
    $.each(contacts, function (idx, c) {
        var rowId = c.ContactPersonId; // TODO: This actually causes a bug when the same person owns multiple selected properties. Fix by creating a key using ContactPersonId + LightstoneId
        var tr = $("<tr id='comm_row_" + rowId + "' ></tr>");

        var contactDetailContent, contactDetailTitle = '', actionStatus = 'Ready';
        var rowIsSelected = 'checked';
        if (c.EmailSent) {
            actionStatus = 'Sent';
        }
        if (c.SendError) {
            actionStatus = 'Error';
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

                if (c.EmailSent) {
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

        var isSelectedCell = $("<td class='commTableRow30' id='comm_selected_" + rowId + "' ></td>").append($("<input type='checkbox' id='comm_selected_checkbox_" + rowId + "' name='comm_selected_checkbox_" + rowId + "' value='' " + rowIsSelected + " />"));
        var name = $("<td class='commTableRow' id='comm_firstname_" + rowId + "' title='" + c.Firstname + "' ></td>").append(toTitleCase(c.Firstname));
        var surname = $("<td class='commTableRow' id='comm_surname_" + rowId + "' title='" + c.Surname + "' ></td>").append(toTitleCase(c.Surname));
        var address = $("<td class='commTableRow120' id='comm_address_" + rowId + "' title='" + c.PropertyAddress + "' ></td>").append(c.PropertyAddress); // change for SS (unit no) + ordering
        var contact = $("<td class='commTableRow' id='comm_contactdetail_" + rowId + "' title='" + contactDetailTitle + "' ></td>").append(contactDetailContent); // change
        var action = $("<td class='commTableRow' id='comm_action_" + rowId + "' ></td>").append(actionStatus);
        var editBtn = $("<a href='' id='comm_edit_contact_" + rowId + "' style='text-decoration:underline!important;'>Edit</a>");
        var edit = $("<td class='commTableRow' id='comm_edit_" + rowId + "' ></td>").append(editBtn);

        tr.append(isSelectedCell).append(name).append(surname).append(address).append(contact).append(action).append(edit);
        body.append(tr);

        if (rowIsSelected == 'checked') {
            tr.addClass('rowSelected');
        }

        if (c.SMSOptout && communicationsMode == "SMS") {
            isSelectedCell.first().prop("disabled", true);
        }
        if (c.EmailOptout && communicationsMode == "EMAIL") {
            isSelectedCell.first().prop("disabled", true);
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
        var ssNameLbl = $("<label> (" + property.SSName + ")</label>");
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

function handleSendMessage() {
    if (communicationsMode == "EMAIL") {
        authorizeAndSendGmail();
    }
    if (communicationsMode == "SMS") {
        sendSMS();
    }
}

function sendSMS() {
    var recipients = [];
    var commSelectedRows = $('#commContactsTable tr.rowSelected');
    $.each(commSelectedRows, function (idx, row) {
        var contactId = $(row).attr("id").replace('comm_row_', '');
        var contact = getContactFromId(contactId);
        contact.SendError = null;

        var message = generateMessageForRecord($("#smsMessageContainer").val().trim(), contact) + createUnSubscribeOption();
        recipients.push({
            ContactpersonId: contactId,
            ProspectingPropertyId: contact.TargetCommPropertyId,
            TargetCellNo: getDefaultCellNo(contact),
            Message: message
        });
    });


    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Sending SMS to recipients...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: "send_sms", TargetRecipients: recipients }),
        dataType: "json"
    }).done(function (data) {
        $.unblockUI();
        if (!handleResponseIfServerError(data)) {
            return;
        }
        // Rem to do the same stuff as for email. Reply|Followup. Billing backend+front-end. Logging etc.
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

        if (emailSubject == '' || emailSubject == defaultSubjectText) {
            alert('Please add a subject line.');
            return false;
        }

        if (emailBody == '' || emailBody == defaultBodyText) {
            alert('Please add a message body.');
            return false;
        }

        return true;
    }
    if (communicationsMode == "SMS") {
        var smsBody = $("#smsMessageContainer").val().trim();

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

        if (!smsBody) {
            alert('Please add the message content');
            return false;
        }
        return true;
    }

    return false;
}

function authorizeAndSendGmail() {
   
    gapi.client.setApiKey('AIzaSyBCYyAhMO9Ia9thqz0LxXzzZL-Kk6b2bNs');
    handleAuth(true);

    function handleAuthResult(authResult) {
        if (authResult['status']['signed_in']) {
            sendEmailMessage();
        } else if (authResult['error'] == "immediate_failed") {
            handleAuth(false);
        }
    }

    function handleAuth(promptUser) {
        window.setTimeout(function () {
            gapi.auth.authorize({ client_id: '196629217199-q1r0dbt6brmk86v7anme6t6rvuksn5ip.apps.googleusercontent.com', scope: 'https://www.googleapis.com/auth/gmail.compose', immediate: promptUser }, handleAuthResult);
        }, 1000);
    }
}

function generateEmailObjectBase64(subjectLine, emailBody, address) {
    var cids = [];
    emailBody = convertInlineImages(emailBody, cids);
    var email = {
        "to": address,
        "subject": subjectLine,
        "from": "me",
        "body": emailBody,
        "cids": cids,
        "attaches": uploadedFiles
    };
    email = createMimeMessage(email);

    var base64EncodedEmail = btoa(email).replace(/\//g, '_').replace(/\+/g, '-');

    return base64EncodedEmail;
}

function convertInlineImages(body, cids) {
    var re = /<img[^>]*src=[\'\"]?data:image\/[^>]*>/g;
    var matches = re.exec(body);
    if (matches) {
        for (var i = 0; i < matches.length; i++) {
            var match = matches[i];
            body = convertInlineImage(body, match, cids);
            return convertInlineImages(body, cids);
        }
    }

    function convertInlineImage(body, match, cids) {
        var typeStartIndex = match.indexOf(':');
        var typeEndIndex = match.indexOf(';');
        var type = match.substring(typeStartIndex+1, typeEndIndex);

        var name = generateUniqueID();

        var base64Start = match.indexOf('base64,');
        var base64 = match.substring(base64Start + 7, match.length).replace('" />', '');

        cids.push({ type: type, name: name, base64: base64 });

        body = body.replace(match, '<img src="cid:' + name + '" />');
        return body;
    }

    return body;
}

function sendEmailMessage() {

    var commSelectedRows = $('#commContactsTable tr.rowSelected');
    var completedRows = [];
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Sending email to recipients...</p>' });
    gapi.client.load('gmail', 'v1', function () {

        $.each(commSelectedRows, function (idx, row) {
            var contactId = $(row).attr("id").replace('comm_row_','');
            var contact = getContactFromId(contactId);
            contact.SendError = null;

            var subjectText = $('#emailSubject').val();
            contact.TargetEmailSubjectText = subjectText;
            var emailObject = null;
            var address = getDefaulEmailAddress(contact);
            contact.TargetEmailAddress = address;
            try {
                var emailMsg = generateMessageForRecord($('#emailMessageBody').val(), contact);
                emailMsg += '<br />' + userEmailSignature + '<p />' + createUnSubscribeOption(contact);

                emailObject = generateEmailObjectBase64(subjectText, emailMsg, address);
                contact.TargetEmailObject = emailObject;
                executeGmailSendMessage(emailObject, function (gmailResult) { handleSendingComplete(gmailResult, row, contact); });
            } catch (e) {
                contact.SendError = e.stack;
                contact.EmailSent = false;
                var commErrorObject = createCommunicationLogRecord('GENERAL', 'EMAIL', contactId, address, contact.TargetLightstonePropertyId, 'NOT SENT', encodeURIComponent(e.stack), emailObject, subjectText);
                saveCommunication(commErrorObject);
                completedRows.push(row);

                handleSendingAllRowsComplete();
            }
        });
    });

    function saveCommunication(commObject) {
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({
                Instruction: "save_communication", CommContext: commObject.CommContext,
                CommType: commObject.CommType,
                TargetContactPersonId: commObject.TargetContactPersonId,
                TargetContactDetail: commObject.TargetContactDetail,
                TargetLightstonePropId: commObject.TargetLightstonePropId,
                SentStatus: commObject.SentStatus,
                SendingError: commObject.SendingError,
                MessageBase64: encodeURIComponent(commObject.MessageBase64),
                SubjectText: commObject.SubjectText
            }),
            dataType: "json"
        }).done(function (data) {
            if (!handleResponseIfServerError(data)) {
                return;
            }
        });
    }

    function executeGmailSendMessage(emailObject, callback) {
        var request = gapi.client.gmail.users.messages.send({
            auth: 'OAuth2Client',
            userId: "me",
            raw: emailObject,
        });
        request.execute(callback);
    }

    function handleSendingAllRowsComplete() {
        if (completedRows.length == commSelectedRows.length) {
            // Update the back-end: log each record result in a table + create an activity for each

            // UI update
            var contacts = [];
            var commAllRows = $('#commContactsTable > tbody > tr');
            $.each(commAllRows, function (idx, row) {
                var contactId = $(row).attr("id").replace('comm_row_', '');
                var contact = getContactFromId(contactId);

                contacts.push(contact);
            });
            buildContactsBody(contacts);
            $.unblockUI();
        }
    }

    function handleSendingComplete(gmailResult, row, contact) {
        if (gmailResult.labelIds.indexOf('SENT') > -1) {
            contact.EmailSent = true;
            var commObject = createCommunicationLogRecord('GENERAL', 'EMAIL', contact.ContactPersonId, contact.TargetEmailAddress, contact.TargetLightstonePropertyId, 'SENT', null, contact.TargetEmailObject, contact.TargetEmailSubjectText);
            saveCommunication(commObject);
        } else {
            contact.EmailSent = false;
            contact.SendError = gmailResult.join();
            var commErrorObject = createCommunicationLogRecord('GENERAL', 'EMAIL', contact.ContactPersonId, contact.TargetEmailAddress, contact.TargetLightstonePropertyId, 'NOT SENT', gmailResult.join(), contact.TargetEmailObject, contact.TargetEmailSubjectText);
            saveCommunication(commErrorObject);
        }
        completedRows.push(row);

        // When sending is complete
        handleSendingAllRowsComplete();
    }
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
        return "Unit " + property.Unit + " " + doorNr + " " + property.SSName;
    } else {
        return property.StreetOrUnitNo + " " + property.PropertyAddress;
    }
}

function handleCommSendBtnClick() {

    if (!validateMessage()) {
        return;
    }

    var commSelectedRows = $('#commContactsTable tr.rowSelected');
    var readyStatus = '\nReady to send communication to ' + commSelectedRows.length + ' contact row(s)';
    if (communicationsMode == "EMAIL") {
        readyStatus += '<p />Please note that you may receive a popup-screen requesting you to log in to your Seeff mail account,\
                                 this will allow Prospecting to send email from your account. <br />Please ensure that your browser is set to allow for pop-ups.';
    }
    
        var dialog = $("#commSendMessageDialog");
        dialog.empty();
        dialog.append(readyStatus).append("<p />");
        
            var previewMsgLabel = $("<p>Preview of the first message:</p>");
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
                              width: 'auto',
                              height: 'auto',
                              buttons: {
                                  "Send": function () {
                                      $(this).dialog("close");
                                      userEmailSignature = signatureData;
                                      handleSendMessage();
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
                                        buttons: { "OK": function () { $(this).dialog("close"); } },
                                        position: ['center', 'center']
                                    });
                }
            });                   
}

function createUnSubscribeOption(contact) {
    if (communicationsMode == "SMS") {
        return smsOptOut;
    }
    if (communicationsMode == "EMAIL") {
        var email = getDefaulEmailAddress(contact);
        var link = 'http://prospecting.seeff.com/UnsubscribeCommunication.html?email=' + email + '&contactid=' + contact.ContactPersonId;
        var unsubscribeOption = "<br /><br /><br /><a href='" + link + "' target='_blank'>Unsubscribe</a>";
        return unsubscribeOption;
    }
}

function createPreviewMessage(signatureData, dialog) {
    var div = $("<div id='previewMsgDiv' />");
    var textarea = $("<textarea id='previewTextarea' style='width:100%;height:80px;' />");
    div.append(textarea);

    var commSelectedRows = $('#commContactsTable tr.rowSelected');

    var contactId = $(commSelectedRows[0]).attr("id").replace('comm_row_', '');
    var firstRecord = getContactFromId(contactId);

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
                CKEDITOR.instances.previewTextarea.setData(preview + '<br />' + signatureData + '<p />' + createUnSubscribeOption(firstRecord));
            }
            dialog.dialog({ position: ['center', 'center'] });
            CKEDITOR.instances.previewTextarea.setReadOnly(true);
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
    templateMsg = replaceAll(templateMsg, '*address*', toTitleCase(record.PropertyAddress));
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