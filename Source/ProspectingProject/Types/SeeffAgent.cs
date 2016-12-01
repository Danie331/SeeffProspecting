using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class SeeffAgent
    {
        public int AgentID { get; set; }
        public string AgentName { get; set; }

        public string label { get { return AgentName; } }
        public int value
        {
            get { return AgentID; }
        }
    }
}