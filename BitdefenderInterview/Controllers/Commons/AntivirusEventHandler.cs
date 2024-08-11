using AvMock;

namespace BitdefenderInterview.Controllers.Commons
{
    public interface IAntivirusEventHandler
    {
        void OnStatusChangedEvent(object sender, StatusEventArgs args);
        void OnThreatDetectedEvent(object sender, ThreatDetectedEventArgs args);
    }

    public class AntivirusEventHandler : IAntivirusEventHandler
    {
        public void OnStatusChangedEvent(object sender, StatusEventArgs args)
        {
            Console.WriteLine("");
        }

        public void OnThreatDetectedEvent(object sender, ThreatDetectedEventArgs args)
        {
            Console.WriteLine("");
        }
    }
}
