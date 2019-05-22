
app.buildHolidayListing = function () {
    app.clearListingSelection();
    $("#listingFieldsContainer").show();
    var content = $("#listingFieldsContent");
    content.append(app.buildLocationSelect());
    content.append(app.buildAddressInformation());
    content.append(app.buildDescription());
    content.append(app.buildAgentBranchSelector());
    content.append(app.buildHolidayStatusSelect());
    content.append(app.buildHolidayListingType());
    content.append(app.buildHolidayPropertyTypes());
    content.append(app.buildEstablishmentNameInput());
    content.append(app.buildEstablishmentNotesInput());
    content.append(app.buildBedroomsSelection());
    content.append(app.buildSleepsSelection());
    content.append(app.buildKingBedsSelection());
    content.append(app.buildQueenBedsSelection());
    content.append(app.buildDoubleBedsSelection());
    content.append(app.buildBunkBedsSelection());
    content.append(app.buildSingleBedsSelection());
    content.append(app.buildBathroomsSelection());
    content.append(app.buildBathsSelection());
    content.append(app.buildShowersSelection());
    content.append(app.buildHandShowersSelection());
    content.append(app.buildLoungesSelection());
    content.append(app.buildDiningRoomsSelection());
    content.append(app.buildGardensSelection());
    content.append(app.buildBraaisSelection());
    content.append(app.buildPoolsSelection());
    content.append(app.buildJacuzziSelection());
    content.append(app.buildUndercoverParkingBaysSelection());
    content.append(app.buildOpenParkingBaysSelection());
    content.append(app.buildPeakSeasonOption());
    content.append(app.buildSemiSeasonOption());
    content.append(app.buildLowSeasonOption());
    content.append(app.buildOutOfSeasonSeasonOption());
    content.append(app.buildFloorSelection());
    content.append(app.buildSeaViewsSelection());
    content.append(app.buildCleaningServiceSelection());
}

app.buildHolidayStatusSelect = function () {
    return "<label for='statusInput' class='fieldAlignmentShortWidth'>Status:</label>\
                    <select id='statusInput' class='centered-aligned'>\
                            <option value='Active'>Active</option>\
                            <option value='Archived'>Archived</option>\
                    </select>\
                    <p class='vertical-spacer' />";
}

app.buildHolidayListingType = function () {
    return "<label for='listingTypeInput' class='fieldAlignmentShortWidth'>Listing Type:</label>\
        <select id='listingTypeInput' class='centered-aligned'>\
            <option value='HolidayLetting'>Holiday Letting</option>\
        </select>\
        <p class='vertical-spacer' />";
}

app.buildHolidayPropertyTypes = function () {
    return "<label for='propertyTypeInput' class='fieldAlignmentShortWidth'>Property Type:</label>\
        <select id='propertyTypeInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>\
            <option value='1'>Farm</option>\
            <option value='2'>Apartment</option>\
            <option value='3'>Duplex</option>\
            <option value='4'>Chalet</option>\
            <option value='5'>Flat</option>\
            <option value='6'>Garden Cottage</option>\
            <option value='7'>House</option>\
            <option value='8'>Simplex</option>\
            <option value='9'>Townhouse</option>\
            <option value='10'>Villa</option>\
        </select>\
        <p class='vertical-spacer' />";
}

app.buildEstablishmentNameInput = function () {
    return "<label for='establishmentNameInput' class='fieldAlignmentShortWidth'>Establishment Name:</label>\
                    <input id='establishmentNameInput' class='fieldAlignmentLongWidth' data-parsley-required data-parsley-length='[1, 100]' />\
                    <p class='vertical-spacer' />";
}

app.buildEstablishmentNotesInput = function () {
    return "<label for='establishmentNotesInput' class='fieldAlignmentShortWidth'>Establishment Notes:</label>\
                    <textarea id='establishmentNotesInput' rows='8' class='fieldAlignmentLongWidth' style='vertical-align:middle' data-parsley-required></textarea>\
                    <p class='vertical-spacer' />";
}

app.buildPartialUnitOptions = function () {
    return "<option value='0'>0</option>\<option value='0.5'>0.5</option>\<option value='1'>1</option>\<option value='1.5'>1.5</option>\<option value='2'>2</option>\<option value='2.5'>2.5</option>\<option value='3'>3</option>\<option value='3.5'>3.5</option>\<option value='4'>4</option>\<option value='4.5'>4.5</option>\<option value='5'>5</option>\<option value='5.5'>5.5</option>\<option value='6'>6</option>\<option value='6.5'>6.5</option>\<option value='7'>7</option>\<option value='7.5'>7.5</option>\<option value='8'>8</option>\<option value='8.5'>8.5</option>\<option value='9'>9</option>\<option value='9.5'>9.5</option>\<option value='10'>10</option>\<option value='10.5'>10.5</option>\<option value='11'>11</option>\<option value='11.5'>11.5</option>\<option value='12'>12</option>\<option value='12.5'>12.5</option>\<option value='13'>13</option>\<option value='13.5'>13.5</option>\<option value='14'>14</option>\<option value='14.5'>14.5</option>\<option value='15'>15</option>\<option value='15.5'>15.5</option>\<option value='16'>16</option>\<option value='16.5'>16.5</option>\<option value='17'>17</option>\<option value='17.5'>17.5</option>\<option value='18'>18</option>\<option value='18.5'>18.5</option>\<option value='19'>19</option>\<option value='19.5'>19.5</option>\<option value='20'>20</option>\<option value='20.5'>20.5</option>\<option value='21'>21</option>\<option value='21.5'>21.5</option>\<option value='22'>22</option>\<option value='22.5'>22.5</option>\<option value='23'>23</option>\<option value='23.5'>23.5</option>\<option value='24'>24</option>\<option value='24.5'>24.5</option>\<option value='25'>25</option>\<option value='25.5'>25.5</option>\<option value='26'>26</option>\<option value='26.5'>26.5</option>\<option value='27'>27</option>\<option value='27.5'>27.5</option>\<option value='28'>28</option>\<option value='28.5'>28.5</option>\<option value='29'>29</option>\<option value='29.5'>29.5</option>\<option value='30'>30</option>";
}

app.buildFullUnitOptions = function () {
    return "<option value='0'>0</option>\<option value='1'>1</option>\<option value='2'>2</option>\<option value='3'>3</option>\<option value='4'>4</option>\<option value='5'>5</option>\<option value='6'>6</option>\<option value='7'>7</option>\<option value='8'>8</option>\<option value='9'>9</option>\<option value='10'>10</option>\<option value='11'>11</option>\<option value='12'>12</option>\<option value='13'>13</option>\<option value='14'>14</option>\<option value='15'>15</option>\<option value='16'>16</option>\<option value='17'>17</option>\<option value='18'>18</option>\<option value='19'>19</option>\<option value='20'>20</option>";
}

app.buildBedroomsSelection = function () {
    return "<label for='bedroomsInput' class='fieldAlignmentShortWidth'>Bedrooms:</label>\
        <select id='bedroomsInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildPartialUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildSleepsSelection = function () {
    return "<label for='sleepsInput' class='fieldAlignmentShortWidth'>Sleeps:</label>\
        <select id='sleepsInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildPartialUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildKingBedsSelection = function () {
    return "<label for='kingBedsInput' class='fieldAlignmentShortWidth'>King Beds:</label>\
        <select id='kingBedsInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildPartialUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildQueenBedsSelection = function () {
    return "<label for='queenBedsInput' class='fieldAlignmentShortWidth'>Queen Beds:</label>\
        <select id='queenBedsInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildPartialUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildDoubleBedsSelection = function () {
    return "<label for='doubleBedsInput' class='fieldAlignmentShortWidth'>Double Beds:</label>\
        <select id='doubleBedsInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildPartialUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildBunkBedsSelection = function () {
    return "<label for='bunkBedsInput' class='fieldAlignmentShortWidth'>Bunk Beds:</label>\
        <select id='bunkBedsInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildPartialUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildSingleBedsSelection = function () {
    return "<label for='singleBedsInput' class='fieldAlignmentShortWidth'>Single Beds:</label>\
        <select id='singleBedsInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildPartialUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildBathroomsSelection = function () {
    return "<label for='bathroomsInput' class='fieldAlignmentShortWidth'>Bathrooms:</label>\
        <select id='bathroomsInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildPartialUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildBathsSelection = function () {
    return "<label for='bathsInput' class='fieldAlignmentShortWidth'>Baths:</label>\
        <select id='bathsInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildFullUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildShowersSelection = function () {
    return "<label for='showersInput' class='fieldAlignmentShortWidth'>Showers:</label>\
        <select id='showersInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildFullUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildHandShowersSelection = function () {
    return "<label for='handShowersInput' class='fieldAlignmentShortWidth'>Hand Showers:</label>\
        <select id='handShowersInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildFullUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildLoungesSelection = function () {
    return "<label for='loungesInput' class='fieldAlignmentShortWidth'>Lounges:</label>\
        <select id='loungesInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildPartialUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildDiningRoomsSelection = function () {
    return "<label for='diningRoomsInput' class='fieldAlignmentShortWidth'>Dining Rooms:</label>\
        <select id='diningRoomsInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildPartialUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildGardensSelection = function () {
    return "<label for='gardenInput' class='fieldAlignmentShortWidth'>Garden:</label>\
        <select id='gardenInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>\
            <option value='Private'>Private</option>\
            <option value='Communal'>Communal</option>\
            <option value='No'>No</option>\
        </select>\
        <p class='vertical-spacer' />";
}

app.buildBraaisSelection = function () {
    return "<label for='braaiInput' class='fieldAlignmentShortWidth'>Braai:</label>\
        <select id='braaiInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>\
            <option value='BothGasandCharcoal'>Both Gas and Charcoal</option>\
            <option value='BuiltIn'>Built-In</option>\
            <option value='CharcoalBuiltin'>Charcoal - Built-in</option>\
            <option value='CharcoalPortable'>Charcoal - Portable</option>\
            <option value='Communal'>Communal</option>\
            <option value='GasBuiltin'>Gas - Built-in</option>\
            <option value='GasPortable'>Gas - Portable</option>\
            <option value='None'>None</option>\
            <option value='Portable'>Portable</option>\
            <option value='Private'>Private</option>\
        </select>\
        <p class='vertical-spacer' />";
}

app.buildPoolsSelection = function () {
    return "<label for='poolInput' class='fieldAlignmentShortWidth'>Pool:</label>\
        <select id='poolInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>\
            <option value='Communal'>Communal</option>\
            <option value='None'>None</option>\
            <option value='Poolwithnet'>Pool with net</option>\
            <option value='Private'>Private</option>\
            <option value='Splashpool'>Splash pool</option>\
            <option value='Splashpoolwithnet'>Splash pool with net</option>\
        </select>\
        <p class='vertical-spacer' />";
}

app.buildJacuzziSelection = function () {
    return "<label for='jacuzziInput' class='fieldAlignmentShortWidth'>Jacuzzi:</label>\
        <select id='jacuzziInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>\
            <option value='Private'>Private</option>\
            <option value='Communal'>Communal</option>\
            <option value='No'>No</option>\
        </select>\
        <p class='vertical-spacer' />";
}

app.buildUndercoverParkingBaysSelection = function () {
    return "<label for='undercoverParkingBaysInput' class='fieldAlignmentShortWidth'>Undercover Parking Bays:</label>\
        <select id='undercoverParkingBaysInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildFullUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildOpenParkingBaysSelection = function () {
    return "<label for='openParkingBaysInput' class='fieldAlignmentShortWidth'>Open Parking Bays:</label>\
        <select id='openParkingBaysInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>" +
        app.buildFullUnitOptions() +
        "</select>\
        <p class='vertical-spacer' />";
}

app.buildPeakSeasonOption = function () {
    return "<label for='peakSeasonInput' class='fieldAlignmentShortWidth'>Peak Season:</label>\
                    <input id='peakSeasonInput' class='fieldAlignmentExtraShortWidth' data-parsley-required data-parsley-type='integer' />\
                    <p class='vertical-spacer' />";
}

app.buildSemiSeasonOption = function () {
    return "<label for='semiSeasonInput' class='fieldAlignmentShortWidth'>Semi Season:</label>\
                    <input id='semiSeasonInput' class='fieldAlignmentExtraShortWidth' data-parsley-required data-parsley-type='integer' />\
                    <p class='vertical-spacer' />";
}

app.buildLowSeasonOption = function () {
    return "<label for='lowSeasonInput' class='fieldAlignmentShortWidth'>Low Season:</label>\
                    <input id='lowSeasonInput' class='fieldAlignmentExtraShortWidth' data-parsley-required data-parsley-type='integer' />\
                    <p class='vertical-spacer' />";
}

app.buildOutOfSeasonSeasonOption = function () {
    return "<label for='outOfSeasonInput' class='fieldAlignmentShortWidth'>Out of Season:</label>\
                    <input id='outOfSeasonInput' class='fieldAlignmentExtraShortWidth' data-parsley-required data-parsley-type='integer' />\
                    <p class='vertical-spacer' />";
}

app.buildFloorSelection = function () {
    return "<label for='floorInput' class='fieldAlignmentShortWidth'>Floor:</label>\
        <select id='floorInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>\
            <option value='111'>N/A</option><option value='112'>Single Storey</option><option value='113'>Double Storey</option><option value='114'>Triple Story</option><option value='115'>Ground Floor</option><option value='116'>1 Floor</option><option value='117'>2 Floor</option><option value='118'>3 Floor</option><option value='119'>4 Floor</option><option value='120'>5 Floor</option><option value='121'>6 Floor</option><option value='122'>7 Floor</option><option value='123'>8 Floor</option><option value='124'>9 Floor</option><option value='125'>10 Floor</option><option value='126'>11 Floor</option><option value='127'>12 Floor</option><option value='128'>13 Floor</option><option value='129'>14 Floor</option><option value='130'>15 Floor</option><option value='131'>16 Floor</option><option value='132'>17 Floor</option><option value='133'>18 Floor</option><option value='134'>19 Floor</option><option value='135'>20 Floor</option><option value='136'>21 Floor</option><option value='137'>22 Floor</option><option value='138'>23 Floor</option><option value='139'>24 Floor</option><option value='140'>25 Floor</option><option value='141'>26 Floor</option><option value='142'>27 Floor</option><option value='143'>28 Floor</option><option value='144'>29 Floor</option><option value='145'>30 Floor</option><option value='146'>31 Floor</option><option value='147'>32 Floor</option><option value='148'>33 Floor</option><option value='149'>34 Floor</option><option value='150'>35 Floor</option><option value='151'>36 Floor</option><option value='152'>37 Floor</option><option value='153'>38 Floor</option><option value='154'>39 Floor</option><option value='155'>40 Floor</option><option value='156'>41 Floor</option><option value='157'>42 Floor</option><option value='158'>43 Floor</option><option value='159'>44 Floor</option><option value='160'>45 Floor</option><option value='161'>46 Floor</option><option value='162'>47 Floor</option><option value='163'>48 Floor</option><option value='164'>49 Floor</option><option value='165'>50 Floor</option>\
        </select>\
        <p class='vertical-spacer' />";
}

app.buildSeaViewsSelection = function () {
    return "<label for='seaViewsInput' class='fieldAlignmentShortWidth'>Sea Views:</label>\
        <select id='seaViewsInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>\
            <option value='1'>None</option><option value='2'>Partial</option><option value='3'>Side View</option><option value='4'>Full Frontal</option><option value='5'>Breaker Sea View</option><option value='6'>Distant</option><option value='7'>Good</option><option value='8'>Full Frontal Sea View</option><option value='9'>Distant Sea View</option><option value='10'>Partial Sea View</option><option value='11'>Forest View</option><option value='12'>Mountain View</option><option value='13'>Inland View</option><option value='14'>Golf Course View</option><option value='15'>View over Zimbali</option><option value='16'>Sea and Forest Views</option><option value='17'>Golf Course and Sea Views</option><option value='18'>Driving Range Views</option>\
        </select>\
        <p class='vertical-spacer' />";
}

app.buildCleaningServiceSelection = function () {
    return "<label for='cleaningServiceInput' class='fieldAlignmentShortWidth'>Cleaning Service:</label>\
        <select id='cleaningServiceInput' class='centered-aligned' data-parsley-required>\
            <option value=''></option>\
            <option value='1'>Monday - Saturday</option><option value='2'>Once a Week</option><option value='3'>On Request</option><option value='4'>Not Serviced</option>\
        </select>\
        <p class='vertical-spacer' />";
}
