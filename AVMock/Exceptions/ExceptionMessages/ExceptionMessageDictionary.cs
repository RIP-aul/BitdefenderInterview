namespace AvMock.Exceptions.ExceptionMessages
{
    public static class ExceptionMessageDictionary
    {
        public static readonly Dictionary<ErrorCodes, string> ErrorCodeDictionary = new()
        {
            { ErrorCodes.OnDemandScanAlreadyRunning, ExceptionMessages.OnDemandScanAlreadyRunning },
            { ErrorCodes.OnDemandScanNotRunning, ExceptionMessages.OnDemandScanNotRunning },
            { ErrorCodes.RealTimeScanAlreadyEnabled, ExceptionMessages.RealTimeScanAlreadyEnabled },
            { ErrorCodes.RealTimeScanAlreadyDisabled, ExceptionMessages.RealTimeScanAlreadyDisabled },
            { ErrorCodes.OnDemandScanNotRunning, ExceptionMessages.EventsNotFound },
        };
    }
}
