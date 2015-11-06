using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class SpatialUser
    {
        public bool LoginSuccess { get; set; }
        public string LoginMessage { get; set; }
        public List<SeeffSuburb> SeeffAreaCollection  { get; set; }
        //
    }
}