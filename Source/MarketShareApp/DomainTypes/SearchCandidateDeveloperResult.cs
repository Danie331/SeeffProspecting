using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketShareApp.DomainTypes
{
    public class SearchCandidateDeveloperResult
    {
        public string CandidateDeveloperName { get; internal set; }
        public int NumRegistrations { get; internal set; }
    }
}