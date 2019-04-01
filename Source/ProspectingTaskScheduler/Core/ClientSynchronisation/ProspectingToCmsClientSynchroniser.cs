using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Hangfire;
using ProspectingTaskScheduler.Core.Housekeeping;

namespace ProspectingTaskScheduler.Core.ClientSynchronisation
{
    //public class ProspectingToCmsClientSynchroniser
    //{

    //    // 1. Add new records to CMS based on ID number match (insert as client system ID 2), include contact details
    //    // 2. Update existing client header information based on ID match and differences in header fields (only for client system ID 2)
    //    // 4. Add contact details based on new details and 'deleted' flag = 0, deleting details from CMS?
    //    // 5. Ensure code is called on CMS to keep search_client table + permissions table etc up-to-date
    //    // 6. Rem to maintain created_by flag
    //    // rem to be careful about updating person_gender + firstname gets split up after first space the latter goes to middlename!!! (except for (unknown firstname))

    //    private static bool _synching;

    //    static ProspectingToCmsClientSynchroniser()
    //    {
    //        _synching = false;
    //    }

    //    public static async Task AddClientSynchronisationRequest(int contactPersonID, Guid? user)
    //    {
    //        try
    //        {
    //            using (var prospecting = new seeff_prospectingEntities())
    //            {
    //                var clientSyncRecord = new client_sync_log
    //                {
    //                    contact_person_id = contactPersonID,
    //                    user_guid = user,
    //                    date_time = DateTime.Now
    //                };
    //                prospecting.client_sync_log.Add(clientSyncRecord);
    //                await prospecting.SaveChangesAsync();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // Log
    //        }
    //    }

    //    [AutomaticRetry(Attempts = 0)]
    //    public static void Synchronise(IJobCancellationToken cancellationToken)
    //    {
    //        if (_synching)
    //            return;

    //        _synching = true;

    //        UpdateInsertClients(cancellationToken);


    //        // 1. Add new records to CMS based on ID number match (insert as client system ID 2) 
    //        //AddNewClientsToCMS();

    //        // 2. Update existing client header information based on differences between prospecting and Client DB
    //        //UpdateExistingClientsOnCMS();

    //        // 3. Update the CMS with new contact details in Prospecting
    //        //AddNewContactDetailsToCMS();

    //        _synching = false;
    //    }

    //    private static void UpdateInsertClients(IJobCancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            var clientPhoneTypes = RetrieveClientPhoneTypes();
    //            var clientEmailTypes = RetrieveClientEmailTypes();
    //            using (var prospecting = new seeff_prospectingEntities())
    //            {
    //                var clientUpdateBatch = prospecting.client_sync_log.Where(cl => !cl.synchronised).Take(30).ToList();
    //                foreach (var clientToSync in clientUpdateBatch)
    //                {
    //                    cancellationToken.ThrowIfCancellationRequested();

    //                    if (UpdateOrInsertClient(clientToSync, clientPhoneTypes, clientEmailTypes))
    //                    {
    //                        clientToSync.synchronised = true;
    //                        prospecting.SaveChanges();
    //                    }
    //                }
    //            }
    //        }
    //        catch (OperationCanceledException)
    //        {
    //            // Suppress and return as the job will be retried during its next scheduled run.
    //            return;
    //        }
    //        catch (Exception ex)
    //        {
    //            LogException(ex, "UpdateInsertClients()");
    //            SendEmailReport(ex.ToString(), "UpdateInsertClients()");
    //        }
    //    }

    //    private static List<int> RetrieveClientEmailTypes()
    //    {
    //        using (var client = new clientEntities())
    //        {
    //            List<int> emailTypeIDs = client.contact_details_type.Where(cdt => new[] { "Email" }.Contains(cdt.type_description)).Select(cdt => cdt.pk_contact_details_type_id).ToList();
    //            return emailTypeIDs;
    //        }
    //    }

    //    private static List<int> RetrieveClientPhoneTypes()
    //    {
    //        using (var client = new clientEntities())
    //        {
    //            List<int> phoneTypeIDs = client.contact_details_type.Where(cdt => new[] { "Landline", "Mobile" }.Contains(cdt.type_description)).Select(cdt => cdt.pk_contact_details_type_id).ToList();
    //            return phoneTypeIDs;
    //        }
    //    }

    //    private static bool UpdateOrInsertClient(client_sync_log prospectingContactToSync, List<int> clientPhoneTypes, List<int> clientEmailTypes)
    //    {
    //        try
    //        {
    //            using (var prospecting = new seeff_prospectingEntities())
    //            {
    //                int prospectingContactRecordId = prospectingContactToSync.contact_person_id;
    //                var prospectingContactRecord = prospecting.prospecting_contact_person.First(cp => cp.contact_person_id == prospectingContactRecordId);
    //                UserContext user = GetUserContextFromGuid(prospectingContactToSync.user_guid);
    //                int clientClientRecordId = -1;
    //                using (var client = new clientEntities())
    //                {
    //                    var existingClientRelationship = client.client_system_relationship.Where(c => c.fk_client_system_id == 2).FirstOrDefault(c => c.fk_system_client_id == prospectingContactRecordId);

    //                    if (existingClientRelationship != null)
    //                    {
    //                        // Client exists in the CMS so update their info
    //                        var clientRecord = client.client.FirstOrDefault(c => c.pk_client_id == existingClientRelationship.fk_client_id);
    //                        if (clientRecord == null)
    //                            throw new Exception("Critical Error in client sync method UpdateOrInsertClient(...) -> client relationship found but no corresponding client in client table!");

    //                        clientRecord.title = MapToClientTitle(prospectingContactRecord.person_title);
    //                        clientRecord.gender = MapToClientGender(prospectingContactRecord.person_gender);
    //                        clientRecord.first_name = prospectingContactRecord.firstname;
    //                        clientRecord.last_name = prospectingContactRecord.surname;
    //                        clientRecord.identity_number = prospectingContactRecord.id_number;
    //                        clientRecord.date_of_birth = GetDateOfBirthFromID(prospectingContactRecord.id_number);
    //                        clientRecord.updated_date = DateTime.Now;
    //                        clientRecord.popi_opt_in = !prospectingContactRecord.is_popi_restricted;
    //                        clientRecord.phone_call_opt_in = !prospectingContactRecord.do_not_contact;
    //                        clientRecord.email_opt_in = !prospectingContactRecord.optout_emails;
    //                        clientRecord.sms_opt_in = !prospectingContactRecord.optout_sms;

    //                        var prospectingContactDetails = prospectingContactRecord.prospecting_contact_detail;
    //                        foreach (var prospectingContactDetail in prospectingContactDetails)
    //                        {
    //                            AddContactDetailToClient(prospectingContactToSync, prospectingContactDetail, clientRecord, user);
    //                        }

    //                        client.SaveChanges();
    //                        clientClientRecordId = clientRecord.pk_client_id;
    //                    }
    //                    else
    //                    {
    //                        // Create a new client, relationship and add to CMS
    //                        client newRecord = new ProspectingTaskScheduler.client
    //                        {
    //                            title = MapToClientTitle(prospectingContactRecord.person_title),
    //                            gender = MapToClientGender(prospectingContactRecord.person_gender),
    //                            first_name = prospectingContactRecord.firstname,
    //                            last_name = prospectingContactRecord.surname,
    //                            identity_number = prospectingContactRecord.id_number,
    //                            date_of_birth = GetDateOfBirthFromID(prospectingContactRecord.id_number),
    //                            updated_date = DateTime.Now,
    //                            popi_opt_in = !prospectingContactRecord.is_popi_restricted,
    //                            phone_call_opt_in = !prospectingContactRecord.do_not_contact,
    //                            email_opt_in = !prospectingContactRecord.optout_emails,
    //                            sms_opt_in = !prospectingContactRecord.optout_sms
    //                        };

    //                        client.client.Add(newRecord);
    //                        client.SaveChanges();

    //                        var prospectingContactDetails = prospectingContactRecord.prospecting_contact_detail;
    //                        foreach (var prospectingContactDetail in prospectingContactDetails)
    //                        {
    //                            AddContactDetailToClient(prospectingContactToSync, prospectingContactDetail, newRecord, user);
    //                        }

    //                        client.SaveChanges();
    //                        clientClientRecordId = newRecord.pk_client_id;

    //                        var newRelationship = new client_system_relationship { fk_client_id = clientClientRecordId, fk_client_system_id = 2, fk_system_client_id = prospectingContactRecordId, created_date = DateTime.Now };
    //                        client.client_system_relationship.Add(newRelationship);
    //                        client.SaveChanges();
    //                    }
    //                }
    //                UpdateClientDependencies(clientClientRecordId, prospectingContactRecordId, clientPhoneTypes, clientEmailTypes, user);
    //            }

    //            return true;
    //        }
    //        catch
    //        {
    //            throw;               
    //        }
    //    }

    //    private static UserContext GetUserContextFromGuid(Guid? user_guid)
    //    {
    //        if (!user_guid.HasValue)
    //        {
    //            return null;
    //        }

    //        using (var boss = new bossEntities())
    //        {
    //            var targetRecord = boss.user_registration.FirstOrDefault(u => u.user_guid.ToLower() == user_guid.ToString().ToLower());
    //            if (targetRecord == null)
    //            {
    //                return null;
    //            }

    //            int registrationID = (int)targetRecord.registration_id;
    //            int branchId = targetRecord.branch_id;
    //            int licenseID = (from lb in boss.license_branches
    //                             join lic in boss.license on lb.license_id equals lic.license_id
    //                             where lb.branch_id == branchId
    //                             select lic.license_id).Distinct().SingleOrDefault();

    //            return new UserContext
    //            {
    //                RegistrationID = registrationID,
    //                BranchID = branchId,
    //                LicenseID = licenseID
    //            };
    //        }
    //    }

    //    private static void UpdateClientDependencies(int pk_client_id, int prospectingContactRecordId, List<int> clientPhoneTypes, List<int> clientEmailTypes, UserContext user)
    //    {
    //        using (var clientDB = new clientEntities())
    //        {
    //            if (clientDB.Database.Connection.State != System.Data.ConnectionState.Open)
    //            {
    //                clientDB.Database.Connection.Open();
    //            }
    //            clientDB.Database.ExecuteSqlCommand(@"DELETE FROM client.dbo.search_client WHERE fk_client_id = " + pk_client_id);

    //            Func<int, int, string> getUpdateQuery = (csID, scID) => string.Format(@"INSERT client.[dbo].[search_client] ([fk_client_id], [fk_client_system_id], [fk_system_client_id], [first_name], [last_name], [middle_name], [preferred_name], [identity_number], [passport_number], [date_of_birth], [phone], [email], is_primary_contact_no, is_primary_email)
    //                                                    SELECT DISTINCT
	   //                                                     c.[pk_client_id],
	   //                                                     {0},
	   //                                                     {4},  
	   //                                                     c.[first_name],
	   //                                                     c.[last_name],
	   //                                                     c.[middle_name],
	   //                                                     c.[preferred_name],
	   //                                                     c.[identity_number],
	   //                                                     c.[passport_number],
	   //                                                     c.[date_of_birth],
	   //                                                     (select case when cd.fk_contact_details_type_id in ({1}) then cd.contact_details_value else null end),
	   //                                                     (select case when cd.fk_contact_details_type_id in ({2}) then cd.[contact_details_value] else null end),
	   //                                                     (select case when cd.fk_contact_details_type_id in ({1}) then cd.is_primary_contact else null end),
	   //                                                     (select case when cd.fk_contact_details_type_id in ({2}) then cd.is_primary_contact else null end)
    //                                                    FROM client.dbo.client c 
    //                                                    LEFT JOIN client.dbo.contact_details cd on cd.fk_client_id = c.pk_client_id and cd.is_active_status = 1
    //                                                    WHERE c.[pk_client_id] = {3} and (c.deleted IS NULL OR c.deleted = 0)", csID,
    //                                                                                        string.Join(",", clientPhoneTypes),
    //                                                                                        string.Join(",", clientEmailTypes),
    //                                                                                        pk_client_id,
    //                                                                                        scID);

    //            var clientSystems = clientDB.client_system_relationship.Where(csr => csr.fk_client_id == pk_client_id).ToList();
    //            if (clientSystems.Any())
    //            {
    //                foreach (var clientSystem in clientSystems)
    //                {
    //                    // a relationship already exists
    //                    string inputQuery = getUpdateQuery(clientSystem.fk_client_system_id, clientSystem.fk_system_client_id);
    //                    clientDB.Database.ExecuteSqlCommand(inputQuery);
    //                }
    //            }

    //            // client types
    //            using (var prospecting = new seeff_prospectingEntities())
    //            {
    //                var contactPropertyRelationships = prospecting.prospecting_person_property_relationship.Where(ppr => ppr.contact_person_id == prospectingContactRecordId);
    //                var personPropertyRelationshipTypes = contactPropertyRelationships.Select(pprt => pprt.relationship_to_property).Distinct().ToList();
    //                if (personPropertyRelationshipTypes.Any())
    //                {
    //                    foreach (var item in personPropertyRelationshipTypes)
    //                    {
    //                        int? clientRelationshipTypeID = MapToClientType(item);
    //                        if (clientRelationshipTypeID.HasValue)
    //                        {
    //                            var recordExists = clientDB.client_type.FirstOrDefault(ct => ct.fk_client_id == pk_client_id && ct.fk_client_type_description_id == clientRelationshipTypeID.Value);
    //                            if (recordExists == null)
    //                            {
    //                                client_type newType = new client_type
    //                                {
    //                                    fk_client_id = pk_client_id,
    //                                    fk_client_type_description_id = clientRelationshipTypeID.Value,
    //                                    created_by = user != null ? user.RegistrationID : -1,
    //                                    created_date = DateTime.Now,
    //                                    is_active_status = true
    //                                };
    //                                clientDB.client_type.Add(newType);
    //                                clientDB.SaveChanges();
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            // contact sources
    //            int contactSourceType = 3;
    //            var contactSourceExists = clientDB.client_contact_source.FirstOrDefault(ccs => ccs.fk_client_id == pk_client_id && ccs.fk_contact_source_type == contactSourceType);
    //            if (contactSourceExists == null)
    //            {
    //                client_contact_source newRecord = new client_contact_source
    //                {
    //                    fk_client_id = pk_client_id,
    //                    fk_contact_source_type = contactSourceType,
    //                    created_by = user != null ? user.RegistrationID : -1,
    //                    created_date = DateTime.Now,
    //                };
    //                clientDB.client_contact_source.Add(newRecord);
    //                clientDB.SaveChanges();
    //            }

    //            if (user != null)
    //            {
    //                // update client_user table if needed
    //                int userRegistrationID = user.RegistrationID;
    //                bool clientExists = clientDB.client_user.Any(cu => cu.fk_client_id == pk_client_id);
    //                if (!clientExists)
    //                {
    //                    client_user newRecord = new client_user { fk_client_id = pk_client_id, fk_user_id = userRegistrationID, created_date = DateTime.Now };
    //                    clientDB.client_user.Add(newRecord);
    //                    clientDB.SaveChanges();
    //                }

    //                // update client_branch table if needed
    //                int userBranchID = user.BranchID;
    //                clientExists = clientDB.client_branch.Any(cb => cb.fk_client_id == pk_client_id);
    //                if (!clientExists)
    //                {
    //                    client_branch newRecord = new client_branch { fk_client_id = pk_client_id, fk_branch_id = userBranchID, created_date = DateTime.Now };
    //                    clientDB.client_branch.Add(newRecord);
    //                    clientDB.SaveChanges();
    //                }

    //                // update client_license table if needed
    //                int userLicenseID = user.LicenseID;
    //                clientExists = clientDB.client_license.Any(cl => cl.fk_client_id == pk_client_id);
    //                if (!clientExists)
    //                {
    //                    client_license newRecord = new client_license { fk_client_id = pk_client_id, fk_license_id = userLicenseID, created_date = DateTime.Now };
    //                    clientDB.client_license.Add(newRecord);
    //                    clientDB.SaveChanges();
    //                }
    //            }
    //        }
    //    }

    //    private static int? MapToClientType(int item)
    //    {
    //        switch (item)
    //        {
    //            case 1: return 1;
    //            case 2: return 2;
    //            default: return null;
    //        }
    //    }

    //    private static void AddContactDetailToClient(client_sync_log clientToSync, prospecting_contact_detail contactDetail, client clientRecord, UserContext user)
    //    {
    //        var existingDetail = clientRecord.contact_details.FirstOrDefault(cd =>
    //        {
    //            if (cd.contact_details_value != null)
    //            {
    //                return cd.contact_details_value.ToLower() == contactDetail.contact_detail.ToLower();
    //            }
    //            return false;
    //        });

    //        if (existingDetail == null)
    //        {
    //            clientRecord.contact_details.Add(new contact_details
    //            {
    //                fk_contact_details_type_id = MapToClientContactDetailType(contactDetail.contact_detail_type),
    //                fk_contact_details_location_type_id = MapToClientContactDetailLocationType(contactDetail.contact_detail_type),
    //                contact_details_value = contactDetail.contact_detail,
    //                is_primary_contact = contactDetail.is_primary_contact,
    //                created_by = user != null ? user.RegistrationID : -1,
    //                created_date = DateTime.Now,
    //                is_active_status = true,
    //                fk_country_code = null // TODO: fix this
    //            });
    //        }
    //    }

    //    private static bool HasValidSAIdentityNumber(string idNumber)
    //    {
    //        int d = -1;
    //        try
    //        {
    //            if (string.IsNullOrWhiteSpace(idNumber) || idNumber.Length != 13)
    //                return false;
    //            string dob = idNumber.Substring(0, 6);
    //            DateTime dobResult;
    //            if (!DateTime.TryParseExact(dob, "yyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dobResult))
    //                return false;
    //            if (!new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }.Any(ch => ch == idNumber[6]))
    //                return false;
    //            if (!new char[] { '0', '1' }.Any(ch => ch == idNumber[10]))
    //                return false;

    //            int a = 0;
    //            for (int i = 0; i < 6; i++)
    //            {
    //                a += int.Parse(idNumber[2 * i].ToString());
    //            }
    //            int b = 0;
    //            for (int i = 0; i < 6; i++)
    //            {
    //                b = b * 10 + int.Parse(idNumber[2 * i + 1].ToString());
    //            }
    //            b *= 2;
    //            int c = 0;
    //            do
    //            {
    //                c += b % 10;
    //                b = b / 10;
    //            }
    //            while (b > 0);
    //            c += a;
    //            d = 10 - (c % 10);
    //            if (d == 10) d = 0;
    //        }
    //        catch {/*ignore*/}
    //        bool result = d != -1 && idNumber[12].ToString() == d.ToString();

    //        return result;
    //    }

    //    private static DateTime? GetDateOfBirthFromID(string idNumber)
    //    {
    //        if (!HasValidSAIdentityNumber(idNumber))
    //        {
    //            return null;
    //        }
    //        string dob = idNumber.Substring(0, 6);
    //        DateTime dobResult = DateTime.ParseExact(dob, "yyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);

    //        return dobResult;
    //    }

    //    //private static void AddNewContactDetailsToCMS()
    //    //{
    //    //    bool success = true;
    //    //    int recordCount = 0;
    //    //    try
    //    //    {
    //    //        using (var prospecting = new ProspectingDataContext())
    //    //        using (var clientDb = new SmartAdmin.Data.ClientModule.clientEntities())
    //    //        {
    //    //            clientDb.Database.CommandTimeout = 360;
    //    //            SmartAdmin.Data.DataRepository dr = new SmartAdmin.Data.DataRepository();
    //    //            int prospectingClientSystemID = dr.GetProspectingClientSystemID();
    //    //            string query = string.Format(@";with cte (contact_person_id, prospecting_contact_detail_id, contact_detail, contact_detail_type,intl_dialing_code_id, eleventh_digit, is_primary_contact )
    //    //                                            as
    //    //                                            (select 
    //    //                                            contact_person_id, 
    //    //                                            prospecting_contact_detail_id,
    //    //                                            contact_detail, 
    //    //                                            contact_detail_type,
    //    //                                             intl_dialing_code_id, 
    //    //                                             eleventh_digit, 
    //    //                                             pcd.is_primary_contact 
    //    //                                              from seeff_prospecting.dbo.prospecting_contact_detail pcd
    //    //                                            left join client.dbo.contact_details ccd on LOWER(contact_details_value) = LOWER(concat(contact_detail, eleventh_digit))
    //    //                                            where contact_details_value is null
    //    //                                               and pcd.deleted = 0
    //    //                                               and (ccd.deleted is null or ccd.deleted = 0)
    //    //                                            group by contact_person_id, 
    //    //                                            prospecting_contact_detail_id,
    //    //                                            contact_detail ,
    //    //                                            contact_detail_type,
    //    //                                             intl_dialing_code_id, 
    //    //                                             eleventh_digit, 
    //    //                                             pcd.is_primary_contact)

    //    //                                            select distinct contact_person_id, prospecting_contact_detail_id, contact_detail, contact_detail_type,intl_dialing_code_id, eleventh_digit, is_primary_contact 
    //    //                                            from cte join client.dbo.search_client on fk_client_system_id = {0} and fk_system_client_id = contact_person_id", prospectingClientSystemID);
    //    //            var resultSet = clientDb.Database.SqlQuery<ProspectingContactDetailRecord>(query).ToList();
    //    //            foreach (var item in resultSet)
    //    //            {
    //    //                string contactValue = item.contact_detail + (item.eleventh_digit.HasValue ? item.eleventh_digit.ToString() : "");
    //    //                int contactDetailType = MapToClientContactDetailType(item.contact_detail_type);
    //    //                int contactDetailLocationType = MapToClientContactDetailLocationType(item.contact_detail_type);
    //    //                int? dialingCode = item.intl_dialing_code_id == 1 ? 205 : (int?)null;
    //    //                int contactDetailID = item.prospecting_contact_detail_id;
    //    //                var contactRecord = prospecting.prospecting_contact_details.First(cd => cd.prospecting_contact_detail_id == contactDetailID);
    //    //                var createdByUser = contactRecord.prospecting_contact_person?.created_by;
    //    //                Guid createdByUserGuid = Guid.Parse("62a85a9d-be7a-4fad-b704-a55edb1d338f");
    //    //                if (createdByUser != null)
    //    //                {
    //    //                    createdByUserGuid = createdByUser.Value;
    //    //                }
    //    //                dr.ExternalAddClientContactDetail(prospectingClientSystemID, item.contact_person_id, contactValue, contactDetailType, contactDetailLocationType, dialingCode, item.is_primary_contact, createdByUserGuid);
    //    //                recordCount++;
    //    //            }
    //    //        }
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        LogException(ex, "AddNewContactDetailsToCMS()");
    //    //        SendEmailReport(ex.ToString(), "AddNewContactDetailsToCMS()");
    //    //        success = false;
    //    //    }

    //    //    if (success)
    //    //    {
    //    //        LogSucess("AddNewContactDetailsToCMS() completed successfully: " + recordCount + " records added.");
    //    //    }
    //    //}

    //    //private static void UpdateExistingClientsOnCMS()
    //    //{
    //    //    bool success = true;
    //    //    int recordCount = 0;
    //    //    try
    //    //    {
    //    //        using (var prospecting = new ProspectingDataContext())
    //    //        using (var clientDb = new SmartAdmin.Data.ClientModule.clientEntities())
    //    //        {
    //    //            SmartAdmin.Data.DataRepository dr = new SmartAdmin.Data.DataRepository();
    //    //            int prospectingClientSystemID = dr.GetProspectingClientSystemID();
    //    //            string query = string.Format(@";with cte (contact_person_id, person_title, person_gender, firstname, middle_name, surname, identity_number)
    //    //                                        as
    //    //                                 (select sc.fk_system_client_id,
    //    //                                   (case c.title
    //    //                                  when 2 then 1 
    //    //                                  when 3 then 2
    //    //                                  when 4 then 3
    //    //                                  when 1 then 4
    //    //                                  when null then null
    //    //                                  end) person_title,
    //    //                                  (case c.gender
    //    //                                     when 1 then 'M'
    //    //                                     when 2 then 'F'
    //    //                                     else ''
    //    //                                     end) person_gender, -- special case here ignore if null
    //    //                                    c.first_name firstname,
    //    //                                    c.middle_name,
    //    //                                    c.last_name surname,
    //    //                                    c.identity_number
    //    //                                   from client.dbo.client c 
    //    //                                  join client.dbo.search_client sc on c.pk_client_id = sc.fk_client_id and sc.fk_client_system_id = {0})

    //    //                                select distinct pcp.contact_person_id
    //    //                                from seeff_prospecting.dbo.prospecting_contact_person pcp
    //    //                                join cte c on c.contact_person_id = pcp.contact_person_id
    //    //                                where isnull(c.person_title,0) != isnull(pcp.person_title,0) OR 
    //    //                                   c.person_gender != pcp.person_gender OR
    //    //                                   concat(ltrim(rtrim(c.firstname)), ' ', ltrim(rtrim(c.middle_name)))  != ltrim(rtrim(pcp.firstname)) OR
    //    //                                   ltrim(rtrim(c.surname)) != ltrim(rtrim(pcp.surname)) OR
    //    //                                   c.identity_number != pcp.id_number", prospectingClientSystemID);

    //    //            var resultSet = clientDb.Database.SqlQuery<int>(query);
    //    //            foreach (var contactPersonID in resultSet)
    //    //            {
    //    //                var contactRecord = prospecting.prospecting_contact_persons.First(cp => cp.contact_person_id == contactPersonID);
    //    //                int? title = MapToClientTitle(contactRecord.person_title);
    //    //                int? gender = MapToClientGender(contactRecord.person_gender);
    //    //                string firstName = null;
    //    //                string surname = contactRecord.surname;
    //    //                string middleName = null;
    //    //                var createdByUser = contactRecord.created_by;
    //    //                if (!string.IsNullOrWhiteSpace(contactRecord.firstname))
    //    //                {
    //    //                    var namePair = contactRecord.firstname.Split(new[] { ' ' });
    //    //                    firstName = namePair[0];
    //    //                    if (namePair.Length == 2)
    //    //                    {
    //    //                        middleName = namePair[1];
    //    //                    }
    //    //                }
    //    //                Guid createdByUserGuid = Guid.Parse("62a85a9d-be7a-4fad-b704-a55edb1d338f");
    //    //                if (createdByUser != null)
    //    //                {
    //    //                    createdByUserGuid = createdByUser.Value;
    //    //                }
    //    //                dr.ExternalSaveClient(prospectingClientSystemID, contactPersonID, title, gender, firstName, middleName, surname, contactRecord.id_number, createdByUserGuid);
    //    //                recordCount++;
    //    //            }
    //    //        }
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        LogException(ex, "UpdateExistingClientsOnCMS()");
    //    //        SendEmailReport(ex.ToString(), "UpdateExistingClientsOnCMS()");
    //    //        success = false;
    //    //    }

    //    //    if (success)
    //    //    {
    //    //        LogSucess("UpdateExistingClientsOnCMS() completed successfully: " + recordCount + " existing clients updated.");
    //    //    }
    //    //}

    //    //private static void AddNewClientsToCMS()
    //    //{
    //    //    bool success = true;
    //    //    int recordCount = 0;
    //    //    try
    //    //    {
    //    //        using (var prospecting = new ProspectingDataContext())
    //    //        using (var clientDb = new SmartAdmin.Data.ClientModule.clientEntities())
    //    //        {
    //    //            SmartAdmin.Data.DataRepository dr = new SmartAdmin.Data.DataRepository();
    //    //            int prospectingClientSystemID = dr.GetProspectingClientSystemID();
    //    //            // Find records in Prospecting that don't have a matching ID number in the CMS
    //    //            string deltaQuery = @"select contact_person_id from seeff_prospecting.dbo.prospecting_contact_person
    //    //                                    left join client.dbo.client on id_number = identity_number
    //    //                                    where identity_number is null and (deleted is null or deleted = 0)";
    //    //            var resultSet = clientDb.Database.SqlQuery<int>(deltaQuery).ToList();
    //    //            foreach (var contactPersonID in resultSet)
    //    //            {
    //    //                var contactRecord = prospecting.prospecting_contact_persons.First(cp => cp.contact_person_id == contactPersonID);
    //    //                int? title = MapToClientTitle(contactRecord.person_title);
    //    //                int? gender = MapToClientGender(contactRecord.person_gender);
    //    //                string firstName = null;
    //    //                string surname = contactRecord.surname;
    //    //                string middleName = null;
    //    //                var createdByUser = contactRecord.created_by;
    //    //                if (!string.IsNullOrWhiteSpace(contactRecord.firstname))
    //    //                {
    //    //                    var namePair = contactRecord.firstname.Split(new[] { ' ' });
    //    //                    firstName = namePair[0];
    //    //                    if (namePair.Length == 2)
    //    //                    {
    //    //                        middleName = namePair[1];
    //    //                    }
    //    //                }
    //    //                Guid createdByUserGuid = Guid.Parse("62a85a9d-be7a-4fad-b704-a55edb1d338f");
    //    //                if (createdByUser != null)
    //    //                {
    //    //                    createdByUserGuid = createdByUser.Value;
    //    //                }
    //    //                dr.ExternalSaveClient(prospectingClientSystemID, contactPersonID, title, gender, firstName, middleName, surname, contactRecord.id_number, createdByUserGuid);
    //    //                recordCount++;
    //    //            }
    //    //        }
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        LogException(ex, "AddNewClientsToCMS()");
    //    //        SendEmailReport(ex.ToString(), "AddNewClientsToCMS()");
    //    //        success = false;
    //    //    }

    //    //    if (success)
    //    //    {
    //    //        LogSucess("AddNewClientsToCMS() completed successfully: " + recordCount + " new clients added.");
    //    //    }
    //    //}

    //    private static int MapToClientContactDetailType(int contact_detail_type)
    //    {
    //        switch (contact_detail_type)
    //        {
    //            case 1:
    //            case 2:
    //                return 2;
    //            case 3:
    //                return 3;
    //            case 4:
    //            case 5:
    //                return 1;
    //            case 6:
    //                return 4;
    //        }

    //        throw new Exception("Invalid contact_detail_type from Prospecting");
    //    }

    //    private static int MapToClientContactDetailLocationType(int contact_detail_type)
    //    {
    //        switch(contact_detail_type)
    //        {
    //            case 1:
    //                return 1;
    //            case 2:
    //            case 5:
    //                return 2;
    //            case 3:
    //                return 3;
    //            case 4:
    //                return 3;
    //            case 6:
    //                return 2; // Check?
    //        }

    //        throw new Exception("Invalid contact_detail_type from Prospecting");
    //    }

    //    private static int? MapToClientGender(string personGender)
    //    {
    //        switch (personGender)
    //        {
    //            case "M": return 1;
    //            case "F": return 2;
    //            default: return null;
    //        }
    //    }

    //    private static int? MapToClientTitle(int? personTitle)
    //    {
    //        if (personTitle == null) return null;

    //        switch (personTitle)
    //        {
    //            case 1: return 2;
    //            case 2: return 3;
    //            case 3: return 4;
    //            case 4: return 1;
    //            default: return null;
    //        }
    //    }

    //    private static void LogException(Exception  ex, string methodName)
    //    {
    //        using (var prospectingDb = new seeff_prospectingEntities())
    //        {
    //            var errorRec = new exception_log
    //            {
    //                friendly_error_msg = "Error occurred in ProspectingToCmsClientSynchroniser." + methodName,
    //                exception_string = ex.ToString(),
    //                user = new Guid(),
    //                date_time = DateTime.Now
    //            };
    //            prospectingDb.exception_log.Add(errorRec);
    //            prospectingDb.SaveChanges();
    //        }
    //    }
    //    private static void LogSucess(string methodName)
    //    {
    //        using (var prospectingDb = new seeff_prospectingEntities())
    //        {
    //            var errorRec = new exception_log
    //            {
    //                friendly_error_msg = "Successfully completed complete step in ProspectingToCmsClientSynchroniser." + methodName,
    //                exception_string = "",
    //                user = new Guid(),
    //                date_time = DateTime.Now
    //            };
    //            prospectingDb.exception_log.Add(errorRec);
    //            prospectingDb.SaveChanges();
    //        }
    //    }

    //    public static void SendEmailReport(string message, string methodName)
    //    {
    //        string emailToAddress = "danie.vdm@seeff.com";
    //        string emailDisplayName = "ProspectingTaskScheduler";
    //        string emailFromAddress = "reports@seeff.com";
    //        string emaiLSubject = "Exception occurred in ProspectingToCmsClientSynchroniser." + methodName;

    //        StatusNotifier.SendEmail(emailToAddress, emailDisplayName, emailFromAddress, null, emaiLSubject, message);
    //    }
    //}

    //public class ProspectingContactDetailRecord
    //{
    //    public int contact_person_id { get; set; }
    //    public string contact_detail { get; set; }
    //    public int contact_detail_type { get; set; }
    //    public int? intl_dialing_code_id { get; set; }
    //    public int? eleventh_digit { get; set; }
    //    public bool is_primary_contact { get; set; }

    //    public int prospecting_contact_detail_id { get; set; }
    //}
}