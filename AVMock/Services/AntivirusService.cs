using AvMock.Enums;
using AvMock.Interfaces;

namespace AvMock.Services
{
    public class AntivirusService : IAntivirusService
    {
        private IOnDemandScanService _onDemandScanService { get; init; }
        private IRealTimeScanService _realTimeScanService { get; init; }

        public delegate void ThreatsDetectedEventHandler(object sender, IEnumerable<ThreatDetectedEventArgs> e);
        public event ThreatsDetectedEventHandler? ThreatsDetectedEvent;

        public delegate void StatusChangedEventHandler(object sender, StatusEventArgsBase e);
        public event StatusChangedEventHandler? StatusChangedEvent;

        public AntivirusService(
            IOnDemandScanService onDemandScanService,
            IRealTimeScanService realTimeScanService)
        {
            _onDemandScanService = onDemandScanService;

            _onDemandScanService.ThreatsDetectedEvent += OnThreatsDetectedEvent;
            _onDemandScanService.StatusChangedEvent += OnStatusChangedEvent;

            _realTimeScanService = realTimeScanService;

            _realTimeScanService.ThreatsDetectedEvent += OnThreatsDetectedEvent;
            _realTimeScanService.StatusChangedEvent += OnStatusChangedEvent;
        }


        #region Real-Time scanning

        public void ActivateRealTimeScan()
            => _realTimeScanService.ActivateScan();

        public void DeactivateRealTimeScan(TemporaryRealTimeScanDisableOptions disableOption)
            => _realTimeScanService.DeactivateScan(disableOption);

        #endregion Real-Time scanning

        #region On-Demand scanning


        public void StartOnDemandScan()
            => _onDemandScanService.StartScan();

        public void StopOnDemandScan(CancellationToken cancellationToken)
            => _onDemandScanService.StopScan(cancellationToken);


        #endregion On-Demand scanning


        #region Event Emitters


        private void OnStatusChangedEvent(object source, StatusEventArgsBase args)
            => StatusChangedEvent?.Invoke(this, args);

        private void OnThreatsDetectedEvent(object sender, IEnumerable<ThreatDetectedEventArgs> args)
            => ThreatsDetectedEvent?.Invoke(sender, args);


        #endregion Event Emitters

    }
}