
using System.Collections.Generic;
namespace ProspectingTaskScheduler.Core.Communication.Emailing.Mandrill
{
    public class MandrillMergVar
    {
        public string rcpt { get; set; }
        public List<MandrillMergeVarVar> vars { get; set; }
    }
}
