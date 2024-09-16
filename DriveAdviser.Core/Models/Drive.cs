using System.Collections.Generic;

namespace DriveAdviser.Core.Models
{
    public class Drive
    {
        public string DriveLetter { get; set; }
        public string Serial { get; set; }
        public string Model { get; set; }
        public string Tempurature { get; set; }
        public int DeviceId { get; set; }
        public int TotalCapacity { get; set; }
        public int FreeSpace { get; set; }
        public string DriveType { get; set; }
        public int HealthPercentage { get; set; } = 100;
        public List<SmartAttributes> Attributes { get; set; }
    }
}
