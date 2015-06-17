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
                var businessUnitUsers = GetBusinessUnitUsers(userGuid);
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
                                      ManagerDetails = userManager,
                                      BusinessUnitUsers = businessUnitUsers,
                                      CommunicationEnabled = user.prospecting_communication
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

        private List<ProspectingUserAuthPacket> GetBusinessUnitUsers(Guid userGuid)
        {
            using (var boss = new BossDataContext())
            {
                List<ProspectingUserAuthPacket> users = new List<ProspectingUserAuthPacket>();
                var thisUser = boss.user_registrations.First(u => u.user_guid == userGuid.ToString());
                List<int> branchIds = new List<int>();
                var licenseBranch = boss.license_branches.First(bu => bu.branch_id == thisUser.branch_id);
                if (licenseBranch.business_unit_id == null)
                {
                    // Get all Then get all the branch id's for license
                    branchIds = boss.license_branches.Where(lb => lb.license_id == licenseBranch.license_id).Select(b => b.branch_id).ToList();
                }
                else
                {
                    branchIds = boss.license_branches.Where(lb => lb.business_unit_id == licenseBranch.business_unit_id).Select(b => b.branch_id).ToList();
                }

                users = (from ur in boss.user_registrations.Where(b => branchIds.Contains(b.branch_id))
                                                           .Where(b => b.prospecting_areas != null)
                         select new ProspectingUserAuthPacket
                         {
                             UserName = ur.user_preferred_name,
                             UserSurname = ur.user_surname,
                             Guid = ur.user_guid
                         }).Distinct().ToList();

                return users;
            }
        }


        public string RetrieveUserSignature(Guid userGuid)
        {
            using (var boss = new BossDataContext())
            {
                var userRegistrationRecord = boss.user_registrations.First(b => b.user_guid == userGuid.ToString());
                var userSignatureRecord = boss.user_signatures.FirstOrDefault(s => s.registration_id == userRegistrationRecord.registration_id);
                if (userSignatureRecord != null)
                {
                    return userSignatureRecord.signature_html;
                }

                return null;
            }
        }
    }
}
