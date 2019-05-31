
using Newtonsoft.Json;

namespace ProspectingProject.Controllers.Models
{
    public class ListingBaseModel: IListingBaseModel
    {
        public int lightstone_id { get; set; }
        public string map_y_position { get; set; }
        public string map_x_position { get; set; }
        public string erf_number { get; set; }
        public string street_name { get; set; }
        public string street_number { get; set; }
        public string complex_name { get; set; }
        public string unit_number { get; set; }
        public int location { get; set; }
        public string description { get; set; }
        public int agent { get; set; }
        public int branch { get; set; }
        public string status { get; set; }
        public decimal price { get; set; }
        public string listing_type { get; set; }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}