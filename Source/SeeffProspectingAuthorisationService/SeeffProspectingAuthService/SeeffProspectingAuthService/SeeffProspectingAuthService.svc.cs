using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SeeffProspectingAuthService
{
    public class SeeffProspectingAuthService : ISeeffProspectingAuthService
    {
        public ProspectingUserAuthPacket GetUserInfo(Guid userGuid)
        {
            using (var boss = new BossDataContext())
            {
                //Adam's education
                //IEnumerable<ProspectingUserAuthPacket> query = from user in boss.user_registrations
                //            where user.user_guid == userGuid.ToString()
                //            select new ProspectingUserAuthPacket
                //            {
                //                SuburbsList = user.prospecting_areas,
                //                AvailableCredit = user.prospecting_credits
                //            };
                //var query = from user in boss.user_registrations
                //                                               where user.user_guid == userGuid.ToString()
                //                                               select new ProspectingUserAuthPacket
                //                                               {
                //                                                   SuburbsList = user.prospecting_areas,
                //                                                   AvailableCredit = user.prospecting_credits
                //                                               };
                var query = from user in boss.user_registrations
                            where user.user_guid == userGuid.ToString()
                            select new ProspectingUserAuthPacket
                            {
                                SuburbsList = user.prospecting_areas,
                                AvailableCredit = user.prospecting_credits
                            };
                var userAuthPacket = (query).FirstOrDefault();
                return userAuthPacket;
            }
        }

        public int TakeOneCredit(Guid userGuid)
        {
            using (var boss = new BossDataContext())
            {
                var user = (from u in boss.user_registrations
                            where u.user_guid == userGuid.ToString()
                            select u).First();
                if (user.prospecting_credits > 0)
                {
                    user.prospecting_credits -= 1;
                } 
                else
                {
                    return -1;
                }

                boss.SubmitChanges();
                return user.prospecting_credits;
            }
        }

        public int ReimburseOneCredit(Guid userGuid)
        {
            using (var boss = new BossDataContext())
            {
                var user = (from u in boss.user_registrations
                            where u.user_guid == userGuid.ToString()
                            select u).First();
                user.prospecting_credits += 1;
                boss.SubmitChanges();
                return user.prospecting_credits;
            }
        }
    }
}
