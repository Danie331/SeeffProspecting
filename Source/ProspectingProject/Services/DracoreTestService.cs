using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class DracoreTestService : DracoreServiceProxy, IDracoreService
    {
        private Token _token;
        public DracoreTestService()
        {
            Url =  Properties.Settings.Default.Dracore_test;

            _token = new Token();
            string now = DateTime.Now.ToString();

            string ipAddress = "154.69.179.56";
            string sUsername = "ghJodie";
            string sPassword = "password";
            string sToHash = sUsername + now + sPassword + ipAddress;

            var sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            var bTokenVerify = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(sToHash));
            string sTokenVerify = Convert.ToBase64String(bTokenVerify);

            _token.Username = "ghJodie";
            _token.TokenVerify = sTokenVerify;
            _token.SubAccountRef = HttpContext.Current.Session["user_guid"].ToString();
            _token.RequestTime = now;
        }

        public Consumer003 ByIdTYPE003(long idNumber)
        {
            Consumer003 result = ByIdTYPE003(_token, 6805055722082);
            result.EMAIL_1 = "test_email@test.com";
            //result.HOME_1 = "6557657767";
            return result;
        }
    }
}