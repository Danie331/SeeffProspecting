
using System.Collections.Generic;

namespace ProspectingProject.Services.Propdata.Models
{
    public class Agent
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public List<int> branches { get; set; }
        public int group { get; set; }
    }
}