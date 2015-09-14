using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace ProspectingTaskScheduler.Core.Communication.SMSing
{
    public class SMSHandler
    {
        public static void SendSMS()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var batch = GetBatch(prospecting);
                int awaitingStatus = CommunicationHelpers.GetCommunicationStatusId("AWAITING_RESPONSE_FROM_API");
                if (batch.Count > 0)
                {
                    foreach (var item in batch)
                    {
                        item.status = awaitingStatus;
                        try
                        {
                            prospecting.SubmitChanges();
                        }
                        catch (Exception e)
                        {
                            LogRecordUpdateException("Error occurred updating status of SMS item to AWAITING_RESPONSE_FROM_API. item_id: " + item.sms_communications_log_id, e);
                        }
                    }
                    SMSBatchResult result = SubmitBatch(prospecting, batch);
                    if (!result.Success)
                    {
                        //ResetBatchToPending(batch);
                        exception_log errorRecord = new exception_log
                        {
                            friendly_error_msg = "Unexpected error occurred when submitting batch of SMS's (batch_id:" + batch.First().batch_id + ")",
                            date_time = DateTime.Now,
                            exception_string = result.ErrorMessage,
                            user = batch.First().created_by_user_guid
                        };
                        prospecting.exception_logs.InsertOnSubmit(errorRecord);
                        try
                        {
                            prospecting.SubmitChanges();
                        }
                        catch { }
                    }
                }
            }
        }

        private static List<sms_communications_log> GetBatch(ProspectingDataContext prospecting)
        {
            List<sms_communications_log> batch = new List<sms_communications_log>();
            int pendingStatus = CommunicationHelpers.GetCommunicationStatusId("PENDING_SUBMIT_TO_API");
            var batchItem = prospecting.sms_communications_logs.FirstOrDefault(sms => sms.status == pendingStatus);
            if (batchItem != null)
            {
                // Get all records with the same batch_id
                string batchId = batchItem.batch_id.ToString();
                batch = prospecting.sms_communications_logs.Where(sms => sms.status == pendingStatus && sms.batch_id.ToString() == batchId).ToList();
            }

            return batch;
        }

        private static string BuildBatch(List<sms_communications_log> batch)
        {
            XDocument doc = new XDocument();
            var root = new XElement("SmsQueue");
            var accountElement = new XElement("Account");
            var user = new XElement("User");
            user.Value = "SanAcc00006";
            accountElement.Add(user);
            var password = new XElement("Password");
            password.Value = "A78BfEPK";
            accountElement.Add(password);
            root.Add(accountElement);
            var messageDataElement = new XElement("MessageData");
            var senderId = new XElement("SenderId");
            senderId.Value = "438292";
            messageDataElement.Add(senderId);
            var dataCoding = new XElement("DataCoding");
            dataCoding.Value = "0";
            messageDataElement.Add(dataCoding);
            root.Add(messageDataElement);
            var messagesElement = new XElement("Messages");

            foreach (var item in batch)
            {
                var messageElement = new XElement("Message");
                var number = new XElement("Number");
                number.Value = item.target_cellphone_no;
                var text = new XElement("Text");
                text.Value = item.msg_body_or_link_id;

                messageElement.Add(number);
                messageElement.Add(text);

                messagesElement.Add(messageElement);
            }
            root.Add(messagesElement);
            doc.Add(root);

            string batchString = doc.ToString();
            return batchString;
        }

        private static SMSBatchResult SubmitBatch(ProspectingDataContext prospecting, List<sms_communications_log> batch)
        {
            string smsBatchXML = BuildBatch(batch);

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://client.sancustelecom.com/");
            var httpContent = new StringContent(smsBatchXML, Encoding.UTF8, "application/xml");
            HttpResponseMessage response = null;
            SMSBatchResult result = new SMSBatchResult();
            try
            {
                response = client.PostAsync("/Rest/Messaging.svc/mtsms?data=", httpContent).Result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
            // Interrogate response here..
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    XDocument responseXml = XDocument.Parse(content);
                    var ns = responseXml.Root.Name.Namespace;
                    string errorCode = responseXml.Root.Element(ns + "ErrorCode").Value;
                    string errorMessage = responseXml.Root.Element(ns + "ErrorMessage").Value;

                    if (errorCode == "000")
                    {
                        result.Success = true;
                        try
                        {
                            UpdateBatchWithResponse(prospecting, batch, responseXml);
                        }
                        catch  (Exception e)
                        {
                            // In this scenario the batch was submitted successfully and we can expect to be billed for it.
                            // Therefore: if an exception occurs on our side subsequent to successful submission of a batch, we cannot regard this as an error.
                            // Set the status of each item in the batch to "Submitted"
                            LogRecordUpdateException("Exception occurred updating SMS records after successful submission - batch id:" + batch.First().batch_id +  " was submitted successfully.", e);
                        }
                    }
                    else
                    {
                        result.Success = false;
                        result.ErrorMessage = errorMessage;
                    }
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.ErrorMessage = ex.Message;
                    return result;
                }
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = response.ReasonPhrase + " StatusCode: " + response.StatusCode;
            }

            return result;
        }

        private static void UpdateBatchWithResponse(ProspectingDataContext prospecting, List<sms_communications_log> batch, XDocument responseXml)
        {
            var ns = responseXml.Root.Name.Namespace;
            var messages = responseXml.Root.Element(ns + "MessageData").Descendants(ns + "Messages");
            foreach (var message in messages)
            {
                var firstMessagePart = message.Descendants(ns + "MessageParts").Descendants(ns + "MessagePart").First();
                string msgId = firstMessagePart.Descendants(ns + "MsgId").First().Value;

                string cellphoneNumber = msgId.Split(new[] { '-' })[0];

                var affectedRecords = batch.Where(item => item.target_cellphone_no == cellphoneNumber);
                foreach (var record in affectedRecords)
                {
                    record.updated_datetime = DateTime.Now;
                    record.status = CommunicationHelpers.GetCommunicationStatusId("SMS_SUBMITTED");
                    record.api_tracking_id = msgId;
                    record.msg_body_or_link_id = null;
                    try
                    {
                        prospecting.SubmitChanges();
                    }
                    catch (Exception e)
                    {
                        LogRecordUpdateException("Error updating SMS record to SMS_SUBMITTED status. Record id: " + record.sms_communications_log_id, e);
                    }
                }
            }
        }


        // Follow-up: when we receive a reply


        // If they reply STOP opt out.

        private static void LogRecordUpdateException(string context, Exception e)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                exception_log errorRecord = new exception_log
                {
                    friendly_error_msg = context,
                    date_time = DateTime.Now,
                    exception_string = e.ToString(),
                    user = Guid.NewGuid()
                };
                prospecting.exception_logs.InsertOnSubmit(errorRecord);
                prospecting.SubmitChanges();
            }
        }

        public static void UpdateDeliveryStatuses()
        {
            int submittedStatus = CommunicationHelpers.GetCommunicationStatusId("SMS_SUBMITTED");
            using (var prospecting = new ProspectingDataContext())
            {
                var yesterday = DateTime.Now.AddDays(-1);
                var submittedItems = prospecting.sms_communications_logs.Where(item => item.status == submittedStatus && item.created_datetime >= yesterday);
                foreach (var record in submittedItems)
                {
                    string trackingId = record.api_tracking_id;
                    string deliveryUri = "http://client.sancustelecom.com/vendorsms/checkdelivery.aspx?user=SanAcc00006&password=A78BfEPK&messageid=" + trackingId;
                    var httpClient = new HttpClient();
                    string content = "";
                    try
                    {
                        content = httpClient.GetStringAsync(deliveryUri).Result;
                    }
                    catch (Exception e)
                    {
                        LogRecordUpdateException("Error while calling SMS delivery API for record id: " + record.sms_communications_log_id, e);
                        return;
                    }
                    record.updated_datetime = DateTime.Now;
                    record.api_delivery_status = content;
                    switch (content)
                    {
                        case "#DELIVRD":
                            record.status = CommunicationHelpers.GetCommunicationStatusId("SMS_DELIVERED");
                            record.activity_log_id = CreateActivityForRecord(record);
                            break;
                        case "#Submitted": // do absolutely nothing
                          case  "#Pending":
                           case  "#Accepted":
                            break;
                        default:
                            record.status = CommunicationHelpers.GetCommunicationStatusId("SMS_OTHER");
                            break;
                    }
                    try
                    {
                        prospecting.SubmitChanges();
                    }
                    catch (Exception e)
                    {
                        LogRecordUpdateException("Error updating delivery status of SMS record id:" + record.sms_communications_log_id, e);
                    }
                }
            }
        }

        private static long CreateActivityForRecord(sms_communications_log smsItem)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var activityRecord = new activity_log
                {
                    lightstone_property_id = smsItem.target_lightstone_property_id,
                    followup_date = null,
                    allocated_to = smsItem.created_by_user_guid,
                    activity_type_id = smsItem.batch_activity_type_id,
                    comment = BuildCommentForSMSActivity(smsItem),
                    created_by = smsItem.created_by_user_guid,
                    created_date = DateTime.Now,
                    contact_person_id = smsItem.target_contact_person_id,
                    // Add the rest later
                    parent_activity_id = null,
                    activity_followup_type_id = null
                };
                prospecting.activity_logs.InsertOnSubmit(activityRecord);
                try
                {
                    prospecting.SubmitChanges();
                }
                catch (Exception e)
                {
                    using (var newContext = new ProspectingDataContext())
                    {
                        string msg = "Error inserting activity record for SMS communication sent. (SMS comm record id: " + smsItem.sms_communications_log_id + ")";
                        exception_log logentry = new exception_log
                        {
                            friendly_error_msg = msg,
                            exception_string = e.ToString(),
                            date_time = DateTime.Now,
                            user = smsItem.created_by_user_guid
                        };
                        newContext.exception_logs.InsertOnSubmit(logentry);
                        newContext.SubmitChanges();
                    }
                }
                return activityRecord.activity_log_id;
            }
        }

        private static string BuildCommentForSMSActivity(sms_communications_log smsItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*** SMS sent to contact person ***");
            sb.AppendLine(string.Format("An SMS was sent to {0} at {1}.", smsItem.target_cellphone_no, DateTime.Now));
            return sb.ToString();
        }
    }
}