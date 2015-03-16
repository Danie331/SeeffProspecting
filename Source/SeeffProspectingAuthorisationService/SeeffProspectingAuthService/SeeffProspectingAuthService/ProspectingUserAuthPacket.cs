using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SeeffProspectingAuthService
{
    [DataContract]
    public class ProspectingUserAuthPacket
    {
        [DataMember]
        public string SuburbsList { get; set; }

        [DataMember]
        public decimal AvailableCredit { get; set; }

        [DataMember]
        public bool Authenticated { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string UserSurname { get; set; }

        [DataMember]
        public bool IsProspectingManager { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public List<ProspectingUserAuthPacket> ManagerDetails { get; set; }

        [DataMember]
        public List<ProspectingUserAuthPacket> BusinessUnitUsers { get; set; }
    }
}