using AvMock.Services.Commons;

namespace AvMock.Services
{
    public interface IOnDemandScanService
    {
        event BaseScanService.ThreatsDetectedHandler ThreatsDetectedEvent;
        event BaseScanService.StatusChangedHandler StatusChangedEvent;

        void StartScan();
        void StopScan(CancellationToken cancellationToken);
    }
}
