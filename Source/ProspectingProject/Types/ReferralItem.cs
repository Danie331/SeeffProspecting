using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ReferralItem
    {
        public int SmartPassId { get; set; }
        public string CurrentStatus { get; set; }
        public int id { get
            {
                return SmartPassId.GetHashCode() + Math.Abs(CurrentStatus.GetHashCode());
            }
        }
    }
}