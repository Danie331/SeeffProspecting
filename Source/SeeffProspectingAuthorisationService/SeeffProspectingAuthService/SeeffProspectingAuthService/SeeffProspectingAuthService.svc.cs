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
        public ProspectingUserAuthPacket AuthenticateAndGetUserInfo(Guid userGuid, Guid sessionKey, bool impersonate)
        {
            using (var boss = new BossDataContext())
            {
                // We only need to authenticate in the live environment
                if (!HttpContext.Current.IsDebuggingEnabled)
                {
                    if (!impersonate)
                    {
                        if (boss.user_auth(userGuid.ToString(), sessionKey, "PROSPECTING") != 1)
                        {
                            return new ProspectingUserAuthPacket { Authenticated = false };
                        }
                    }
                }

                bool exportPermission = GetListExportPermissionStatus(userGuid);
                var userManager = GetProspectingManagerDetails(userGuid);
                int? businessUnitID = GetBusinessUnitID(userGuid);
                var businessUnitUsers = GetBusinessUnitUsers(userGuid);
                string permissionLevelLists = GetPermissionLevelForLists(userGuid);
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
                                      CommunicationEnabled = user.prospecting_communication,
                                      BusinessUnitID = businessUnitID,
                                      TrustLookupsEnabled = user.trust_lookup,
                                      BranchID = user.branch_id,
                                      RegistrationId = Convert.ToInt32(user.registration_id),
                                      ExportPermission = exportPermission,
                                      PermissionLevelLists = permissionLevelLists,
                                      CanCreateListing = userGuid.ToString().ToLower() == "a2c48f98-14fb-425e-bbd2-312cfb89980c"
                                  }).FirstOrDefault();

                return userRecord;
            }
        }

        private string GetPermissionLevelForLists(Guid userGuid)
        {
            using (var boss = new BossDataContext())
            {
                var thisUser = boss.user_registrations.First(u => u.user_guid == userGuid.ToString());
                var permissionSection = boss.permission_sections.FirstOrDefault(ps => ps.permission_section_name == "Client Contact Lists");
                if (permissionSection != null)
                {
                    var permissions = boss.permission.Where(p => p.fk_permission_section_id == permissionSection.permission_section_id && p.fk_registration_id == thisUser.registration_id);
                    var permissionsWithLevel = permissions.Where(p => p.fk_permission_level_id != null);
                    if (!permissionsWithLevel.Any())
                    {
                        return "User"; // default to user-level permissions when no levels specified
                    }
                    // find the "highest" permission level for this user and section
                    var permissionLevel = boss.permission_level.FirstOrDefault(pl => pl.permission_level_description == "National");
                    if (permissionLevel != null && permissionsWithLevel.Any(pl => pl.fk_permission_level_id == permissionLevel.pk_permission_level_id)) return permissionLevel.permission_level_description;

                    permissionLevel = boss.permission_level.FirstOrDefault(pl => pl.permission_level_description == "License");
                    if (permissionLevel != null && permissionsWithLevel.Any(pl => pl.fk_permission_level_id == permissionLevel.pk_permission_level_id)) return permissionLevel.permission_level_description;

                    permissionLevel = boss.permission_level.FirstOrDefault(pl => pl.permission_level_description == "Branch");
                    if (permissionLevel != null && permissionsWithLevel.Any(pl => pl.fk_permission_level_id == permissionLevel.pk_permission_level_id)) return permissionLevel.permission_level_description;

                    permissionLevel = boss.permission_level.FirstOrDefault(pl => pl.permission_level_description == "User");
                    if (permissionLevel != null && permissionsWithLevel.Any(pl => pl.fk_permission_level_id == permissionLevel.pk_permission_level_id)) return permissionLevel.permission_level_description;
                }
            }
            return "User";
        }

        private bool GetListExportPermissionStatus(Guid userGuid)
        {
            using (var boss = new BossDataContext())
            {
                var thisUser = boss.user_registrations.First(u => u.user_guid == userGuid.ToString());
                var permissionSection = boss.permission_sections.FirstOrDefault(ps => ps.permission_section_name == "Client Contact Details Report");
                if (permissionSection != null)
                {
                    int permissionSectionId = permissionSection.permission_section_id;
                    return boss.permission.Any(p => p.fk_permission_section_id == permissionSectionId && p.fk_registration_id == thisUser.registration_id);
                }

                return false;
            }
        }

        private int? GetBusinessUnitID(Guid userGuid)
        {
            using (var boss = new BossDataContext())
            {
                var thisUser = boss.user_registrations.First(u => u.user_guid == userGuid.ToString());
                var licenseBranch = boss.license_branches.First(bu => bu.branch_id == thisUser.branch_id);
                return licenseBranch.business_unit_id;
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
                if (licenseBranch.business_unit_id == null || licenseBranch.business_unit_id == 0)
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
                                                           .Where(b => b.active)
                         select new ProspectingUserAuthPacket
                         {
                             UserName = ur.user_preferred_name,
                             UserSurname = ur.user_surname,
                             Guid = ur.user_guid,
                             RegistrationId = Convert.ToInt32(ur.registration_id),
                             BranchID = ur.branch_id
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

        public SpatialUserAuthPacket AuthenticateAndLoadSpatialUser(Guid userGuid, Guid sessionKey)
        {
            using (var boss = new BossDataContext())
            {
                // We only need to authenticate in the live environment
                if (!HttpContext.Current.IsDebuggingEnabled)
                {
                    if (boss.user_auth(userGuid.ToString(), sessionKey, "MAPPING") != 1)
                    {
                        return new SpatialUserAuthPacket { Authenticated = false, AuthMessage = "Failed to authenticate user GUID with BOSS" };
                    }
                }

                var userRecord = boss.user_registrations.FirstOrDefault(u => u.user_guid == userGuid.ToString());
                if (userRecord != null)
                {
                    if (userRecord.mapping)
                    {
                        return new SpatialUserAuthPacket
                        {
                            Authenticated = true,
                            MarketShareSuburbsList = userRecord.ms_area_permissions,
                            ProspectingSuburbsList = userRecord.prospecting_areas
                        };
                    }
                    else
                    {
                        return new SpatialUserAuthPacket { Authenticated = false, AuthMessage = "You are not authorised for mapping. Please request access to mapping from the administrator." };
                    }
                }
                else
                {
                    return new SpatialUserAuthPacket { Authenticated = false, AuthMessage = "Unable to find user record in database." };
                }
            }
        }


        public MarketShareUserAuthPacket AuthenticateMSUser(Guid userGuid, Guid sessionKey)
        {
            using (var boss = new BossDataContext())
            {
                // We only need to authenticate in the live environment
                if (!HttpContext.Current.IsDebuggingEnabled)
                {
                    if (boss.user_auth(userGuid.ToString(), sessionKey, "MARKET SHARE") != 1)
                    {
                        return new MarketShareUserAuthPacket { Authenticated = false, AuthMessage = "Failed to authenticate user GUID with BOSS" };
                    }
                }

                 var userRecord = boss.user_registrations.FirstOrDefault(u => u.user_guid == userGuid.ToString());
                 if (userRecord != null)
                 {
                     return new MarketShareUserAuthPacket
                     {
                         AreaPermissionsList = userRecord.ms_area_permissions,
                         Authenticated = true
                     };
                 }

                // Failure
                 return new MarketShareUserAuthPacket
                 {
                      Authenticated = false,
                      AuthMessage = "Unable to find user in the database."
                 };
            }
        }

        public Guid GetUserGuidByEmail(string email)
        {
            using (var boss = new BossDataContext())
            {
                var targetUser = boss.user_registrations.FirstOrDefault(u => u.user_email_address.ToLower() == email.ToLower());
                Guid result;
                Guid.TryParse(targetUser?.user_guid, out result);
                return result;
            }
        }
    }
}
