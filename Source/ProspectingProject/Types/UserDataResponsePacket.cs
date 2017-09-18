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
        public bool HasCommAccess { get; set; }
        public int? BusinessUnitID { get; set; }
        public bool TrustLookupsEnabled { get; set; }
        public UserDataResponsePacket ProspectingManager { get; set; }

        public List<UserDataResponsePacket> BusinessUnitUsers { get; set; }

        public List<FollowUpActivity> FollowupActivities { get; set; }

        public int TotalFollowups { get; set; }

        public string Fullname
        {
            get { return UserName + " " + UserSurname; }
        }

        public List<KeyValuePair<int, string>> ActivityFollowupTypes { get; internal set; }
        public List<KeyValuePair<int, string>> ActivityTypes { get; internal set; }

        public bool IsTrainingMode { get; set; }
        public int BranchID { get; internal set; }
        public int RegistrationId { get; set; }
        public bool ExportPermission { get; set; }
        public string PermissionLevelForLists { get; internal set; }
    }
}