using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using ProspectingTaskScheduler.Core.Housekeeping;

namespace ProspectingTaskScheduler.Core.Notifications
{
    public class NotificationsGenerator
    {
        public static void SendProspectingFollowupsNotification()
        {
            try
            {
                foreach (var user in RetrieveRecipientsForProspectingFollowups())
                {
                    ProspectingFollowupsForUser followupsForUser = RetrieveFollowupsForUser(user.user_guid);
                    string emailContent = CreateMessageBody(followupsForUser);
                    StatusNotifier.SendEmail(user.user_email_address, "Prospecting", "reports@seeff.com", "danie.vdm@seeff.com", "Prospecting Follow Ups", emailContent);                                        
                }
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
            sb.Append("<p style='font-size:14px;'>Please take note of your prospecting follow ups listed below.</p>");

            //Follow ups today
            CreateContentSection(sb, followupsForUser.TodaysFollowups, "Today's Follow Ups - " + DateTime.Now.ToShortDateString(), "#06791f", false);

            //Future follow ups
            CreateContentSection(sb, followupsForUser.FutureDatedFollowups, "Future dated Prospecting Follow ups", "#da8d00", true);

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
                sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;'>Activity:</td>");
                sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'>" + item.ActivityTypeName + "</td>");
                sb.Append("             </tr>");
                if (item.FollowupActivityTypeName != "n/a")
                {
                    sb.Append("             <tr>");
                    sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;'>Follow-up Type:</td>");
                    sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'>" + item.FollowupActivityTypeName + "</td>");
                    sb.Append("             </tr>");
                }
                if (showFollowupDate)
                {
                    sb.Append("             <tr>");
                    sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;'>Follow-up Date:</td>");
                    sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'>" + item.FollowupDate.ToShortDateString() + "</td>");
                    sb.Append("             </tr>");
                }
                sb.Append("             <tr>");
                sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;'>Created By:</td>");
                sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'>" + item.CreatedByUsername + "</td>");
                sb.Append("             </tr>");
                sb.Append("             <tr>");
                sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;'>Related To:</td>");
                sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'>" + item.RelatedToContactPerson + "</td>");
                sb.Append("             </tr>");
                sb.Append("             <tr>");
                sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;'>Primary Phone:</td>");
                sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'><a href='tel:" + item.PrimaryContactNo + "'>" + item.PrimaryContactNoFormatted + "</a></td>");
                sb.Append("             </tr>");
                sb.Append("             <tr>");
                sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;'>Primary Email:</td>");
                sb.Append("                 <td style='padding:5px 10px; background-color:#fff;'><a href='mailto:" + item.PrimaryEmailAddress + "'>" + item.PrimaryEmailAddressFormatted + "</a></td>");
                sb.Append("             </tr>");
                sb.Append("             <tr>");
                sb.Append("                 <td style='padding:5px; font-weight:bold; background-color:#fff;'>Comment:</td>");
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
            using (var prospecting = new ProspectingDataContext())
            {
                Guid userGuid = Guid.Parse(user_guid);
                results.Username = GetBossUsername(userGuid);
                List<long?> parentIds = prospecting.activity_logs.Where(a => a.parent_activity_id != null).Select(a => a.parent_activity_id).ToList();

                var todaysFollowups = prospecting.activity_logs.Where(act => act.allocated_to == userGuid &&
                                                                                act.followup_date != null &&
                                                                                act.followup_date.Value.Date == DateTime.Today).ToList();
                todaysFollowups = todaysFollowups.Where(act => !parentIds.Contains(act.activity_log_id)).ToList();
                todaysFollowups = todaysFollowups.OrderByDescending(act => act.followup_date).ToList();
                foreach (var item in todaysFollowups)
                {
                    ProspectingFollowup followup = LoadFollowup(item);
                    results.TodaysFollowups.Add(followup);
                }

                var futureFollowups = prospecting.activity_logs.Where(act => act.allocated_to == userGuid &&
                                                                                act.followup_date != null &&
                                                                                act.followup_date.Value.Date > DateTime.Today).ToList();
                futureFollowups = futureFollowups.Where(act => !parentIds.Contains(act.activity_log_id)).ToList();
                futureFollowups = futureFollowups.OrderByDescending(act => act.followup_date).ToList();
                foreach (var item in futureFollowups)
                {
                    ProspectingFollowup followup = LoadFollowup(item);
                    results.FutureDatedFollowups.Add(followup);
                }

                var unactionedFollowups = prospecting.activity_logs.Where(act => act.allocated_to == userGuid &&
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
            followup.FollowupDate = item.followup_date.Value;
            followup.PropertyAddress = GetFormattedAddress(item.lightstone_property_id);
            followup.CreatedByUsername = GetBossUsername(item.created_by);
            followup.Comment = item.comment;
            followup.RelatedToContactPerson = GetContactPersonName(item.contact_person_id);
            var primaryContactNoKvp = GetContactsPrimaryPhoneNo(item.contact_person_id);
            followup.PrimaryContactNo = primaryContactNoKvp.Key;
            followup.PrimaryContactNoFormatted = primaryContactNoKvp.Value;
            var primaryEmailKvp = GetContactsPrimaryEmailAddress(item.contact_person_id);
            followup.PrimaryEmailAddress = primaryEmailKvp.Key;
            followup.PrimaryEmailAddressFormatted = primaryEmailKvp.Value;
            followup.ActivityTypeName = item.activity_type != null ? item.activity_type.activity_name : "n/a";
            followup.FollowupActivityTypeName = item.activity_followup_type != null ? item.activity_followup_type.activity_name : "n/a";

            return followup;
        }

        private static KeyValuePair<string, string> GetContactsPrimaryEmailAddress(int? contact_person_id)
        {
            if (contact_person_id == null)
                return new KeyValuePair<string, string>("n/a", "n/a");

            using (var prospecting = new ProspectingDataContext())
            {
                var target = prospecting.prospecting_contact_persons.First(cp => cp.contact_person_id == contact_person_id);
                var primaryContactDetail = target.prospecting_contact_details.FirstOrDefault(cd => new[] { 4, 5 }.Contains(cd.contact_detail_type) && cd.is_primary_contact && !cd.deleted);

                return primaryContactDetail != null ? 
                     new KeyValuePair<string, string>(primaryContactDetail.contact_detail, string.Concat(primaryContactDetail.contact_detail, " (", primaryContactDetail.prospecting_contact_detail_type.type_desc, ")"))
                     :  new KeyValuePair<string, string>("n/a", "n/a");
            }
        }

        private static KeyValuePair<string, string> GetContactsPrimaryPhoneNo(int? contact_person_id)
        {
            if (contact_person_id == null)
                return new KeyValuePair<string, string>("n/a", "n/a");

            using (var prospecting = new ProspectingDataContext())
            {
                var target = prospecting.prospecting_contact_persons.First(cp => cp.contact_person_id == contact_person_id);
                var primaryContactDetail = target.prospecting_contact_details.FirstOrDefault(cd => new[] { 1, 2, 3 }.Contains(cd.contact_detail_type) && cd.is_primary_contact && !cd.deleted);

                if (primaryContactDetail != null)
                {
                    string formattedContactDetail = string.Concat(primaryContactDetail.contact_detail.Substring(0, 3), " ", primaryContactDetail.contact_detail.Substring(3, 3), " ", primaryContactDetail.contact_detail.Substring(6, 4));
                    return new KeyValuePair<string, string>(primaryContactDetail.contact_detail, string.Concat(formattedContactDetail, " (", primaryContactDetail.prospecting_contact_detail_type.type_desc, ")"));
                }
                               
                 return new KeyValuePair<string, string>("n/a", "n/a");
            }
        }

        private static string GetContactPersonName(int? contact_person_id)
        {
            if (contact_person_id == null)
                return "n/a";

            using (var prospecting = new ProspectingDataContext())
            {
                var target = prospecting.prospecting_contact_persons.First(cp => cp.contact_person_id == contact_person_id);
                return string.Concat(target.firstname, " ", target.surname);
            }
        }

        private static string GetBossUsername(Guid created_by)
        {
            using (var boss = new bossEntities())
            {
                var target = boss.user_registration.FirstOrDefault(ur => ur.user_guid == created_by.ToString().ToLower());
                return target != null ? string.Concat(target.user_name, " ", target.user_surname) : "n/a";
            }
        }

        public static string GetFormattedAddress(int lightstonePropertyId)
        {
            using (var prospectingContext = new ProspectingDataContext())
            {
                var property = prospectingContext.prospecting_properties.First(pp => pp.lightstone_property_id == lightstonePropertyId);
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