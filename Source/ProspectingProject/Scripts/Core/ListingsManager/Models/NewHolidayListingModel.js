


app.createHolidayListingModel = function () {
    var baseModel = new NewListingModelBase().create();
    return Object.assign(baseModel, {
        property_type: $("#propertyTypeInput option:selected").text(),
        establishment_name: $("#establishmentNameInput").val(),
        establishment_notes: $("#establishmentNotesInput").val(),
        bedrooms: $("#bedroomsInput option:selected").text(),
        sleeps: $("#sleepsInput option:selected").text(),

        king_beds: $("#kingBedsInput option:selected").text(),
        queen_beds: $("#queenBedsInput option:selected").text(),
        double_beds: $("#doubleBedsInput option:selected").text(),
        bunk_beds: $("#bunkBedsInput option:selected").text(),
        single_beds: $("#singleBedsInput option:selected").text(),
        bathrooms: $("#bathroomsInput option:selected").text(),
        baths: $("#bathsInput option:selected").text(),
        showers: $("#showersInput option:selected").text(),
        hand_showers: $("#handShowersInput option:selected").text(),
        lounge: $("#loungesInput option:selected").text(),
        dining_room: $("#diningRoomsInput option:selected").text(),
        garden: $("#gardenInput option:selected").text(),
        braai: $("#braaiInput option:selected").text(),
        pool: $("#poolInput option:selected").text(),
        jacuzzi: $("#jacuzziInput option:selected").text(),
        undercover_parking_bays: $("#undercoverParkingBaysInput option:selected").text(),
        open_parking_bays: $("#openParkingBaysInput option:selected").text(),
        peak_season: $("#peakSeasonInput").val(),
        semi_season: $("#semiSeasonInput").val(),
        low_season: $("#lowSeasonInput").val(),
        out_of_season: $("#outOfSeasonInput").val(),
        floor: $("#floorInput option:selected").text(),
        sea_views: $("#seaViewsInput option:selected").text(),
        cleaning_service: $("#cleaningServiceInput option:selected").text()
    });
}