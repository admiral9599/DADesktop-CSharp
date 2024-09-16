namespace DriveAdviser.Core.Models
{
    public class SmartAttributes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public long Value { get; set; }
        public long Worst { get; set; }
        public long Threshold { get; set; }
        public long Raw { get; set; }
    }
}
