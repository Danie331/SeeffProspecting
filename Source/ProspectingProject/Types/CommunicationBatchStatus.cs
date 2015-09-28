using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class CommunicationBatchStatus
    {
        public bool SuccessfullySubmitted {get; set;}
        public string ErrorMessage { get; set; }

        public decimal WalletBalance { get; set; }
    }
}