using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SeeffProspectingAuthService
{
    [DataContract]
    public class MarketShareUserAuthPacket
    {
        [DataMember]
        public string AreaPermissionsList { get; set; }

        [DataMember]
        public bool Authenticated { get; set; }

        [DataMember]
        public string AuthMessage { get; set; }
    }
}