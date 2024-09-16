using System;
using System.IO;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;

using WixSharpSetup.Dialogs;
using File = WixSharp.File;

namespace WixSharpSetup
{
    class Program
    {
        static void Main()
        {
#if DEBUG
            var build = "Debug";
#else
            var build = "Release";
#endif

            try
            {
               
                Tasks.DigitalySign($@"..\DriveAdviser.UI\bin\{build}\Drive Adviser.exe", "SchrockInnovations.pfx",
                    "http://timestamp.comodoca.com/rfc3161", "Innovate201", "/d \"Drive Adviser\" /fd SHA256",  wellKnownLocations: @"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86");
                var project = new ManagedProject("Drive Adviser",
                    new Dir(@"%ProgramFiles%\Drive Adviser",
                        new File($@"..\DriveAdviser.UI\bin\{build}\Drive Adviser.exe",
                            new FileShortcut("Drive Adviser", @"%Desktop%"),
                            new FileShortcut("Drive Adviser", @"%ProgramMenu%")),
                        new DirFiles($@"..\DriveAdviser.UI\bin\{build}\*.dll"),
                        new File($@"..\DriveAdviser.UI\bin\{build}\SysInfo_localization.txt"),
                        new File(
                            $@"..\DriveAdviserUpdate\bin\{build}\DriveAdviserUpdate.exe")),
                    new Dir(@"%CommonAppDataFolder%\Drive Adviser")
                );


                project.InstallScope = InstallScope.perMachine;

                project.ControlPanelInfo.Manufacturer = "Schrock Innovations";
                project.GUID = new Guid("4FD75CE1-3FED-4229-ABB6-39F8E67F8A48");
                project.ManagedUI = new ManagedUI();
                project.ProductId = Guid.NewGuid();
                project.SetVersionFromFile($@"..\DriveAdviser.UI\bin\{build}\Drive Adviser.exe");

                //project.ControlPanelInfo.ProductIcon = @"..\DriveAdviser.UI\images\Green32.ico";


                project.InstallerVersion = 300;
                project.LicenceFile = "license.rtf";
                project.ControlPanelInfo.ProductIcon = @"..\DriveAdviser.UI\images\Green32.ico";
                project.MajorUpgrade = new MajorUpgrade()
                {
                    AllowDowngrades = false,
                    AllowSameVersionUpgrades = true,

                    DowngradeErrorMessage =
                        "A later version of [ProductName] is already installed. Setup will now exit."
                };

                //project.UI = WUI.WixUI_Minimal;
                Console.WriteLine("UI first");

                project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                    .Add<LicenceDialog>()
                    .Add<ProgressDialog>()
                    .Add<ExitDialog>();
                Console.WriteLine("UI first");
                project.ManagedUI.ModifyDialogs.Add<MaintenanceTypeDialog>()
                    .Add<ProgressDialog>()
                    .Add<ExitDialog>();


                var msi = Compiler.BuildMsi(project, $"DriveAdviser{build}{project.Version}.msi");

                Tasks.DigitalySign(msi, "SchrockInnovations.pfx", "http://timestamp.comodoca.com/rfc3161",
                    "Innovate201", "/d \"Drive Adviser\" /fd SHA256", wellKnownLocations: @"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86");
                //Console.Read();
                //var bootstrapper = new Bundle("Drive Adviser",


                //    new MsiPackage(msi)
                //    {
                //        Id = "MainMsi",
                //        DisplayInternalUI = true

                //    });

                //bootstrapper.Manufacturer = "Schrock Innovations";
                //
                //bootstrapper.Application = new SilentBootstrapperApplication("MainMsi");
                //bootstrapper.Version = Tasks.GetVersionFromFile(msi);

                //bootstrapper.UpgradeCode = new Guid("4FD75CE1-3FED-4229-ABB6-39F8E67F8A48");
                //bootstrapper.Build("Drive Adviser.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}