using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ActivityBundle
    {
        public ActivityBundle()
        {
            Activities = new List<ProspectingActivity>();
            BusinessUnitUsers = new List<UserDataResponsePacket>();
        }
        public List<ProspectingActivity> Activities { get; set; }
        public List<UserDataResponsePacket> BusinessUnitUsers { get; set; }
        public List<KeyValuePair<int, string>> ActivityTypes { get; set; }
    }
}