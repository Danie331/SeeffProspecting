using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class EmailAddressComparer : IEqualityComparer<EmailRecipient>
    {
        public bool Equals(EmailRecipient x, EmailRecipient y)
        {
            return x.ToEmailAddress.ToLower().Trim() == y.ToEmailAddress.ToLower().Trim();
        }

        public int GetHashCode(EmailRecipient obj)
        {
            return obj.ToEmailAddress.GetHashCode();
        }
    }
}