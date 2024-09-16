namespace DriveAdviser.Core.Enums
{
    public enum AttributeType
    {
        TextDescription = 0x20,
        AttributeStatus = 0x40,
        Type = 0x80,
        NormalizedValue = 0x01,
        RawValue = 0x02,
        WorstValue = 0x04,
        ThresholdValue = 0x08

    };
}
