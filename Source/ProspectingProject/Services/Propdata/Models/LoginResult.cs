
using System.Collections.Generic;

namespace ProspectingProject.Services.Propdata.Models
{
    public class LoginResult
    {
        public List<LoginAgent> agents { get; set; }
        public List<LoginClient> clients { get; set; }
    }
}