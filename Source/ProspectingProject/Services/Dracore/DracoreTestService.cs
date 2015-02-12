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

            string ipAddress = "41.150.204.243";
            string sUsername = "ghJodie";
            string sPassword = "password";
            string sToHash = sUsername + now + sPassword + ipAddress;

            var sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            var bTokenVerify = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(sToHash));
            string sTokenVerify = Convert.ToBase64String(bTokenVerify);

            _token.Username = "ghJodie";
            _token.TokenVerify = sTokenVerify;
            _token.RequestTime = now;
        }

        // Phone only
        public Consumer001 ByIdTYPE001(long prospectingIdNumber)
        {
            Consumer001 result = ByIdTYPE001(_token, 6805055722082);
            result.HOME_1 = "0215558160";
            result.WORK_1 = "0214568759";
            return result;
        }

        // Email only
        public Consumer002 ByIdTYPE002(long prospectingIdNumber)
        {
            Consumer002 result = ByIdTYPE002(_token, 6805055722082);
            result.EMAIL_1 = "some_bogus_email@test.com";
            return result;
        }
    }
}