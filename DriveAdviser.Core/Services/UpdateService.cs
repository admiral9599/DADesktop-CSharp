using DriveAdviser.Core.Extensions;
using DriveAdviser.Core.Models;
using DriveAdviser.Core.Services;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using Serilog;

namespace DriveAdviser.Services
{
    public class UpdateService
    {
        public UpdateService(Version version)
        {
            CheckForUpdate(version);
        }

        public bool IsUpdateAvailable { get; set; }
        public string UpdateUrl { get; set; }

        private void CheckForUpdate(Version version)
        {
            //todo Use HttpClient Async Calls

            using (var webClient = new WebClient())
            {

                var response = webClient.DownloadString("http://api.driveadviser.com/index.php/api/CheckVersion");
                var result = JsonConvert.DeserializeObject<Update>(response);
                //var stuff = Assembly

                IsUpdateAvailable = result.Version.ToVersion() > version;
                UpdateUrl = result.Url;
            }

            //var updateText = File.ReadAllLines("myfile.txt");
            //var newVersion = updateText[0].ToVersion();



            //IsUpdateAvailable = newVersion > currentVersion;
            //UpdateUrl = updateText[1];
        }

        public void StartUpdate()
        {

            try
            {
                Process.Start(@"DriveAdviserUpdate.exe", UpdateUrl);
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                var api = new DriveAdviserApi();
                var settings = new Settings();

                api.LogException(e.Message, e.StackTrace, e.InnerException?.Message, e.InnerException?.StackTrace, settings.GetSettingValueString(Headers.Computer, Names.Id));
            }


        }

        public static void Update(Version version)
        {
                Log.Information("Checking for updates");
                using (var webClient = new WebClient())
                {
                    
                    var response = webClient.DownloadString("http://api.driveadviser.com/index.php/api/CheckVersion");
                    var result = JsonConvert.DeserializeObject<Update>(response);
                    //var stuff = Assembly


                    if (result.Version.ToVersion() <= version) return;
                    Process.Start(@"DriveAdviserUpdate.exe", result.Url);
                    Environment.Exit(0);
                }

            }
         
           

        }
    }
