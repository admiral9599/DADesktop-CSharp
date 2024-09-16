using DriveAdviser.Core.Models;
using DriveAdviser.Core.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace DriveAdviser.Services
{
    public class AlertService
    {
        public ServerJsonResult SendAlert(string emailAddress, string driveletter, int healthPercentage)
        {
            try
            {
                var content =
                    new
                    {
                        email = emailAddress,
                        type = 2,
                        driveLetter = driveletter,
                        driveHealth = healthPercentage,
                        computer = Environment.MachineName
                    };
                var dataContent = JsonConvert.SerializeObject(content);
                using (var client = new HttpClient())
                {
                    var response =
                        client.GetStringAsync("http://api.driveadviser.com/Api/email?data=" + dataContent).Result;
                    var result = JsonConvert.DeserializeObject<ServerJsonResult>(response);

                    return result;
                }
            }
            catch (Exception e)
            {
                var api = new DriveAdviserApi();
                var settings = new Settings();

                api.LogException(e.Message, e.StackTrace, e.InnerException?.Message, e.InnerException?.StackTrace, settings.GetSettingValueString(Headers.Computer, Names.Id));
                return new ServerJsonResult();
            }
        }
    }
}