using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class UserDataResponsePacket
    {
        public Guid UserGuid { get; set; }
        public string AvailableSuburbs { get; set; }
        public string StaticProspectingData { get; set; }
        public decimal AvailableCredit { get; set; }
        public bool Authenticated { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public bool IsProspectingManager { get; set; }
        public string EmailAddress { get; set; }

        public UserDataResponsePacket ProspectingManager { get; set; }

        public List<UserDataResponsePacket> BusinessUnitUsers { get; set; }

        public List<FollowUpActivity> FollowupActivities { get; set; }
    }
}