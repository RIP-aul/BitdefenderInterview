namespace AvMock.Interfaces
{
    public interface IAntivirus
    {
        OnDemandScanStatuses OnDemandScanStatus { get; }
        RealTimeScanStatuses RealTimeScanStatus { get; }

        void SetOnDemandScanStatus(OnDemandScanStatuses status);
        void SetRealTimeScanStatus(RealTimeScanStatuses status);
    }
}