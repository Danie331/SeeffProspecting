using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ContactOptoutRequest
    {
        public int ContactPersonId { get; set; }
        public string ContactDetail { get; set; } // NB.: this can be an email address or cell no.
        public string OptoutFromWhat { get; set; }
    }
}