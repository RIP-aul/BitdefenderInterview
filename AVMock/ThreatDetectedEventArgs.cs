namespace AvMock
{
    public class ThreatDetectedEventArgs : EventArgs
    {
        public DateTime? TimeOfEvent { get; set; }
        public AntivirusDetectionResult? AntivirusDetectionResult { get; set; }

        public ThreatDetectedEventArgs(AntivirusDetectionResult antivirusDetectionResult)
        {
            TimeOfEvent = DateTime.Now;
            AntivirusDetectionResult = antivirusDetectionResult;
        }

        public override string ToString()
            => $"Time of event: {TimeOfEvent}\r\n{AntivirusDetectionResult}";
    }
}