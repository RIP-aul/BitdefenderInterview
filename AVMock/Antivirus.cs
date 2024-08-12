using AvMock.Enums;
using AvMock.Interfaces;

namespace AvMock
{
    public class Antivirus : IAntivirus
    {
        public OnDemandScanStatuses OnDemandScanStatus { get; private set; } = OnDemandScanStatuses.StandingBy;
        public RealTimeScanStatuses RealTimeScanStatus { get; private set; } = RealTimeScanStatuses.Disabled;

        public void SetOnDemandScanStatus(OnDemandScanStatuses status)
            => OnDemandScanStatus = status;

        public void SetRealTimeScanStatus(RealTimeScanStatuses status)
            => RealTimeScanStatus = status;
    }
}