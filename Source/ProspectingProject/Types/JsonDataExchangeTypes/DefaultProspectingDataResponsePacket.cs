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

        public string ErrorMsg { get; set; }
    }
}