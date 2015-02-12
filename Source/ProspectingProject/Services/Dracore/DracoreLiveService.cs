using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class DracoreLiveService : DracoreServiceProxy, IDracoreService
    {
        Token _token = null;
        public DracoreLiveService()
        {
            this.Url = Properties.Settings.Default.Dracore_live;
            _token = new Token();
            string now = DateTime.Now.ToString();

            string ipAddress = "154.70.214.213";
            string sUsername = "gvuAdam";
            string sPassword = "c69d5da5SEEFF";
            string sToHash = sUsername + now + sPassword + ipAddress;

            var sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            var bTokenVerify = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(sToHash));
            string sTokenVerify = Convert.ToBase64String(bTokenVerify);

            _token.Username = "gvuAdam";
            _token.TokenVerify = sTokenVerify;
            _token.SubAccountRef = "109";
            _token.RequestTime = now;
        }

        public Consumer001 ByIdTYPE001(long prospectingIdNumber)
        {
            return ByIdTYPE001(_token, prospectingIdNumber);
        }

        public Consumer002 ByIdTYPE002(long prospectingIdNumber)
        {
            return ByIdTYPE002(_token, prospectingIdNumber);
        }
    }
}