using static AvMock.AntivirusService;

namespace AvMock.Interfaces
{
    public interface IAntivirusService
    {
        event AntivirusOnDemandStatusChangeHandler AntivirusOnDemandStatusChangeEvent;
        event ThreatDetectedHandler ThreatDetectedEvent;

        void StartOnDemandScan();
        void StopOnDemandScan(CancellationToken cancellationToken);

        void ActivateRealTimeScan();
        void DeactivateRealTimeScan(TemporaryRealTimeScanDisableOptions option);
    }
}