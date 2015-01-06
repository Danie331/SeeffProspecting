var currentPersonContact; // Stores a reference to the currently active (selected) contact
var currentCompanyContact;
var contactDetailsWidget = null;
var expanderWidget = null;

function buildPersonContactMenu(propertyContacts, update) {
    if (!propertyContacts) propertyContacts = [];
    var contactDetailsDiv = $('#contactDetailsDiv');

    contactDetailsDiv.empty();
    var contactDashboard = buildContactDashboard(propertyContacts);
    contactDetailsDiv.append(contactDashboard);
    contactDetailsDiv.append("<p />");

    if (update) {
        // If updating the dashboard rebuild as above and open current contact person (which should already be open)
        openExpanderWidget(currentPersonContact);
    }
}

function openExpanderWidget(contact) {
    // build the expander widget
    var contactDetailsDiv = $('#contactDetailsDiv');
    expanderWidget = buildExpanderWidget(contact);
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
    var contactNos = [];
    $.each(infoPacket.ContactRows, function (idx, item) {
        var phoneNumberTypeId = getProspectingPhoneTypeId(item.Type);
        var contactRow = newContactItem(generateUniqueID(), phoneNumberTypeId, item.Phone, false, true, 'PHONE', getSADialingCodeId(), null);
        contactNos.push(contactRow);
    });

    var ownerRelationshipType = $.grep(prospectingContext.PersonPropertyRelationshipTypes, function(val) {
        return val.Value == 'Owner';
    })[0];

    infoPacket.OwnerName = toTitleCase(infoPacket.OwnerName);
    infoPacket.OwnerSurname = toTitleCase(infoPacket.OwnerSurname);
    var newContact = newPersonContact(infoPacket.OwnerName, infoPacket.OwnerSurname, null, infoPacket.IdCkNo, ownerRelationshipType.Key, contactNos, null, false, infoPacket.OwnerGender);
    return newContact;
}

function buildExpanderWidget(contact) {
    $('#personGeneral').empty();
    var personGeneralInfo = buildContentExpanderItem('personGeneral', 'Assets/person.png', "Contact Person", buildGeneralInfoHtml(contact));
    $('#personContact').empty();
    var personContactInfo = buildContentExpanderItem('personContact', 'Assets/contact.png', "Contact Details", buildContactDetailsHtml(contact));

    return new ContentExpanderWidget('#contentarea', [personGeneralInfo, personContactInfo], "contactsExpander");
}

function buildGeneralInfoHtml(contact) {
    $('#contactPersonHeaderDiv').remove();
    var html = $("<div id='contactPersonHeaderDiv' />");
    html.append("<label class='fieldAlignment'>Gender: </label>");
    html.append(buildPersonGenderCombo());
    html.append("<br />");
    html.append("<label class='fieldAlignment'>Title: </label>");
    html.append(buildPersonTitleCombo());
    html.append("<br />");
    html.append("<label class='fieldAlignment'>First name:</label><input type='text' id='firstNameTextBox' name='firstNameTextBox' size='40'/>");
    html.append("<br />");
    html.append("<label class='fieldAlignment'>Surname:</label><input type='text' id='surnameTextBox' name='surnameTextBox' size='40'/>");
    html.append("<br />");
    html.append("<label class='fieldAlignment'>ID no.:</label><input type='text' id='idOrCkTextBox' name='idOrCkTextBox' size='40'/>");
    html.append("<br />");

    var relationshipText = currentProperty.ContactCompanies.length > 0 ? "Relationship to property/company: " : "Relationship to property: ";
    html.append("<label class='fieldAlignment'>" + relationshipText + "</label>");
    var relationshipToPropertySelect = buildRelationshipToPropertyTypesCombo();
    html.append(relationshipToPropertySelect);

    html.append("<br />");
    var companySelect = buildCompanySelectorCombo();
    html.append(companySelect);

    // TODO: POPI
    //var popiBox = $("<label><input type='checkbox' id='popiCheckbox' />POPI restricted</label>");   
    //popiBox.find('#popiCheckbox').click(handlePOPIclick);
    //html.append("<br />");
    //html.append(popiBox);

    // TODO:
    //html.append("<br />");
    //html.append("<label class='fieldAlignment'>This contact is related to: </label>");
    //var relationshipsToContact = buildRelationshipsToContact(contact.LinkedContacts);
    //html.append(relationshipsToContact);

    html.append("<p />");
    var saveBtn = $("<input type='button' id='personGeneralSaveBtn' value='Save..' />");
    html.append(saveBtn);
    saveBtn.click(function () { handleSaveContactPerson(); });

    //if (!contact) {
    //    saveBtn.css('display', 'none');
    //}
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

function buildContactDetailsHtml(contact) {

    var phoneNumbers = contact ? contact.PhoneNumbers : [];
    var emailAddresses = contact ? contact.EmailAddresses : [];
    var canEdit = contact ? !contact.IsPOPIrestricted : true;
    contactDetailsWidget = new ContactDetailsEditorWidget('#contentarea', phoneNumbers, emailAddresses, handleSaveContactDetails, canEdit);
    return contactDetailsWidget.construct();
}

function handleSaveContactDetails(phoneNumbers, emailAddresses) {

    if (currentPersonContact) {
        currentPersonContact.PhoneNumbers = phoneNumbers;
        currentPersonContact.EmailAddresses = emailAddresses;
        if (!validateCorrectnessOfInputs(phoneNumbers, emailAddresses)) {
            alert('Some of the contact details you have specified are not valid.');
            return;
        }
    }

    handleSaveContactPerson(function () {

        if (currentPersonContact) {
            currentPersonContact.PhoneNumbers = phoneNumbers;
            currentPersonContact.EmailAddresses = emailAddresses;
            if (!validateCorrectnessOfInputs(phoneNumbers, emailAddresses)) {
                alert('Some of the contact details you have specified are not valid.');
                return;
            }

            determineIfDetailsAreAvailableForUse(phoneNumbers, emailAddresses, function (existingContact) {
                if (existingContact == null) {
                    saveContact(currentPersonContact, currentProperty, function (data) {
                        // currentPersonContact.ContactPersonId = data.ContactPersonId;                
                        currentPersonContact.PhoneNumbers = data.PhoneNumbers;
                        currentPersonContact.EmailAddresses = data.EmailAddresses;

                        contactDetailsWidget.resetItemArrays(currentPersonContact.PhoneNumbers, currentPersonContact.EmailAddresses);
                        contactDetailsWidget.rebuildDivs(!currentPersonContact.IsPOPIrestricted);
                        addOrUpdateContactToCurrentProperty(data);
                        //currentProperty.HasContactsWithDetails = hasAnyContactDetails(currentProperty);

                        // Icon must change as soon as we add contact details for the property:

                        //TODO -- Not critical 
                        //currentProperty.Marker.setIcon(getIconForMarker(currentProperty.Marker)); 
                        //updateSuburbSelectionStats(currentProperty.Marker.Suburb);

                        //alert('Contact details saved successfully!');
                        //This function is no longer needed as the details are fetched from the server 
                        //when the user clicks on the property in a suburb. 2014-10-08
                        //updateSuburbContactsList();
                        //showSavedSplashDialog('Details Saved!');
                    });
                } else {
                    alert('A contact with these details already exists. ');
                }
            });
        }
    });
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
            actionToRun(data);
        });
    } else {
        actionToRun(null);
    }
}

function handleSaveContactPerson(saveDetailsFunction) {

    if (!validateCorrectnessOfPersonInfo()) {
        alert('Cannot save: One or more fields are missing or the data has been entered incorrectly');
        return;
    }

    if (currentPersonContact) {
        currentPersonContact.PhoneNumbers = contactDetailsWidget.getPhoneNumbers();
        currentPersonContact.EmailAddresses = contactDetailsWidget.getEmailAddresses();
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
            // Test for existing or new contact
            if (currentPersonContact) {   // existing..
                currentPersonContact.Title = title;
                currentPersonContact.Firstname = firstname;
                currentPersonContact.Surname = surname;
                currentPersonContact.Fullname = firstname + ' ' + surname;
                currentPersonContact.IdNumber = idNo;

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
                currentPersonContact.IsPOPIrestricted = false;//$('#popiCheckbox').is(":checked");
                currentPersonContact.Gender = gender;                
            }
            else {
                // Must be a new contact
                if (propertyRelationship) {
                    currentPersonContact = newPersonContact(firstname, surname, title, idNo, propertyRelationship, null, null, /*$('#popiCheckbox').is(":checked")*/false, gender, null, null);
                }
                else if (companyRelationship) {
                    currentPersonContact = newPersonContact(firstname, surname, title, idNo, null, null, null, /*$('#popiCheckbox').is(":checked")*/false, gender, companyRelationship, contactCompanyId);
                }
            }
            saveContact(currentPersonContact, currentProperty, function (data) {
                //alert('Contact saved successfully!');
                showSavedSplashDialog('Contact Saved!');
                currentPersonContact.ContactPersonId = data.ContactPersonId;
                addOrUpdateContactToCurrentProperty(data);
                buildPersonContactMenu(currentProperty.Contacts, true);

                currentPersonContact.PhoneNumbers = data.PhoneNumbers;
                currentPersonContact.EmailAddresses = data.EmailAddresses;

                if (currentPersonContact.IsPOPIrestricted) {
                    // If the popi option was selected, then delete all their contact info
                    handleSaveContactDetails([], []);
                }

                //This function is no longer needed as the details are fetched from the server 
                //when the user clicks on the property in a suburb. 2014-10-08
                //updateSuburbContactsList();

                if (saveDetailsFunction) {
                    saveDetailsFunction();
                }
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
                    buildPersonContactMenu(currentProperty.Contacts, false);

                    if (currentPersonContact.IsPOPIrestricted) {
                        // If the popi option was selected, then delete all their contact info
                        handleSaveContactDetails([], []);
                    }

                    //This function is no longer needed as the details are fetched from the server 
                    //when the user clicks on the property in a suburb. 2014-10-08
                    //updateSuburbContactsList();

                    //if (saveDetailsFunction) {
                    //    saveDetailsFunction();
                    //}
                    setCurrentMarker(currentSuburb, currentProperty);
                });
            });
        }
    });
}

//This function is no longer needed as the details are fetched from the server 
//when the user clicks on the property in a suburb. 2014-10-08
//function updateSuburbContactsList() {
//    currentSuburb.AllContacts = [];
//    var allPropsInSuburb = currentSuburb.ProspectingProperties;
//    if (allPropsInSuburb && allPropsInSuburb.length > 0) {
//        for (var s = 0; s < allPropsInSuburb.length; s++) {
//            var property = allPropsInSuburb[s];
//            for (var c = 0; c < property.Contacts.length; c++) {
//                var contact = property.Contacts[c];
//                currentSuburb.AllContacts.push(contact);
//            }
//        }
//    }
//}

function validateCorrectnessOfPersonInfo() {
    var title = $('#personTitle').children(":selected").attr('value');
    var firstname = $('#firstNameTextBox').val();
    var surname = $('#surnameTextBox').val();
    var relType = $('#relTypesSelect').children(":selected").attr('value');
    var gender = $('#genderCheckbox').children(":selected").attr('value');
    var idNo = $('#idOrCkTextBox').val();

    return firstname.length > 0 && surname.length > 0 && relType.length > 0 && gender.length > 0 && idNo.length > 0;
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
        continueWithAction(data);
    });
}

function validateCorrectnessOfInputs(phoneNumbers, emailAddresses) {

    if (!currentPersonContact) return false;

    var isValid = true;
    if (phoneNumbers) {
        $.each(phoneNumbers, function (idx, item) {
            if (!item.ItemType || item.ItemType == 'nothing') isValid = false;
            if (!item.ItemContent || item.ItemContent.length == 0) isValid = false;
        });
    }

    if (emailAddresses) {
        $.each(emailAddresses, function (idx, item) {
            if (!item.ItemType || item.ItemType == 'nothing') isValid = false;
            if (!item.ItemContent || item.ItemContent.length == 0) isValid = false;
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
    else {
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

    var headerIcon = property.SS_FH == "FH" ? "fh_edit.png" : "ss_edit.png";
    var headerHtml = "<label style='cursor:pointer;'>" + property.StreetOrUnitNo + " " + property.PropertyAddress + "</label>";
    var editBtnMock = "<input style='cursor:pointer;' type='button' value='Edit..' style='display: inline-block;' />";
    headerHtml = headerHtml + "&nbsp;&nbsp;" + editBtnMock;
    var addressEditorSection = buildContentExpanderItem('propertyDetailsEditor', 'Assets/' + headerIcon, headerHtml, addressContent);
    return new ContentExpanderWidget('#contentarea', [addressEditorSection], "propertyEditorExpander");
}

// Represents the view that enables a user view/edit all owners (and/or other contacts) associated with the property
function buildContactDashboard(contacts) {

    var container = $("<div id='contactsDashboard' style='padding: 10px;' class='personGeneralInfoContainer' />");
    var propertyDetailsEditor = buildPropertyAddressEditor(currentProperty);
    var propertyDetailsEditorContent = propertyDetailsEditor.construct();
    container.append(propertyDetailsEditorContent);
    container.append("<hr /><p />");

    //container.append("Use the 'TPS' button to perform a search for contact details of the selected owner. ");
    //container.append("The cost of such an enquiry, if successful, is 60c (excl. VAT).");
    //container.append("<p />");

    var availableCreditLabel = $("<label id='availableCreditLabel' style='color: red;' />");
    container.append("Available Prospecting credit: ");
    container.append(availableCreditLabel);
    //container.append(" Prospecting credits available.");
    availableCreditLabel.text(availableCredit);

    container.append("<br />");
    container.append("ID number of contact person: <input type='text' id='knownIdTextbox' style='padding:2px;' />&nbsp;");
    var searchKnownIdBtn = $("<input type='button' id='knownIdSearch' value='Lookup' style='display:inline-block;vertical-align:bottom;' />");
    container.append(searchKnownIdBtn);
    var tpsInfo = $("<img src='Assets/tps_info.png' style='cursor:pointer;vertical-align:bottom;padding-left:10px;' />");
    container.append(tpsInfo);

    tpsInfo.on('mouseover', function () {
        tooltip.pop(this, "Performs a search for contact details of the selected owner. The cost of such an enquiry, if successful, is 60c (excl. VAT).", { sticky: true, maxWidth: 500 });
    });
    tpsInfo.on('mouseout', function () {
        tooltip.hide();
    });

    searchKnownIdBtn.click(function () {
        var idNumber = $('#knownIdTextbox').val();
        if (idNumber.length > 0) {
            performPersonLookup(idNumber, true);
        }
    });
    container.append("<br />");
    container.append("<hr /><br />");

    // Companies
    if (currentProperty.ContactCompanies.length) {
        container.append("<b>Company details</b>");
        container.append("<p />");
        var counter = 1;
        $.each(currentProperty.ContactCompanies, function (idx, cc) {
            if (counter > 1 && counter <= currentProperty.ContactCompanies.length) {
                container.append(" / ");
            }
            container.append(cc.CompanyName + "(" + cc.CKNumber + ")");
            counter++;
        });
        container.append("<br />");
        container.append("<hr /><br />");
    }

    var table = $("<table id='contactsTbl' style='width: 100%;' />");
    table.append($("<tr> <th>Contact Name</th> <th>ID No.</th> <th>Has Details</th> <th>Person Lookup</th> </tr>"));

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

            // Find the contact to display
            //var selectedValue = $(this).val();
            //if (selectedValue == '') return;
            //if (selectedValue == 'new_contact_person') {
            //    openExpanderWidget(newContactDetails);
            //    currentPersonContact = null;
            //    return;
            //}
            var contactPerson = $.grep(contacts, function (c) {
                return idNumber == c.IdNumber; // person or company
            })[0];

            currentPersonContact = contactPerson;
            openExpanderWidget(currentPersonContact);
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
            phoneEmailIdicatorData.append($("<img src='Assets/phone_icon.png' style='display:inline-block;padding:2px;float:right;' title='Has contact phone number(s)' />"));
        }
        if (hasEmailAddress) {
            phoneEmailIdicatorData.append($("<img src='Assets/email_icon.png' style='display:inline-block;padding:2px;float:right;' title='Has contact email address(es)' />"));
        }
        tableRow.append(phoneEmailIdicatorData);

        var personLookupBtnId = 'lookup_btn_' + contact.IdNumber;
        var lookupTd = $('<td />');
        var performLookupBtn = $("<input type='button' id='" + personLookupBtnId + "' value='TPS' title='Performs a search for contact details of the selected owner. The cost of such an enquiry, if successful, is 60c (excl. VAT).' style='cursor:pointer;' />");
        performLookupBtn.click(function (e) {
            e.stopPropagation();
            performPersonLookup(contact.IdNumber, false);
        });
        lookupTd.append(performLookupBtn);
        //performLookupBtn.on('mouseover', function () {
        //    tooltip.pop(performLookupBtn, "Performs a search for contact details of the selected owner. The cost of such an enquiry, if successful, is 60c (excl. VAT).", { sticky: true, maxWidth: 500 });
        //});
        //performLookupBtn.on('mouseout', function () {
        //    tooltip.hide();
        //});

        tableRow.append(lookupTd);

        return tableRow;
    }

    container.append(table);
    //container.append("<br />");
    var addNewContactBtn = $("<input type='button' id='createNewContactBtn' value='Manually Capture Contact' style='display:inline-block;float: right;' />");
    container.append(addNewContactBtn);
    addNewContactBtn.click(function () {
        $('#contactsExpander').remove();
        openExpanderWidget();
        currentPersonContact = null;
    });

    return container;
}

function populateContactLookupInfo(infoPacket) {

    var idNumber = infoPacket.IdCkNo;
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

    var contactNos = [];
    $.each(contactRows, function (idx, item) {
        var phoneNumberTypeId = getProspectingPhoneTypeId(item.Type);
        var contactRow = newContactItem(generateUniqueID(), phoneNumberTypeId, item.Phone, false, true, 'PHONE', getSADialingCodeId(), null);
        contactNos.push(contactRow);
    });

    $.each(contactNos, function (idx, ph) {
        var itemPresent = false;
        if (!currentPersonContact.PhoneNumbers) {
            currentPersonContact.PhoneNumbers = [];
        }
        $.each(currentPersonContact.PhoneNumbers, function (idx2, ph2) {
            if (ph.ItemContent == ph2.ItemContent)            {
                itemPresent = true;
            }
        });

        if (!itemPresent) {
            currentPersonContact.PhoneNumbers.push(ph);
            // TODO: also validate here whether this number doesn't belong to another contact
        }
    });

    $('#contactsExpander').remove();
    openExpanderWidget(currentPersonContact);
}