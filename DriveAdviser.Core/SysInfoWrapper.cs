using DriveAdviser.Core.Enums;
using DriveAdviser.Core.Extensions;
using DriveAdviser.Core.Interop;
using DriveAdviser.Core.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DriveAdviser.Core
{
    internal static class SysInfoWrapper  
   {
       private static IntPtr pointer;
        public static SysInfoReturnCode InitializeSmart()
        {
            pointer = Marshal.StringToCoTaskMemUni("debuglog.txt");
            var value = SysInfoApi.SysInfoDll_Smart_Init(false, pointer, false);
           // Marshal.FreeCoTaskMem(pointer);
            return value;
        }

       public static void CleanupSmart()
       {
           SysInfoApi.SysInfoDll_Smart_Cleanup();
       }
   

       public static string GetSerialNumber(int deviceId)
       {
           var buffer = 256;
           var serialBuffer = Marshal.AllocCoTaskMem(buffer);

           SysInfoApi.SysInfoDll_Smart_GetDeviceSerialNumber(serialBuffer,out buffer, deviceId);
           var serial = Marshal.PtrToStringUni(serialBuffer);
            Marshal.FreeCoTaskMem(serialBuffer);
           return serial;
       }

        public static string GetModelNumber(int deviceId)
        {
            var buffer = 256;
            var modelBuffer = Marshal.AllocCoTaskMem(buffer);

            SysInfoApi.SysInfoDll_Smart_GetDeviceModelNumber(modelBuffer, out buffer, deviceId);
            var serial = Marshal.PtrToStringUni(modelBuffer);
            Marshal.FreeCoTaskMem(modelBuffer);
            return serial;
        }

        
        public static string GetDriveLetterById(int deviceId)
       {
            int[] integerArray = new int[10];
            char letter = '\0';
            int numberOfPartitions = 10;
            if (numberOfPartitions <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfPartitions));
            var errorCode = SysInfoApi.SysInfoDll_Smart_GetDevicePartitionInfo(integerArray, out numberOfPartitions, deviceId);
            if ( errorCode != 0)
            {
                throw new ArgumentException(errorCode.ToString());
            }

           SysInfoApi.SysInfoDll_Smart_GetDriveLetterFromDriveNum(ref letter, integerArray[0]);
            return char.ToString(letter);
        }

       public static List<SmartAttributes> GetSmartAttributesForDrive(int deviceID)

       {
           Console.WriteLine("Starting attibuter");
            
            var attributesList = new List<SmartAttributes>();
           var buffer = 256;
           IntPtr tmpAttributeValue = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(ulong)));
           var bufferClear = buffer;
           Console.WriteLine("looping");

            if (SysInfoApi.SysInfoDll_Smart_IsValidSmartDevice(deviceID) == SysInfoReturnCode.SmartStatusSuccess)
               {
                   Console.WriteLine("Valid");
                   for (int k = SysInfoApi.SysInfoDll_Smart_GetSmartAttributeIDFirst(deviceID); k > 0; k= SysInfoApi.SysInfoDll_Smart_GetSmartAttributeIDNext(deviceID))
                   {
                    var attributes = new SmartAttributes();
                    Console.WriteLine("Second Loop");
                       bufferClear = Marshal.SizeOf(typeof(ulong));
                        
                    Helpers.Interop.ZeroMemoryPointer(tmpAttributeValue,bufferClear);
                      SysInfoApi.SysInfoDll_Smart_GetSmartAttributeByID(tmpAttributeValue, out bufferClear, k,(int)AttributeType.RawValue, deviceID);
                        
                       attributes.Id = k;
                       Console.WriteLine(k + " " +Marshal.ReadInt64(tmpAttributeValue));
                       attributes.Raw = Marshal.ReadInt64(tmpAttributeValue).ToHex();
                       attributesList.Add(attributes);
                   }
               
                    
           }
           return attributesList;
           //for (int k = SysInfoDll_Smart_GetSmartAttributeIDFirst(i); k > 0; k = SysInfoDll_Smart_GetSmartAttributeIDNext(i))
           //             {
           //                 tempBufLen = TMP_BUF_LEN;
           //                 SysInfoDll_Smart_GetSmartAttributeByID(tempBuf, out tempBufLen, k, ATTR_TYPE_STR_DESC, i);
           //                 tempBufLen = Marshal.SizeOf(typeof(ulong));
           //                 RtlZeroMemory(tmpAttrValue, tempBufLen);
           //                 SysInfoDll_Smart_GetSmartAttributeByID(tmpAttrValue, out tempBufLen, k, ATTR_TYPE_ULONGLONG_RAW, i);
           //                 System.Console.WriteLine("Description: {0,-36}, Raw value = {1:X12}", Marshal.PtrToStringUni(tempBuf), Marshal.ReadInt64(tmpAttrValue));
           //             }
       }
   }
}
