using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SeeffProspectingAuthService
{
    [DataContract]
    public class SpatialUserAuthPacket
    {
        [DataMember]
        public bool Authenticated { get; set; }

        [DataMember]
        public string AuthMessage { get; set; }

        [DataMember]
        public string ProspectingSuburbsList { get; set; }

        [DataMember]
        public string MarketShareSuburbsList { get; set; }
    }
}