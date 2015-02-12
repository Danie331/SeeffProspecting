using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace ProspectingProject
{
    public class TracepsLiveService : ITracepsService
    {
        public XDocument GetResponseXML(string idNumber)
        {
            XDocument outputXml = null;
            Uri uri = new Uri(@"http://ws.traceps.co.za/ws/idnLookup/11222122/" + idNumber);
            try
            {
                WebRequest req = WebRequest.Create(uri);
                req.Timeout = 15 * 1000;
                req.Method = "GET";
                req.Credentials = new NetworkCredential("GEM001", "gEm25m");
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                        string xml = reader.ReadToEnd();
                        outputXml = XDocument.Parse(xml);
                    }
                }
            }
            catch
            {
                throw; // Let caller handle
            }

            return outputXml;
        }
    }
}