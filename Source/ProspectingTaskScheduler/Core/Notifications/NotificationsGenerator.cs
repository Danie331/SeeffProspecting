using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Hangfire;
using ProspectingTaskScheduler.Core.Housekeeping;

namespace ProspectingTaskScheduler.Core.Notifications
{
    public class NotificationsGenerator
    {
        [AutomaticRetry(Attempts = 0)]
        public static void SendProspectingFollowupsNotification(IJobCancellationToken cancellationToken)
        {
            try
            {
                foreach (var user in RetrieveRecipientsForProspectingFollowups())
                {
                    if (cancellationToken != null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    ProspectingFollowupsForUser followupsForUser = RetrieveFollowupsForUser(user.user_guid);
                    if (followupsForUser.HasResults)
                    {
                        string emailContent = CreateMessageBody(followupsForUser);
                        StatusNotifier.SendEmail(user.user_email_address, "Prospecting", "reports@seeff.com", null, "Prospecting Follow Ups", emailContent);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Suppress and return as the job will be retried during its next scheduled run.
                return;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex);
            }
        }

        private static string CreateMessageBody(ProspectingFollowupsForUser followupsForUser)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            sb.Append("<body>");
            sb.Append("<table width='600' color='#07063f' style='margin:0 auto; font-family:verdana;' border='0' cellpadding='0' cellspacing='0' border='n'>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<a href='http://www.seeff.com' border='n'><img src='http://www.seeff.com/Images/Email/seeffemaillogo.jpg' width='600' border='n'></a>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<h3 style='font-size:16px;'>Good Day " + followupsForUser.Username + "</h3>");
            string unsubscribeTarget = "http://154.70.214.213/ProspectingTaskScheduler/UnsubscribeProspectingUser.html?reg_id=" + followupsForUser.UserRegistrationID;
            string unsuscribeLink = "<a href='" + unsubscribeTarget + "' target='_blank'>here</a>";
            sb.Append("<p style='font-size:14px;'>Please take note of your Prospecting follow ups listed below. If you would like to unsubscribe from this daily reminder, please click " + unsuscribeLink + "</p>");

            //Follow ups today
            CreateContentSection(sb, followupsForUser.TodaysFollowups, "Today's Follow Ups - " + DateTime.Today.ToString("yyyy/MM/dd"), "#06791f", false);

            //Future follow ups
            CreateContentSection(sb, followupsForUser.FutureDatedFollowups, "Upcoming Follow Ups (next 3 days)", "#da8d00", true);

            //un-actioned/open follow ups
            CreateContentSection(sb, followupsForUser.UnactionedFollowups, "Un-actioned (open) Follow Ups", "#dc2305", true);
       
            sb.Append("<p style='font-size:14px;'>Please contact support@seeff.com should you have any questions in this regard.</p>");
            sb.Append("<p style='font-size:14px;'><b>Kind Regards <br/>Seeff Head Office</b></p>");

            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<br/>");
            sb.Append("<img src='http://www.seeff.com/images/mymail/redline.PNG' width='600' border='n'>");
            sb.Append("<p width='600' style='width:600px; margin-top:10px; font-size:12px;'>Copyright &copy; " + DateTime.Now.Year.ToString() + " Seeff. All rights reserved.</p>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</body>");
            sb.Append("</html>");

            return sb.ToString();
        }

        private static void CreateContentSection(StringBuilder sb, List<ProspectingFollowup> targetList, string heading, string colour, bool showFollowupDate)
        {
            sb.Append("<div style='margin-bottom:20px;'>");
            sb.Append("<h5 style='font-size:14px; background-color:" + colour + "; padding:10px 0px 10px 10px; color:#ffffff; margin:0; border-radius:5px 5px 0px 0px; width:590px;'>" + heading + "</h5>");
            sb.Append("<table style='list-style:none; font-size:14px; background-color:#ececec; padding:5px 0px 5px 0px; color:#07063f; margin:0;' width='600'>");

            foreach (var item in targetList)
            {
                sb.Append(" <tr >");
                sb.Append("     <td style='background-color:#0b5179; padding:5px; color:#fff; font-weight:bold;'>" + item.PropertyAddress + "</td>");
                sb.Append(" </tr>");
                sb.Append(" <tr>");
                sb.Append("     <td>");
                sb.Append("         <table style='width:100%;'>");
                sb.Append("             <tr>");
                sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;width:150px;'>Activity:</td>");
                sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'>" + item.ActivityTypeName + "</td>");
                sb.Append("             </tr>");
                if (item.FollowupActivityTypeName != "n/a")
                {
                    sb.Append("             <tr>");
                    sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;width:150px;'>Follow-up Type:</td>");
                    sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'>" + item.FollowupActivityTypeName + "</td>");
                    sb.Append("             </tr>");
                }
                if (showFollowupDate)
                {
                    sb.Append("             <tr>");
                    sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;width:150px;'>Follow-up Date:</td>");
                    sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'>" + item.FollowupDate.ToString("yyyy/MM/dd") + "</td>");
                    sb.Append("             </tr>");
                }
                sb.Append("             <tr>");
                sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;width:150px;'>Created By:</td>");
                sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'>" + item.CreatedByUsername + "</td>");
                sb.Append("             </tr>");
                sb.Append("             <tr>");
                sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;width:150px;'>Related To:</td>");
                sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'>" + item.RelatedToContactPerson + "</td>");
                sb.Append("             </tr>");
                //sb.Append("             <tr>");
                //sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;width:150px;'>Primary Phone:</td>");
                //sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'><a href='tel:" + item.PrimaryContactNo + "'>" + item.PrimaryContactNoFormatted + "</a></td>");
                //sb.Append("             </tr>");
                //sb.Append("             <tr>");
                //sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;width:150px;'>Primary Email:</td>");
                //sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'><a href='mailto:" + item.PrimaryEmailAddress + "'>" + item.PrimaryEmailAddressFormatted + "</a></td>");
                //sb.Append("             </tr>");
                sb.Append("             <tr>");
                sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;width:150px;'>Comment:</td>");
                sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'>" + item.Comment + "</td>");
                sb.Append("             </tr>");
                sb.Append("         </table>");
                sb.Append("     </td>");
                sb.Append(" </tr>");
            }

            if (targetList.Count == 0)
            {
                sb.Append(" <tr >");
                sb.Append("<td style='padding:5px; font-weight:bold; background-color:#fff;'>No follow ups in this category</td>");
                sb.Append(" </tr>");
            }

            sb.Append("</table>");
            sb.Append("</div>");
        }

        private static ProspectingFollowupsForUser RetrieveFollowupsForUser(string user_guid)
        {
            ProspectingFollowupsForUser results = new ProspectingFollowupsForUser();
            using (var prospecting = new seeff_prospectingEntities())
            {
                Guid userGuid = Guid.Parse(user_guid);
                var user = GetBossUser(userGuid);
                results.Username = user != null ? string.Concat(user.user_name, " ", user.user_surname) : "n/a";
                results.UserRegistrationID = user != null ? user.registration_id : -1;
                List<long?> parentIds = prospecting.activity_log.Where(a => a.parent_activity_id != null).Select(a => a.parent_activity_id).ToList();

                var todaysFollowups = prospecting.activity_log.Where(act => act.allocated_to == userGuid &&
                                                                                act.followup_date != null &&
                                                                                act.followup_date.Value.Date == DateTime.Today).ToList();
                todaysFollowups = todaysFollowups.Where(act => !parentIds.Contains(act.activity_log_id)).ToList();
                todaysFollowups = todaysFollowups.OrderByDescending(act => act.followup_date).ToList();
                foreach (var item in todaysFollowups)
                {
                    ProspectingFollowup followup = LoadFollowup(item);
                    results.TodaysFollowups.Add(followup);
                }

                DateTime next3days = DateTime.Today.AddDays(3.0).Date;
                var futureFollowups = prospecting.activity_log.Where(act => act.allocated_to == userGuid &&
                                                                                act.followup_date != null &&
                                                                                act.followup_date.Value.Date > DateTime.Today &&
                                                                                act.followup_date.Value.Date <= next3days).ToList();
                futureFollowups = futureFollowups.Where(act => !parentIds.Contains(act.activity_log_id)).ToList();
                futureFollowups = futureFollowups.OrderBy(act => act.followup_date).ToList();
                foreach (var item in futureFollowups)
                {
                    ProspectingFollowup followup = LoadFollowup(item);
                    results.FutureDatedFollowups.Add(followup);
                }

                var unactionedFollowups = prospecting.activity_log.Where(act => act.allocated_to == userGuid &&
                                                                                act.followup_date != null &&
                                                                                act.followup_date.Value.Date < DateTime.Today).ToList();
                unactionedFollowups = unactionedFollowups.Where(act => !parentIds.Contains(act.activity_log_id)).ToList();
                unactionedFollowups = unactionedFollowups.OrderByDescending(act => act.followup_date).ToList();
                foreach (var item in unactionedFollowups)
                {
                    ProspectingFollowup followup = LoadFollowup(item);
                    results.UnactionedFollowups.Add(followup);
                }

                return results;
            }
        }

        private static ProspectingFollowup LoadFollowup(activity_log item)
        {
            ProspectingFollowup followup = new ProspectingFollowup();
            var user = GetBossUser(item.created_by);
            followup.FollowupDate = item.followup_date.Value;
            followup.PropertyAddress = GetFormattedAddress(item.lightstone_property_id);
            followup.CreatedByUsername = user != null ? string.Concat(user.user_name, " ", user.user_surname) : "n/a";
            followup.Comment = item.comment;
            followup.RelatedToContactPerson = GetContactPersonName(item.contact_person_id);
            //var primaryContactNoKvp = GetContactsPrimaryPhoneNo(item.contact_person_id);
            //followup.PrimaryContactNo = primaryContactNoKvp.Key;
            //followup.PrimaryContactNoFormatted = primaryContactNoKvp.Value;
            //var primaryEmailKvp = GetContactsPrimaryEmailAddress(item.contact_person_id);
            //followup.PrimaryEmailAddress = primaryEmailKvp.Key;
            //followup.PrimaryEmailAddressFormatted = primaryEmailKvp.Value;
            followup.ActivityTypeName = item.activity_type != null ? item.activity_type.activity_name : "n/a";
            followup.FollowupActivityTypeName = item.activity_followup_type != null ? item.activity_followup_type.activity_name : "n/a";

            return followup;
        }

        private static KeyValuePair<string, string> GetContactsPrimaryEmailAddress(int? contact_person_id)
        {
            if (contact_person_id == null)
                return new KeyValuePair<string, string>("n/a", "n/a");

            using (var prospecting = new seeff_prospectingEntities())
            {
                var target = prospecting.prospecting_contact_person.First(cp => cp.contact_person_id == contact_person_id);
                var primaryContactDetail = target.prospecting_contact_detail.FirstOrDefault(cd => new[] { 4, 5 }.Contains(cd.contact_detail_type) && cd.is_primary_contact && !cd.deleted);

                return primaryContactDetail != null ? 
                     new KeyValuePair<string, string>(primaryContactDetail.contact_detail, string.Concat(primaryContactDetail.contact_detail, " (", primaryContactDetail.prospecting_contact_detail_type.type_desc, ")"))
                     :  new KeyValuePair<string, string>("n/a", "n/a");
            }
        }

        private static KeyValuePair<string, string> GetContactsPrimaryPhoneNo(int? contact_person_id)
        {
            if (contact_person_id == null)
                return new KeyValuePair<string, string>("n/a", "n/a");

            using (var prospecting = new seeff_prospectingEntities())
            {
                var target = prospecting.prospecting_contact_person.First(cp => cp.contact_person_id == contact_person_id);
                var primaryContactDetail = target.prospecting_contact_detail.FirstOrDefault(cd => new[] { 1, 2, 3 }.Contains(cd.contact_detail_type) && cd.is_primary_contact && !cd.deleted);

                if (primaryContactDetail != null)
                {
                    string formattedContactDetail = primaryContactDetail.contact_detail.Length == 10 ? string.Concat(primaryContactDetail.contact_detail.Substring(0, 3), " ", primaryContactDetail.contact_detail.Substring(3, 3), " ", primaryContactDetail.contact_detail.Substring(6, 4))
                                                                                                     : primaryContactDetail.contact_detail;
                    return new KeyValuePair<string, string>(primaryContactDetail.contact_detail, string.Concat(formattedContactDetail, " (", primaryContactDetail.prospecting_contact_detail_type.type_desc, ")"));
                }
                               
                 return new KeyValuePair<string, string>("n/a", "n/a");
            }
        }

        private static string GetContactPersonName(int? contact_person_id)
        {
            if (contact_person_id == null)
                return "n/a";

            using (var prospecting = new seeff_prospectingEntities())
            {
                var target = prospecting.prospecting_contact_person.First(cp => cp.contact_person_id == contact_person_id);
                return string.Concat(target.firstname, " ", target.surname);
            }
        }

        private static user_registration GetBossUser(Guid created_by)
        {
            using (var boss = new bossEntities())
            {
                var target = boss.user_registration.FirstOrDefault(ur => ur.user_guid == created_by.ToString().ToLower());
                return target;
            }
        }

        public static string GetFormattedAddress(int lightstonePropertyId)
        {
            using (var prospectingContext = new seeff_prospectingEntities())
            {
                var property = prospectingContext.prospecting_property.First(pp => pp.lightstone_property_id == lightstonePropertyId);
                if (property.ss_fh == "SS" || property.ss_fh == "FS")
                {
                    if (!string.IsNullOrEmpty(property.ss_door_number))
                    {
                        return "Unit " + property.unit + " (Door no.: " + property.ss_door_number + ") " + new CultureInfo("en-US", false).TextInfo.ToTitleCase(property.ss_name.ToLower()).Replace("Ss ", "SS ");
                    }

                    return "Unit " + property.unit + " " + new CultureInfo("en-US", false).TextInfo.ToTitleCase(property.ss_name.ToLower()).Replace("Ss ", "SS ");
                }

                return property.street_or_unit_no + " " + new CultureInfo("en-US", false).TextInfo.ToTitleCase(property.property_address.ToLower());
            }
        }

        private static List<user_registration> RetrieveRecipientsForProspectingFollowups()
        {
            using (var boss = new bossEntities())
            {
                var targetPreferenceType = boss.user_preference_type.FirstOrDefault(pt => pt.preference_type_description == "Prospecting follow up daily email notifications");
                if (targetPreferenceType != null)
                {
                    var targetUsers = boss.user_preference.Where(up => up.fk_user_preference_type_id == targetPreferenceType.pk_user_preference_type_id)
                                                          .Select(up => up.user_registration)
                                                          .Distinct();
                    return targetUsers.ToList();
                }
            }
            return new List<user_registration>();
        }
    }
}