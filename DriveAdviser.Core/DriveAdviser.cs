using DriveAdviser.Services;

namespace DriveAdviser.Core
{
    public static class DriveAdviser
    {
        public static BuildType Build
        {
            get { return Settings.GetBuildType(); }
            
        }
    }
}
