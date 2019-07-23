using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketShareApp.DomainTypes
{
    public class TransactionExport : BaseDataRequestPacket
    {
        public List<LightstoneListing> Transactions { get; set; }
    }
}