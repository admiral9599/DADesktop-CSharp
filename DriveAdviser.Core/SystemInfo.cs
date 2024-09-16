using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace DriveAdviser.Core
{
    public static class SystemInfo
    {
        public static string CpuName { get; private set; }
        public static string CpuSpeed { get; private set; }
        public static long HardDriveSize { get; private set; }
        public static string OsVersion { get; private set; }
        public static string OsName { get; private set; }
        public static string OsBuild { get; private set; }
        public static string RamCapacity { get; private set; }


        public static async Task GetSystemInfoAsync()
        {
            var tasks = new List<Task>();
            //TODO Make Hardware Calls Async
            tasks.Add(Task.Run((() => GetCpuInformation())));
            tasks.Add(Task.Run((() => GetHardDriveInformation())));

            tasks.Add(Task.Run((() => GetOperatingSystemInformation())));

            tasks.Add(Task.Run((() => GetPhysicalMemory())));


            await Task.WhenAll(tasks);


        }

        private static void GetCpuInformation()
        {
            if (string.IsNullOrEmpty(CpuName))
            {
                var cpu =
                    new ManagementObjectSearcher("select * from Win32_Processor")
                        .Get()
                        .Cast<ManagementObject>()
                        .First();


                CpuName = (string)cpu["Name"];

                CpuSpeed = $"{.001f * (uint)cpu["MaxClockSpeed"]:0.0#}";


                CpuName =
                    CpuName
                        .Replace("(TM)", "")
                        .Replace("(tm)", "")
                        .Replace("(R)", "")
                        .Replace("(r)", "")
                        .Replace("(C)", "")
                        .Replace("(c)", "")
                        .Replace("    ", " ")
                        .Replace("  ", " ");

            }
        }
        private static void GetHardDriveInformation()
        {

            var allDrives = System.IO.DriveInfo.GetDrives();
            foreach (var drive in allDrives)
            {
                if (drive.Name == "C:\\")
                {

                    HardDriveSize = drive.TotalSize / 1048576;
                }

            }
        }

        private static void GetOperatingSystemInformation()
        {
            //if (string.IsNullOrEmpty(OsName))
            //{
            //    var wmi =
            //        new ManagementObjectSearcher("select * from Win32_OperatingSystem")
            //            .Get()
            //            .Cast<ManagementObject>()
            //            .First();

            //    // OsName = ((string)wmi["Caption"]).Trim();
            //    OsVersion = (string)wmi["Version"];

            //}
            OsName = GetProductName();
            OsBuild = GetReleaseId();
            //  GetOsName(OsVersion);

        }

        private static string GetProductName()
        {
            return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName",
                "").ToString();
        }    
        private static string GetReleaseId()
        {
            var id = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId",
                "").ToString();

            return !string.IsNullOrWhiteSpace(id)
                ? id
                : Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild",
                    "").ToString();
        }
        private static void GetOsName(string version)
        {
            Char delimiter = '.';
            var versionarr = version.Split(delimiter);
            var major = Convert.ToInt32(versionarr[0]);
            var minor = Convert.ToInt32(versionarr[1]);
            switch (major)
            {
                case 10:
                    OsName = "Windows 10";
                    break;
                case 6:
                    switch (minor)
                    {
                        case 3:
                            OsName = "Windows 8.1";
                            break;
                        case 2:
                            OsName = "Windows 8";
                            break;
                        case 1:
                            OsName = "Windows 7";
                            break;
                        case 0:
                            OsName = "Windows Vista";
                            break;
                        default:
                            OsName = "Unknown";
                            break;



                    }
                    break;


            }
        }

        private static void GetPhysicalMemory()
        {
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
            ManagementObjectCollection oCollection = oSearcher.Get();

            long memSize = 0;
            long mCap = 0;

            // In case more than one Memory sticks are installed
            foreach (ManagementObject obj in oCollection)
            {
                mCap = Convert.ToInt64(obj["Capacity"]);
                memSize += mCap;
            }
            memSize = (memSize / 1024) / 1024 / 1024;
            RamCapacity = $"{memSize:0.0#}";
        }
    }
}
