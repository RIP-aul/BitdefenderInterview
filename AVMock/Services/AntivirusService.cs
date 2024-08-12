using AvMock.Interfaces;

namespace AvMock.Services
{
    public class AntivirusService : IAntivirusService
    {
        private IOnDemandScanService _onDemandScanService { get; set; }

        public delegate void ThreatsDetectedEventHandler(object sender, IEnumerable<ThreatDetectedEventArgs> e);
        public event ThreatsDetectedEventHandler? ThreatsDetectedEvent;

        public delegate void StatusChangedEventHandler(object sender, StatusEventArgsBase e);
        public event StatusChangedEventHandler? StatusChangedEvent;

        //public Task RealTimeScanningTask { get; set; } = Task.CompletedTask;
        //private CancellationTokenSource _realTimeScanningTaskCancellationTokenSource { get; set; }
        //    = new CancellationTokenSource();

        //public RealTimeScanStatuses RealTimeScanStatus
        //{
        //    get => Antivirus.RealTimeScanStatus;
        //    set => Antivirus.SetRealTimeScanStatus(value);
        //}

        //private ManualResetEventSlim _pauseEvent = new(true);


        public AntivirusService(IOnDemandScanService onDemandScanService)
        {
            _onDemandScanService = onDemandScanService;

            _onDemandScanService.ThreatsDetectedEvent += OnThreatsDetectedEvent;
            _onDemandScanService.StatusChangedEvent += OnStatusChangedEvent;
        }


        #region Real-Time scanning

        //public void ActivateRealTimeScan()
        //{
        //    if (RealTimeScanStatus == RealTimeScanStatuses.Enabled)
        //        throw new RealTimeScanAlreadyEnabledException(
        //            ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.RealTimeScanAlreadyEnabled]);

        //    if (RealTimeScanStatus == RealTimeScanStatuses.Paused)
        //    {
        //        RealTimeScanStatus = RealTimeScanStatuses.Enabled;
        //        _pauseEvent.Set();
        //    }

        //    var cancellationToken = _realTimeScanningTaskCancellationTokenSource.Token;

        //    RealTimeScanningTask = Task.Run(() => RealTimeScan(cancellationToken), cancellationToken);
        //}

        //public void DeactivateRealTimeScan(TemporaryRealTimeScanDisableOptions option)
        //{
        //    if (RealTimeScanStatus != RealTimeScanStatuses.Enabled)
        //        throw new RealTimeScanAlreadyDisabledException(
        //            ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.RealTimeScanAlreadyDisabled]);

        //    if (option != TemporaryRealTimeScanDisableOptions.None)
        //        PauseRealTimeScan(option);

        //    else
        //    {
        //        _realTimeScanningTaskCancellationTokenSource?.Cancel();
        //        RealTimeScanStatus = RealTimeScanStatuses.Disabled;
        //    }
        //}

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

        #region Private Methods



        //private async void RealTimeScan(CancellationToken cancellationToken)
        //{
        //    RealTimeScanStatus = RealTimeScanStatuses.Enabled;

        //    while (!cancellationToken.IsCancellationRequested)
        //    {
        //        // Check if the task should be paused
        //        _pauseEvent.Wait(cancellationToken); // Waits until the event is set

        //        if (RealTimeScanStatus == RealTimeScanStatuses.Enabled)
        //        {
        //            // await 1 second to mock a real-time scan per file and emit event if a threat is detected
        //            // threat probability 1.0%
        //            const float threatProbability = 0.1f;

        //            await Task.Delay(1000);
        //            GenerateFile(threatProbability, out var isThreat, out var file);
        //            DetectThreat(isThreat, file);
        //        }
        //    }
        //}

        //private void DetectThreat(bool isThreat, AntivirusDetectionResult file)
        //{
        //    if (isThreat)
        //        OnThreatsDetectedEvent(new List<ThreatDetectedEventArgs>() { new ThreatDetectedEventArgs(file) });
        //}

        //private async void PauseRealTimeScan(TemporaryRealTimeScanDisableOptions option)
        //{
        //    var pauseTime = (int)option; // time in minutes
        //    RealTimeScanStatus = RealTimeScanStatuses.Paused;

        //    // pause real-time scan for the specified amount of time
        //    _pauseEvent.Reset();
        //    await Task.Delay(TimeSpan.FromMinutes(pauseTime));

        //    // resume real-time scan
        //    RealTimeScanStatus = RealTimeScanStatuses.Enabled;
        //    _pauseEvent.Set();
        //}



        #endregion Private Methods
    }
}