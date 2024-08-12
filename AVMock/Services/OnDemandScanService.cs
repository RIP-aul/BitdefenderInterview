using AvMock.Enums;
using AvMock.Exceptions;
using AvMock.Exceptions.ExceptionMessages;
using AvMock.Interfaces;
using AvMock.Services.Commons;
using static AvMock.Services.Commons.ServiceCommons;

namespace AvMock.Services
{
    public class OnDemandScanService : BaseScanService, IOnDemandScanService
    {
        event ThreatsDetectedHandler IOnDemandScanService.ThreatsDetectedEvent
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

        event StatusChangedHandler IOnDemandScanService.StatusChangedEvent
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

        public Task OnDemandScanningTask { get; set; } = Task.CompletedTask;
        private CancellationTokenSource _scanningTaskCancellationTokenSource { get; set; }
            = new CancellationTokenSource();

        private List<AntivirusDetectionResult> _detectedThreats = new List<AntivirusDetectionResult>();

        public OnDemandScanStatuses ScanStatus
        {
            get => Antivirus.OnDemandScanStatus;
            set => ChangeStatus(value);
        }

        public OnDemandScanService(IAntivirus antivirus) : base(antivirus) { }

        public void StartScan()
        {
            if (ScanStatus == OnDemandScanStatuses.Scanning)
                throw new OnDemandScanAlreadyRunningException(
                    ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.OnDemandScanAlreadyRunning]);

            ScanSystem(_scanningTaskCancellationTokenSource.Token);
        }

        public void StopScan(CancellationToken cancellationToken)
        {
            if (ScanStatus != OnDemandScanStatuses.Scanning)
                throw new OnDemandScanNotRunningException(
                    ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.OnDemandScanNotRunning]);

            _scanningTaskCancellationTokenSource.Cancel();
            ScanStatus = OnDemandScanStatuses.StoppedByUser;
        }


        private void ScanSystem(CancellationToken token)
        {
            ScanStatus = OnDemandScanStatuses.Scanning;
            OnDemandScanningTask = GenerateFiles(token);
        }

        // threatProbability weight of 0.01f means 1.0%
        // threatProbability weight of 0.9f means 90.0%
        // threatProbability weight of 1.0f or higher means 100.0%

        // default is set to 5.0% probability of a file being a threat
        private async Task GenerateFiles(CancellationToken token)
        {
            const int oneSecondDelay = 1000;
            const float threatProbability = 0.05f;
            ScanStatus = OnDemandScanStatuses.Scanning;

            // random number of seconds between 10 and 30 equal to the amount of time the scan will take
            // also number of files to be generated, one per second
            var scanTimeInSeconds = Faker.Random.Int(10, 30);

            for (var i = 0; i < scanTimeInSeconds && !token.IsCancellationRequested; i++)
            {
                await Task.Delay(oneSecondDelay);
                GenerateFile(threatProbability, out var isThreat, out var file);

                if (isThreat)
                    _detectedThreats.Add(file);
            }

            if (!token.IsCancellationRequested)
                ScanStatus = OnDemandScanStatuses.ScanFinished;
        }

        private void ChangeStatus(OnDemandScanStatuses newStatus)
        {
            OnStatusChangedEvent(this, new OnDemandStatusEventArgs(DateTime.Now, ScanStatus, newStatus));
            EmitThreatEventIfAnyFound(newStatus);

            Antivirus.SetOnDemandScanStatus(newStatus);
        }

        private void EmitThreatEventIfAnyFound(OnDemandScanStatuses newStatus)
        {
            if (newStatus != OnDemandScanStatuses.Scanning
                && ScanStatus == OnDemandScanStatuses.Scanning
                && _detectedThreats.Any())
            {
                // determine whether the scan either finished or it was cancelled by user
                // emit an event if any threats were detected
                OnThreatsDetectedEvent(_detectedThreats.Select(x => new ThreatDetectedEventArgs(x)));

                // clear old detections, they are being persisted later on
                _detectedThreats = new List<AntivirusDetectionResult>();
            }
        }
    }
}
