using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication
{
    public class CommunicationHelpers
    {
        public static int GetCommunicationStatusId(string statusDesc)
        {
            using (var prospecting = new seeff_prospectingEntities())
            {
                var commType = prospecting.communications_status.First(ct => ct.status_desc == statusDesc);
                return commType.communications_status_id;
            }
        }
    }
}