
using System.Collections.Generic;
namespace ProspectingTaskScheduler.Core.Communication.Emailing.Mandrill
{
    public class MandrillEmailMessage
    {
        public string html { get; set; }
        public string text { get; set; }
        public string subject { get; set; }
        public string from_email { get; set; }
        public string from_name { get; set; }
        public List<MandrillMessageTo> to { get; set; }
        public MandrillMessageHeaders headers { get; set; }
        public bool important { get; set; }
        public object track_opens { get; set; }
        public object track_clicks { get; set; }
        public object auto_text { get; set; }
        public object auto_html { get; set; }
        public object inline_css { get; set; }
        public object url_strip_qs { get; set; }
        public object preserve_recipients { get; set; }
        public object view_content_link { get; set; }
        public string bcc_address { get; set; }
        public object tracking_domain { get; set; }
        public object signing_domain { get; set; }
        public object return_path_domain { get; set; }
        public bool merge { get; set; }
        public string merge_language { get; set; }
        public List<MandrillGlobalMergeVar> global_merge_vars { get; set; }
        public List<MandrillMergVar> merge_vars { get; set; }
        public List<string> tags { get; set; }
        public string subaccount { get; set; }
        public List<string> google_analytics_domains { get; set; }
        public string google_analytics_campaign { get; set; }
        public MandrillMetaData metadata { get; set; }
        public List<MandrillRecipientMetaData> recipient_metadata {get; set;}
        public List<MandrillAttachment> attachments { get; set; }
        public List<MandrillImage> images {get; set;}

    }
}
