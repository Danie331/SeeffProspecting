

namespace ProspectingProject.Services.Propdata.Models
{
    public class ListingResult
    {
        public int id { get; set; }
        public int lightstone_id { get; set; }
        public string status { get; set; }
        public int agent { get; set; }

        public string URL { get; set; }        
        public string JsonPayload { get; set; }
        public int ActiveListingId { get; internal set; }
    }
}