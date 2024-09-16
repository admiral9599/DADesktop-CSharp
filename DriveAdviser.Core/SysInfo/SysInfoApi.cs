using DriveAdviser.Core.Enums;
using System;
using System.Runtime.InteropServices;

namespace DriveAdviser.Core.Interop
{
    internal class SysInfoApi
    {
        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_Init(bool enableDebug, IntPtr debugFile, bool recordTec);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_Refresh(bool recordTec);


        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_SetInitTimeout(uint timeout);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SysInfoDll_GetVersion();

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_GetNumDetectedDrives();

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_GetNumSmartDevices();

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SysInfoDll_Smart_GetSmartDeviceIDFirst();

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SysInfoDll_Smart_GetSmartDeviceIDNext();

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_IsValidSmartDevice(int i);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_GetDevicePartitionInfo(int[] partitionsOnSameDisk, out int partitonNumber,
            int deviceId);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_GetDriveLetterFromDriveNum(ref char driveLetter, int driveNum);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_GetDeviceSerialNumber(IntPtr ioBuffer, out int ioLength, int deviceId);  

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_GetDeviceCapacity(IntPtr ioBuffer, int deviceId);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_GetDeviceModelNumber(IntPtr ioBuffer, out int ioLength, int deviceId);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_GetDeviceCapacity(ref int capacity, int deviceId);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SysInfoDll_Smart_GetSmartAttributeIDFirst(int deviceId);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SysInfoDll_Smart_GetSmartAttributeIDNext(int deviceId);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SysInfoDll_Smart_GetSmartAttributeByID(IntPtr ioBuffer, out int ioLength, int iAttrID,
            int inAttrType, int deviceId);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_ExportReport(IntPtr filename, int deviceId, IntPtr iHeaderText,
            int iHeaderTextLen, int iExportFlags, int iHistoryAttr);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SysInfoDll_Smart_Cleanup();

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_GetTECDataSetCount(ref int ioCount, int deviceId);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_ExportTECData(Int64[] ioTimeArray, int[] ioNormValArray,
            int[] ioWorstValArray, ref int ioArraySize, int iAttrID, int deviceId);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_GetTECDataAtIndex(ref Int64 ioTimeArray, ref int ioNormValArray,
            ref int ioWorstValArray, int inIndex, int iAttrID, int deviceId);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_EstimateTECByAttributeID(ref Int64 ioEarliest, ref Int64 ioLatest,
            int iAttrID, int deviceId);

        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SysInfoReturnCode SysInfoDll_Smart_EstimateTEC(ref Int64 ioEarliest, ref Int64 ioLatest, ref int oAttrID,
            int deviceId);


        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SysInfoDll_Smart_GetPhysicalDeviceIDFirst();


        [DllImport("SysInfo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SysInfoDll_Smart_GetPhysicalDeviceIDNext();

        
    }
}
