 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.BuildEngine;

namespace MarketShareAppReleaseOptimiser
{
    /// <summary>
    /// Run this code manually after publishing to the ReleaseFolder.
    /// </summary>
    class Program
    {
        // Add files here that must be excluded from the build
        private static string[] excludedFiles = new[] { "Web.config", "SamplePost.aspx", "SamplePost.aspx.cs" };
        // Add folders here that must be excluded from the build
        private static string[] excludedFolders = new[] { "Database scripts", "Readme", "SiteOffline" };

        // These paths must correspond to the way your development environment is set up.
        private static string srcDirectory = @"C:\Users\Adam\Google Drive\Code2013\MarketShare\MarketShareApp";
        private static string destReleaseFolder = @"C:\Users\Adam\Google Drive\Code2013\MarketShare\MarketShareProjectReleaseBuilder\ReleaseOutput";
        private static string minifierPath = @"C:\Users\Adam\Google Drive\Code2013\MarketShare\MarketShareProjectReleaseBuilder\closure-compiler\compiler.jar";
        //
        // The primary purpose of this executable is to do create a deployable package for the MarketShare frontend
        // The following actions are taken and results copied to an output folder:
        // 1. Only files relevant to the runtime execution of the app are copied (these exclude the web.config file (the live version should always be used), and database change scripts etc)
        // 2. JS source code is minified and saved to output folder
        static void Main(string[] args)
        {        
            CreateReleaseFolder();
        }

        private static void CreateReleaseFolder()
        {
            // Create the dev folder in the output folder
            //Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(srcDirectory, destReleaseFolder, true);

            // Remove uneeded files and directories from the output
            RemoveDirectoriesToExclude(destReleaseFolder);
            
            // Remove unwanted files and get the remaining files
            var remainingFiles = RemoveFilesToExcludeAndGetRemainder(AllFiles(destReleaseFolder));

            // Compress all compressible files (.js)
            CompressFiles(remainingFiles);
        }

        private static void RemoveDirectoriesToExclude(string destReleaseFolder)
        {            
            string[] allDirs = Directory.GetDirectories(destReleaseFolder, "*", SearchOption.AllDirectories);
            foreach (var folder in allDirs)
            {
                string dirName = new DirectoryInfo(folder).Name;
                if (excludedFolders.Contains(dirName))
                {
                    Directory.Delete(folder, true);
                }
            }
        }

        private static string[] AllFiles(string folderWithSubfolders)
        {
            return Directory.GetFiles(folderWithSubfolders, "*", SearchOption.AllDirectories);
        }

        private static List<string> RemoveFilesToExcludeAndGetRemainder(string[] files)
        {
            List<string> remainingFiles = new List<string>();
            foreach (var file in files)
            {
                string filename = Path.GetFileName(file);
                if (excludedFiles.Contains(filename))
                {
                    File.Delete(file);
                }
                else
                {
                    remainingFiles.Add(file);
                }
            }

            return remainingFiles;
        }

        /// <summary>
        ///  True if this is a .js file
        /// </summary>
        private static bool CanMinify(string file)
        {
            return Path.GetExtension(file) == ".js";
        }

        private static string EscapeFilePath(string inputPath)
        {
            return string.Format("\"{0}\"", inputPath);
        }

        private static void CompressFiles(List<string> files)
        {
            foreach (var file in files)
            {
                if (CanMinify(file))
                {
                    string filename = Path.GetFileName(file);
                    string tempOutput = Path.Combine(Path.GetDirectoryName(minifierPath), filename);
                    string arguments = string.Format(@"-jar {0} --js {1} --js_output_file {2}", EscapeFilePath(minifierPath), EscapeFilePath(file), EscapeFilePath(tempOutput));
                    ProcessStartInfo psi = new ProcessStartInfo("java.exe", arguments);
                     psi.UseShellExecute = false;
                    psi.RedirectStandardOutput = true;
                    Process proc = new Process();
                    proc.StartInfo = psi;
                    proc.Start();
                    StreamReader myStreamReader = proc.StandardOutput;
                    // Read the standard output of the spawned process. 
                    string myString = myStreamReader.ReadLine();
                    Console.WriteLine(myString);
                    proc.WaitForExit();
                    proc.Close();                    

                    File.Delete(file);
                    File.Move(tempOutput, file);
                }
            }
        }
    }
}
