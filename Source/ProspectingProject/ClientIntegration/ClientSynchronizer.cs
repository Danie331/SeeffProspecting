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
                _clientSystemID = _dr.GetProspectingClientSystemID();
            }
            catch (Exception ex)
            {
                LogException(ex, "ClientSynchronizer()");
            }
        }

        public static void AddOrUpdateClientToCMS(int contactPersonID, int? personTitle, string personGender, string firstname, string surname, string idNumber, Guid? userGuid)
        {
            // this method must go through the whole save and merge process and identify the client as coming from "Prospecting client system" + log error
            // check when adding new client with exiting ID number - OR- editing existing client to an ID number that already exists in CMS
            //
            try
            {
                int? title = MapToClientTitle(personTitle);
                int? gender = MapToClientGender(personGender);

                _dr.ExternalSaveClient(_clientSystemID, contactPersonID, title, gender, firstname, surname, idNumber, userGuid, true);
            }
            catch (Exception ex)
            {
                LogException(ex, "AddClientToCMS()");
            }
        }

        private static int? MapToClientGender(string personGender)
        {
            switch (personGender)
            {
                case "M": return 1;
                default: return 2;
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

        private static void LogException(Exception ex, string invokingMethod)
        {
            using (var prospectingDb = new ProspectingDataContext())
            {
                var errorRec = new exception_log
                {
                    friendly_error_msg = "Error occurred in ClientSynchronizer." + invokingMethod,
                    exception_string = ex.ToString(),
                    user = RequestHandler.GetUserSessionObject().UserGuid,
                    date_time = DateTime.Now
                };
                prospectingDb.exception_logs.InsertOnSubmit(errorRec);
                prospectingDb.SubmitChanges();
            }
        }

    }
}