using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class DuplicatePropertyRecordException : Exception
    {
        public string ErrorMsg { get; set; }
        public int PropertyRecordId { get; set; }
        public int SeeffAreaId { get; set; }
    }
}