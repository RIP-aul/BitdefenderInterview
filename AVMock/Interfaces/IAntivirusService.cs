using AvMock.Enums;
using static AvMock.Services.AntivirusService;

namespace AvMock.Interfaces
{
    public interface IAntivirusService
    {
        event ThreatsDetectedEventHandler? ThreatsDetectedEvent;
        event StatusChangedEventHandler? StatusChangedEvent;

        void StartOnDemandScan();
        void StopOnDemandScan(CancellationToken cancellationToken);

        void ActivateRealTimeScan();
        void DeactivateRealTimeScan(TemporaryRealTimeScanDisableOptions option);

    }
}