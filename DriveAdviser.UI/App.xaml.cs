using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using DriveAdviser.Core.Helpers;
using DriveAdviser.Core.Services;
using DriveAdviser.Helpers;
using DriveAdviser.Services;
using DriveAdviser.UI.Schrock;
using Fclp;
using FluentScheduler;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Constants = DriveAdviser.Core.Constants;
using DriveInfo = DriveAdviser.Core.DriveInfo;

namespace DriveAdviser.UI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Mutex mutex = new Mutex(true, "{8EC2EDB7-23C0-4659-A3D2-F445F8A55E2F}");
        private readonly IApi api = new Api();
        private FluentCommandLineParser<CommandLine> cli;
        private MainWindow mainWindow;
        private Settings settings;

        protected override void OnStartup(StartupEventArgs e)

        {
            // DO NOT UPDATE THE NUGET PACKAGE FOR MAHAPPS METRO. THIS WILL BREAK THE UI.
            // WAIT FOR THE MaterialDesignThemes.MahApps PACKAGE TO BECOME AVAILABLE.
            Tray.RefreshTrayArea();
            //This checks for any other instance of Drive Adviser. It uses the static mutex above
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                //We see if we can find the handle for the window
                var handle = Interop.FindWindow(null,
                    $"Drive Adviser {Assembly.GetExecutingAssembly().GetName().Version}");
                if (handle == IntPtr.Zero)
                    return;
                //If the handle is visible we just bring it to the foreground
                if (Interop.IsWindowVisible(handle))
                    Interop.SetForegroundWindow(handle);
                //if the visibilty is not set that means we are running in the tray
                else
                    Interop.PostMessage((IntPtr) Interop.HWND_BROADCAST, Interop.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                // 

                //Interop.ShowWindow(handle, Restore);

                //MessageBox.Show("Drive Adviser is already running.");
                Current.Shutdown(0);
            }
            //This is a static logger used through the entire code. You can add files as needed
            if (Administrator.IsProcessOpen("Drive Adviser"))
            {
                //MessageBox.Show("Drive Adviser is already running.");

                Environment.Exit(0);
            }
            var levelSwitch = new LoggingLevelSwitch {MinimumLevel = LogEventLevel.Information};
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.File(Constants.LogFile,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                .WriteTo.File($@"{Constants.LogDir}\Exceptions.txt", LogEventLevel.Warning,
                    "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            cli = new FluentCommandLineParser<CommandLine>();

            cli.Setup(arg => arg.Tray).As("tray");
            cli.Setup(arg => arg.Debug).As("Debug").SetDefault(false);
            cli.Setup(arg => arg.EmailAddress).As("email");
            cli.Setup(arg => arg.Drive).As("drive");

            var result = cli.Parse(e.Args);
            if (result.HasErrors)
                Log.Information(result.ErrorText);

            
            levelSwitch.MinimumLevel = LogEventLevel.Debug;
            //This is only being kept around until mutex is verified to be working on machines
            if (!Dependency.IsVcRedisInstalled())
            {
                Log.Debug("Installing Dependency");
                Log.Debug("Install Code: {Code}", Dependency.DownloadandInstallVc().ToString());
            }
            if (!string.IsNullOrWhiteSpace(cli.Object.EmailAddress))
            {
                var settings = new Settings();
                settings.UpdateAndCreateSetting(Headers.User, Names.EmailAddress, cli.Object.EmailAddress);
                var userStatus =  new DriveAdviserApi().SaveUserAsync(cli.Object.EmailAddress).GetAwaiter().GetResult();
            }
            if (!string.IsNullOrWhiteSpace(cli.Object.Drive))
            {
                var drives = new DriveInfo();
               var drive =  drives.GetDrives().GetAwaiter().GetResult().FirstOrDefault(x => x.DriveLetter.Contains(cli.Object.Drive));
                Log.Information("{drive}",drive?.HealthPercentage);
                
                Console.Out.WriteLine(drive?.HealthPercentage);
              
                //Environment.Exit(0); 
            }
            JobManager.Initialize(new Registry());


            settings = new Settings();


            Log.Information("Starting {ProgramName}  Version {version} Build {BuildType}", Constants.ProgramName,
                Assembly.GetExecutingAssembly().GetName().Version, Settings.GetBuildType());

            //This sets the update task to run every six hours.
            //Nonreentrant means this task will not run concurrently with other tasks.
            JobManager.AddJob(() => UpdateService.Update(Assembly.GetExecutingAssembly().GetName().Version),
                v => v.NonReentrant().ToRunNow().AndEvery(6).Hours());

            //Here we are making sure that vcredist is installed. If not then we download and install it.
            // todo Move to wixsharp installer.     
         

      
            // We need check if we are in a SI Service and popup the dislplay to make the customer a schrock customer.
            if (Network.HasInternetConnection &&
                api.CheckForSchrockInnovationsIp(Network.GetIpAddress()).Result.Success &&
                Settings.GetBuildType() == BuildType.Public)
            {
                Log.Information("Schrock Innovations IP Detected");

                var tag = new TagDisplay {DataContext = new TagDisplayViewModel()};
                Log.Information("Launching Tag Display");
                tag?.Show();
            }
            else
            {
                //We do not need to set the data context becasue that is set in the ViewModelLocator
                mainWindow = new MainWindow();
                if (cli.Object.Tray)
                {
                    Log.Information("Argument found Starting in Tray");
                    mainWindow.Visibility = Visibility.Hidden;
                }
                else
                {
                    mainWindow.Show();
                    Log.Information("Launching Main Window");
                }
            }


            TaskScheduler.CreateTask();
        }


        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Warning(e.Exception,"Unhandle Exception");

            //var exception =
            //    api.LogExceptionAsync(e.Exception.Message, e.Exception.StackTrace, e.Exception.InnerException?.Message,
            //            e.Exception.InnerException?.StackTrace,
            //            settings.GetSettingValueString(Headers.Computer, Names.Id))
            //        .Result;
            //Log.Information($"Exception saved = {exception.Success}");
            e.Handled = true;
            if (mainWindow != null) return;

            mainWindow = new MainWindow();
            if (cli.Object.Tray)
            {
                Log.Debug("Tray Enabled");
                mainWindow.Visibility = Visibility.Hidden;
            }
            else
            {
                mainWindow.Show();
                Log.Information("Launching Main Window");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}