using AvMock;

namespace BitdefenderInterview.Controllers.Commons
{
    public interface IAntivirusEventHandler
    {
        IList<EventArgs> Events { get; set; }
        void OnStatusChangedEvent(object sender, StatusEventArgs args);
        void OnThreatDetectedEvent(object sender, ThreatDetectedEventArgs args);
    }

    public class AntivirusEventHandler : IAntivirusEventHandler
    {
        public IList<EventArgs> Events { get; set; } = new List<EventArgs>();

        public void OnStatusChangedEvent(object sender, StatusEventArgs args)
        {
            Events.Add(args);
        }

        public void OnThreatDetectedEvent(object sender, ThreatDetectedEventArgs args)
        {
            Events.Add(args);
        }
    }
}
