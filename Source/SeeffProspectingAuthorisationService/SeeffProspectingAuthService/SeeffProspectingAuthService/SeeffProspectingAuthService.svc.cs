using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace SeeffProspectingAuthService
{
    public class SeeffProspectingAuthService : ISeeffProspectingAuthService
    {
        public ProspectingUserAuthPacket AuthenticateAndGetUserInfo(Guid userGuid, Guid sessionKey)
        {
            using (var boss = new BossDataContext())
            {
                // We only need to authenticate in the live environment
                if (!HttpContext.Current.IsDebuggingEnabled)
                {
                    if (boss.user_auth(userGuid.ToString(), sessionKey, "PROSPECTING") != 1)
                    {
                        return new ProspectingUserAuthPacket { Authenticated = false };
                    }
                }

                var userManager = GetProspectingManagerDetails(userGuid);
                var userRecord = (from user in boss.user_registrations
                                  where user.user_guid == userGuid.ToString()
                                  select new ProspectingUserAuthPacket
                                  {
                                      SuburbsList = user.prospecting_areas,
                                      AvailableCredit = user.prospecting_credits,
                                      UserName = user.user_preferred_name,
                                      UserSurname = user.user_surname,
                                      IsProspectingManager = user.prospecting_control,
                                      EmailAddress = user.user_email_address,
                                      Guid = user.user_guid,
                                      Authenticated = true,
                                      ManagerDetails = userManager
                                  }).FirstOrDefault();

                return userRecord;
            }
        }

        /// <summary>
        /// Deducts a monetary amount from the user's Prospecting balance
        /// If there are insufficient funds to deduct the amount, the amount is not deducted however the value returned
        /// is the balance less the amount (which would be a negative value).
        /// </summary>
        public decimal DebitUserBalance(decimal amount, Guid userGuid)
        {
            using (var boss = new BossDataContext())
            {
                var user = (from u in boss.user_registrations
                            where u.user_guid == userGuid.ToString()
                            select u).First();

                user.prospecting_credits -= amount;
                if (user.prospecting_credits >= 0.0m)
                {
                    boss.SubmitChanges();
                }

                return user.prospecting_credits;
            }
        }

        /// <summary>
        /// Refunds said amount into user Prospecting balance
        /// </summary>
        public decimal CreditUserBalance(decimal amount, Guid userGuid)
        {
            using (var boss = new BossDataContext())
            {
                var user = (from u in boss.user_registrations
                            where u.user_guid == userGuid.ToString()
                            select u).First();

                user.prospecting_credits += amount;
                boss.SubmitChanges();

                return user.prospecting_credits;
            }
        }

        private List<ProspectingUserAuthPacket> GetProspectingManagerDetails(Guid userGuid)
        {
            using (var boss = new BossDataContext())
            {
                List<ProspectingUserAuthPacket> managerDetails = new List<ProspectingUserAuthPacket>();

                var details = boss.prospecting_controller(userGuid);
                foreach (var detail in details)
                {
                    managerDetails.Add(new ProspectingUserAuthPacket
                    {
                        UserName = detail.user_preferred_name,
                        UserSurname = detail.user_surname,
                        EmailAddress = detail.user_email_address,
                        IsProspectingManager = true,
                        Guid = detail.user_guid
                    });
                }

                return managerDetails;
            }
        }
    }
}
