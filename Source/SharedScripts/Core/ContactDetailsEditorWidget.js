

function ContactDetailsEditorWidget(containerElementId, arrayOfPhoneNumberObjects, arrayOfEmailAddressObjects, saveBtnClick, canEdit) {

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Public functions
    //
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    this.containerDiv = null;

    var _phoneNumbers = arrayOfPhoneNumberObjects;
    var _emailAddresses = arrayOfEmailAddressObjects;

    var _phoneTypes = prospectingContext.ContactPhoneTypes;
    var _emailTypes = prospectingContext.ContactEmailTypes;

    var _canEdit = canEdit;

    this.construct = function () {
        containerDiv = $(containerElementId);

        var html = $("<div id='contactDetails' />");
        //if (!_canEdit) {
        //    html.append("<label style='color:red;font-size:14px;'>This contact has opted out and may not be modified.</label><br />");
        //}

        var phoneNumbersDiv = $("<form id='phoneNumbersDiv' class='phoneContactSection' />");
        var emailAddressesDiv = $("<form id='emailAddressesDiv' class='emailContactSection' />");

        buildPhoneNumbers(phoneNumbersDiv);
        buildEmailAddresses(emailAddressesDiv);

        html.append("<label>Contact Phone Numbers</label>");
        html.append(phoneNumbersDiv);
        html.append("<hr><p />");
        html.append("<label>Contact Email Addresses</label>");
        html.append(emailAddressesDiv);
        html.append("<hr />");
        html.append(buildSaveBtn());

        toggleSaveBtnEnabled();

        return html;
    };

    this.resetItemArrays = function (arrayPhoneNumbers, arrayEmailAddresses) {
        _phoneNumbers = arrayPhoneNumbers;
        _emailAddresses = arrayEmailAddresses;
    };

    this.rebuildDivs = function (canEdit) {
        _canEdit = canEdit;
        var phoneNumbersDiv = $("#phoneNumbersDiv");
        var emailAddressesDiv = $("#emailAddressesDiv");
        phoneNumbersDiv.empty();
        emailAddressesDiv.empty();
        buildPhoneNumbers(phoneNumbersDiv);
        buildEmailAddresses(emailAddressesDiv);

        $('#saveContactDetailsBtn').prop('disabled', !_canEdit);
        toggleSaveBtnEnabled();
    };

    this.getPhoneNumbers = function () {
        return _phoneNumbers;
    };

    this.getEmailAddresses = function () {
        return _emailAddresses;
    };

    function buildSaveBtn() {
        var button = $("<input type='button' id='saveContactDetailsBtn' value='Save..' style='cursor:pointer;' />");
        button.prop('disabled', !_canEdit);
        button.on('click', function () { saveBtnClick(_phoneNumbers, _emailAddresses); });

        if ((!_phoneNumbers || _phoneNumbers.length == 0) && (!_emailAddresses || _emailAddresses.length == 0)) {
            button.css('display', 'none');
        }

        return button;
    }

    function toggleSaveButtonVisible() {
        if ((!_phoneNumbers || _phoneNumbers.length == 0) && (!_emailAddresses || _emailAddresses.length == 0)) {
            $('#saveContactDetailsBtn').css('display', 'none');
            $('#personGeneralSaveBtn').css('display', 'block');
        }
        else {
            $('#saveContactDetailsBtn').css('display', 'block');
            $('#personGeneralSaveBtn').css('display', 'none');
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Internal functions
    //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    function buildPhoneNumbers(phoneNumbersDiv) {
        addContactPhoneCaptureOptions(phoneNumbersDiv);
        if (_phoneNumbers) {
            $.each(_phoneNumbers, function (index, item) {
                var elements = buildPhoneContactRow(item, _phoneTypes);
                addContactRow(phoneNumbersDiv, elements);
            });
        }
    }

    function buildEmailAddresses(emailAddressesDiv) {
        addContactEmailCaptureOptions(emailAddressesDiv);
        if (_emailAddresses) {
            $.each(_emailAddresses, function (index, item) {
                var elements = buildEmailContactRow(item, _emailTypes);
                addContactRow(emailAddressesDiv, elements);
            });
        }
    }

    function constructTypeCheckBox(item, typesArray) {
        var id = item.ContactItemType + "_checkbox" + item.ItemId;
        var html = $("<select id='" + id + "' class='contact-itemType' />");
        html.append("<option value='nothing'></option>");
        $.each(typesArray, function (idx, type) {
            html.append("<option value='" + type.Key + "'>" + type.Value + "</option>");
        });

        // Set selected type if available
        html.find("option[value='" + item.ItemType + "']").attr('selected', true);

        // Wire up the change handler
        html.on('change', function () {
            var selectedType = $(this).find(':selected').val();
            item.ItemType = selectedType;
        });

        return html;
    }

    function constructDialingCodes(item) {
        var id = item.ContactItemType + "_checkbox_dialing_code" + item.ItemId;
        var html = $("<select id='" + id + "' class='contact-itemType' style='width:150px;' />");

        $.each(prospectingContext.IntlDialingCodes, function (idx, code) {
            html.append("<option value='" + code.Key + "'>" + code.Value + "</option>");
        });

        // Set the the recorded value. Default should be SA (+27)
        html.find("option[value='" + item.IntDialingCode + "']").attr('selected', true);

        // Wire up the change handler
        html.on('change', function () {
            var selectedCode = $(this).find(':selected').val();
            item.IntDialingCode = selectedCode;
        });

        return html;
    }

    function constructContentTextBox(item) {
        var id = item.ContactItemType + "_textbox" + item.ItemId;
        var cssClass = item.ContactItemType == 'PHONE' ? 'contact-phoneContent' : 'contact-emailContent';
        var html = $("<input type='text' id='" + id + "' class='" + cssClass + "' />");

        // Set item content if there exists
        html.val(item.ItemContent);
        if (typeof item.IsValid == 'undefined') {
            var isValid = false;
            if (item.ContactItemType == 'PHONE') {
                isValid = validPhoneNumber(item.ItemContent);
            }
            else {
                isValid = validEmailAddress (item.ItemContent);
            }
            item["IsValid"] = isValid;
        }
        var tooltipShown = false;
        // Wire up text change handler
        html.on('input', function () {
            var value = $(this).val();
            var itemType = item.ContactItemType;
            var thisTextbox = this;
            if (validInput(itemType, value)) {
                determineIfContactDetailExists(value, function (data) {
                    if (tooltipShown) {
                        tooltip.hide();
                        tooltipShown = false;
                    }
                    if (data) {
                        $(thisTextbox).addClass('input-box-warning');
                        tooltip.pop(thisTextbox, buildInvalidContentTooltip(data), { sticky: true, maxWidth: 500 });
                        tooltipShown = true;
                        item.IsValid = true;
                        toggleSaveBtnEnabled();
                    }
                    else {
                        $(thisTextbox).removeClass('input-box-warning');
                        if (tooltipShown) {
                            tooltip.hide();
                            tooltipShown = false;
                        }
                        item.IsValid = true;
                        toggleSaveBtnEnabled();
                    }
                });
            }
            else {
                if (tooltipShown) {
                    tooltip.hide();
                    tooltipShown = false;
                }
                $(thisTextbox).addClass('input-box-warning');
                if (itemType == 'PHONE') {
                    tooltip.pop(thisTextbox, 'Phone number must be 10-digits in length', { sticky: true, maxWidth: 500 });
                    item.IsValid = false;
                    tooltipShown = true;
                }
                else {
                    tooltip.pop(thisTextbox, 'Email address must be in a valid format', { sticky: true, maxWidth: 500 });
                    item.IsValid = false;
                    tooltipShown = true;
                }
                toggleSaveBtnEnabled();
            }

            item.ItemContent = value;
        });

        function validInput(type, value) {
            return type == 'PHONE' ? validPhoneNumber(value) : validEmailAddress(value);
        }

        function buildInvalidContentTooltip(existingContact) {
            var str = 'Warning: This contact detail is already owned by: ' + existingContact.Firstname + ' ' + existingContact.Surname;
            str += '<br />ID number: ' + existingContact.IdNumber;

            return str;
        }        

        function determineIfContactDetailExists(value, action) {
            var idNumber = currentPersonContact ? currentPersonContact.IdNumber : "999999999999";
            $.ajax({
                type: "POST",
                url: "RequestHandler.ashx",
                data: JSON.stringify({ Instruction: 'check_for_existing_contact', PhoneNumbers: [value], EmailAddresses: [value], IdNumber: idNumber }),
                dataType: "json",
            }).done(function (data) {
                action(data);
            });
        }

        return html;
    }

    function toggleSaveBtnEnabled(enabledState) {

        toggleSaveButtonVisible();

        if (typeof enabledState != 'undefined') {
            $('#saveContactDetailsBtn').prop('disabled', !enabledState);
            return;
        }

        var invalidPhoneNumbers = [];
        if (_phoneNumbers != null) {
            invalidPhoneNumbers = $.grep(_phoneNumbers, function (ph) {
                return ph.IsValid == false;
            });
        }

        var invalidEmailAddresses = [];
        if (_emailAddresses != null) {
            invalidEmailAddresses = $.grep(_emailAddresses, function (em) {
                return em.IsValid == false;
            });
        }

        var disabled = invalidPhoneNumbers.length > 0 || invalidEmailAddresses.length > 0;
        $('#saveContactDetailsBtn').prop('disabled', disabled);
    }

    function constructDeleteItemLink(item) {
        var id = item.ContactItemType + "_delete" + item.ItemId;
        var html = $("<img src='Assets/bin.png' id='" + id + "' class='contact-itemDelete' title='Remove' />");

        containerDiv.on('click', '#' + id, function () {
            if (item.ContactItemType == 'PHONE') {
                deletePhoneContactItem(item);
            } else {
                deleteEmailContactItem(item);
            }
        });

        return html;
    }

    function deletePhoneContactItem(item) {
        $('#PHONE_checkbox' + item.ItemId).remove();
        $('#PHONE_textbox' + item.ItemId).remove();
        $('#PHONE_primary' + item.ItemId).remove();
        $('#PHONE_delete' + item.ItemId).remove();

        $('#PHONE_checkbox_dialing_code' + item.ItemId).remove();
        $('#PHONE_textbox_eleventh_digit' + item.ItemId).remove();

        // Remove from array
        _phoneNumbers = $.grep(_phoneNumbers, function (target) {
            return target.ItemId != item.ItemId;
        });

        toggleSaveBtnEnabled();
    }

    function deleteEmailContactItem(item) {
        $('#EMAIL_checkbox' + item.ItemId).remove();
        $('#EMAIL_textbox' + item.ItemId).remove();
        $('#EMAIL_primary' + item.ItemId).remove();
        $('#EMAIL_delete' + item.ItemId).remove();

        // Remove from array
        _emailAddresses = $.grep(_emailAddresses, function (target) {
            return target.ItemId != item.ItemId;
        });

        toggleSaveBtnEnabled();
    }

    function constructPrimaryContactLink(item) {
        var id = item.ContactItemType + "_primary" + item.ItemId;
        var imgSrc = item.IsPrimary ? "Assets/primary_contact.png" : "Assets/make_primary_contact.png";

        function determineAltTextOfItem(item) {
            if (item.ContactItemType == 'EMAIL') {
                return item.IsPrimary ? "Primary email address" : "Make primary email address";
            }
            else {
                // PHONE
                if (item.IsPrimary) {
                    var cell = 3, home = 1, work = 2; // TODO: do this correctly...
                    switch (item.ItemType) {
                        case cell: return "Primary cellphone number";
                        case home: return "Primary home landline";
                        case work: return "Primary work landline";
                    }
                }
                return "Make primary contact number";
            }
        }
        
        //var title = item.IsPrimary ? "Primary Contact" : "Make Primary Contact";
        var html = $("<img src='" + imgSrc + "' id='" + id + "' class='contact-itemPrimary' title='" + determineAltTextOfItem(item) + "' />");

        containerDiv.unbind('click.' + id).on('click.' + id, '#' + id, function () {
            var itemId;
            var targetItem;
            var targetArray;
            if (item.ContactItemType == 'PHONE') {
                itemId = $(this).attr("id").replace('PHONE_primary', '');
                targetArray = _phoneNumbers;
            } else {
                itemId = $(this).attr("id").replace('EMAIL_primary', '');
                targetArray = _emailAddresses;
            }
            //TODO: test this by selecting between different suburbs, change title text accordingly, only select one per item type.
            targetItem = $.grep(targetArray, function (i) {
                return i.ItemId == itemId;
            })[0];
            
            if (targetItem.IsPrimary) {
                targetItem.IsPrimary = false;
                $(this).attr('src', "Assets/make_primary_contact.png");
            }
            else {
                // Make primary
                targetItem.IsPrimary = true;
                $(this).attr('src', "Assets/primary_contact.png");
            }
            $(this).attr('title', determineAltTextOfItem(targetItem));
            // Change the icon                      
            // Remove primary flag from the others, and reset their icons.
            var targetItemType = targetItem.ItemType;
            $.each(targetArray, function (index, i) {
                if (targetItem.IsPrimary && i.ItemId != targetItem.ItemId) {
                    // If an item exists with the same type as the target item, remove its primary status.
                    if (i.ItemType == targetItemType) {
                        i.IsPrimary = false;
                        $('#' + targetItem.ContactItemType + '_primary' + i.ItemId).attr('src', "Assets/make_primary_contact.png");
                        $('#' + targetItem.ContactItemType + '_primary' + i.ItemId).attr('title', determineAltTextOfItem(i));
                    }
                }
            });
        });

        return html;
    }

    function constructEleventhDigit(item) {
        var id = item.ContactItemType + "_textbox_eleventh_digit" + item.ItemId;
        var cssClass = item.ContactItemType == 'PHONE' ? 'contact-phoneContent' : 'contact-emailContent';
        var html = $("<input type='text' id='" + id + "' class='" + cssClass + "' title='11th digit if phone number exceeds 10-digits' style='width:20px;' />");

        // Set item content if there exists
        if (item.EleventhDigit != null) {
            html.val(item.EleventhDigit);
        }

        var tooltipShown = false;
        html.on('input', function () {
            var value = $(this).val();
            item.EleventhDigit = value;
            var thisTextbox = this;
            // validate
            if (!valueIsSingleDigit(value)) {
                if (tooltipShown) {
                    tooltip.hide();
                    tooltipShown = false;
                }

                $(thisTextbox).addClass('input-box-warning');
                tooltip.pop(thisTextbox, 'Must be single digit', { sticky: true, maxWidth: 500 });
                    item.IsValid = false;
                    tooltipShown = true;
                
                toggleSaveBtnEnabled();
            }
            else {
                $(thisTextbox).removeClass('input-box-warning');
                if (tooltipShown) {
                    tooltip.hide();
                    tooltipShown = false;
                }
                item.IsValid = true;
                toggleSaveBtnEnabled();
            }
        });

        return html;
    }

    function buildPhoneContactRow(item, itemTypes) {

        var typeCheckBox = constructTypeCheckBox(item, itemTypes);
        var dialingCodeCheckBox = constructDialingCodes(item);
        var contentTextBox = constructContentTextBox(item);
        var eleventhDigit = constructEleventhDigit(item);
        var deleteItemLink = constructDeleteItemLink(item);
        var isPrimaryContactLink = constructPrimaryContactLink(item);

        return [typeCheckBox, dialingCodeCheckBox, contentTextBox, eleventhDigit, deleteItemLink, isPrimaryContactLink];
    }

    function buildEmailContactRow(item, itemTypes) {
        var typeCheckBox = constructTypeCheckBox(item, itemTypes);
        var contentTextBox = constructContentTextBox(item);
        var deleteItemLink = constructDeleteItemLink(item);
        var isPrimaryContactLink = constructPrimaryContactLink(item);

        return [typeCheckBox, contentTextBox, deleteItemLink, isPrimaryContactLink];
    }

    function addContactRow(someDiv, elements) {
        //var theDiv = $('#' + div.attr('id'));
        var html = $('<div />');
        $.each(elements, function (index, element) {
            html.append(element);
        });

        someDiv.append(html);
        someDiv.append("<p class='contact-newItemSplitter' />");
    }

    function addContactPhoneCaptureOptions(targetDiv) {

        var lookupsDiv = $("<div id='contactPhoneLookups' />");
        var dracoreBtn = $("<input id='dracorePhoneLookupBtn' type='button' style='margin-right: 5px;display:inline-block;cursor:pointer;' value='Dracore Look-up' title='This will cost R 0.60c (excl. VAT) if successful'></input>");
        var tracepsBtn = $("<input id='tracepsLookupBtn' type='button' style='margin-right: 5px;display:inline-block;cursor:pointer;' value='TracePS Look-up' title='This will cost R 0.60c (excl. VAT) if successful'></input>");
        var link = $("<input type='button' id='addPhone' value='Manually add new number..' class='addNewItemBtn' style='display:inline-block;cursor:pointer;' />");
        lookupsDiv.append(dracoreBtn).append(tracepsBtn).append(link).append('<p />');

        targetDiv.append(lookupsDiv);
        dracoreBtn.prop('disabled', !_canEdit);
        tracepsBtn.prop('disabled', !_canEdit);
        link.prop('disabled', !_canEdit);

        containerDiv.unbind('click.addPhone').on('click.addPhone', '#addPhone', function () {

            if (!_phoneNumbers) _phoneNumbers = [];
            var itemId = generateUniqueID();
            var newRow = newContactItem(itemId, null, null, null, true, 'PHONE', getSADialingCodeId(), null);
            var elements = buildPhoneContactRow(newRow, _phoneTypes);
            addContactRow(targetDiv, elements);

            _phoneNumbers.push(newRow);
            toggleSaveBtnEnabled(false);
        });

        containerDiv.unbind('click.dracorePhoneLookupBtn').on('click.dracorePhoneLookupBtn', '#dracorePhoneLookupBtn', function () {            
            var idNumber = getIDNumberForLookup();
            if (idNumber) {
                performPersonLookup(idNumber, 'DRACORE_PHONE');
            } else {
                alert('The ID number specified is not valid.');
            }
        });

        containerDiv.unbind('click.tracepsLookupBtn').on('click.tracepsLookupBtn', '#tracepsLookupBtn', function () {
            var idNumber = getIDNumberForLookup();
            if (idNumber) {
                performPersonLookup(idNumber, 'TRACEPS');
            } else {
                alert('The ID number specified is not valid.');
            }
        });
    }

    function getIDNumberForLookup() {
        // currentPersonContact != null, currentPersonContact == null, currentPersonContact == null BUT an ID number has been entered (not saved - test before and after save) 
        var idNumber = null;
        if (currentPersonContact != null) {
            idNumber = currentPersonContact.IdNumber;
        }
        if (currentPersonContact == null) {
            // Test whether an ID number has been entered when capturing manually
            var idNo = $('#idOrCkTextBox').val().trim();
            if (idNo.length == 13) { // change this to do a proper ID check.
                idNumber = idNo;
            }
        }

        return idNumber;
    }

    function addContactEmailCaptureOptions(targetDiv) {
        var lookupDiv = $("<div id='contactEmailLookups' />");
        var dracoreEmail = $("<input id='dracoreEmailLookupBtn' type='button' style='margin-right: 5px;display:inline-block;cursor:pointer;' value='Dracore Look-up' title='This will cost R 0.40c (excl. VAT) if successful'></input>");
        var link = $("<input type='button' id='addEmail' value='Manually add new email..' class='addNewItemBtn' style='display:inline-block;cursor:pointer;' />");
        lookupDiv.append(dracoreEmail).append(link).append('<p />');

        targetDiv.append(lookupDiv);
        dracoreEmail.prop('disabled', !_canEdit);
        link.prop('disabled', !_canEdit);

        containerDiv.unbind('click.addEmail').on('click.addEmail', '#addEmail', function () {

            if (!_emailAddresses) _emailAddresses = [];
            var itemId = generateUniqueID();
            var newRow = newContactItem(itemId, null, null, null, true, 'EMAIL', null, null);
            var elements = buildEmailContactRow(newRow, _emailTypes);
            addContactRow(targetDiv, elements);

            _emailAddresses.push(newRow);
            toggleSaveBtnEnabled(false);
        });

        containerDiv.unbind('click.dracoreEmailLookupBtn').on('click.dracoreEmailLookupBtn', '#dracoreEmailLookupBtn', function () {
            var idNumber = getIDNumberForLookup();
            if (idNumber) {
                performPersonLookup(idNumber, 'DRACORE_EMAIL');
            } else {
                alert('The ID number specified is not valid.');
            }
        });
    }

    return this;
}