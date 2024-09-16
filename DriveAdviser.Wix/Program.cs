using System;
using System.Diagnostics;
using System.IO;
using WixSharp;

namespace DriveAdviser.Wix
{
    class Program
    {
        static void Main()
        {
            try
            {
                ////Tasks.DigitalySign(@"..\DriveAdviser.UI\bin\Debug\Drive Adviser.exe", "SI.pfx", "http://timestamp.comodoca.com/rfc3161", "Innovate1", "/d \"Drive Adviser\" /fd SHA256");
                //var project = new ManagedProject("Drive Adviser",

                //    new Dir(@"%ProgramFiles%\Drive Adviser",
                //        new File(@"..\DriveAdviser.UI\bin\Debug\Drive Adviser.exe",
                //            new FileShortcut("Drive Adviser", @"%Desktop%")),
                //        new DirFiles(@"..\DriveAdviser.UI\bin\Debug\*.dll"),
                //            new File(@"C:\Users\Adam\Desktop\Git Projects\DriveAdviser\DriveAdviserUpdate\bin\Debug\DriveAdviserUpdate.exe")),
               
                //    new Dir(@"%CommonAppDataFolder%\Drive Adviser"));


                //project.InstallScope = InstallScope.perMachine;
                //project.ControlPanelInfo.Manufacturer = "Schrock Innovations";
                //project.GUID = new Guid("4FD75CE1-3FED-4229-ABB6-39F8E67F8A48");
                
                //project.ProductId = Guid.NewGuid();
                //project.SetVersionFromFile(@"..\DriveAdviser.UI\bin\Debug\Drive Adviser.exe");
                //project.SetNetFxPrerequisite("NETFRAMEWORK45 >= '#378389'","Please Install .NET 4.5.2");
                //project.ControlPanelInfo.ProductIcon = @"..\DriveAdviser.UI\images\tray.ico";
                //project.InstallerVersion = 300;
                //project.MajorUpgrade = new MajorUpgrade()
                //{
                //    AllowDowngrades = false,
                //    AllowSameVersionUpgrades = true,

                //    DowngradeErrorMessage =
                //        "A later version of [ProductName] is already installed. Setup will now exit."

                //};

                //project.UI = WUI.WixUI_Minimal;

                ////project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
                ////    .Add(Dialogs.Licence)
                ////    .Add(Dialogs.Progress)
                ////    ;

                ////project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)

                ////                      .Add(Dialogs.Progress)
                ////                      .Add(Dialogs.Exit);
                //project.AfterInstall += project_AfterInstall;
                //var msi = Compiler.BuildMsi(project);

                //var exitCode = Tasks.DigitalySign(msi, "SI.pfx", "http://timestamp.comodoca.com/rfc3161", "Innovate1", "/d \"Drive Adviser\" /fd SHA256");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }

        }

        static void project_AfterInstall(SetupEventArgs e)
        {
            try
            {
                if (e.IsInstalling)
                {
                    var app = System.IO.Path.Combine(e.Session.Property("INSTALLDIR"),
                        @"Drive Adviser.exe");


                  
                    var info = new ProcessStartInfo();
                    info.WorkingDirectory = Path.GetDirectoryName(app);
                    info.FileName = app;
                    info.Arguments = "/tray";
                    Process.Start(info);
                }
            }
            catch (Exception ex)
            {

                using (StreamWriter writer =
        new StreamWriter(@"C:\nimportant.txt"))
                {
                    writer.WriteLine(ex.ToString());

                }
            }
        }
    }
}