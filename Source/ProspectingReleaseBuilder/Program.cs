using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace ProspectingReleaseBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            bool success = true;
            try
            {
                ProcessStart();
            }
            catch (Exception ex)
            {
                success = false;
                Abandon(ex);
            }

            if (success)
            {
                PerformChecksumComplete();
            }
        }

        static void ProcessStart()
        {
            Console.WriteLine("Folder to which you have published the source: " + BuildDefaults._publishTarget);
            Console.WriteLine("Source file containing script references: " + BuildDefaults._scriptReferenceFile);
            Console.WriteLine("Javascript source location: " + BuildDefaults._javascriptSourceFolder);
            Console.WriteLine("Stylesheets source location: " + BuildDefaults._stylesheetSourceFolder);
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();

            // Core js files
            var ordering = GetOrdering(BuildDefaults._javascriptSourceFolder, BuildDefaults._coreJavascriptSectionName);
            string coreJavascript = CombineFiles(ordering, true, ".js");

            // Combine the 3rd party javascript
            ordering = GetOrdering(BuildDefaults._3rdPartyFolder, BuildDefaults._3rdpartyJavascriptSectionName);
            string _3rdpartyJavascript = CombineFiles(ordering, true, ".js");

            // Core css
            ordering = GetOrdering(BuildDefaults._stylesheetSourceFolder, BuildDefaults._coreStylesheetSectionName);
            string coreCSS = CombineFiles(ordering, false, ".css");

            // 3rd party CSS
            ordering = GetOrdering(BuildDefaults._stylesheetSourceFolder, BuildDefaults._3rdPartyStylesheetSectionName);
            string _3rdPartyCSS = CombineFiles(ordering, false, ".css");

            // Delete the javascript and stylesheet source folder 
            //Directory.Delete(Path.Combine(BuildDefaults._publishTarget, BuildDefaults._publishScripts), true);
            // Update the referencing file
            UpdateScriptReferences(_3rdpartyJavascript + coreJavascript, _3rdPartyCSS + coreCSS);
        }

        private static List<string> GetOrdering(string sourceFolder, string sectionName)
        {
            List<string> order = new List<string>();

            string referencingContent = File.ReadAllText(Path.Combine(BuildDefaults._publishTarget, BuildDefaults._scriptReferenceFile));
            string tagStart = "<%-- BEGIN_SECTION:" + sectionName + " --%>";
            string tagEnd = "<%-- END_SECTION:" + sectionName + " --%>";
            int start = referencingContent.IndexOf(tagStart);
            int end = referencingContent.IndexOf(tagEnd);
            string block = referencingContent.Substring(start, end - start);

            foreach (var item in block.Split(new[] {Environment.NewLine}, StringSplitOptions.None).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                var filename = new Regex(@"(\\?"")(.*?)\1").Match(item).Value;
                filename = filename.Replace("\"", "");
                filename = Path.Combine(BuildDefaults._publishTarget, filename);
                order.Add(filename);
            }
            return order;
        }

        private static void UpdateScriptReferences(string minJs, string minCSS)
        {
            Func<string, string, string, string> replaceContent = (sectionName, outputFileLink, contentToReplace) =>
            {
                string tagStart = "<%-- BEGIN_SECTION:" + sectionName + " --%>";
                string tagEnd = "<%-- END_SECTION:" + sectionName + " --%>";

                int start = contentToReplace.IndexOf(tagStart);
                int end = contentToReplace.IndexOf(tagEnd);
                string blockToReplace = contentToReplace.Substring(start, end - start);
                return contentToReplace.Replace(blockToReplace, outputFileLink);
            };

            string referencingContent = File.ReadAllText(Path.Combine(BuildDefaults._publishTarget, BuildDefaults._scriptReferenceFile));
            // Replace the stylesheet references
            referencingContent = replaceContent(BuildDefaults._stylesheetSectionName, "<style>" + minCSS + "</style>", referencingContent);
            // Replace the javascript references 
            referencingContent = replaceContent(BuildDefaults._javascriptSectionName, "<script>" + minJs + "</script>", referencingContent);

            referencingContent = referencingContent.Replace(@"../../Assets/", "Assets/");

            File.WriteAllText(Path.Combine(BuildDefaults._publishTarget, BuildDefaults._scriptReferenceFile), referencingContent, Encoding.Unicode);
        }

        static string CombineFiles(List<string> files, bool minify, string fileType)
        {
            StringBuilder output = new StringBuilder();
            foreach (var script in files)
            {
                string result = File.ReadAllText(script);
                if (minify)
                {
                    result = Minify(result, Path.GetExtension(script));
                }
                if (string.IsNullOrWhiteSpace(result))
                {
                    throw new Exception("Error minifying a script: " + script);
                }
                if (fileType == ".js")
                {
                    output.Append(result).Append(";").Append(Environment.NewLine);
                }
                else
                {
                    output.Append(result).Append(Environment.NewLine);
                }
            }

            return output.ToString();
        }
        static string Minify(string contents, string type)
        {
            string result = "";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("content-type", "application/x-www-form-urlencoded");
                if (type == ".js")
                {
                    string data = string.Format(BuildDefaults._jsMinifierPostData, HttpUtility.UrlEncode(contents));
                    result = client.UploadString(BuildDefaults._jsMinifierAPI, data);
                }
                else if (type == ".css")
                {
                    string data = string.Format(BuildDefaults._cssMinifierPostData, HttpUtility.UrlEncode(contents));
                    result = client.UploadString(BuildDefaults._cssMiniferAPI, data);
                }

                return result;
            }
        }
        static void CleanTargetFolder(string targetFolder) { }
        static void Abandon(Exception ex) { Console.ReadLine(); }
        static void PerformChecksumComplete() { }
    }

    static class BuildDefaults
    {
        public static string _buildVersion = GetBuildVersion();
        public static string _publishTarget = @"C:\Users\Louise\Desktop\Repos2019\SeeffProspecting\Source\ProspectingProject\publish";
        public static string _publishScripts = @"Scripts\";
        public static string _javascriptSourceFolder = @"Scripts\Core\";
        public static string _stylesheetSourceFolder = @"Scripts\StyleSheets\";
        public static string _3rdPartyFolder = @"Scripts\3rdParty\";
        public static string _scriptReferenceFile = @"Default.aspx";
        public static string _minJS = Path.Combine(_publishTarget, "javascript_min_" + _buildVersion + ".js");
        public static string _minCSS = Path.Combine(_publishTarget, "css_min_" + _buildVersion + ".css");
        public static string _stylesheetSectionName = "STYLESHEETS";
        public static string _javascriptSectionName = "JAVASCRIPT";
        public static string _3rdpartyJavascriptSectionName = "3RD_PARTY_SCRIPTS";
        public static string _coreJavascriptSectionName = "CORE_SCRIPTS";
        public static string _3rdPartyStylesheetSectionName = "3RD_PARTY_STYLESHEETS";
        public static string _coreStylesheetSectionName = "CORE_STYLESHEETS";

        public static string _jsMinifierPostData = "js_code={0}&output_info=compiled_code&compilation_level=SIMPLE_OPTIMIZATIONS&language=ECMASCRIPT6";
        public static string _jsMinifierAPI = "https://closure-compiler.appspot.com/compile";

        public static string _cssMiniferAPI = "http://cssminifier.com/raw";
        public static string _cssMinifierPostData = "input={0}";

        static string GetBuildVersion()
        {
            int prospectingMajorVersion = 1;
            int prospectingBuildNo = new Random().Next(0, 5000);
            return prospectingMajorVersion + "_" + prospectingBuildNo.ToString();
        }

    }
}
