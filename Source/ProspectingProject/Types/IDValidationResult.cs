using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class IDValidationResult
    {
        public bool Result { get; set; }
        public string ErrorMessage { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}