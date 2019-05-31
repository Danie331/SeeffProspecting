namespace ProspectingProject.Controllers.Models
{
    public class NewHolidayListingModel : ListingBaseModel
    {
        public string property_type { get; set; }
        public string establishment_name { get; set; }
        public string establishment_notes { get; set; }
        public string bedrooms { get; set; }
        public string sleeps { get; set; }
        public string king_beds { get; set; }
        public string queen_beds { get; set; }
        public string double_beds { get; set; }
        public string bunk_beds { get; set; }
        public string single_beds { get; set; }
        public string bathrooms { get; set; }
        public int baths { get; set; }
        public int showers { get; set; }
        public int hand_showers { get; set; }
        public string lounge { get; set; }
        public string dining_room { get; set; }
        public string garden { get; set; }
        public string braai { get; set; }
        public string pool { get; set; }
        public string jacuzzi { get; set; }
        public int undercover_parking_bays { get; set; }
        public int open_parking_bays { get; set; }
        public int peak_season { get; set; }
        public int semi_season { get; set; }
        public int low_season { get; set; }
        public int out_of_season { get; set; }
        public string floor { get; set; }
        public string sea_views { get; set; }
        public string cleaning_service { get; set; }
    }
}