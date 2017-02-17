using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject.ClientIntegration
{
    public class ClientSynchronizer
    {
        private static SmartAdmin.Data.DataRepository _dr;
        private static int _clientSystemID;

        static ClientSynchronizer()
        {
            try
            {
                _dr = new SmartAdmin.Data.DataRepository();
                using (var clientDB = new SmartAdmin.Data.ClientModule.clientEntities())
                {
                    _clientSystemID = clientDB.client_system.First(cs => cs.system_name == "Prospecting").pk_client_system_id;
                }
            }
            catch (Exception ex)
            {
                // Log
            }
        }

        //public static void UpsertNewClient(int contactPersonID, int? personTitle, string personGender, string firstname, string surname, string idNumber, Guid? createdBy)
        //{
        //    // this method must go through the whole save and merge process and identify the client as coming from "Prospecting client system" + log error
        //    try
        //    {
        //        int? registrationID = null;
        //        if (createdBy.HasValue)
        //        {
        //            var user = _dr.FetchUserRegistrationByUserGuid(createdBy.Value.ToString());
        //            registrationID = Convert.ToInt32(user.RegistrationId);
        //        }
        //        SmartAdmin.Models.ClientModels.Client newClient = new SmartAdmin.Models.ClientModels.Client() { ClientId = -1 };
        //        newClient.Title = MapToClientTitle(personTitle);
        //        newClient.Gender = MapToClientGender(personGender);
        //        newClient.FirstName = firstname;
        //        newClient.LastName = surname;
        //        newClient.IDNumber = idNumber;
        //        newClient.CreatedBy = registrationID;

        //        // check for existing client-system relationship and manually add one here if not found
        //        using (var clientDB = new SmartAdmin.Data.ClientModule.clientEntities())
        //        {
        //            var clientExists = clientDB.search_client.FirstOrDefault(sc => sc.fk_client_system_id == _clientSystemID && sc.fk_system_client_id == contactPersonID);
        //            if (clientExists != null)
        //            {
        //                int clientClientID = 
        //            }
        //        }

        //            var result = _dr.SaveClient(newClient, true);
        //    }
        //    catch (Exception ex)
        //    {
        //        // log
        //    }
        //}       

    }
}