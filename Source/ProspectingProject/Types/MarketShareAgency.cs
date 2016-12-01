using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class MarketShareAgency
    {
        public int AgencyID { get; set; }
        public string AgencyName { get; set; }

        public string label { get { return AgencyName; } }
        public int value {
            get { return AgencyID; }
        }
    }
}