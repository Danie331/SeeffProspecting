using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public interface ICompanyEnquiryService
    {
        decimal DeductEnquiryCost();

        void DoEnquiry();

        decimal ReverseEnquiryCost();

        void SetError(Exception ex);

        void LogEnquiry();

        void InitResponsePacket(CompanyEnquiryResponsePacket results);
    }
}