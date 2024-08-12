namespace AvMock.Exceptions
{
    [Serializable]
    public class RealTimeScanAlreadyDisabledException : Exception
    {
        public RealTimeScanAlreadyDisabledException(string message) : base(message) { }
    }
}

