using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Notifications
{
    public class ProspectingFollowupsForUser
    {
        public string Username { get; set; }
        public List<ProspectingFollowup> TodaysFollowups { get; set; }
        public List<ProspectingFollowup> FutureDatedFollowups { get; set; }
        public List<ProspectingFollowup> UnactionedFollowups { get; set; }
        public ProspectingFollowupsForUser()
        {
            TodaysFollowups = new List<Notifications.ProspectingFollowup>();
            FutureDatedFollowups = new List<ProspectingFollowup>();
            UnactionedFollowups = new List<ProspectingFollowup>();
        }

        public bool HasResults
        {
            get { return TodaysFollowups.Any() || FutureDatedFollowups.Any() || UnactionedFollowups.Any(); }
        }

        public long UserRegistrationID { get; set; }
    }
}