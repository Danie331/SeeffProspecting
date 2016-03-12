using ProspectingTaskScheduler.Core.Housekeeping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ProspectingTaskScheduler.Core.LightstoneTakeOn
{
    // Pre-take-on steps:
    // 1) Restore database Seeff_Deeds to server
    // 2) Apply permissions to Seeff_Deeds for task scheduler:
    //      USE[Seeff_Deeds]
    //      GO
    //      CREATE USER[IIS APPPOOL\prospectingtaskscheduler] FOR LOGIN[IIS APPPOOL\prospectingtaskscheduler] WITH DEFAULT_SCHEMA =[dbo]
    //      GO

    public partial class LightstoneTakeOn
    {
        public static void PerformBaseDataTakeOn()
        {
            StringBuilder reportBuilder = new StringBuilder();
            try
            {
                // Step 1: check if seeff_deeds DB exists
                if (!SeeffDeedsExists(reportBuilder)) return;

                SendEmailReport("Starting Lightstone take-on, Step1 confirms existence of Seeff_Deeds. Starting Step2 at: " + DateTime.Now, reportBuilder);

                // Step 2: Prepare the Seeff_Deeds_Monthly table and flag the records that will be taken-on into base_data 
                FlagRecordsForInsert(reportBuilder);

                SendEmailReport("<br /> Step2 completed successfully. Starting Step3 at: " + DateTime.Now, reportBuilder);

                // Step 3: This performs the data enrichment of all take-on records prior to insert.
                ProcessRecordsForInsert(reportBuilder);

                SendEmailReport("<br /> Step3 completed successfully. Starting Step4 at: " + DateTime.Now, reportBuilder);

                // Step 4: Perform the actual take-on of records into base_data (previously the "delta set")
                InsertRecords(reportBuilder);

                SendEmailReport("<br /> Step4 completed successfully. Starting Step5 at: " + DateTime.Now, reportBuilder);

                // Step 5: Update the agency for Seeff deals
                UpdateSeeffDeals(reportBuilder);

                SendEmailReport("<br /> Step5 completed successfully. Starting Step6 at: " + DateTime.Now, reportBuilder);

                // Step 6: Apply certain rules to transactions
                ApplyAdditionalRules(reportBuilder);

                SendEmailReport("<br /> Step6 completed successfully. Starting Step7 at: " + DateTime.Now, reportBuilder);

                // Step 7: Re-populate the area fating table
                UpdateAreaFatingTable(reportBuilder);

                SendEmailReport("<br /> Step7 completed successfully. Starting Step8 at: " + DateTime.Now, reportBuilder);

                // Step 8: Update new registrations to reflect a change of ownership on Prospecting
                FlagNewRegistrations(reportBuilder);

                SendEmailReport("<br /> Step8 completed successfully. Starting Step9 at: " + DateTime.Now, reportBuilder);

                // Step 9: Drop the Seeff_Deeds database
                DropSeeffDeeds(reportBuilder);

                SendEmailReport("<br /> Step 9 completed successfully. Starting Step 10 at: " + DateTime.Now, reportBuilder);

                // Step 10: base_data - fate transactions that belong to property developers as Developments.
                AutoFateDevelopments(reportBuilder);

                SendEmailReport("<br /> Step 10 completed successfully.<p /><p />------------ Take-on completed succesfully at: " + DateTime.Now + " ------------", reportBuilder);
            }
            catch (Exception ex)
            {
                SendEmailReport("<p /> Fatal error occurred in Lightstone data take-on. Process aborted with error: " + ex.ToString() +
                                "<br /> -------- Stack Trace --------" +
                                "<p />" + ex.StackTrace, reportBuilder);
            }
        }

        private static void SendEmailReport(string message, StringBuilder reportBuilder)
        {
            reportBuilder.AppendLine(message);

            string emailToAddress = "danie.vdm@seeff.com";
            string emailDisplayName = "ProspectingTaskScheduler";
            string emailFromAddress = "reports@seeff.com";
            string emaiLSubject = "Notification: Lightstone data take-on";

            StatusNotifier.SendEmail(emailToAddress, emailDisplayName, emailFromAddress, null, emaiLSubject, reportBuilder.ToString());
        }
    }
}