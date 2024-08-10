using static AvMock.AntivirusService;

namespace AvMock
{
    public interface IAntivirusService
    {
        event AntivirusStatusChangeHandler AntivirusStatusChangeEvent;
        void StartOnDemandScan();
        void StopOnDemandScan(CancellationToken cancellationToken);
    }
}