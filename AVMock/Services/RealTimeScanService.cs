using AvMock.Enums;
using AvMock.Exceptions;
using AvMock.Exceptions.ExceptionMessages;
using AvMock.Interfaces;
using AvMock.Services.Commons;
using static AvMock.Services.Commons.ServiceCommons;

namespace AvMock.Services
{
    public class RealTimeScanService : BaseScanService, IRealTimeScanService
    {
        public Task ScanningTask { get; set; } = Task.CompletedTask;
        private CancellationTokenSource _scanningTaskCancellationTokenSource { get; set; }
            = new CancellationTokenSource();

        public RealTimeScanStatuses ScanStatus
        {
            get => Antivirus.RealTimeScanStatus;
            set => Antivirus.SetRealTimeScanStatus(value);
        }

        private ManualResetEventSlim _pauseEvent = new(true);

        event ThreatsDetectedHandler IRealTimeScanService.ThreatsDetectedEvent
        {
            add
            {
                // Forward the add operation to the base class event
                ThreatsDetectedEvent += value;
            }
            remove
            {
                // Forward the remove operation to the base class event
                ThreatsDetectedEvent -= value;
            }
        }

        event StatusChangedHandler IRealTimeScanService.StatusChangedEvent
        {
            add
            {
                // Forward the add operation to the base class event
                StatusChangedEvent += value;
            }
            remove
            {
                // Forward the remove operation to the base class event
                StatusChangedEvent -= value;
            }
        }

        public RealTimeScanService(IAntivirus antivirus) : base(antivirus) { }


        public void ActivateScan()
        {
            if (ScanStatus == RealTimeScanStatuses.Enabled)
                throw new RealTimeScanAlreadyEnabledException(
                    ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.RealTimeScanAlreadyEnabled]);

            if (ScanStatus == RealTimeScanStatuses.Paused)
            {
                ScanStatus = RealTimeScanStatuses.Enabled;
                _pauseEvent.Set();
            }
            else
            {
                _scanningTaskCancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _scanningTaskCancellationTokenSource.Token;
                ScanningTask = Task.Run(() => RealTimeScan(cancellationToken), cancellationToken);
            }
        }

        public void DeactivateScan(TemporaryRealTimeScanDisableOptions option)
        {
            if (ScanStatus != RealTimeScanStatuses.Enabled)
                throw new RealTimeScanAlreadyDisabledException(
                    ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.RealTimeScanAlreadyDisabled]);

            if (option == TemporaryRealTimeScanDisableOptions.None)
            {
                _scanningTaskCancellationTokenSource?.Cancel();
                ScanStatus = RealTimeScanStatuses.Disabled;
            }
            else
                PauseScan(option);
        }

        private async void RealTimeScan(CancellationToken cancellationToken)
        {
            ScanStatus = RealTimeScanStatuses.Enabled;

            while (!cancellationToken.IsCancellationRequested)
            {
                // Check if the task should be paused
                _pauseEvent.Wait(cancellationToken); // Waits until the event is set

                if (ScanStatus == RealTimeScanStatuses.Enabled)
                {
                    // await 1 second to mock a real-time scan per file and emit event if a threat is detected
                    // threat probability 1.0%
                    const float threatProbability = 0.1f;

                    await Task.Delay(1000);
                    GenerateFile(threatProbability, out var isThreat, out var file);
                    DetectThreat(isThreat, file);
                }
            }
        }

        private void DetectThreat(bool isThreat, AntivirusDetectionResult file)
        {
            if (isThreat)
                OnThreatsDetectedEvent(new List<ThreatDetectedEventArgs>() { new ThreatDetectedEventArgs(file) });
        }

        private async void PauseScan(TemporaryRealTimeScanDisableOptions option)
        {
            var pauseTime = (int)option; // time in minutes
            ScanStatus = RealTimeScanStatuses.Paused;

            // pause real-time scan for the specified amount of time
            _pauseEvent.Reset();
            await Task.Delay(TimeSpan.FromMinutes(pauseTime));

            // resume real-time scan
            ScanStatus = RealTimeScanStatuses.Enabled;
            _pauseEvent.Set();
        }
    }
}
