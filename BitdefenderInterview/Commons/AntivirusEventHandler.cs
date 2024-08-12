using AvMock;
using BitdefenderInterview.Commons.Interfaces;

namespace BitdefenderInterview.Commons
{
    public class AntivirusEventHandler : IAntivirusEventHandler
    {
        public List<EventArgs> EventsLog { get; set; } = new List<EventArgs>();

        public void OnStatusChangedEvent(object sender, StatusEventArgsBase args)
            => EventsLog.Add(args);

        public void OnThreatsDetectedEvent(object sender, IEnumerable<ThreatDetectedEventArgs> args)
        {
            foreach (var arg in args)
                EventsLog.Add(arg);
        }
    }
}
