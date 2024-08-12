using AvMock.Interfaces;

namespace AvMock.Services.Commons
{
    public class BaseScanService
    {
        public IAntivirus Antivirus { get; init; }

        public delegate void ThreatsDetectedHandler(object source, IEnumerable<ThreatDetectedEventArgs> args);
        public event ThreatsDetectedHandler? ThreatsDetectedEvent;

        public delegate void StatusChangedHandler(object source, StatusEventArgsBase args);
        public event StatusChangedHandler StatusChangedEvent;

        public BaseScanService(IAntivirus antivirus)
            => Antivirus = antivirus;

        public void OnThreatsDetectedEvent(IEnumerable<ThreatDetectedEventArgs> e)
            => ThreatsDetectedEvent?.Invoke(this, e);

        public void OnStatusChangedEvent(object source, StatusEventArgsBase e)
            => StatusChangedEvent?.Invoke(this, e);
    }
}
