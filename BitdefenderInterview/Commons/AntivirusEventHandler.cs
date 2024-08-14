using AvMock;
using AvMock.Interfaces;
using BitdefenderInterview.Commons.Interfaces;

namespace BitdefenderInterview.Commons
{
    public class AntivirusEventHandler : IAntivirusEventHandler
    {
        public List<EventArgs> EventsLog { get; set; } = new List<EventArgs>();
        private IAntivirusService _antivirusService { get; init; }

        public AntivirusEventHandler(IAntivirusService antivirusService)
        {
            _antivirusService = antivirusService;

            _antivirusService.StatusChangedEvent += OnStatusChangedEvent;
            _antivirusService.ThreatsDetectedEvent += OnThreatsDetectedEvent;
        }

        public void OnStatusChangedEvent(object sender, StatusEventArgsBase args)
            => EventsLog.Add(args);

        public void OnThreatsDetectedEvent(object sender, IEnumerable<ThreatDetectedEventArgs> args)
        {
            foreach (var arg in args)
                EventsLog.Add(arg);
        }

        ~AntivirusEventHandler()
        {
            _antivirusService.StatusChangedEvent -= OnStatusChangedEvent;
            _antivirusService.ThreatsDetectedEvent -= OnThreatsDetectedEvent;
        }
    }
}
