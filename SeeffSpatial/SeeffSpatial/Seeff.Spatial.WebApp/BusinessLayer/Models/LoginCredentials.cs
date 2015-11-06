using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class LoginCredentials
    {
        public Guid UserGuid { get; set; }
        public Guid SessionKey { get; set; }
    }
}