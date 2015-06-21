using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ContactPersonSmsComparer : IEqualityComparer<ProspectingContactPerson>
    {
        public bool Equals(ProspectingContactPerson x, ProspectingContactPerson y)
        {
            return x.TargetContactCellphoneNumber.Trim() == y.TargetContactCellphoneNumber.Trim();
        }

        public int GetHashCode(ProspectingContactPerson obj)
        {
            return obj.TargetContactCellphoneNumber.GetHashCode();
        }
    }
}