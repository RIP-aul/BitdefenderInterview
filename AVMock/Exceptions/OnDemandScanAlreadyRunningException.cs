namespace AvMock.Exceptions
{
    [Serializable]
    public class OnDemandScanAlreadyRunningException : Exception
    {
        public OnDemandScanAlreadyRunningException(string message) : base(message) { }
    }
}
