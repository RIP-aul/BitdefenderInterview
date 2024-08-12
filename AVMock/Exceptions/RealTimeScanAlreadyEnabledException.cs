namespace AvMock.Exceptions
{
    [Serializable]
    public class RealTimeScanAlreadyEnabledException : Exception
    {
        public RealTimeScanAlreadyEnabledException(string message) : base(message) { }
    }
}
