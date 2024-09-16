using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Serilog;

namespace DriveAdviser.Core.Helpers
{
    public static class Dependency
    {
        public static bool IsVcRedisInstalled()
        {
            var value = Registry.GetValue(Constants.VcLocation, "Installed",null);
            return value!=null && !string.Equals(value,"0");
        }

        public static int DownloadandInstallVc()
        {
            var filePath = Constants.IniFolder;
            try { 
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(@"https://driveadviser.s3-us-west-2.amazonaws.com/download/vcredist_x86.exe", $@"{filePath}\vcredist.exe");

            }
           
                Console.WriteLine(filePath);
                Console.WriteLine("starting");

                var install = new Process();

                //install.StartInfo.RedirectStandardOutput = true;
                //install.StartInfo.UseShellExecute = false;
                //install.StartInfo.CreateNoWindow = true;


                install.StartInfo.FileName = filePath+"\\vcredist.exe";
                install.StartInfo.Arguments = "/install /quiet /norestart";


                install.Start();

                Console.WriteLine("waiting");
                install.WaitForExit();

                return install.ExitCode;


            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                Log.Information(ex,"Redist");
                return 12345;

            }

           
        }
    }
}
