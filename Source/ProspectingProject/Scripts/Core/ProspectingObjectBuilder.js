
/// Javascript mapping to C# domain types

function newContactItem(itemId, itemType, itemContent, isPrimary, isNewRecord, contactItemType, intlDialingCode, eleventhDigit) {
    return { ItemId: itemId, ItemType: itemType, ItemContent: itemContent, IsPrimary: isPrimary, IsNewRecord: isNewRecord, ContactItemType: contactItemType, IntDialingCode: intlDialingCode, EleventhDigit: eleventhDigit };
}

// Person data is saved by section, independently. Therefore, when the phone/email arrays are null, ignore it when saving to the database. When the array is empty, update the DB.
function newPersonContact(firstname, surname, title, idNo, propRelationshipType, arrayPhoneNumbers, arrayEmailAddresses, isPOPIrestricted, gender, companyRelationshipType, contactCompanyId) {

    var personPropertyRelationships = [];
    if (propRelationshipType != null) {
        personPropertyRelationships.push({ Key: currentProperty.ProspectingPropertyId, Value: propRelationshipType });
    }
    return { ContactPersonId: null, Firstname: firstname, Surname: surname, Title: title, IdNumber: idNo, PersonPropertyRelationships: personPropertyRelationships, PhoneNumbers: arrayPhoneNumbers, EmailAddresses: arrayEmailAddresses, IsPOPIrestricted: isPOPIrestricted, Gender: gender, PersonCompanyRelationshipType: companyRelationshipType, ContactCompanyId: contactCompanyId };
}