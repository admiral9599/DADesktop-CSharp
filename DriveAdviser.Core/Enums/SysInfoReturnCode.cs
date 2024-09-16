namespace DriveAdviser.Core.Enums
{
    public enum SysInfoReturnCode
    {

        SmartStatusSuccess = 0,
        DeviceIdDoesNotExist = -1000,
        SmartNotSupported = -1001,
        DeviceOpen = -1002,
        DriveNumberInvalid = -1100,
        DriveNumberDoesNotExist = -1101,
        DriveNotFixed = -1102,
        BufferPointsToNull = -2000,
        BufferTooSmall = -2001,
        FailedToAllocateBuffer = -2002,
        SmartDeviceIdListEmpty = -3001,
        SmartDeviceEndOfList = -3002,
        SmartAttributeEmpty = -4001,
        SmartAttributeEndOfList = -4002,
        SmartAttributeInvalid = -4003,
        SmartAttributeUnknown = -4004,
        OpenFileFailure = -5001,
        FileWriteFailure = -5002,
        FileReadFailure = -5003,
        NoReadWritePermissions = -5101,
        NoReadPermissions = -5102,
        FileSeekFail = -5201,
        FailToGetDeviceCapacity = -6001,
        NoDriveInfoAccess = -7001,
        NoAdminRights = -7002,
        SelfTestFailed = -8001,
        FortyEightBitUnsupportedError = -9001,
        FortyEightBitUnsupportedWarning = -9002,
        SmartInitializationNotCalled = -10001,
        SmartInitializationFailedMemoryAlloc = -10002,
        WindowsNotSupported = -10003,
        EmptyStringReturned = -10004,
        ParamaterInvalid = -10005,
        NoAttributesToRecord = -20001,
        NoTecSignatureHead = -20002,
        NoTecSignatureTail = -20003,
        TecFileDoesNotExist = -20004,
        NotEnoughTecData = -20005,
        AttributeThresholdNa = -20006,
        UnableToPredictTec = -20007,
        UnexpectedSmartError = -30001

    };
}
