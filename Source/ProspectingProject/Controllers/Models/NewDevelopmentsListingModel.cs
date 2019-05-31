
using System.Collections.Generic;

namespace ProspectingProject.Controllers.Models
{
    public class NewDevelopmentsListingModel : ListingBaseModel
    {
        public string name { get; set; }
        public string category { get; set; }
        public List<DevelopmentPropertyType> property_types { get; set; }
    }
}