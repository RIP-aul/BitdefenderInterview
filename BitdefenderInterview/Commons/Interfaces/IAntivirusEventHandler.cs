using AvMock;

namespace BitdefenderInterview.Commons.Interfaces
{
    public interface IAntivirusEventHandler
    {
        List<EventArgs> EventsLog { get; set; }

        void OnStatusChangedEvent(object sender, StatusEventArgsBase args);
        void OnThreatsDetectedEvent(object sender, IEnumerable<ThreatDetectedEventArgs> args);
    }
}
