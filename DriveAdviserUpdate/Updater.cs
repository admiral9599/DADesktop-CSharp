using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace DriveAdviserUpdate
{
    public class Updater
    {
        public static int UpdateProgram(string FilePath, string Arguments)
        {
            try
            {
                Console.WriteLine(FilePath);
                Console.WriteLine("starting");
                
                var install = new Process();
                install.StartInfo.FileName = FilePath;
                install.StartInfo.Arguments = Arguments;


                install.Start();

                Console.WriteLine("waiting");
                install.WaitForExit();
               
                return install.ExitCode;


            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                return 12345;

            }
        }

        public static void DownloadUpdate(string Url)
        {
            //todo Use HttpClient Async Calls
            Console.WriteLine("Downloading");
            using (WebClient webClient = new WebClient())
            {
                
                webClient.DownloadFile(Url, $@"{Path.GetTempPath()}DriveAdviser.msi");

            }

           
        }
       
    }
}
