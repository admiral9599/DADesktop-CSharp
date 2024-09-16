using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DriveAdviser.Core.Extensions;
using DriveAdviser.Core.Models;
using DriveAdviser.Helpers;
using DriveAdviser.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace DriveAdviser.Core.Services
{
    public class Api : IApi
    {
        public async Task<ServerJsonResult> SaveUserAsync(string emailAddress)
        {
            if (await Task.Run(() => !Network.HasInternetConnection).ConfigureAwait(false))
                return new ServerJsonResult() {Msg = "No Internet Connection Detected"};
            var json = new
            {
                username = emailAddress,
                password = NumberGen.CreateRandomNumber(32),
                email = emailAddress
            };
            var response = string.Empty;
            try
            {

                var encodedJson = await Task.Run(() => HttpUtility.UrlEncode(JsonConvert.SerializeObject(json))).ConfigureAwait(false);
#if DEBUG
                Console.WriteLine($@"{Constants.DriveAdviserApi}/saveUser?data={encodedJson}");
#endif

                using (var client = new HttpClient())
                {
                    response =
                        await client.GetStringAsync($@"{Constants.DriveAdviserApi}/saveUser?data={encodedJson}");
                    
                    return await Task.Run(() => JsonConvert.DeserializeObject<ServerJsonResult>(response)).ConfigureAwait(false);

                }


            }
            catch (Exception e)
            {

                await LogExceptionAsync(e, $"Sent: {json} Received: {response.ToFormattedJson()}");
                Log.Warning(e,"Saving User");
                Log.Debug($"Sent: { JsonConvert.SerializeObject(json, Formatting.Indented)} Received: { response.ToFormattedJson()}");
                return new ServerJsonResult() {};
            }

        }

        public async Task<ServerJsonResult> SaveComputerInfoAsync(string emailAddress)
        {
            await SystemInfo.GetSystemInfoAsync();
            var json = new
            {
                username = emailAddress,
                cpu = SystemInfo.CpuName,
                hddCapacity = SystemInfo.HardDriveSize,
                os = SystemInfo.OsName,
                cpuSpeed=SystemInfo.CpuSpeed,
                ramCapacity = SystemInfo.RamCapacity,
                wkey = HardwareId.GetID(),
                computerName = Environment.MachineName
            };
            var response = string.Empty;
            try
            {

                var encodedJson= await Task.Run(() => JsonConvert.SerializeObject(json)).ConfigureAwait(false);
                using (var client = new HttpClient())
                {
#if DEBUG
                    Console.WriteLine($@"{Constants.DriveAdviserApi}/saveComputer?data={encodedJson}");
                    
#endif
                    response = await
                        client.GetStringAsync($@"{Constants.DriveAdviserApi}/saveComputer?data={encodedJson}");
                    return await Task.Run(() => JsonConvert.DeserializeObject<ServerJsonResult>(response)).ConfigureAwait(false);

                   
                }
            }
            catch (Exception e)
            {
               
                await LogExceptionAsync(e, $"Sent: {json} Received: {response.ToFormattedJson()}");
                Log.Warning(e, "Saving User");
                Log.Debug($"Sent: { JsonConvert.SerializeObject(json, Formatting.Indented)} Received: { response.ToFormattedJson()}");
                return new ServerJsonResult();
            }
        }

        public async Task<ServerJsonResult> GetCustomerBySiTagAsync(string siTag)
        {
            var json = new
            {
                siTag
            };
            var response = string.Empty;
            try
            {

                var encodedJson = await Task.Run(() => JsonConvert.SerializeObject(json)).ConfigureAwait(false);
                using (var client = new HttpClient())
                {
#if DEBUG
                    Console.WriteLine($@"{Constants.DriveAdviserApi}/getCustomerBySiTag?data={encodedJson}");

#endif
                    response =
                        await client.GetStringAsync($@"{Constants.SchrockApi}/getCustomerBySiTag?data={encodedJson}");
                    return await Task.Run(() => JsonConvert.DeserializeObject<ServerJsonResult>(response)).ConfigureAwait(false);

                   
                }
            }
            catch (Exception e)
            {
                await LogExceptionAsync(e, $"Sent: {json} Received: {response.ToFormattedJson()}");
                Log.Warning(e, "Saving User");
                Log.Debug($"Sent: { JsonConvert.SerializeObject(json, Formatting.Indented)} Received: { response.ToFormattedJson()}");
                return new ServerJsonResult();
            }
        }

        public async Task<ServerJsonResult> SaveDriveAsync(string emailAddress, string driveLetter, int healthPercentage,
            string serialNumber, string model, int totalCapacity, int freeSpace,string driveType)
        {
            var json = new
            {
                username = emailAddress,
                wkey = HardwareId.GetID(),
                driveLetter,
                driveHealth = healthPercentage,
                serial = serialNumber,
                model,
                totalCapacity,
                freeSpace,
                driveType,
                computerDetail = new
                {

                    cpu = SystemInfo.CpuName,
                    hddCapacity = SystemInfo.HardDriveSize,
                    os = SystemInfo.OsName,
                    dateCreated = DateTime.UtcNow,
                    buildVersion = SystemInfo.OsBuild,
                    cpuSpeed = SystemInfo.CpuSpeed,
                    ramCapacity = SystemInfo.RamCapacity,

                }

            };
            var response = string.Empty;
            Log.Debug("Save Drive");
            try
            {
                var encodedJson = await Task.Run(() => JsonConvert.SerializeObject(json, Formatting.Indented)).ConfigureAwait(false);
                

                using (var client = new HttpClient())
                {
#if DEBUG
                    Console.WriteLine($@"{Constants.DriveAdviserApi}/saveComputerRecord?data={encodedJson}");

#endif
                    response = await
                        client.GetStringAsync($@"{Constants.DriveAdviserApi}/saveComputerRecord?data={encodedJson}");
                    return await Task.Run(() => JsonConvert.DeserializeObject<ServerJsonResult>(response)).ConfigureAwait(false);
                    // Log.Debug("{Json}", response.ToFormattedJson());
                   
                }
            }
            catch (Exception e)
            {
                await LogExceptionAsync(e, $"Sent: {json} Received: {response.ToFormattedJson()}");
                Log.Warning(e, "Saving User");
                Log.Debug($"Sent: { JsonConvert.SerializeObject(json, Formatting.Indented)} Received: { response.ToFormattedJson()}");
                return new ServerJsonResult();
            }
        }

        public async Task<ServerJsonResult> EmailAlert(string emailAddress, string driveletter, int healthPercentage,
            string computerId)
        {

            var json =
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

                var encodedJson = await Task.Run(() => JsonConvert.SerializeObject(json)).ConfigureAwait(false);
                using (var client = new HttpClient())
                {
#if DEBUG
                    Console.WriteLine($@"{Constants.DriveAdviserApi}/email?data={encodedJson}");

#endif
                    response = await client.GetStringAsync($@"{Constants.DriveAdviserApi}/email?data={encodedJson}");
                    return await Task.Run(() => JsonConvert.DeserializeObject<ServerJsonResult>(response)).ConfigureAwait(false);

                }
            }
            catch (Exception e)
            {
                await LogExceptionAsync(e, $"Sent: {json} Received: {response.ToFormattedJson()}");
                Log.Warning(e, "Saving User");
                Log.Debug($"Sent: { JsonConvert.SerializeObject(json, Formatting.Indented)} Received: { response.ToFormattedJson()}");
                return new ServerJsonResult();
            }
        }

        public async Task<ServerJsonResult> CheckForSchrockInnovationsIp(string ip)
        {
            var json = new
            {
                ip
            };
            var response = string.Empty;
            try
            {
               
                var encodedJson = await Task.Run(() => JsonConvert.SerializeObject(json)).ConfigureAwait(false);
                using (var client = new HttpClient())
                {
#if DEBUG
                    Console.WriteLine($@"{Constants.SchrockApi}/checkIp?data={encodedJson}");

#endif
                    response = await client.GetStringAsync($@"{Constants.SchrockApi}/checkIp?data={encodedJson}");
                    return await Task.Run(() => JsonConvert.DeserializeObject<ServerJsonResult>(response)).ConfigureAwait(false);

                   
                }
            }
            catch (Exception e)
            {
               // await LogExceptionAsync(e, $"Sent: {json} Received: {response.ToFormattedJson()}");
                Log.Warning(e, "Saving User");
                Log.Debug($"Sent: { JsonConvert.SerializeObject(json, Formatting.Indented)} Received: { response.ToFormattedJson()}");
                return new ServerJsonResult();
            }
        }

        public async Task<ServerJsonResult> LogExceptionAsync(Exception exception,string systemInfo = "")
        {
            
            var settings = new Settings();
            var json = new
            {
                message = exception.Message,
                stackTrace = exception.StackTrace,
                innerMessage = exception.InnerException?.Message,
                innerStackTrace = exception.InnerException?.StackTrace,
                systemInfo,
                idComputer = settings.GetSettingValueString(Headers.Computer, Names.Id)
            };
            var response = string.Empty;
            try
            {
                var encodedJson = Task.Run(() => HttpUtility.UrlEncode(JsonConvert.SerializeObject(json))).ConfigureAwait(false);
                using (var client = new HttpClient())
                {
#if DEBUG
                    Console.WriteLine($@"{Constants.DriveAdviserApi}/insException?data={encodedJson}");

#endif
                    response = await 
                        client.GetStringAsync($@"{Constants.DriveAdviserApi}/insException?data={encodedJson}");

                    return await Task.Run(() => JsonConvert.DeserializeObject<ServerJsonResult>(response)).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Log.Warning(e, "Logging Exception");
                Log.Debug($"Sent: {JsonConvert.SerializeObject(json, Formatting.Indented)} Received: {response.ToFormattedJson()}");
              
                return new ServerJsonResult();
            }
        }

        public async Task<ServerJsonResult> LogExceptionAsync(string message, string stackTrace, string innerMessage, string innerStackTrace, string computerId,
            string systemInfo = "")
        {

            var settings = new Settings();
            var json = new
            {
                message,
                stackTrace,
                innerMessage,
                innerStackTrace,
                systemInfo,
                idComputer = settings.GetSettingValueString(Headers.Computer, Names.Id)
            };
            var response = string.Empty;
            try
            {
                var encodedJson = await Task.Run(() => HttpUtility.UrlEncode(JsonConvert.SerializeObject(json))).ConfigureAwait(false);
                using (var client = new HttpClient())
                {
#if DEBUG
                    Console.WriteLine($@"{Constants.DriveAdviserApi}/insException?data={encodedJson}");

#endif
                    response = await
                        client.GetStringAsync($@"{Constants.DriveAdviserApi}/insException?data={encodedJson}");

                    return await Task.Run(() => JsonConvert.DeserializeObject<ServerJsonResult>(response)).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Log.Warning(e, "Logging Exception");
                Log.Debug($"Sent: {JsonConvert.SerializeObject(json, Formatting.Indented)} Received: {response.ToFormattedJson()}");

                return new ServerJsonResult();
            }
        }
    }
}
