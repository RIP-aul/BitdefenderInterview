namespace AvMock.Exceptions
{
    [Serializable]
    public class NoLogsFoundException : Exception
    {
        public NoLogsFoundException(string? message) : base(message) { }
    }
}
