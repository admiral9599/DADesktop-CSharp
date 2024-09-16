using System;

namespace DriveAdviser.Core
{
    public static class Constants
    {
        public static readonly string ProgramName = "Drive Adviser";

        public static readonly string IniFolder =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\Drive Adviser";

        public static readonly string IniName = "DriveAdviser.ini";
        public static readonly string IniFullPath = $@"{IniFolder}\{IniName}";
        public static readonly string LogDir = $@"{IniFolder}\logs";
        public static readonly string LogFile = $@"{LogDir}\DriveAdviser.txt";
        public static readonly string TagLocation = "HKEY_LOCAL_MACHINE\\Software\\SchrockInnovations";
        public static readonly string TagSubKey = @"Software\SchrockInnovations";
        public static readonly string DriveAdviserApi = @"http://api.driveadviser.com/index.php/api";
        public static readonly string SchrockApi = @"http://api.schrockinnovations.com/index.php/api";
        public static readonly string VcLocation = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\12.0\VC\Runtimes\x86";
        
    }
}