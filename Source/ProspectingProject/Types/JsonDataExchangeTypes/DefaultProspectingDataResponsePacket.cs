using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    /// <summary>
    /// Data results after performing an enquiry against a person
    /// </summary>
    public class ProspectingDataResponsePacket
    {
        public ProspectingDataResponsePacket()
        {
            ContactRows = new List<ContactRow>();
        }

        public string IdCkNo { get; set; }
        public List<ContactRow> ContactRows { get; set; }
        public string OwnerName { get; set; }
        public string OwnerSurname { get; set; }
        public string OwnerGender { get; set; }
        public bool EnquirySuccessful { get; set; }
        public int? AvailableTracePsCredits { get; set; }

        // New fields by Dracore
        public string Title { get; set; }
        public string DeceasedStatus { get; set; }
        public string AgeGroup { get; set; }
        public string Location { get; set; }
        public string MaritalStatus { get; set; }
        public string HomeOwnership { get; set; }
        public string Directorship { get; set; }
        public string PhysicalAddress { get; set; }
        public string Employer { get; set; }
        public string Occupation { get; set; }
        public string BureauAdverseIndicator { get; set; }
        public string Citizenship { get; set; }

        public string ErrorMsg { get; set; }
    }
}