using AvMock.Enums;
using AvMock.Services.Commons;

namespace AvMock.Interfaces
{
    public interface IRealTimeScanService
    {
        event BaseScanService.ThreatsDetectedHandler ThreatsDetectedEvent;
        event BaseScanService.StatusChangedHandler StatusChangedEvent;

        void ActivateScan();
        void DeactivateScan(TemporaryRealTimeScanDisableOptions option);
    }
}
