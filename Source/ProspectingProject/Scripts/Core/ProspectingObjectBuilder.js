
/// Javascript mapping to C# domain types

function newContactItem(itemId, itemType, itemContent, isPrimary, isNewRecord, contactItemType, intlDialingCode, eleventhDigit) {
    return { ItemId: itemId, ItemType: itemType, ItemContent: itemContent, IsPrimary: isPrimary, IsNewRecord: isNewRecord, ContactItemType: contactItemType, IntDialingCode: intlDialingCode, EleventhDigit: eleventhDigit };
}

// Person data is saved by section, independently. Therefore, when the phone/email arrays are null, ignore it when saving to the database. When the array is empty, update the DB.
function newPersonContact(firstname,
                            surname,
                            title,
                            idNo,
                            propRelationshipType,
                            arrayPhoneNumbers,
                            arrayEmailAddresses,
                            isPOPIrestricted,
                            gender,
                            companyRelationshipType,
                            contactCompanyId,
                            deceasedStatus,
                            ageGroup ,
                            location ,
                            maritalStatus ,
                            homeOwnership ,
                            directorship ,
                            physicalAddress ,
                            employer,
                            occupation ,
                            bureauAdverseIndicator,
                            citizenship,
                            emailOptout,
                            smsOptout,
                            doNotContact) {

    var personPropertyRelationships = [];
    if (propRelationshipType != null) {
        personPropertyRelationships.push({ Key: currentProperty.ProspectingPropertyId, Value: propRelationshipType });
    }
    return {
        ContactPersonId: null,
        Firstname: firstname,
        Surname: surname,
        Title: title,
        IdNumber: idNo,
        PersonPropertyRelationships: personPropertyRelationships,
        PhoneNumbers: arrayPhoneNumbers,
        EmailAddresses: arrayEmailAddresses,
        IsPOPIrestricted: isPOPIrestricted,
        Gender: gender,
        PersonCompanyRelationshipType: companyRelationshipType,
        ContactCompanyId: contactCompanyId,
        
        // Dracore fields
        DeceasedStatus: deceasedStatus,
        AgeGroup: ageGroup ,
        Location: location ,
        MaritalStatus: maritalStatus ,
        HomeOwnership: homeOwnership ,
        Directorship: directorship ,
        PhysicalAddress: physicalAddress ,
        Employer: employer,
        Occupation: occupation,
        BureauAdverseIndicator: bureauAdverseIndicator,
        Citizenship: citizenship,
        EmailOptout: emailOptout,
        SMSOptout: smsOptout,
        DoNotContact: doNotContact
    };
}

function createMenuItem(itemName, itemId, itemContent, onClickFunction, itemCounter) {

    var itemContentDiv = $("<div id='" + itemId + "_content" + "' />");
    itemContentDiv.append(itemContent);
    var item = { MenuItemName: itemName, MenuItemId: itemId, MenuItemContent: itemContentDiv, MenuItemDiv: null };

    var menuItemPanel = $('#menuitempanel');

    var div = $("<div id='itemdiv' style='padding:5px 5px 5px 5px;' />");

    var button = null;
    if (itemCounter != null) {
        if (itemCounter > 10) {
            itemCounter = '10+';
        }
        button = $("<a href='' id='" + itemId + "' style='vertical-align:middle;padding-right:20px'>" + itemName + "</a><span class='menu-item-counter' >" + itemCounter + "</span>");
    } else {
        button = $("<a href='' id='" + itemId + "'>" + itemName + "</a>");
    }

    // attach click handler to button
    button.unbind('click').bind('click', function (event) {

        event.preventDefault();
        var btn = $(this);
        clearMenuSelection();

        //btn.append("<img src='Assets/tick.png' />");
        btn.parent().css('background-color', '#E0E0E0');

        showContentForItem($(this).attr("id"));

        if (onClickFunction) {
            onClickFunction();
        }
    });

    div.append(button);
    menuItemPanel.append(div);

    item.MenuItemDiv = div;

    return item;
}

function createCommunicationLogRecord(commContext, commType, targetContactPersonId, targetContactDetail, targetLightstonePropId, sentStatus, sendingError, msgContent, subjectText) {
    return { CommContext: commContext, CommType: commType, TargetContactPersonId: targetContactPersonId, TargetContactDetail: targetContactDetail, TargetLightstonePropId: targetLightstonePropId, SentStatus: sentStatus, SendingError: sendingError, MessageBase64: msgContent, SubjectText: subjectText };
}