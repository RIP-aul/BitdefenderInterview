using AvMock.Enums;

namespace AvMock
{
    public class AntivirusDetectionResult
    {
        public string Path { get; private set; }
        public string ThreatName => ThreatNameEnum.ToString();
        private SecurityThreatNames ThreatNameEnum { get; }

        public AntivirusDetectionResult(string path, SecurityThreatNames threatName)
        {
            Path = path;
            ThreatNameEnum = threatName;
        }

        public bool IsThreat()
            => !ThreatNameEnum.HasFlag(SecurityThreatNames.None);
    }
}