using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using DriveAdviser.Core.Extensions;
using DriveAdviser.Core.Models;
using DriveAdviser.Helpers;
using DriveAdviser.Services;
using Newtonsoft.Json;
using Serilog;

namespace DriveAdviser.Core.Services
{
    public class DriveAdviserApi
    {
        public ServerJsonResult SaveUser(string emailAddress)
        {
            if (!Network.HasInternetConnection) return new ServerJsonResult();
            var content =
                  new
                  {
                      username = emailAddress,
                      password = NumberGen.CreateRandomNumber(32),
                      email = emailAddress
                  };
            var response = string.Empty;
            try
            {
              
                var dataContent = JsonConvert.SerializeObject(content);
                using (var client = new HttpClient())
                {
                     response =
                        client.GetStringAsync("http://api.driveadviser.com/Api/saveUser?data=" + dataContent).Result;
                    var result = JsonConvert.DeserializeObject<ServerJsonResult>(response);

                    return result;
                }
            }
            catch (Exception e)
            {
                var api = new DriveAdviserApi();
                var settings = new Settings();

                api.LogException(e.Message, e.StackTrace, e.InnerException?.Message, e.InnerException?.StackTrace,
                    settings.GetSettingValueString(Headers.Computer, Names.Id),$"Sent:{content}");
                Log.Warning(e, "Unable to SaveUser");
                return new ServerJsonResult();
            }
           
        }

        public async Task<ServerJsonResult> SaveUserAsync(string emailAddress)
        {
            if (!Network.HasInternetConnection) return new ServerJsonResult();
            var content =
                  new
                  {
                      username = emailAddress,
                      password = NumberGen.CreateRandomNumber(32),
                      email = emailAddress
                  };
            var response = string.Empty;
            try
            {
           
                var dataContent = JsonConvert.SerializeObject(content);
                using (var client = new HttpClient())
                {
                    response = await 
                       client.GetStringAsync("http://api.driveadviser.com/Api/saveUser?data=" + dataContent).ConfigureAwait(false);
                    var result = JsonConvert.DeserializeObject<ServerJsonResult>(response);

                    return result;
                }
            }
            catch (Exception e)
            {
                var api = new DriveAdviserApi();
                var settings = new Settings();

                api.LogException(e.Message, e.StackTrace, e.InnerException?.Message, e.InnerException?.StackTrace,
                    settings.GetSettingValueString(Headers.Computer, Names.Id), $"Sent:{content} {Environment.NewLine} Received:{response}");
                Log.Warning(e,"Unable to SaveUserAsync");
                return new ServerJsonResult();
            }
        }

        public ServerJsonResult SendAlert(string emailAddress, string driveletter, int healthPercentage,
            string computerId)
        {
            var content =
                   new
                   {
                       username = emailAddress,
                       type = 2,
                       driveLetter = driveletter,
                       driveHealth = healthPercentage,
                       computerID = computerId
                   };
            var response = string.Empty;
            try
            {
               
                var dataContent = JsonConvert.SerializeObject(content);
                using (var client = new HttpClient())
                {
                    Console.WriteLine($@"{Constants.DriveAdviserApi}/email?data={dataContent}");
                     response =
                        client.GetStringAsync($@"{Constants.DriveAdviserApi}/email?data={dataContent}").Result;
                    var result = JsonConvert.DeserializeObject<ServerJsonResult>(response);

                    return result;
                }
            }
            catch (Exception e)
            {
                var api = new DriveAdviserApi();
                var settings = new Settings();
                Log.Warning(e,"Error while Sending Email");
                api.LogException(e.Message, e.StackTrace, e.InnerException?.Message, e.InnerException?.StackTrace,
                    settings.GetSettingValueString(Headers.Computer, Names.Id), $"Sent:{content} {Environment.NewLine} Received:{response}");
                return new ServerJsonResult();
            }
        }

        public ServerJsonResult CheckIp(string Ip)
        {
            try
            {
                var content = new
                {
                    ip = Ip
                };
                var dataContent = JsonConvert.SerializeObject(content);
                using (var client = new HttpClient())
                {
                    var response = client.GetStringAsync($@"{Constants.SchrockApi}/checkIp?data={dataContent}").Result;
                    var result = JsonConvert.DeserializeObject<ServerJsonResult>(response);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new ServerJsonResult();
            }
        }

        public ServerJsonResult SaveComputer(string emailAddress, string cpu, long hardDriveCapacity, string os,
            string cpuSpeed, string ram)
        {
            var content = new
            {
                username = emailAddress,
                cpu,
                hddCapacity = hardDriveCapacity,
                os,
                cpuSpeed,
                ramCapacity = ram,
                wkey = HardwareId.GetID(),
                computerName = Environment.MachineName
            };
            var response = string.Empty;
            try
            {
            
                var dataContent = JsonConvert.SerializeObject(content);
                using (var client = new HttpClient())
                {
                    Console.WriteLine($@"{Constants.DriveAdviserApi}/saveComputer?data={dataContent}");
                     response =
                        client.GetStringAsync($@"{Constants.DriveAdviserApi}/saveComputer?data={dataContent}").Result;
                    var result = JsonConvert.DeserializeObject<ServerJsonResult>(response);

                    return result;
                }
            }
            catch (Exception e)
            {
                var api = new DriveAdviserApi();
                var settings = new Settings();
                Log.Warning(e,"Unable to Save Computer");
                api.LogException(e.Message, e.StackTrace, e.InnerException?.Message, e.InnerException?.StackTrace,
                    settings.GetSettingValueString(Headers.Computer, Names.Id), $"Sent:{content} {Environment.NewLine} Received:{response}");
                return new ServerJsonResult();
            }
        }

        public async Task<ServerJsonResult> SaveComputerAsync(string emailAddress, string cpu, long hardDriveCapacity,
            string Os,
            string cpuSpeed, string ram)
        {
            var content = new
            {
                username = emailAddress,
                cpu,
                hddCapacity = hardDriveCapacity,
                os = Os,
                cpuSpeed,
                ramCapacity = ram,
                wkey = HardwareId.GetID(),
                computerName = Environment.MachineName
            };
            var response = string.Empty;
            try
            {
               
                var dataContent = await Task.Run(() => JsonConvert.SerializeObject(content));
                using (var client = new HttpClient())
                {
                    Console.WriteLine($@"{Constants.DriveAdviserApi}/saveComputer?data={dataContent}");
                    response = await
                        client.GetStringAsync($@"{Constants.DriveAdviserApi}/saveComputer?data={dataContent}");
                    var result = await Task.Run(() => JsonConvert.DeserializeObject<ServerJsonResult>(response));

                    return result;
                }
            }
            catch (Exception e)
            {
                var api = new DriveAdviserApi();
                var settings = new Settings();
                Log.Warning(e,"Unable to SaveComputerAsync");
                api.LogException(e.Message, e.StackTrace, e.InnerException?.Message, e.InnerException?.StackTrace,
                    settings.GetSettingValueString(Headers.Computer, Names.Id), $"Sent:{content} {Environment.NewLine} Received:{response}");
                return new ServerJsonResult();
            }
        }

        public async Task<ServerJsonResult> GetCustomerBySiTag(string siTag)
        {
            var content = new
            {
                siTag
            };
            var response = string.Empty;
            try
            {
              
                var dataContent = JsonConvert.SerializeObject(content);
                using (var client = new HttpClient())
                {
                    Console.WriteLine($@"{Constants.SchrockApi}/getCustomerBySiTag?data={dataContent}");
                    response =
                        await client.GetStringAsync($@"{Constants.SchrockApi}/getCustomerBySiTag?data={dataContent}");
                    var result = JsonConvert.DeserializeObject<ServerJsonResult>(response);

                    return result;
                }
            }
            catch (Exception e)
            {
                // Console.WriteLine(ex.ToString());
                var api = new DriveAdviserApi();
                var settings = new Settings();
               
                api.LogException(e.Message, e.StackTrace, e.InnerException?.Message, e.InnerException?.StackTrace,
                    settings.GetSettingValueString(Headers.Computer, Names.Id), $"Sent:{content} {Environment.NewLine} Received:{response}");
                Log.Warning(e,"Unable to get customer");
                return new ServerJsonResult
                {
                    All = new All {FirstName = "a", LastName = "a", Model = "a", Serial = "a", Email = "a"}
                };
            }
        }

        public async Task<ServerJsonResult> SaveDriveAsync(string emailAddress, string driveLetter, int healthPercentage,
            string serialNumber, string model)
        {
            var content = new
            {
                username = emailAddress,
                wkey = HardwareId.GetID(),
                driveLetter,
                driveHealth = healthPercentage,
                serial = serialNumber,
                model
            };
            var response = string.Empty;
            Log.Debug("Save Drive");
            try
            {
                var dataContent = await Task.Run(() => JsonConvert.SerializeObject(content, Formatting.Indented));
                Log.Debug(dataContent);

                using (var client = new HttpClient())
                {
                    Console.WriteLine($@"{Constants.DriveAdviserApi}/saveComputerRecord?data={dataContent}");
                    response = await
                        client.GetStringAsync($@"{Constants.DriveAdviserApi}/saveComputerRecord?data={dataContent}");
                    var result = await Task.Run(() => JsonConvert.DeserializeObject<ServerJsonResult>(response));
                   // Log.Debug("{Json}", response.ToFormattedJson());
                    return result;
                }
            }
            catch (Exception e)
            {
                var api = new DriveAdviserApi();
                var settings = new Settings();
                Log.Warning(e,"Savedriveasync");
                api.LogException(e.Message, e.StackTrace, e.InnerException?.Message, e.InnerException?.StackTrace,
                    settings.GetSettingValueString(Headers.Computer, Names.Id), $"Sent:{content} {Environment.NewLine} Received:{response}");
                return new ServerJsonResult();
            }
        }

        public ServerJsonResult CreateDatabaseRelation(string siTag, string computerId)
        {
            var content = new
            {
                siTag,
                id_computer = computerId
            };
            var response = string.Empty;
            try
            {
              
                var dataContent = JsonConvert.SerializeObject(content);
                using (var client = new HttpClient())
                {
                    Console.WriteLine($@"{Constants.DriveAdviserApi}/doubleVerifyCustomerInfo?data={dataContent}");
                    response =
                        client.GetStringAsync(
                            $@"{Constants.DriveAdviserApi}/doubleVerifyCustomerInfo?data={dataContent}").Result;
                    var result = JsonConvert.DeserializeObject<ServerJsonResult>(response);

                    return result;
                }
            }
            catch (Exception e)
            {
                var api = new DriveAdviserApi();
                var settings = new Settings();
                Log.Warning(e,"Relation");
                api.LogException(e.Message, e.StackTrace, e.InnerException?.Message, e.InnerException?.StackTrace,
                    settings.GetSettingValueString(Headers.Computer, Names.Id), $"Sent:{content} {Environment.NewLine} Received:{response}");
                return new ServerJsonResult();
            }
        }

        public void LogException(string message, string stackTrace, string innerMessage, string innerStackTrace,
            string computerId, string systemInfo="")
        {
            try
            {
                var content = new
                {
                    message,
                    stackTrace,
                    innerMessage,
                    innerStackTrace,
                    systemInfo,
                    idComputer = computerId
                };

                var dataContent = HttpUtility.UrlEncode(JsonConvert.SerializeObject(content));

                using (var client = new HttpClient())
                {
                    Console.WriteLine($@"{Constants.DriveAdviserApi}/insException?data={dataContent}");
                    var response =
                        client.GetStringAsync($@"{Constants.DriveAdviserApi}/insException?data={dataContent}").Result;
                    var result = JsonConvert.DeserializeObject<ServerJsonResult>(response);
                }
            }
            catch (Exception e)
            {
                Log.Warning(e,"Error while logging exception");
            }
        }
    }
}