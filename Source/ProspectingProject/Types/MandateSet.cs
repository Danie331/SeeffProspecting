using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class MandateSet
    {
        public MandateSet()
        {
            ListOfMandates = new List<PropertyMandateAgency>();
        }

        public List<PropertyMandateAgency> ListOfMandates { get; set; }
        public string ErrorMessage { get; set; }
    }
}