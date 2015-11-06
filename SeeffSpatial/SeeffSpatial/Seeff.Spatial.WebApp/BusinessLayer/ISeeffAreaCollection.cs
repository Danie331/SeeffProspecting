using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seeff.Spatial.WebApp.BusinessLayer
{
    public interface ISeeffAreaCollection
    {
        IList<SeeffSuburb> Suburbs { get; set; }
    }
}
