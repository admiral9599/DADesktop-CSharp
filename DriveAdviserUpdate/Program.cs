using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DriveAdviserUpdate
{
    class Program
    {
        static  void Main(string[] args)
        {
            var filePath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            try
            {
        //        using (StreamWriter writer =
        //new StreamWriter(@"C:\important.txt"))
        //        {
        //            writer.Write("Word ");
        //            writer.WriteLine("word 2");
        //            writer.WriteLine("Line");
        //        }
                if (args.Length < 1)
                {
                    Environment.Exit(1000);
                }
                Console.WriteLine(filePath);
                UpdateTray.ShowTray();
                Console.WriteLine(args[0]);
                Updater.DownloadUpdate(args[0]);
                
                var code = Updater.UpdateProgram(Path.GetTempPath() +"DriveAdviser.msi","/qn /norestart");
                Console.WriteLine(code);
                if (code == 0)
                {
                    var app = @ProgramFilesx86() + @"\Drive Adviser\Drive Adviser.exe";



                    var info = new ProcessStartInfo();
                    info.WorkingDirectory = Path.GetDirectoryName(app);
                    info.FileName = app;
                    info.Arguments = "/tray";
                    Process.Start(info);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                
            }
           
        }
        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }
    }
}
