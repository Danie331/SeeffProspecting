using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class SearchInputPacket
    {
        public SearchInputPacket()
        {
            CurrentSuburbId = null;
            IDNumber = null;
            EmailAddress = null;
            PhoneNumber = null;
            PropertyAddress = null;
        }

        public int? CurrentSuburbId { get; set; }
        //public int[] CurrentlyLoadedSuburbs { get; set; }
        //public int[] AvailableSuburbs { get; set; }

        public string IDNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PropertyAddress { get; set; }

        public string StreetOrUnitNo { get; set; }

        public string DeedTown { get; set; }
        public string Suburb { get; set; }
        public string StreetName { get; set; }
        public string SSName { get; set; }
        public string SSNumber { get; set; }
        public string Unit { get; set; }

        public string ErfNo { get; set; }
        public string Portion { get; set; }

        public string EstateName { get; set; }
    }
}