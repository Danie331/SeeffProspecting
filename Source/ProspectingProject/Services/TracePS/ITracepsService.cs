using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProspectingProject
{
    public interface ITracepsService
    {
        XDocument GetResponseXML(string idNumber);
    }
}
