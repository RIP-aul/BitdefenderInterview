namespace AvMock.Exceptions
{
    [Serializable]
    public class OnDemandScanNotRunningException : Exception
    {
        public OnDemandScanNotRunningException(string message) : base(message) { }
    }
}
