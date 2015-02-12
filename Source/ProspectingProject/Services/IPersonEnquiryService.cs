using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectingProject
{
    /// <summary>
    /// Represents an object capable of doing a person enquiry/lookup
    /// </summary>
    public interface IPersonEnquiryService
    {
        // Calls out to the SeeffProspectingAuthService to deduct the enquiry-specific cost
        decimal DeductEnquiryCost();
        // Performs the enquiry returning the enquiry results
        void DoEnquiry();
        // Reverses the cost of the enquiry
        decimal ReverseEnquiryCost();
        // If an exception occurs when the consuming code calls any of these methods, set an internal error for logging purposes.
        void SetError(Exception ex);
        // Logs the enquiry to the database.
        void LogEnquiry();

        void InitResponsePacket(PersonEnquiryResponsePacket results);
    }
}
