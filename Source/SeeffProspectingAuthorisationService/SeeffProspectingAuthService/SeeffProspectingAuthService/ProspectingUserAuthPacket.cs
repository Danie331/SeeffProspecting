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
        public string UserType { get; set; }

        [DataMember]
        public string SuburbsList { get; set; }

        [DataMember]
        public decimal AvailableCredit { get; set; }

        [DataMember]
        public bool Authenticated { get; set; }
    }
}