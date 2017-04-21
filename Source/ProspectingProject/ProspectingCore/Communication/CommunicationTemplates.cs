using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Web;

namespace ProspectingProject
{
    public sealed partial class ProspectingCore
    {
        // Retrieving, adding + updating, deleting templates. For which comm_type, user defined or system, for which activity type.
        public static CommunicationTemplate GetTemplate(CommTemplateRequest input)
        {
            CommunicationTemplate result = null;
            if (input.IsFromUrl.HasValue && input.IsFromUrl == true)
            {
                string url = input.URL;
                string content = new HttpClient().GetStringAsync(url).Result;
                result = new CommunicationTemplate
                {
                    TemplateContent = HttpContext.Current.Server.HtmlEncode(content),
                    TemplateName = "Seeff Newsletter - " + DateTime.Today.ToString("MMMM yyyy"),
                    TemplateActivityTypeId = ProspectingLookupData.SystemActivityTypes.First(act => act.Value == "Newsletter").Key
                };
                return result;
            } 
            using (var prospectingDB = new ProspectingDataContext())
            {
                switch (input.IsSystemTemplate)
                {
                    case true:                                                                                // Maybe use activity_type_id only?              
                        var systemRecord = prospectingDB.system_communication_templates.FirstOrDefault(sct => sct.communication_type == input.CommunicationType &&
                                                                                                                sct.template_name == input.TemplateName &&
                                                                                                                sct.active);
                        if (systemRecord != null)
                        {
                            result = new CommunicationTemplate
                            {
                                 TemplateName = systemRecord.template_name,
                                TemplateContent = systemRecord.template_content,
                                 ActivityName = systemRecord.activity_type.activity_name,
                                 TemplateActivityTypeId = systemRecord.activity_type_id
                            };
                        }
                        break;
                    case false:
                        UserDataResponsePacket user = RequestHandler.GetUserSessionObject();
                        var userRecord = prospectingDB.user_communication_templates.FirstOrDefault(uct => uct.created_by == user.UserGuid &&
                                                                                                        uct.communication_type == input.CommunicationType && 
                                                                                                        uct.template_name == input.TemplateName && 
                                                                                                        !uct.deleted);
                        if (userRecord != null)
                        {
                            result = new CommunicationTemplate
                            {
                                TemplateName = userRecord.template_name,
                                TemplateContent = userRecord.template_content,
                                TemplateActivityTypeId = userRecord.activity_type_id.HasValue ? userRecord.activity_type_id.Value : ProspectingLookupData.SystemActivityTypes.First(act => act.Value == "General (comm)").Key
                            };
                        }
                        break;
                }

                return result;
            }
        }

        private static string ConvertFromB64(string encodedString)
        {
            byte[] data = Convert.FromBase64String(encodedString);
            string decodedString = Encoding.UTF8.GetString(data);
            return decodedString;
        }
        
        // This will always apply to user defined templates. System templates to be added manually.
        public static void AddOrUpdateTemplate(CommTemplateRequest input)
        {
            UserDataResponsePacket user = RequestHandler.GetUserSessionObject();
            using (var prospectingDB = new ProspectingDataContext())
            {
                user_communication_template record = prospectingDB.user_communication_templates.FirstOrDefault(uct => uct.created_by == user.UserGuid &&
                                                                                                                      uct.template_name == input.TemplateName &&
                                                                                                                      uct.communication_type == input.CommunicationType);

                int commGeneralTypeId = ProspectingLookupData.SystemActivityTypes.First(act => act.Value == "General (comm)").Key;
                input.TemplateContent = ConvertFromB64(input.TemplateContent);
                if (record != null)
                {
                    record.updated_date = DateTime.Now;
                    record.template_content = input.TemplateContent;
                    record.deleted = false;
                    record.activity_type_id = input.ActivityTypeId.HasValue ? input.ActivityTypeId.Value : record.activity_type_id;
                }
                else
                {
                    record = new user_communication_template
                    {
                        created_by = user.UserGuid,
                        created_date = DateTime.Now,
                        template_content = input.TemplateContent,
                        template_name = input.TemplateName,
                        communication_type = input.CommunicationType,
                        activity_type_id = input.ActivityTypeId.HasValue ? input.ActivityTypeId.Value : commGeneralTypeId
                    };
                    prospectingDB.user_communication_templates.InsertOnSubmit(record);
                }

                prospectingDB.SubmitChanges();
            }
        }

        public static void DeleteTemplate(CommTemplateRequest input) 
        {
            UserDataResponsePacket user = RequestHandler.GetUserSessionObject();
            using (var prospectingDB = new ProspectingDataContext())
            {
                user_communication_template record = prospectingDB.user_communication_templates.FirstOrDefault(uct => uct.created_by == user.UserGuid &&
                                                                                                                      uct.template_name == input.TemplateName &&
                                                                                                                         uct.communication_type == input.CommunicationType);
                if (record != null)
                {
                    record.deleted = true;
                }

                prospectingDB.SubmitChanges();
            }
        }

        public static List<CommunicationTemplate> GetListOfUserTemplates(CommTemplateRequest input)
        {
            List<CommunicationTemplate> results = new List<CommunicationTemplate>();
            UserDataResponsePacket user = RequestHandler.GetUserSessionObject();
            using (var prospectingDB = new ProspectingDataContext())
            {
                var records = prospectingDB.user_communication_templates.Where(uct => uct.created_by == user.UserGuid &&
                                                                                                        uct.communication_type == input.CommunicationType &&
                                                                                                        !uct.deleted);
                foreach (var record in records)
                {
                    CommunicationTemplate result = new CommunicationTemplate
                    {
                        TemplateName = record.template_name,
                        //TemplateContent = record.template_content
                    };
                    results.Add(result);
                }
            }
            return results;
        }

        public static List<CommunicationTemplate> GetListOfSystemTemplates(CommTemplateRequest input) 
        {
            List<CommunicationTemplate> results = new List<CommunicationTemplate>();
            using (var prospectingDB = new ProspectingDataContext())
            {
                var records = prospectingDB.system_communication_templates.Where(sct => sct.communication_type == input.CommunicationType && sct.active);

                foreach (var record in records)
                {
                    CommunicationTemplate result = new CommunicationTemplate
                    {
                        TemplateName = record.template_name,
                        //TemplateContent = record.template_content
                    };
                    results.Add(result);
                }
            }
            return results;
        }
    }
}