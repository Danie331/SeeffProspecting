
using DataManager.DataContexts;
using DataManager.DomainTypes;
using System;
using System.Linq;

namespace DataManager.Client
{
    /// <summary>
    /// An instance of this class should act as the central point of access to data in the client database.
    /// </summary>
    public class ClientManager
    {
        private int _clientSystemID;
        private Guid? _user;

        /// <summary>
        /// Use this contructor by default
        /// </summary>
        /// <param name="applicationID">the client system ID of the calling application as listed in the dbo.client_system table</param>
        /// <param name="user">the logged-in user (if applicable)</param>
        public ClientManager(ClientSystemContext clientApplication, Guid? user)
        {
            _clientSystemID = (int)clientApplication;
            _user = user;
        }

        #region public methods

        public Client RetrieveClientByID(int localSystemID)
        {
            try
            {
                using (var client = DataContextRetriever.GetClientEntities())
                {
                    var targetClient = client.search_client.FirstOrDefault(c => c.fk_client_system_id == _clientSystemID && c.fk_system_client_id == localSystemID);
                    ThrowIfNull(targetClient, "targetClient");

                    Client result = RetrieveClient(targetClient, false);
                }
            }
            catch(ClientException ce)
            {
                throw new Exception(ce.Message);
            }
            catch (Exception ex)
            {
                // log ... 
                throw ex; // let the caller handle the exception normally.
            }
        }

        #endregion

        private Client RetrieveClient(search_client targetClient, bool retrieveContactDetails)
        {
            Client client = new DataManager.Client
            {
                LocalRecordID = targetClient.fk_system_client_id,
                Firstname = targetClient.client.first_name,
                Surname = targetClient.client.last_name,
            };
        }

        /// <summary>
        /// Use this method to throw an exception back to the caller if the input parameter is expected to have a valid value.
        /// A record will be written to the error_log containing the stacktrace.
        /// </summary>
        /// <param name="value">value to be tested for null</param>
        /// <param name="variableName">name of the variable as per the source code</param>
        private void ThrowIfNull(object value, string variableName)
        {
            if (value == null)
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                int errorLogID;
                using (var client = DataContextRetriever.GetClientEntities())
                {
                    error_log error = new error_log();
                    error.source_application = _clientSystemID;
                    error.friendly_error_msg = variableName + " cannot be null.";
                    error.exception_string = "STACKTRACE: " + st.ToString();
                    error.user = _user;
                    error.date_time = DateTime.Now;

                    client.error_log.Add(error);
                    client.SaveChanges();
                    errorLogID = error.error_log_id;
                }

                throw new ClientException("ClientManager threw an exception while executing the request - client.dbo.error_log -> id = " + errorLogID);
            }
        }
    }
}
