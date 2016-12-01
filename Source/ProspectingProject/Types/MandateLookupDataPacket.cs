using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class MandateLookupDataPacket
    {
        public MandateLookupDataPacket()
        {
            MarketshareAgencies = new List<MarketShareAgency>();
            SeeffAgents = new List<SeeffAgent>();
            MandateTypes = new List<MandateType>();
            MandateStatuses = new List<MandateStatus>();
            MandateFollowupTypes = new List<KeyValuePair<int, string>>();
        }

        public List<MarketShareAgency> MarketshareAgencies { get; set; }
        public List<SeeffAgent> SeeffAgents { get; set; }

        public string ErrorMessage { get; set; }
        public List<MandateType> MandateTypes { get; internal set; }
        public List<MandateStatus> MandateStatuses { get; internal set; }
        public List<KeyValuePair<int,string>> MandateFollowupTypes { get; internal set; }
    }
}