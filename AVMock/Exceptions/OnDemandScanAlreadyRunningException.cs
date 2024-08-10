namespace AvMock.Exceptions
{
    public class OnDemandScanAlreadyRunningException : Exception
    {
        public OnDemandScanAlreadyRunningException() : base() { }
        public OnDemandScanAlreadyRunningException(string message) : base(message) { }
    }
}
