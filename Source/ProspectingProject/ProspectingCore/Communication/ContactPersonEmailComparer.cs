using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ContactPersonEmailComparer : IEqualityComparer<ProspectingContactPerson>
    {
        public bool Equals(ProspectingContactPerson x, ProspectingContactPerson y)
        {
            return x.TargetContactEmailAddress.ToLower().Trim() == y.TargetContactEmailAddress.ToLower().Trim();
        }

        public int GetHashCode(ProspectingContactPerson obj)
        {
            return obj.TargetContactEmailAddress.GetHashCode();
        }
    }
}