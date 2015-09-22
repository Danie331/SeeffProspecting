var currentPersonContact; // Stores a reference to the currently active (selected) contact
var currentCompanyContact;
var contactDetailsWidget = null;
var expanderWidget = null;

function buildPersonContactMenu(propertyContacts, update, context) {
    if (!propertyContacts) propertyContacts = [];
    var contactDetailsDiv = $('#contactDetailsDiv');

    contactDetailsDiv.empty();
    var contactDashboard = buildContactDashboard(propertyContacts, context);
    contactDetailsDiv.append(contactDashboard);
    contactDetailsDiv.append("<p />");

    if (update) {
        // If updating the dashboard rebuild as above and open current contact person (which should already be open)
        openExpanderWidget(currentPersonContact, context);
    }
}

function openExpanderWidget(contact, context) {
    // build the expander widget
    var contactDetailsDiv = $('#contactDetailsDiv');
    expanderWidget = buildExpanderWidget(contact, context);
    var div = $('#contactPlaceHolder').length == 0 ? $("<div id='contactPlaceHolder' style='padding:10px;' />") : $('#contactPlaceHolder');
    div.empty();
    div.append(expanderWidget.construct());
    contactDetailsDiv.append(div);

    // "Click" the first div
    expanderWidget.open("personGeneral");
    expanderWidget.open("personContact");
}

function getProspectingPhoneTypeId(itemTypeText) {
    switch (itemTypeText)
    {
        case "cell": return $.grep(prospectingContext.ContactPhoneTypes, function (i) { return i.Value == 'cell'; })[0].Key;
        case "work": return $.grep(prospectingContext.ContactPhoneTypes, function (i) { return i.Value == 'work phone'; })[0].Key;
        case "home": return $.grep(prospectingContext.ContactPhoneTypes, function (i) { return i.Value == 'home phone'; })[0].Key;
    }
}

function openAndPopulateNewContactPerson(infoPacket) {
    var contactNos = [], contactEmails = [];
    $.each(infoPacket.ContactRows, function (idx, item) {
        switch (item.Type) {
            case "cell": case "home": case "work":
                var phoneNumberTypeId = getProspectingPhoneTypeId(item.Type);
                var contactRow = newContactItem(generateUniqueID(), phoneNumberTypeId, item.Phone, false, true, 'PHONE', getSADialingCodeId(), null);
                contactNos.push(contactRow);
                break;
            case "email":
                var emailType = $.grep(prospectingContext.ContactEmailTypes, function (i) { return i.Value == 'private email'; })[0].Key;
                var emailRow = newContactItem(generateUniqueID(), emailType, item.EmailAddress, false, true, 'EMAIL', null, null);
                contactEmails.push(emailRow);
                break;
        }
    });

    var ownerRelationshipType = $.grep(prospectingContext.PersonPropertyRelationshipTypes, function (val) {
        return val.Value == 'Owner';
    })[0];

    infoPacket.OwnerName = infoPacket.OwnerName != null ? toTitleCase(infoPacket.OwnerName) : null;
    infoPacket.OwnerSurname = infoPacket.OwnerSurname != null ? toTitleCase(infoPacket.OwnerSurname) : null;
    var newContact = newPersonContact(infoPacket.OwnerName,
        infoPacket.OwnerSurname,
        infoPacket.Title,
        infoPacket.IdNumber,
        ownerRelationshipType.Key,
        contactNos, contactEmails,
        false,
        infoPacket.OwnerGender,
        null,
        null,
        infoPacket.DeceasedStatus,
        infoPacket.AgeGroup,
        infoPacket.Location,
        infoPacket.MaritalStatus,
        infoPacket.HomeOwnership,
        infoPacket.Directorship,
        infoPacket.PhysicalAddress,
        infoPacket.Employer,
        infoPacket.Occupation,
        infoPacket.BureauAdverseIndicator,
        infoPacket.Citizenship);
    return newContact;
}

function buildExpanderWidget(contact, context) {
    $('#personGeneral').empty();
    var personGeneralInfo = buildContentExpanderItem('personGeneral', 'Assets/person.png', "Contact Person", buildGeneralInfoHtml(contact, context));
    $('#personContact').empty();
    var personContactInfo = buildContentExpanderItem('personContact', 'Assets/contact.png', "Contact Details", buildContactDetailsHtml(contact, context));

    return new ContentExpanderWidget('#contentarea', [personGeneralInfo, personContactInfo], "contactsExpander");
}

function buildGeneralInfoHtml(contact, context) {
    $('#contactPersonHeaderDiv').remove();
    var html = $("<div id='contactPersonHeaderDiv' />");

    // POPI opt-out box(es)
    var optoutDiv = $("<div></div>");
    var optoutFieldset = $("<fieldset style='border:1px solid gray'></fieldset>");
    optoutFieldset.append("<legend style='padding: 0.2em 0.5em; border:1px solid gray; color:gray;font-size:90%;'>Communication</legend>");
    var checked = '';
    if (contact && contact.IsPOPIrestricted) {
        checked = 'checked';
    }
    var popiDisabledSetting = userIsProspectingManager ? '' : 'disabled';
    optoutFieldset.append("<label style='cursor:pointer;vertical-align:middle' title='Select this option to indicate that this person is no longer to be contacted'>POPI opt-out<input type='checkbox' name='popiCheckbox' id='popiCheckbox' style='cursor:pointer;vertical-align:middle;margin-right:20px' " + checked + " " + popiDisabledSetting + "  /></label>");
    if (prospectingContext.UserHasCommAccess) { // check if POPi still works
        var emailChecked = '', emailDisabledOption = '';
        if (contact && contact.EmailOptout) {
            emailChecked = 'checked';
            emailDisabledOption = 'disabled';
        }
        optoutFieldset.append("<label style='cursor:pointer;vertical-align:middle' title='Select this option to indicate that this person is not to be contacted by email'>Opt-out from email<input type='checkbox' name='emailOptoutCheckbox' id='emailOptoutCheckbox' style='cursor:pointer;vertical-align:middle;margin-right:20px' " + emailChecked + " " + emailDisabledOption + "  /></label>");
        var smsChecked = '', smsDisabledOption = '';
        if (contact && contact.SMSOptout) {
            smsChecked = 'checked';
            smsDisabledOption = 'disabled';
        }
        optoutFieldset.append("<label style='cursor:pointer;vertical-align:middle' title='Select this option to indicate that this person is not to be contacted by SMS'>Opt-out from SMS<input type='checkbox' name='smsOptoutCheckbox' id='smsOptoutCheckbox' style='cursor:pointer;vertical-align:middle;margin-right:20px' " + smsChecked + " " + smsDisabledOption + "  /></label>");
    }
    optoutDiv.append(optoutFieldset);
    html.append(optoutDiv);

    html.append("<br />");
    html.append("<label class='fieldAlignment'>Gender: </label>");
    html.append(buildPersonGenderCombo());
    html.append("<br />");
    html.append("<label class='fieldAlignment'>Title: </label>");
    html.append(buildPersonTitleCombo());
    html.append("<br />");
    var makeFirstNameTitleCase = $('<input type="image" src="Assets/titlecase_icon.png" title="Convert to Title Case" style="display:inline-block;vertical-align:middle; border: 1px outset lightgray;margin-left:1px;" />'); // title=, inline-block, test browser resize, add a new contact + retest, cross browser, change of ownership test.
    var firstNameInputItem = $("<label class='fieldAlignment'>First name:</label><input type='text' id='firstNameTextBox' name='firstNameTextBox' size='25' style='display:inline-block;vertical-align:middle;'/>");
    html.append(firstNameInputItem).append(makeFirstNameTitleCase);
    makeFirstNameTitleCase.click(function (e) {
        e.preventDefault();
        var value = $('#firstNameTextBox').val();
        if (value.length) {
            value = toTitleCase(value);
            $('#firstNameTextBox').val(value);
        }
    });

    html.append("<br />");
    var makeSurnameTitleCase = $('<input type="image" src="Assets/titlecase_icon.png" title="Convert to Title Case" style="display:inline-block;vertical-align:middle; border: 1px outset lightgray;margin-left:1px;" />');
    var surnameInputItem = $("<label class='fieldAlignment'>Surname:</label><input type='text' id='surnameTextBox' name='surnameTextBox' size='25' style='display:inline-block;vertical-align:middle;' />");
    html.append(surnameInputItem).append(makeSurnameTitleCase);
    makeSurnameTitleCase.click(function (e) {
        e.preventDefault();
        var value = $('#surnameTextBox').val();
        if (value.length) {
            value = toTitleCase(value);
            $('#surnameTextBox').val(value);
        }
    });

    html.append("<br />");
    html.append("<label class='fieldAlignment'>ID no.:</label><input type='text' id='idOrCkTextBox' name='idOrCkTextBox' size='15'/>");
    html.append("<br />");

    var relationshipText = currentProperty.ContactCompanies.length > 0 ? "Relationship to property/company: " : "Relationship to property: ";
    html.append("<label class='fieldAlignment'>" + relationshipText + "</label>");
    var relationshipToPropertySelect = buildRelationshipToPropertyTypesCombo();
    html.append(relationshipToPropertySelect);

    html.append("<br />");
    var companySelect = buildCompanySelectorCombo();
    html.append(companySelect);

    // default to related to.
    if (currentPersonContact) {       

        if (context && context.FromFollowup) {
            //check for SS, FH, button for property only as well as RelatedTo, problem: when user updates contact must preserve feedback btn., then hide btn., remove Add Actvity btn.
            html.append("<br />");
            var followupBtn = $("<button type='text' id='feedbackFromContactBtn' style='cursor:pointer;display:inline-block;vertical-align:middle;margin-left:5px'><img src='Assets/add_activity.png' style='display:inline-block;vertical-align:middle;margin-right:5px' /><label style='vertical-align:middle'>Feedback</label></button>");
            html.append(followupBtn);
            followupBtn.click(function (e) {
                e.preventDefault();
                handleAddFollowupActivity(context.FromFollowup, function () {
                    var index = globalFollowUps.indexOf(context.FromFollowup);
                    globalFollowUps.splice(index, 1);

                    performFollowupFiltering(globalFollowUps, $('#followupContent'));

                    followupBtn.remove();
                    //if (sourceFollowups.length == 0) {
                    //    loadMoreFollowups();
                    //}
                });
            });
        } else {
            html.append("<br />");
            var addActivityBtn = $("<button type='text' id='contactAddActivity' style='cursor:pointer;display:inline-block;vertical-align:middle'><img src='Assets/add_activity.png' style='display:inline-block;vertical-align:middle;margin-right:5px' /><label style='vertical-align:middle'>Add Activity</label></button>");
            html.append(addActivityBtn);
            addActivityBtn.click(function (e) {
                e.preventDefault();
                handleAddActivityFromContact();
            });
        }
    }

    html.append("<p />");
    var saveBtn = $("<input type='button' id='personGeneralSaveBtn' value='Save..' style='cursor:pointer;' />");
    html.append(saveBtn);
    saveBtn.click(function () {
        handleSaveContactPerson(null, context);
    });

    if (contact) {
        // If this contact has any contact details, hide the save button on the person header section
        if ((contact.PhoneNumbers && contact.PhoneNumbers.length > 0) || (contact.EmailAddresses && contact.EmailAddresses.length > 0)) {
            saveBtn.css('display', 'none');
        }

        html.find('#personTitle').val(contact.Title);
        html.find('#firstNameTextBox').val(contact.Firstname);
        html.find('#surnameTextBox').val(contact.Surname);
        html.find('#idOrCkTextBox').val(contact.IdNumber);
        
        if (contact.PersonPropertyRelationships && contact.PersonPropertyRelationships.length) {
            var relationship = $.grep(contact.PersonPropertyRelationships, function (rel) {
                return currentProperty.ProspectingPropertyId == rel.Key;
            })[0];

            html.find("#relTypesSelect").val('pr' + (relationship ? relationship.Value : 1));
            companySelect.css('display', 'none');
        } else if (contact.PersonCompanyRelationshipType) {
            html.find("#relTypesSelect").val('cr' + contact.PersonCompanyRelationshipType);
            html.find('#companySelector').val(contact.ContactCompanyId);
            companySelect.css('display', 'block');
        }
        html.find('#genderCheckbox').val(contact.Gender);
        //popiBox.find('#popiCheckbox').prop('checked', contact.IsPOPIrestricted);

        // Determine if POPI flag is set for this contact, render info accordingly.
        if (contact.IsPOPIrestricted) {
            html.prepend("<label style='color:red;font-size:14px;'>This contact has opted out and may not be modified.</label><br />");
            html.find('input').attr('disabled', true);
            html.find('select').prop('disabled', true);
            html.find('button').prop('disabled', true);
        }
    }

    return html;
}

function buildRelationshipsToContact(contacts) {
    var relatedContactRows = $("div />");
    $.each(contacts, function (index, contact) {
        var row = buildContactRelationshipRow(contact);
        relatedContactRows.append(row);
    });

    return relatedContactRows;
}

function buildContactRelationshipRow(contact) {
    var rowItem = $("<label class='fieldAlignment' />");
    rowItem.append("<input type='text' value='" + contact.ContactPersonId + "' readonly>" + contact.Fullname + "</input>");
    var select = $("<select />");
    select.append("<option value='' ></option>");
    $.each(prospectingContext.PersonPersonRelationshipTypes, function (idx, type) {
        select.append("<option value='" + type.Key + "' >" + type.Value + "</option>");
    });

    if (contact.RelationshipToContactType > 0) {
        select.val(contact.RelationshipToContactType);
    }
    rowItem.append(select);
    return rowItem;
}

function buildPersonTitleCombo() {
    var personTitleSelect = $("<select id='personTitle' />");
    personTitleSelect.append("<option value='' />");
    $.each(prospectingContext.ContactPersonTitle, function (idx, item) {
        personTitleSelect.append("<option value='" + item.Key +"'>" + item.Value + "</option>");
    });

    return personTitleSelect;
}

function buildPersonGenderCombo() {
    var personGenderSelect = $("<select id='genderCheckbox' />");
    personGenderSelect.append("<option value='' />");
    personGenderSelect.append("<option value='M'>M</option>");
    personGenderSelect.append("<option value='F'>F</option>");

    return personGenderSelect;
}

function buildCompanySelectorCombo() {
    var selectorDiv = $("<div id='companySelectorContainer' style='display:none;'><label class='fieldAlignment'>Associated company: </label><select id='companySelector' /></div>");
    var selector = selectorDiv.find('#companySelector');
    $.each(currentProperty.ContactCompanies, function (idx, c) {
        var option = $("<option value='" + c.ContactCompanyId + "'>" + c.CompanyName + "</option>");
        selector.append(option);
    });

    return selectorDiv;
}

function buildRelationshipToPropertyTypesCombo() {
    var relationshipSelect = $("<select id='relTypesSelect' />");
    relationshipSelect.append("<option value=''></option>");
    $.each(prospectingContext.PersonPropertyRelationshipTypes, function (idx, rel) {
        var option = $("<option data-type='propertyRelationship' value='pr" + rel.Key + "'>" + rel.Value + "</option>");
        relationshipSelect.append(option);
    });

    if (currentProperty.ContactCompanies.length) {
        $.each(prospectingContext.PersonCompanyRelationshipTypes, function (idx, rel) {
            var option = $("<option data-type='companyRelationship' value='cr" + rel.Key + "'>" + rel.Value + "</option>");
            relationshipSelect.append(option);
        });
    }

    relationshipSelect.change(function () {
        // get the type of ownership that was selected ie a property association or company association
        var associationType = $(this).find(':selected').data('type');
        switch (associationType) {
            case 'companyRelationship':
                showCompanySelect();
                break;
            default:
                removeCompanySelect();
                break;
        }
    });   

    return relationshipSelect;
}

function showCompanySelect() {
    var selector = $('#companySelectorContainer');
    selector.css('display', 'block');
}

function removeCompanySelect() {
    var selector = $('#companySelectorContainer');
    selector.css('display', 'none');
}

function buildContactDetailsHtml(contact, context) {

    var phoneNumbers = contact ? contact.PhoneNumbers : [];
    var emailAddresses = contact ? contact.EmailAddresses : [];
    var canEdit = contact ? !contact.IsPOPIrestricted : false;
    contactDetailsWidget = new ContactDetailsEditorWidget('#contentarea', phoneNumbers, emailAddresses,
function (ph, em) {
    handleSaveContactDetails(ph, em, context);
}, canEdit);
    return contactDetailsWidget.construct();
}

function handleSaveContactDetails(phoneNumbers, emailAddresses, context) {

    if (currentPersonContact) {
        currentPersonContact.PhoneNumbers = phoneNumbers;
        currentPersonContact.EmailAddresses = emailAddresses;
        var invalidItemsArray = [];
        if (!validateCorrectnessOfInputs(phoneNumbers, emailAddresses, invalidItemsArray)) {
            var displayText = 'Some of the contact details you have specified are not valid:\n\n';
            $.each(invalidItemsArray, function (idx, i) {
                displayText += i + "\n";
            });
            alert(displayText);
            return;
        }
    }

    handleSaveContactPerson(function () {

        if (currentPersonContact) {

            if (currentPersonContact.IsPOPIrestricted) {
                currentPersonContact.PhoneNumbers = [];
                currentPersonContact.EmailAddresses = [];
            }
            var invalidItemsArray = [];
            if (!validateCorrectnessOfInputs(phoneNumbers, emailAddresses, invalidItemsArray)) {
                var displayText = 'Some of the contact details you have specified are not valid:\n\n';
                $.each(invalidItemsArray, function (idx, i) {
                    displayText += i + "\n";
                });
                alert(displayText);
                return;
            }

            determineIfDetailsAreAvailableForUse(phoneNumbers, emailAddresses, function (existingContact) {
                //if (existingContact == null) {
                //    saveContact(currentPersonContact, currentProperty, function (data) {
                //        // currentPersonContact.ContactPersonId = data.ContactPersonId;                
                //        currentPersonContact.PhoneNumbers = data.PhoneNumbers;
                //        currentPersonContact.EmailAddresses = data.EmailAddresses;

                //        contactDetailsWidget.resetItemArrays(currentPersonContact.PhoneNumbers, currentPersonContact.EmailAddresses);
                //        contactDetailsWidget.rebuildDivs(!currentPersonContact.IsPOPIrestricted);
                //        addOrUpdateContactToCurrentProperty(data);

                //        if (currentProperty != null) {
                //            var hasContactDetails = false;
                //            if (data.PhoneNumbers && data.PhoneNumbers.length > 0)
                //                hasContactDetails = true;
                //            if (data.EmailAddresses && data.EmailAddresses.length > 0)
                //                hasContactDetails = true;

                //            currentProperty.Prospected = hasContactDetails;
                //        }
                //    });
                //} else {
                //    alert('A contact with these details already exists. ');
                //}
                if (existingContact != null) {
                    alert('A contact with these details already exists. ');
                }
            });
        }
    }, context);
}

function determineIfDetailsAreAvailableForUse(phoneNumbers, emailAddresses, actionToRun) {

    var phoneNumberContents = [];
    if (phoneNumbers != null) {
        $.map(phoneNumbers, function (ph) {
            return ph.ItemContent;
        });
    }

    var emailContents = [];
    if (emailAddresses!=null) {
        $.map(emailAddresses, function (em) {
            return em.ItemContent;
        });
    }

    if (currentPersonContact == null) {
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({ Instruction: 'check_for_existing_contact', PhoneNumbers: phoneNumberContents, EmailAddresses: emailContents, IdNumber: currentPersonContact.IdNumber }),
            dataType: "json",
        }).done(function (data) {
            if (!handleResponseIfServerError(data)) {
                return;
            }

            actionToRun(data);
        });
    } else {
        actionToRun(null);
    }
}

function handleSaveContactPerson(saveDetailsFunction, context) {

    if (!validateCorrectnessOfPersonInfo()) {
        alert('Cannot save: One or more fields are missing or the data has been entered incorrectly');
        return;
    }

    // Handle the POPI checkbox
    var popiOptionSelected = $('#popiCheckbox').is(':checked');
    if (popiOptionSelected) {
        var message = 'Warning: By selecting the POPI option this contact\'s details will be removed from the system and the record will become read-only.' + 
                      ' Only use this option if this person no longer wishes to be contacted through this system.' + 
                      '\n\nClick \'OK\' to continue with this action, \'Cancel\' to abort.';
        if (!confirm(message)) {
            return;
        }
    }

    if (currentPersonContact) {
        currentPersonContact.IsPOPIrestricted = popiOptionSelected;
        if (!popiOptionSelected) {
            currentPersonContact.PhoneNumbers = contactDetailsWidget.getPhoneNumbers();
            currentPersonContact.EmailAddresses = contactDetailsWidget.getEmailAddresses();
        }
        else {
            currentPersonContact.PhoneNumbers = [];
            currentPersonContact.EmailAddresses = [];
        }
    }

    savePersonInfo(function (existingContact) {
        if (existingContact == null) {
            var title = $('#personTitle').children(":selected").attr('value');
            var firstname = $('#firstNameTextBox').val();
            var surname = $('#surnameTextBox').val();
            var idNo = $('#idOrCkTextBox').val();

            var relationshipType = $('#relTypesSelect').children(":selected").data('type');
            var propertyRelationship = null, companyRelationship = null, contactCompanyId = null;
            switch (relationshipType) {
                case 'companyRelationship':
                    companyRelationship = $('#relTypesSelect').children(":selected").attr('value').replace('cr','');
                    contactCompanyId = $('#companySelector').find(":selected").val();
                    break;
                default: propertyRelationship = $('#relTypesSelect').children(":selected").attr('value').replace('pr','');
                    break;
            }
            //var relType = $('#relTypesSelect').children(":selected").attr('value');
            var gender = $('#genderCheckbox').children(":selected").attr('value');
            var emailOptout = $('#emailOptoutCheckbox').is(':checked');
            var smsOptout = $('#smsOptoutCheckbox').is(':checked');
            // Test for existing or new contact
            if (currentPersonContact) {   // existing..
                currentPersonContact.Title = title;
                currentPersonContact.Firstname = firstname;
                currentPersonContact.Surname = surname;
                currentPersonContact.Fullname = firstname + ' ' + surname;
                currentPersonContact.IdNumber = idNo;
                currentPersonContact.IsPOPIrestricted = popiOptionSelected;

                currentPersonContact.EmailOptout = emailOptout;
                currentPersonContact.SMSOptout = smsOptout;

                if (propertyRelationship) {
                    // Search for an existing relationship with property, if found modify its value
                    var relationship = $.grep(currentPersonContact.PersonPropertyRelationships, function (rel) {
                        return rel.Key == currentProperty.ProspectingPropertyId;
                    })[0];
                    
                    if (relationship) {
                        relationship.Value = propertyRelationship;
                    }
                    else {
                        currentPersonContact.PersonPropertyRelationships.push({ Key: currentProperty.ProspectingPropertyId, Value: propertyRelationship });
                    }
                    //currentPersonContact.PersonPropertyRelationshipType = propertyRelationship;

                    currentPersonContact.ContactCompanyId = null;
                    currentPersonContact.PersonCompanyRelationshipType = null;
                } else if (companyRelationship) {
                    currentPersonContact.PersonCompanyRelationshipType = companyRelationship;
                    currentPersonContact.ContactCompanyId = contactCompanyId;
                    currentPersonContact.PersonPropertyRelationships = [];
                }
                currentPersonContact.Gender = gender;                
            }
            else {
                // Must be a new contact
                if (propertyRelationship) {
                    currentPersonContact = newPersonContact(firstname, surname, title, idNo, propertyRelationship, null, null, popiOptionSelected, gender, null, null, emailOptout, smsOptout);
                }
                else if (companyRelationship) {
                    currentPersonContact = newPersonContact(firstname, surname, title, idNo, null, null, null, popiOptionSelected, gender, companyRelationship, contactCompanyId, emailOptout, smsOptout);
                }
            }
            saveContact(currentPersonContact, currentProperty, function (data) {
                //alert('Contact saved successfully!');
                showSavedSplashDialog('Contact Saved!');
                currentPersonContact.ContactPersonId = data.ContactPersonId;
                addOrUpdateContactToCurrentProperty(data);
                buildPersonContactMenu(currentProperty.Contacts, true, context);

                currentPersonContact.PhoneNumbers = data.PhoneNumbers;
                currentPersonContact.EmailAddresses = data.EmailAddresses;

                if (saveDetailsFunction) {
                    saveDetailsFunction();
                }
                updateProspectedStatus();
                setCurrentMarker(currentSuburb, currentProperty);                               
            });
        }
        else {
            //alert('Cannot save this contact: A contact with this ID number already exists.');
            showDialogExistingContactFound(existingContact, function () {
                currentPersonContact = existingContact;
                saveContact(currentPersonContact, currentProperty, function (data) {
                    //alert('Contact saved successfully!');
                    showSavedSplashDialog('Contact Saved!');
                    addOrUpdateContactToCurrentProperty(data);
                    buildPersonContactMenu(currentProperty.Contacts, false, context);
                    updateProspectedStatus();
                    setCurrentMarker(currentSuburb, currentProperty);
                });
            });
        }
    });
}

function updateProspectedStatus(property) {

    var targetProperty = currentProperty;
    if (property) {
        targetProperty = property;
    }

    if (targetProperty != null) {
        
        var hasContactDetails = false;
        $.each(targetProperty.Contacts, function (idx, c) {
            if (c.PhoneNumbers != null && c.PhoneNumbers.length > 0)
                hasContactDetails = true;

            if (c.EmailAddresses != null && c.EmailAddresses.length > 0)
                hasContactDetails = true;
        });
        
        targetProperty.Prospected = hasContactDetails;
        targetProperty.Marker.setIcon(getIconForMarker(targetProperty.Marker));
        //if (targetProperty.Prospected) {
        //    if (targetProperty.SS_FH == "FH") {
        //        targetProperty.Marker.setIcon('Assets/marker_icons/prospecting/prospected.png');
        //    } else {
        //        changeBgColour(targetProperty.LightstonePropertyId, "#009900");
        //    }
        //}
        //else {
        //    if (targetProperty.SS_FH == "FH") {
        //        targetProperty.Marker.setIcon('Assets/marker_icons/prospecting/unprospected.png');
        //    } else {
        //        changeBgColour(targetProperty.LightstonePropertyId, "#FBB917");
        //    }
        //}

        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({ Instruction: 'update_prospected_flag', LightstonePropertyId: targetProperty.LightstonePropertyId, Prospected: targetProperty.Prospected }),
            dataType: "json"
        }).done(function (data) {
            if (!handleResponseIfServerError(data)) {
                return;
            }
        });
    }
}

function validateCorrectnessOfPersonInfo() {
    var title = $('#personTitle').children(":selected").attr('value');
    var firstname = $('#firstNameTextBox').val();
    var surname = $('#surnameTextBox').val();
    var relType = $('#relTypesSelect').children(":selected").attr('value');
    var gender = $('#genderCheckbox').children(":selected").attr('value');
    var idNo = $('#idOrCkTextBox').val().trim();

    return firstname.length > 0 && surname.length > 0 && relType.length > 0 && gender.length > 0 && idNo.length == 13;
}

// Valid if the contact has at least firstname, surname, and relationship type
function savePersonInfo(continueWithAction) {
    var idNumber = null;
    if (!currentPersonContact) {
        idNumber = $('#idOrCkTextBox').val();
    }
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: 'check_for_existing_contact', PhoneNumbers: [], EmailAddresses: [], IdNumber: idNumber }),
        dataType: "json",
    }).done(function (data) {
        if (!handleResponseIfServerError(data)) {
            return;
        }

        continueWithAction(data);
    });
}

function validateCorrectnessOfInputs(phoneNumbers, emailAddresses, invalidItemsArray) {

    if (!currentPersonContact) return false;

    var isValid = true;
    if (phoneNumbers) {
        $.each(phoneNumbers, function (idx, item) {
            if (!item.ItemType || item.ItemType == 'nothing') {
                isValid = false;
                invalidItemsArray.push("You need to select a phone number 'type'.");
            }
            if (!item.ItemContent || item.ItemContent.length == 0 || item.ItemContent.length > 10) {
                isValid = false;
                invalidItemsArray.push("This phone number is not valid: " + item.ItemContent);
            }
        });
    }

    if (emailAddresses) {
        $.each(emailAddresses, function (idx, item) {
            if (!item.ItemType || item.ItemType == 'nothing') {
                isValid = false;
                invalidItemsArray.push("You need to select an email 'type'.");
            }
            if (!item.ItemContent || item.ItemContent.length == 0) {
                isValid = false;
                invalidItemsArray.push("This email address is not valid: " + item.ItemContent);
            }
        });
    }

    return isValid;
}

function buildNotesHtml(contact) {

    var notesDiv = $("<div id='personNotesDiv' />");
    var notesContent = contact && contact.Comments ? contact.Comments : "Add notes for this person here.";
    notesDiv.append("<textarea id='personNotesCommentsTextArea' style='width:100%;' rows='8'>" + notesContent + "</textarea>");
   
    notesDiv.append("<p />");
    var saveBtn = $("<input type='button' id='personNotesCommentsSaveBtn' value='Save..' />");
    notesDiv.append(saveBtn);
    saveBtn.click(handleSavePersonNotes);

    return notesDiv;
}

function handleSavePersonNotes() {
 
    if (currentPersonContact) {
        var textarea = $('#personNotesCommentsTextArea');
        currentPersonContact.Comments = textarea.val();
        saveContact(currentPersonContact, currentProperty, function (data) {            
            addOrUpdateContactToCurrentProperty(data);
            //alert('Notes/comments saved successfully!');
            showSavedSplashDialog('Saved!');
        });
    }
    else {
        alert('You must first save a contact before adding contact information.');
    }
}

function handlePOPIclick() {

    var checkbox = this;
    if (checkbox.checked) {
        // box is being checked
        var popiText = 'Selecting this option indicates that this person has been contacted previously and does not wish to be contacted through this system in the future.\
                        \nThe POPI act requires that in this instance all contact information be deleted and no further contact be made with this person';
        alert(popiText);
    }
    else {
        // box is being unchecked, check whether this contact was previously saved with the popi flag.
        if (currentPersonContact.IsPOPIrestricted) {
            alert('Warning: this person has been flagged as not-contactable. By deselecting this option ensure that you have perimission to do so.');
        }
    }

    // Pop a dialog explaining that: this person has been contacted and refused and according to the popi act, may not contact them
    // in addition, all there contact info will be deleted
    // If user confirms, then check the box and continue, else uncheck it and break.
    // If the box is already checked and they are unchecking it: warn them what they are doing
    // Set popi_rest of contact in memory, ensure dynamic changes are pushed to the database *ONLY* when save button is clicked.
    // Delete all contact info, disable buttons/toggle based on click
    // If icnoming data is popi_restricted, disable buttons
}

function buildPropertyAddressEditor(property) {
    var addressComponents = stripPropertyAddress(property); // StreetOrUnitNo: streetOrUnitNo, StreetName: streetPortion, Suburb: suburbPortion, CityTown: townPortion
    var addressContent = $("<div />");
    var updateAddressBtn = $("<input type='button' id='updatePropertyDetailsBtn' value='Update Details..' />")
    if (property.SS_FH == "FH") {
        addressContent.append("<label class='fieldAlignment'>Street number:</label><input type='text' id='streetNoTextBox' name='streetNoTextBox' size='5' value='" + addressComponents.StreetOrUnitNo + "' />");
        addressContent.append("<br />");
        addressContent.append("<label class='fieldAlignment'>Street name:</label><input type='text' id='streetNameTextBox' name='streetNameTextBox' size='40' value='" + addressComponents.StreetName + "' />");
        addressContent.append("<br />");
        addressContent.append("<label class='fieldAlignment'>Suburb:</label><input type='text' id='suburbTextBox' name='suburbTextBox' size='40' value='" +  addressComponents.Suburb + "' />");
        addressContent.append("<br />");
        addressContent.append("<label class='fieldAlignment'>City/Town:</label><input type='text' id='cityTownTextBox' name='cityTownTextBox' size='40' value='" + addressComponents.CityTown + "' />");
        addressContent.append("<p />");
        addressContent.append("<label class='fieldAlignment'>ERF no.:</label><input type='text' id='erfTextBox' name='erfTextBox' size='10' value='" + property.ErfNo + "' readonly/>");
        addressContent.append("<p />");
        addressContent.append(updateAddressBtn);
    }
    else if (property.SS_FH == 'SS' || property.SS_FH == 'FS') {
        addressContent.append("<label class='fieldAlignment'>Sectional scheme:</label><input type='text' id='ssNameTextBox' name='ssNameTextBox' size='40' value='" + property.SSName + "' readonly />");
        addressContent.append("<br />");
        addressContent.append("<label class='fieldAlignment'>Lightstone unit no.:</label><input type='text' id='unitNrTextBox' name='unitNrTextBox' size='5' value='" + property.Unit + "' readonly />");
        addressContent.append("<br />");
        addressContent.append("<label class='fieldAlignment'>Door no.:</label><input type='text' id='doorNoTextBox' name='doorNoTextBox' size='5' value='" + (property.SSDoorNo ? property.SSDoorNo : '') + "' />");
        addressContent.append("<p />");
        addressContent.append("<label class='fieldAlignment'>ERF no.:</label><input type='text' id='erfTextBox' name='erfTextBox' size='10' value='" + property.ErfNo + "' readonly/>");
        addressContent.append("<p />");
        addressContent.append(updateAddressBtn);
    }
    else {
        // FRM
        addressContent.append("Name of farm: " + property.FarmName + "<br />" + "Erf no.: " + property.ErfNo + "<br />" + "Portion: " + property.Portion);
    }

    updateAddressBtn.unbind('click').on('click', function () {
        if (property.SS_FH == "FH") {
            var streetOrUnitNo = $('#streetNoTextBox').val();
            var streetName = $('#streetNameTextBox').val();
            var suburb = $('#suburbTextBox').val();
            var city = $('#cityTownTextBox').val();
            var propAddress = streetName + ", " + suburb + ", " + city;
            updateProspectingRecord({ StreetOrUnitNo: streetOrUnitNo, PropertyAddress: propAddress, SS_FH: "FH", ProspectingPropertyId: property.ProspectingPropertyId });
        }
        else {
            var unitDoor = $('#doorNoTextBox').val();
            updateProspectingRecord({ SSDoorNo: unitDoor, SS_FH: "SS", ProspectingPropertyId: property.ProspectingPropertyId });
        }        
    });

    var headerIcon = '';
    switch (property.SS_FH) {
        case "FH": headerIcon = "fh_edit.png"; break;
        case "SS":
        case "FS":
            headerIcon = "ss_edit.png"; break;
        case "FRM": headerIcon = "frm_edit.png"; break;
    }
    var headerHtml = '';
    var editBtnMock = '';
    if (property.SS_FH != 'FRM' && !property.FarmName) {
        headerHtml = "<label style='cursor:pointer;'>" + property.StreetOrUnitNo + " " + property.PropertyAddress + "</label>";
        editBtnMock = "<input style='cursor:pointer;' type='button' value='Edit..' style='display: inline-block;' />";
    }
    else {
        headerHtml = "<label style='cursor:pointer;'>Farm: " + property.FarmName + " (" + property.LightstoneSuburb + ")" + "</label>";
    }
    headerHtml = headerHtml + "&nbsp;&nbsp;" + editBtnMock;
    var addressEditorSection = buildContentExpanderItem('propertyDetailsEditor', 'Assets/' + headerIcon, headerHtml, addressContent);
    return new ContentExpanderWidget('#contentarea', [addressEditorSection], "propertyEditorExpander");
}

// Represents the view that enables a user view/edit all owners (and/or other contacts) associated with the property
function buildContactDashboard(contacts, context) {

    var container = $("<div id='contactsDashboard' style='padding: 10px;' class='personGeneralInfoContainer' />");
    var propertyDetailsEditor = buildPropertyAddressEditor(currentProperty);
    var propertyDetailsEditorContent = propertyDetailsEditor.construct();
    container.append(propertyDetailsEditorContent);
    container.append("<hr /><p />");

    if (context && context.FromFollowup && !context.FromFollowup.RelatedToContactPersonId) {
        var feedbackBtn = $("<button type='text' id='feedbackFromPropertyBtn' style='cursor:pointer;vertical-align:middle;margin-left:5px;'><img src='Assets/add_activity.png' style='vertical-align:middle;margin-right:5px' /><label style='vertical-align:middle'>Feedback</label></button>");
        container.append(feedbackBtn);
        feedbackBtn.click(function (e) {
            e.preventDefault();
            handleAddFollowupActivity(context.FromFollowup, function () {
                var index = globalFollowUps.indexOf(context.FromFollowup);
                globalFollowUps.splice(index, 1);

                performFollowupFiltering(globalFollowUps, $('#followupContent'));

                feedbackBtn.remove();
                //if (sourceFollowups.length == 0) {
                //    loadMoreFollowups();
                //}
            });
        });
        container.append("<hr /><p />");
    }

    var availableCreditLabel = $("<label id='availableCreditLabel' style='color: red;' />");
    container.append("Available Prospecting amount: R ");
    container.append(availableCreditLabel);
    availableCreditLabel.text(availableCredit.toFixed(2));

    container.append("<br />");
    //container.append("ID number of contact person: <input type='text' id='knownIdTextbox' style='padding:2px;' />&nbsp;");
    //var searchKnownIdBtn = $("<input type='button' id='knownIdSearch' value='Contact Lookup' style='display:inline-block;vertical-align:bottom;cursor:pointer;' />");
    //container.append(searchKnownIdBtn);
    //var tpsInfo = $("<img src='Assets/tps_info.png' style='cursor:pointer;vertical-align:bottom;padding-left:10px;' />");
    //container.append(tpsInfo);

    //tpsInfo.on('mouseover', function () {
    //    tooltip.pop(this, "Searches for contact details for the specified ID number. The cost of such an enquiry, if successful, is R 1.00 (excl. VAT).", { sticky: true, maxWidth: 500 });
    //});
    //tpsInfo.on('mouseout', function () {
    //    tooltip.hide();
    //});

    //searchKnownIdBtn.click(function () {
    //    var idNumber = $('#knownIdTextbox').val().trim();
    //    if (idNumber.length == 13) {
    //        performPersonLookup(idNumber, true);
    //    } else {
    //        alert('The ID number you entered is not valid.');
    //    }
    //});
    //container.append("<br />");
    container.append("<hr /><br />");

    // Companies
    if (currentProperty.ContactCompanies.length) {
        container.append("<b>Company&#47;Organisation details</b>");
        container.append("<p />");
        var counter = 1;
        $.each(currentProperty.ContactCompanies, function (idx, cc) {
            if (counter > 1 && counter <= currentProperty.ContactCompanies.length) {
                container.append("<br />");
            }

            var companyEnquiryBtn = $("<input type='button' id='companyEnquiryBtn' value='Lookup Company Directors' style='display:inline-block;float:right' />");
            var regNo = cc.CKNumber && cc.CKNumber.indexOf("UNKNOWN_CK_") == -1 ? "(" + cc.CKNumber + ")" : '';
            container.append(cc.CompanyName + regNo);
            if (cc.CompanyType != 'TR' && regNo) {
                container.append(companyEnquiryBtn);
                companyEnquiryBtn.click(function () {
                    performCompanyEnquiry(cc.ContactCompanyId);
                });
            }

            counter++;
        });        

        container.append("<br />");
        container.append("<hr /><br />");
    }

    var table = $("<table id='contactsTbl' style='width: 100%;' />");
    table.append($("<tr> <th>Contact Name</th> <th>ID No.</th> <th>Has Details</th> </tr>"));

    $.each(contacts, function (idx, c) {
        var contactRow = buildContactRow(c);
        table.append(contactRow);
    });

    function buildContactRow(contact) {
        var rowId = 'contact_person_' + contact.IdNumber;
        var tableRow = $("<tr id='" + rowId + "' style='cursor:pointer;' />");

        tableRow.click(function () {
            // Remove the widget 
            $('#contactsExpander').remove();
            var row = $(this);
            $('#contactsTbl tr').removeClass('highlight');
            row.addClass('highlight');

            var rowId = row.attr('id');
            var idNumber = rowId.replace('contact_person_', '');

            var contactPerson = $.grep(contacts, function (c) {
                return idNumber == c.IdNumber; // person or company
            })[0];

            currentPersonContact = contactPerson;
            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Contact Details...</p>' });
            window.setTimeout(function () { openExpanderWidget(currentPersonContact); $.unblockUI(); }, 150);
        });

        var nameData = $("<td />");
        nameData.append(contact.Firstname + " " + contact.Surname);
        tableRow.append(nameData);

        var idNumberData = $("<td />");
        idNumberData.append(contact.IdNumber);
        tableRow.append(idNumberData);

        var phoneEmailIdicatorData = $("<td />");
        var hasPhoneNumber = contact.PhoneNumbers != null && contact.PhoneNumbers.length > 0;
        var hasEmailAddress = contact.EmailAddresses != null && contact.EmailAddresses.length > 0;
        if (hasPhoneNumber) {
            phoneEmailIdicatorData.append($("<img src='Assets/phone_icon.png' style='display:inline-block;padding:2px;float:left;' title='Has contact phone number(s)' />"));
        }
        if (hasEmailAddress) {
            phoneEmailIdicatorData.append($("<img src='Assets/email_icon.png' style='display:inline-block;padding:2px;float:left;' title='Has contact email address(es)' />"));
        }
        phoneEmailIdicatorData.append($("<img src='Assets/view_btn.png' style='display:inline-block;padding:2px;float:right;' />"));
        tableRow.append(phoneEmailIdicatorData);

        //var personLookupBtnId = 'lookup_btn_' + contact.IdNumber;
        //var lookupTd = $('<td />');
        //var disabled = contact.IsPOPIrestricted ? 'disabled' : '';
        //var performLookupBtn = $("<input type='button' id='" + personLookupBtnId + "' value='Contact Lookup' title='Searches for contact details for the selected owner. The cost of such an enquiry, if successful, is R 1.00 (excl. VAT).' style='cursor:pointer;' " + disabled + "/>");
        //performLookupBtn.click(function (e) {
        //    e.stopPropagation();
        //    performPersonLookup(contact.IdNumber, false);
        //});
        //lookupTd.append(performLookupBtn);
        //tableRow.append(lookupTd);

        return tableRow;
    }

    container.append(table);
    //container.append("<br />");
    var addNewContactBtn = $("<input type='button' id='createNewContactBtn' value='Manually Capture Contact' style='display:inline-block;float: right;cursor: pointer;' />");
    container.append(addNewContactBtn);
    addNewContactBtn.click(function () {
        $('#contactsExpander').remove();
        currentPersonContact = null;
        openExpanderWidget();
    });

    return container;
}

function performCompanyEnquiry(contactCompanyId) {
    var companyEnquiryInfoDiv = $("<div title='Company Enquiry Service Request' style='font-family:Verdana;font-size:12px;' />").empty()
        .append('You are about to perform an enquiry to retrieve the directors of this company. Please note that you will need to perform a separate enquiry to retrieve their contact details.')
        .append("<p />")
        .append('If successful, this enquiry will cost R14.40 (excl. VAT) and will be deducted from your Prospecting balance.');

    companyEnquiryInfoDiv.dialog(
                {
                    modal: true,
                    closeOnEscape: false,
                    width: '550',
                    buttons: {
                        "Proceed": function () {
                            $(this).dialog("close");

                            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Performing Lookup...</p>' });
                            $.ajax({
                                type: "POST",
                                url: "RequestHandler.ashx",
                                data: JSON.stringify({ Instruction: 'perform_company_enquiry', ContactCompanyId: contactCompanyId, ProspectingPropertyId: currentProperty.ProspectingPropertyId }),
                                dataType: "json"
                            }).done(function (data) {
                                $.unblockUI();
                                if (!handleResponseIfServerError(data)) {
                                    return;
                                }

                                if (data.WalletBalance != null) {
                                    // If the AvailableTracePsCredits is not null, then update the availableCredit variable
                                    availableCredit = data.WalletBalance;
                                    $('#availableCreditLabel').text(availableCredit.toFixed(2));
                                }
                                if (data.EnquirySuccessful) {
                                    currentPersonContact = null;
                                    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Refreshing Contacts...</p>' });
                                    setCurrentMarker(currentSuburb, currentProperty, function (data) {
                                        updateExistingPropertyFromProperty(currentProperty, data);
                                        buildPersonContactMenu(currentProperty.Contacts, false);
                                        updateProspectedStatus();
                                        $.unblockUI();
                                    });
                                } else {
                                    // feedback to user
                                    alert(data.ErrorMsg);
                                }
                            });
                        },
                        "Cancel": function () { $(this).dialog("close"); }
                    },
                    position: ['center', 'center']
                });
}

function handleAddActivityFromContact() {
    loadExistingProspectAddActivity(currentProperty, { RelatedTo: currentPersonContact.ContactPersonId }, null);
}

function populateContactLookupInfo(infoPacket) {

    var idNumber = infoPacket.IdNumber;
    var contactRows = infoPacket.ContactRows;

    var contactPerson = $.grep(currentProperty.Contacts, function (c) {
        return idNumber == c.IdNumber; // person or company
    })[0];

    // If the contact does not exist on the property
    if (!contactPerson) {
        currentPersonContact = openAndPopulateNewContactPerson(infoPacket);
    } else {
        currentPersonContact = contactPerson;
    }

    // We cannot determine person's title from Lightstone, but we can from Dracore - try set title here
    if (!currentPersonContact.Title) {
        currentPersonContact.Title = infoPacket.Title;
    }
    // Populate the object with the rest of the dracore fields
    currentPersonContact.DeceasedStatus = infoPacket.DeceasedStatus;
    currentPersonContact.AgeGroup = infoPacket.AgeGroup;
    currentPersonContact.Location = infoPacket.Location;
    currentPersonContact.MaritalStatus = infoPacket.MaritalStatus;
    currentPersonContact.HomeOwnership = infoPacket.HomeOwnership;
    currentPersonContact.Directorship = infoPacket.Directorship;
    currentPersonContact.PhysicalAddress = infoPacket.PhysicalAddress;
    currentPersonContact.Employer = infoPacket.Employer;
    currentPersonContact.Occupation = infoPacket.Occupation;
    currentPersonContact.BureauAdverseIndicator = infoPacket.BureauAdverseIndicator;
    currentPersonContact.Citizenship = infoPacket.Citizenship;

    var contactNos = [], contactEmails = [];
    $.each(contactRows, function (idx, item) {
        switch (item.Type) {
            case "cell": case "home": case "work":
                var phoneNumberTypeId = getProspectingPhoneTypeId(item.Type);
                var contactRow = newContactItem(generateUniqueID(), phoneNumberTypeId, item.Phone, false, true, 'PHONE', getSADialingCodeId(), null);
                contactNos.push(contactRow);
                break;
            case "email":
                var emailType = $.grep(prospectingContext.ContactEmailTypes, function (i) { return i.Value == 'private email'; })[0].Key;
                var emailRow = newContactItem(generateUniqueID(), emailType, item.EmailAddress, false, true, 'EMAIL', null, null);
                contactEmails.push(emailRow);
                break;
        }       
    });

    if (!currentPersonContact.PhoneNumbers) {
        currentPersonContact.PhoneNumbers = [];
    }
    if (!currentPersonContact.EmailAddresses) {
        currentPersonContact.EmailAddresses = [];
    }
    // Check the phone no's on the current contact person against the incoming data, exclude new data that matches existing data
    $.each(contactNos, function (idx, ph) {
        var itemPresent = false;        
        $.each(currentPersonContact.PhoneNumbers, function (idx2, ph2) {
            if (ph.ItemContent == ph2.ItemContent) {
                itemPresent = true;
            }
        });

        if (!itemPresent) {
            currentPersonContact.PhoneNumbers.push(ph);
            // TODO: also validate here whether this number doesn't belong to another contact
        }
    });

    // Check emails against incoming data - exclude new data that already exists
    $.each(contactEmails, function (idx, em) {
        var itemPresent = false;        
        $.each(currentPersonContact.EmailAddresses, function (idx2, em2) {
            if (em.ItemContent == em2.ItemContent) {
                itemPresent = true;
            }
        });

        if (!itemPresent) {
            currentPersonContact.EmailAddresses.push(em);
        }
    });

    $('#contactsExpander').remove();
    openExpanderWidget(currentPersonContact);
}