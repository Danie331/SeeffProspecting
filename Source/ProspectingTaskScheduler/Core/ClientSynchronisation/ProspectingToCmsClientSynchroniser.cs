using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ProspectingTaskScheduler.Core.Housekeeping;

namespace ProspectingTaskScheduler.Core.ClientSynchronisation
{
    public class ProspectingToCmsClientSynchroniser
    {

        // 1. Add new records to CMS based on ID number match (insert as client system ID 2), include contact details
        // 2. Update existing client header information based on ID match and differences in header fields (only for client system ID 2)
        // 4. Add contact details based on new details and 'deleted' flag = 0, deleting details from CMS?
        // 5. Ensure code is called on CMS to keep search_client table + permissions table etc up-to-date
        // 6. Rem to maintain created_by flag
        // rem to be careful about updating person_gender + firstname gets split up after first space the latter goes to middlename!!! (except for (unknown firstname))

        private static bool _synching;

        static ProspectingToCmsClientSynchroniser()
        {
            _synching = false;
        }

        public static void Synchronise()
        {
            return;
            if (_synching)
                return;

            _synching = true;

            // 1. Add new records to CMS based on ID number match (insert as client system ID 2) 
            AddNewClientsToCMS();

            // 2. Update existing client header information based on differences between prospecting and Client DB
            UpdateExistingClientsOnCMS();

            // 3. Update the CMS with new contact details in Prospecting
            AddNewContactDetailsToCMS();

            _synching = false;
        }

        private static void AddNewContactDetailsToCMS()
        {
            bool success = true;
            int recordCount = 0;
            try
            {
                using (var prospecting = new ProspectingDataContext())
                using (var clientDb = new SmartAdmin.Data.ClientModule.clientEntities())
                {
                    clientDb.Database.CommandTimeout = 360;
                    SmartAdmin.Data.DataRepository dr = new SmartAdmin.Data.DataRepository();
                    int prospectingClientSystemID = dr.GetProspectingClientSystemID();
                    string query = string.Format(@";with cte (contact_person_id, prospecting_contact_detail_id, contact_detail, contact_detail_type,intl_dialing_code_id, eleventh_digit, is_primary_contact )
                                                    as
                                                    (select 
                                                    contact_person_id, 
                                                    prospecting_contact_detail_id,
                                                    contact_detail, 
                                                    contact_detail_type,
                                                     intl_dialing_code_id, 
                                                     eleventh_digit, 
                                                     pcd.is_primary_contact 
                                                      from seeff_prospecting.dbo.prospecting_contact_detail pcd
                                                    left join client.dbo.contact_details ccd on LOWER(contact_details_value) = LOWER(concat(contact_detail, eleventh_digit))
                                                    where contact_details_value is null
	                                                      and pcd.deleted = 0
	                                                      and (ccd.deleted is null or ccd.deleted = 0)
                                                    group by contact_person_id, 
                                                    prospecting_contact_detail_id,
                                                    contact_detail ,
                                                    contact_detail_type,
                                                     intl_dialing_code_id, 
                                                     eleventh_digit, 
                                                     pcd.is_primary_contact)

                                                    select distinct contact_person_id, prospecting_contact_detail_id, contact_detail, contact_detail_type,intl_dialing_code_id, eleventh_digit, is_primary_contact 
                                                    from cte join client.dbo.search_client on fk_client_system_id = {0} and fk_system_client_id = contact_person_id", prospectingClientSystemID);
                    var resultSet = clientDb.Database.SqlQuery<ProspectingContactDetailRecord>(query).ToList();
                    foreach (var item in resultSet)
                    {
                        string contactValue = item.contact_detail + (item.eleventh_digit.HasValue ? item.eleventh_digit.ToString() : "");
                        int contactDetailType = MapToClientContactDetailType(item.contact_detail_type);
                        int contactDetailLocationType = MapToClientContactDetailLocationType(item.contact_detail_type);
                        int? dialingCode = item.intl_dialing_code_id == 1 ? 205 : (int?)null;
                        int contactDetailID = item.prospecting_contact_detail_id;
                        var contactRecord = prospecting.prospecting_contact_details.First(cd => cd.prospecting_contact_detail_id == contactDetailID);
                        var createdByUser = contactRecord.prospecting_contact_person?.created_by;
                        Guid createdByUserGuid = Guid.Parse("62a85a9d-be7a-4fad-b704-a55edb1d338f");
                        if (createdByUser != null)
                        {
                            createdByUserGuid = createdByUser.Value;
                        }
                        dr.ExternalAddClientContactDetail(prospectingClientSystemID, item.contact_person_id, contactValue, contactDetailType, contactDetailLocationType, dialingCode, item.is_primary_contact, createdByUserGuid);
                        recordCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex, "AddNewContactDetailsToCMS()");
                SendEmailReport(ex.ToString(), "AddNewContactDetailsToCMS()");
                success = false;
            }

            if (success)
            {
                LogSucess("AddNewContactDetailsToCMS() completed successfully: " + recordCount + " records added.");
            }
        }

        private static void UpdateExistingClientsOnCMS()
        {
            bool success = true;
            int recordCount = 0;
            try
            {
                using (var prospecting = new ProspectingDataContext())
                using (var clientDb = new SmartAdmin.Data.ClientModule.clientEntities())
                {
                    SmartAdmin.Data.DataRepository dr = new SmartAdmin.Data.DataRepository();
                    int prospectingClientSystemID = dr.GetProspectingClientSystemID();
                    string query = string.Format(@";with cte (contact_person_id, person_title, person_gender, firstname, middle_name, surname, identity_number)
                                                as
	                                        (select sc.fk_system_client_id,
	                                          (case c.title
		                                        when 2 then 1 
		                                        when 3 then 2
		                                        when 4 then 3
		                                        when 1 then 4
		                                        when null then null
		                                        end) person_title,
		                                        (case c.gender
		                                           when 1 then 'M'
		                                           when 2 then 'F'
		                                           else ''
		                                           end) person_gender, -- special case here ignore if null
		                                          c.first_name firstname,
		                                          c.middle_name,
		                                          c.last_name surname,
		                                          c.identity_number
		                                         from client.dbo.client c 
		                                        join client.dbo.search_client sc on c.pk_client_id = sc.fk_client_id and sc.fk_client_system_id = {0})

                                        select distinct pcp.contact_person_id
                                        from seeff_prospecting.dbo.prospecting_contact_person pcp
                                        join cte c on c.contact_person_id = pcp.contact_person_id
                                        where isnull(c.person_title,0) != isnull(pcp.person_title,0) OR 
	                                          c.person_gender != pcp.person_gender OR
	                                          concat(ltrim(rtrim(c.firstname)), ' ', ltrim(rtrim(c.middle_name)))  != ltrim(rtrim(pcp.firstname)) OR
	                                          ltrim(rtrim(c.surname)) != ltrim(rtrim(pcp.surname)) OR
	                                          c.identity_number != pcp.id_number", prospectingClientSystemID);

                    var resultSet = clientDb.Database.SqlQuery<int>(query);
                    foreach (var contactPersonID in resultSet)
                    {
                        var contactRecord = prospecting.prospecting_contact_persons.First(cp => cp.contact_person_id == contactPersonID);
                        int? title = MapToClientTitle(contactRecord.person_title);
                        int? gender = MapToClientGender(contactRecord.person_gender);
                        string firstName = null;
                        string surname = contactRecord.surname;
                        string middleName = null;
                        var createdByUser = contactRecord.created_by;
                        if (!string.IsNullOrWhiteSpace(contactRecord.firstname))
                        {
                            var namePair = contactRecord.firstname.Split(new[] { ' ' });
                            firstName = namePair[0];
                            if (namePair.Length == 2)
                            {
                                middleName = namePair[1];
                            }
                        }
                        Guid createdByUserGuid = Guid.Parse("62a85a9d-be7a-4fad-b704-a55edb1d338f");
                        if (createdByUser != null)
                        {
                            createdByUserGuid = createdByUser.Value;
                        }
                        dr.ExternalSaveClient(prospectingClientSystemID, contactPersonID, title, gender, firstName, middleName, surname, contactRecord.id_number, createdByUserGuid);
                        recordCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex, "UpdateExistingClientsOnCMS()");
                SendEmailReport(ex.ToString(), "UpdateExistingClientsOnCMS()");
                success = false;
            }

            if (success)
            {
                LogSucess("UpdateExistingClientsOnCMS() completed successfully: " + recordCount + " existing clients updated.");
            }
        }

        private static void AddNewClientsToCMS()
        {
            bool success = true;
            int recordCount = 0;
            try
            {
                using (var prospecting = new ProspectingDataContext())
                using (var clientDb = new SmartAdmin.Data.ClientModule.clientEntities())
                {
                    SmartAdmin.Data.DataRepository dr = new SmartAdmin.Data.DataRepository();
                    int prospectingClientSystemID = dr.GetProspectingClientSystemID();
                    // Find records in Prospecting that don't have a matching ID number in the CMS
                    string deltaQuery = @"select contact_person_id from seeff_prospecting.dbo.prospecting_contact_person
                                            left join client.dbo.client on id_number = identity_number
                                            where identity_number is null and (deleted is null or deleted = 0)";
                    var resultSet = clientDb.Database.SqlQuery<int>(deltaQuery).ToList();
                    foreach (var contactPersonID in resultSet)
                    {
                        var contactRecord = prospecting.prospecting_contact_persons.First(cp => cp.contact_person_id == contactPersonID);
                        int? title = MapToClientTitle(contactRecord.person_title);
                        int? gender = MapToClientGender(contactRecord.person_gender);
                        string firstName = null;
                        string surname = contactRecord.surname;
                        string middleName = null;
                        var createdByUser = contactRecord.created_by;
                        if (!string.IsNullOrWhiteSpace(contactRecord.firstname))
                        {
                            var namePair = contactRecord.firstname.Split(new[] { ' ' });
                            firstName = namePair[0];
                            if (namePair.Length == 2)
                            {
                                middleName = namePair[1];
                            }
                        }
                        Guid createdByUserGuid = Guid.Parse("62a85a9d-be7a-4fad-b704-a55edb1d338f");
                        if (createdByUser != null)
                        {
                            createdByUserGuid = createdByUser.Value;
                        }
                        dr.ExternalSaveClient(prospectingClientSystemID, contactPersonID, title, gender, firstName, middleName, surname, contactRecord.id_number, createdByUserGuid);
                        recordCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex, "AddNewClientsToCMS()");
                SendEmailReport(ex.ToString(), "AddNewClientsToCMS()");
                success = false;
            }

            if (success)
            {
                LogSucess("AddNewClientsToCMS() completed successfully: " + recordCount + " new clients added.");
            }
        }

        private static int MapToClientContactDetailType(int contact_detail_type)
        {
            switch (contact_detail_type)
            {
                case 1:
                case 2:
                    return 2;
                case 3:
                    return 3;
                case 4:
                case 5:
                    return 1;
                case 6:
                    return 4;
            }

            throw new Exception("Invalid contact_detail_type from Prospecting");
        }

        private static int MapToClientContactDetailLocationType(int contact_detail_type)
        {
            switch(contact_detail_type)
            {
                case 1:
                    return 1;
                case 2:
                case 5:
                    return 2;
                case 3:
                    return 3;
                case 4:
                    return 3;
                case 6:
                    return 2; // Check?
            }

            throw new Exception("Invalid contact_detail_type from Prospecting");
        }

        private static int? MapToClientGender(string personGender)
        {
            switch (personGender)
            {
                case "M": return 1;
                case "F": return 2;
                default: return null;
            }
        }

        private static int? MapToClientTitle(int? personTitle)
        {
            if (personTitle == null) return null;

            switch (personTitle)
            {
                case 1: return 2;
                case 2: return 3;
                case 3: return 4;
                case 4: return 1;
                default: return null;
            }
        }

        private static void LogException(Exception  ex, string methodName)
        {
            using (var prospectingDb = new ProspectingDataContext())
            {
                var errorRec = new exception_log
                {
                    friendly_error_msg = "Error occurred in ProspectingToCmsClientSynchroniser." + methodName,
                    exception_string = ex.ToString(),
                    user = new Guid(),
                    date_time = DateTime.Now
                };
                prospectingDb.exception_logs.InsertOnSubmit(errorRec);
                prospectingDb.SubmitChanges();
            }
        }
        private static void LogSucess(string methodName)
        {
            using (var prospectingDb = new ProspectingDataContext())
            {
                var errorRec = new exception_log
                {
                    friendly_error_msg = "Successfully completed complete step in ProspectingToCmsClientSynchroniser." + methodName,
                    exception_string = "",
                    user = new Guid(),
                    date_time = DateTime.Now
                };
                prospectingDb.exception_logs.InsertOnSubmit(errorRec);
                prospectingDb.SubmitChanges();
            }
        }

        public static void SendEmailReport(string message, string methodName)
        {
            string emailToAddress = "danie.vdm@seeff.com";
            string emailDisplayName = "ProspectingTaskScheduler";
            string emailFromAddress = "reports@seeff.com";
            string emaiLSubject = "Exception occurred in ProspectingToCmsClientSynchroniser." + methodName;

            //StatusNotifier.SendEmail(emailToAddress, emailDisplayName, emailFromAddress, null, emaiLSubject, message);
        }
    }

    public class ProspectingContactDetailRecord
    {
        public int contact_person_id { get; set; }
        public string contact_detail { get; set; }
        public int contact_detail_type { get; set; }
        public int? intl_dialing_code_id { get; set; }
        public int? eleventh_digit { get; set; }
        public bool is_primary_contact { get; set; }

        public int prospecting_contact_detail_id { get; set; }
    }
}