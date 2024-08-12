using AvMock;

namespace BitdefenderInterview.Controllers.Commons
{
    public interface IAntivirusEventHandler
    {
        List<EventArgs> Events { get; set; }

        void OnStatusChangedEvent(object sender, StatusEventArgsBase args);
        void OnThreatsDetectedEvent(object sender, IEnumerable<ThreatDetectedEventArgs> args);
    }

    public class AntivirusEventHandler : IAntivirusEventHandler
    {
        public List<EventArgs> Events { get; set; } = new List<EventArgs>();

        public void OnStatusChangedEvent(object sender, StatusEventArgsBase args)
            => Events.Add(args);

        public void OnThreatsDetectedEvent(object sender, IEnumerable<ThreatDetectedEventArgs> args)
        {
            foreach (var x in args)
                Events.Add(x);
        }
    }
}
