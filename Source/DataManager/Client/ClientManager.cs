
using DataManager.DataContexts;
using DataManager.DomainTypes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Caching;
using System.Web;
using SmartAdmin.Client.SharedTypes;

namespace DataManager.Client
{
    /// <summary>
    /// An instance of this class should act as the central point of access to data in the client database.
    /// </summary>
    public class ClientManager
    {
        private int _clientSystemID;
        private Guid? _user;
        private IClientRepository _clientRepository;

        /// <summary>
        /// Use this contructor by default
        /// </summary>
        /// <param name="applicationID">the client system ID of the calling application as listed in the dbo.client_system table</param>
        /// <param name="user">the logged-in user (if applicable)</param>
        public ClientManager(ClientSystemContext clientApplication, IClientRepository clientRepository, Guid? user)
        {
            _clientSystemID = (int)clientApplication;
            _clientRepository = clientRepository;
            _user = user;

            clientRepository.InitUserSessionInfo(_clientSystemID, user);
        }

        #region public methods

        public IClientModel RetrieveClientByIdThrowIfNull(int? localSystemID)
        {
            try
            {
                var client = _clientRepository.RetrieveClientByID(_clientSystemID, localSystemID);
                LogAndThrowIfNull(client, "client");

                return client;
            }
            catch (ClientException ce)
            {
                // TODO Log here
                throw new Exception(ce.Message);
            }
        }

        public IClientModel RetrieveClientById(int? localSystemID)
        {
            try
            {
                var client = _clientRepository.RetrieveClientByID(_clientSystemID, localSystemID);

                return client;
            }
            catch (ClientException ce)
            {
                // TODO Log here
                throw new Exception(ce.Message);
            }
        }

        public void SaveClient(IClientModel clientToSave)
        {

        }

        #endregion

        private DomainTypes.Client RetrieveClient(search_client searchClient, bool retrieveContactDetails)
        {
           // -- TODO: check if the take on of deleted items is working
        }

        /// <summary>
        /// Use this method to throw an exception back to the caller if the input parameter is expected to have a valid value.
        /// A record will be written to the client.dbo.error_log containing the stacktrace.
        /// </summary>
        /// <param name="value">value to be tested for null</param>
        /// <param name="variableName">name of the variable as per the source code</param>
        private void LogAndThrowIfNull(object value, string variableName)
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

                throw new ClientException("Exception in ClientManager - value cannot be null - client.dbo.error_log -> id = " + errorLogID);
            }
        }
    }
}
