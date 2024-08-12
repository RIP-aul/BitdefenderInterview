using AvMock.Enums;
using AvMock.Interfaces;
using BitdefenderInterview.Commons.Interfaces;

namespace AntivirusSDK
{
    public class AntivirusSDK
    {
        private readonly IAntivirusService _antivirusService;
        private readonly IAntivirusEventHandler _antivirusEventHandler;

        public AntivirusSDK(IAntivirusService antivirusService, IAntivirusEventHandler antivirusEventHandler)
        {
            _antivirusService = antivirusService;
            _antivirusEventHandler = antivirusEventHandler;
        }

        public void ActivateRealTimeScan()
        {
            _antivirusService.ActivateRealTimeScan();
        }

        public void DeactivateRealTimeScan(TemporaryRealTimeScanDisableOptions disableOption)
        {
            _antivirusService.DeactivateRealTimeScan(disableOption);
        }

        public void StartOnDemandScan()
        {
            _antivirusService.StartOnDemandScan();
        }

        public void StopOnDemandScan(CancellationToken cancellationToken)
        {
            _antivirusService.StopOnDemandScan(cancellationToken);
        }

        public List<EventArgs> GetEventLog()
        {
            if (_antivirusEventHandler.EventsLog.Any())
                return _antivirusEventHandler.EventsLog;

            throw new NoLogsFoundException();
        }
    }
}
