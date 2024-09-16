using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DriveAdviser.Core.Enums;
using DriveAdviser.Core.Extensions;
using DriveAdviser.Core.Interop;
using DriveAdviser.Core.Models;
using Serilog;

namespace DriveAdviser.Core
{
    public class DriveInfo : IDriveInfo, IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }


        public async Task<List<Drive>> GetDrives()
        {
            Log.Debug("Initializing Smart");
            var HardDrives = new List<Drive>();
            try
            {
                SysInfoApi.SysInfoDll_Smart_SetInitTimeout(20000);
                // ("smart ready {0}",SmartReady);
                await Task.Run(() => SysInfoWrapper.InitializeSmart()).ConfigureAwait(false);
                

                for (
                    var i =
                        await Task.Run(() => SysInfoApi.SysInfoDll_Smart_GetSmartDeviceIDFirst()).ConfigureAwait(false);
                    i >= 0;
                    i = await Task.Run(() => SysInfoApi.SysInfoDll_Smart_GetSmartDeviceIDNext()).ConfigureAwait(false))
                {
                    var drive = new Drive
                    {
                        DeviceId = i,
                        Model = await Task.Run(() => GetModelNumber(i)),
                        Serial = await Task.Run(() => GetSerialNumber(i)),
                        DriveLetter = await Task.Run(() => GetDriveLetterById(i)),
                        Attributes = await Task.Run(() => GetSmartAttributesForDrive(i)),
                        TotalCapacity = await Task.Run(() => GetCapactiyForDrive(i)),
                        
                        FreeSpace = await Task.Run( () =>
                        {
                            try
                            {
                                if (System.IO.DriveInfo.GetDrives()[i].IsReady)
                                {
                                    return Task.FromResult(
                                        (int) (System.IO.DriveInfo.GetDrives()[i].TotalFreeSpace / 1048576L));

                                }
                                return Task.FromResult(0);
                            }
                            catch (Exception e)
                            {
                                return Task.FromResult(0);
                            }
                            
                        }),
                        DriveType = await Task.Run(() => GetDriveType(i))

                    };
                    //We get the temp separately because we need to initialize the attributes first
                    //drive.Tempurature = (from t in drive.Attributes
                    //    where t.Id == 194 ||
                    //          t.Id == 190
                    //    select t.Raw.ToHex().ToString()).FirstOrDefault();
                    drive.Tempurature =
                        drive.Attributes.Where(t => t.Id == 190 || t.Id == 194 || (t.Id==1 && t.Name.ToUpper().Contains("TEMPERATURE")))
                            .Select(t => t.Raw.ToString()).FirstOrDefault();



                    foreach (var attribute in drive.Attributes)
                        if (CriticalSmartAttributes.CriticalAttributes.ContainsKey(attribute.Id)
                            && (string.IsNullOrWhiteSpace(CriticalSmartAttributes.CriticalAttributes[attribute.Id].Name) 
                            || CriticalSmartAttributes.CriticalAttributes[attribute.Id].Name.ToUpper() == attribute.Name.ToUpper()))
                            drive.HealthPercentage = CalculateHealth(drive.HealthPercentage, attribute.Raw,
                                CriticalSmartAttributes.CriticalAttributes[attribute.Id].AttributeWeight,
                                CriticalSmartAttributes.CriticalAttributes[attribute.Id].PercentageLimit,
                                CriticalSmartAttributes.CriticalAttributes[attribute.Id].StartValue);


                    HardDrives.Add(drive);
                }
                SysInfoApi.SysInfoDll_Smart_Cleanup();
                return HardDrives;

            }
            catch (Exception e)
            {
                Log.Information(e, "Getting Drives");
                return HardDrives;
            }
         
            
        }

        public Task<SysInfoReturnCode> RefreshSmart()
        {
            throw new NotImplementedException();
        }

        public void SmartCleanup()
        {
            throw new NotImplementedException();
        }

        public static string GetDriveType(int deviceId)
        {
            using (var partitionSearcher = new ManagementObjectSearcher(
                @"\\localhost\ROOT\Microsoft\Windows\Storage",
                $"SELECT MediaType FROM MSFT_PHYSICALDISK WHERE DeviceID='{deviceId}'"))
            {
                var partition = partitionSearcher.Get().Cast<ManagementBaseObject>().Single();

                return partition.GetPropertyValue("MediaType").ToString() == "4" ? "SSD" : "HDD";
            }
        }
    

        public async Task<SysInfoReturnCode> InitializeSmartAsync()
        {
            return await Task.Run(() => SysInfoWrapper.InitializeSmart()).ConfigureAwait(false);
        }

        public Task<List<SmartAttributes>> ReadSmartAsync(int deviceId)
        {
            throw new NotImplementedException();
        }


        public Task<SysInfoReturnCode> InitializeSmart()
        {
            throw new NotImplementedException();
        }


        private static List<SmartAttributes> GetSmartAttributesForDrive(int deviceID)

        {
            var attributesList = new List<SmartAttributes>();
            var buffer = 256;
            var tmpAttributeValue = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(ulong)));

            var tempBuf = Marshal.AllocCoTaskMem(buffer);
            var bufferClear = Marshal.SizeOf(typeof(ulong));
            var isValid = SysInfoApi.SysInfoDll_Smart_IsValidSmartDevice(deviceID);
            Log.Information("Valid SMART check for drive {0} returned {1}",deviceID,isValid);
            if (isValid == SysInfoReturnCode.SmartStatusSuccess)
                for (var k = SysInfoApi.SysInfoDll_Smart_GetSmartAttributeIDFirst(deviceID);
                    k >= 0;
                    k = SysInfoApi.SysInfoDll_Smart_GetSmartAttributeIDNext(deviceID))
                {
                    
                    var attributes = new SmartAttributes();
                    var tempBufLength = buffer;
                     Log.Debug("Attribute {attr}",k);
                    SysInfoApi.SysInfoDll_Smart_GetSmartAttributeByID(tempBuf, out tempBufLength, k,
                        (int) AttributeType.TextDescription, deviceID);
                    // Log.Debug("Name");
                    Helpers.Interop.ZeroMemoryPointer(tmpAttributeValue, bufferClear);

                    SysInfoApi.SysInfoDll_Smart_GetSmartAttributeByID(tmpAttributeValue, out tempBufLength, k,
                        (int) AttributeType.NormalizedValue, deviceID);
                    //  Log.Debug("Normal");
                    attributes.Value = Marshal.ReadInt64(tmpAttributeValue);
                    Helpers.Interop.ZeroMemoryPointer(tmpAttributeValue, bufferClear);

                    SysInfoApi.SysInfoDll_Smart_GetSmartAttributeByID(tmpAttributeValue, out tempBufLength, k,
                        (int) AttributeType.WorstValue, deviceID);
                    // Log.Debug("worst");
                    attributes.Worst = Marshal.ReadInt64(tmpAttributeValue);
                    Helpers.Interop.ZeroMemoryPointer(tmpAttributeValue, bufferClear);

                    SysInfoApi.SysInfoDll_Smart_GetSmartAttributeByID(tmpAttributeValue, out tempBufLength, k,
                        (int) AttributeType.ThresholdValue, deviceID);
                    // Log.Debug("Thresh");
                    attributes.Threshold = Marshal.ReadInt64(tmpAttributeValue);
                    Helpers.Interop.ZeroMemoryPointer(tmpAttributeValue, bufferClear);

                    SysInfoApi.SysInfoDll_Smart_GetSmartAttributeByID(tmpAttributeValue, out tempBufLength, k,
                        (int) AttributeType.RawValue, deviceID);
                    // Log.Debug("Raw");
                  

                    attributes.Id = k;
                    attributes.Name = Marshal.PtrToStringUni(tempBuf);
                    //Temperature needs to be changed to hex
                   
                    
                    attributes.Raw = (k == 190 || k == 194 )
                        ? Marshal.ReadInt64(tmpAttributeValue).ToHex()
                        : Marshal.ReadInt64(tmpAttributeValue);
                    if (k == 1 && attributes.Name.ToUpper().Contains("TEMPERATURE"))
                    {
                        attributes.Raw = Marshal.ReadInt64(tmpAttributeValue) -273;
                    }
#if DEBUG
                    Console.WriteLine(
                        $"{attributes.Id} {attributes.Name} {Marshal.ReadInt64(tmpAttributeValue)} {attributes.Raw.ToHex()}");

#endif
                    Helpers.Interop.ZeroMemoryPointer(tmpAttributeValue, bufferClear);
                    attributesList.Add(attributes);
                }
            return attributesList;
        }

        private static int CalculateHealth(int healthPercent, long attributeValue, float attributeWeight,
            int percentageLimit, int startValue)
        {
            //This makes sure that the value is not too low to consider
            if (attributeValue < startValue) return healthPercent;

            //Every Drive should start at 100% then we calcualte health from there
            //We then test the attribute to see if it will affect the drive
            var degradationpercent = Math.Abs((100 - attributeValue * attributeWeight) / 100);

            //We then take out current health times the degraded health 
            var calculatePercent = healthPercent * degradationpercent;

            //Now we check and me sure the value did not go beyond the limit.
            // if the percentageLinit is 70 then the greatest amount the health can be drop is 70%
            if (healthPercent - calculatePercent > percentageLimit || healthPercent - calculatePercent < 0)
                return healthPercent * (100 - percentageLimit) / 100;


            if (calculatePercent < 0)
                calculatePercent = 0;
            //We return our final calculated percentage
            return (int) calculatePercent;
        }

        private static string GetSerialNumber(int deviceId)
        {
            var buffer = 256;
            var serialBuffer = Marshal.AllocCoTaskMem(buffer);

            SysInfoApi.SysInfoDll_Smart_GetDeviceSerialNumber(serialBuffer, out buffer, deviceId);
            var serial = Marshal.PtrToStringUni(serialBuffer);
            Marshal.FreeCoTaskMem(serialBuffer);
            return serial;
        }

        private static string GetModelNumber(int deviceId)
        {
            var buffer = 256;
            var modelBuffer = Marshal.AllocCoTaskMem(buffer);

            SysInfoApi.SysInfoDll_Smart_GetDeviceModelNumber(modelBuffer, out buffer, deviceId);
            var serial = Marshal.PtrToStringUni(modelBuffer);
            Marshal.FreeCoTaskMem(modelBuffer);
            return serial;
        }

        private static int GetCapactiyForDrive(int deviceId)
        {
            var buffer = Marshal.AllocCoTaskMem(256);
            SysInfoApi.SysInfoDll_Smart_GetDeviceCapacity(buffer, deviceId);
            var size = Marshal.PtrToStructure<int>(buffer);
            Marshal.FreeCoTaskMem(buffer);
            return size;
        }

        private static string GetDriveLetterById(int deviceId)
        {
            var integerArray = new int[10];
            var letter = '\0';
            var numberOfPartitions = 10;
            if (numberOfPartitions <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfPartitions));
            var errorCode = SysInfoApi.SysInfoDll_Smart_GetDevicePartitionInfo(integerArray, out numberOfPartitions,
                deviceId);

            if (errorCode != 0)
                throw new ArgumentException(errorCode.ToString());
            if (numberOfPartitions == 0)
                return "NA";
            SysInfoApi.SysInfoDll_Smart_GetDriveLetterFromDriveNum(ref letter, integerArray[0]);

            return char.ToString(letter);
        }
    }
}