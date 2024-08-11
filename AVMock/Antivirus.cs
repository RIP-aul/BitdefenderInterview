using AvMock.Enums;
using AvMock.Exceptions;
using AvMock.Exceptions.ExceptionMessages;
using AvMock.Interfaces;
using Bogus;

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

    public class AntivirusService : IAntivirusService
    {
        public IAntivirus Antivirus { get; init; }

        public Task OnDemandScanningTask { get; set; } = Task.CompletedTask;
        private CancellationTokenSource _onDemandScanningTaskCancellationTokenSource;

        public Task RealTimeScanningTask { get; set; } = Task.CompletedTask;
        private CancellationTokenSource _realTimeScanningTaskCancellationTokenSource { get; set; } = new CancellationTokenSource();

        public OnDemandScanStatuses OnDemandScanStatus
        {
            get => Antivirus.OnDemandScanStatus;
            set => ChangeOnDemandStatus(value);
        }

        public RealTimeScanStatuses RealTimeScanStatus
        {
            get => Antivirus.RealTimeScanStatus;
            set => Antivirus.SetRealTimeScanStatus(value);
        }

        public delegate void AntivirusOnDemandStatusChangeHandler(object source, StatusEventArgs args);
        public event AntivirusOnDemandStatusChangeHandler? AntivirusOnDemandStatusChangeEvent;

        public delegate void ThreatDetectedHandler(object source, ThreatDetectedEventArgs args);
        public event ThreatDetectedHandler? ThreatDetectedEvent;

        private Faker _faker { get; } = new("en_US");

        private ManualResetEventSlim _pauseEvent = new(true);


        public AntivirusService(IAntivirus antivirus)
            => Antivirus = antivirus;

        public void ActivateRealTimeScan()
        {
            if (RealTimeScanStatus == RealTimeScanStatuses.Enabled)
                throw new RealTimeScanAlreadyEnabledException(
                    ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.RealTimeScanAlreadyEnabled]);

            if (RealTimeScanStatus == RealTimeScanStatuses.Paused)
            {
                RealTimeScanStatus = RealTimeScanStatuses.Enabled;
                _pauseEvent.Set();
            }

            var cancellationToken = _realTimeScanningTaskCancellationTokenSource.Token;

            RealTimeScanningTask = Task.Run(() => RealTimeScan(cancellationToken), cancellationToken);
        }

        public void DeactivateRealTimeScan(TemporaryRealTimeScanDisableOptions option)
        {
            if (RealTimeScanStatus != RealTimeScanStatuses.Enabled)
                throw new RealTimeScanAlreadyDisabledException(
                    ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.RealTimeScanAlreadyDisabled]);

            if (option != TemporaryRealTimeScanDisableOptions.None)
                PauseRealTimeScan(option);

            else
            {
                _realTimeScanningTaskCancellationTokenSource?.Cancel();
                RealTimeScanStatus = RealTimeScanStatuses.Disabled;
            }
        }

        //private async void RealTimeScan(CancellationToken cancellationToken)
        //{
        //    RealTimeScanStatus = RealTimeScanStatuses.Enabled;

        //    while (RealTimeScanStatus == RealTimeScanStatuses.Enabled)
        //    {
        //        // await 1 second to mock a real-time scan per file and emit event if a threat is detected
        //        // threat probability 1.0%
        //        const float threatProbability = 0.5f;

        //        await Task.Delay(1000);
        //        GenerateFile(threatProbability, out var isThreat, out var file);

        //        DetectThreat(isThreat, file);
        //    }
        //}


        private async void RealTimeScan(CancellationToken cancellationToken)
        {
            RealTimeScanStatus = RealTimeScanStatuses.Enabled;

            while (!cancellationToken.IsCancellationRequested)
            {
                // Check if the task should be paused
                _pauseEvent.Wait(cancellationToken); // Waits until the event is set

                if (RealTimeScanStatus == RealTimeScanStatuses.Enabled)
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

        private async void PauseRealTimeScan(TemporaryRealTimeScanDisableOptions option)
        {
            var pauseTime = (int)option; // time in minutes
            RealTimeScanStatus = RealTimeScanStatuses.Paused;

            // pause real-time scan for the specified amount of time
            _pauseEvent.Reset();
            await Task.Delay(TimeSpan.FromMinutes(pauseTime));

            // resume real-time scan
            RealTimeScanStatus = RealTimeScanStatuses.Enabled;
            _pauseEvent.Set();
        }

        public void StartOnDemandScan()
        {
            if (OnDemandScanStatus == OnDemandScanStatuses.Scanning)
                throw new OnDemandScanAlreadyRunningException(
                    ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.OnDemandScanAlreadyRunning]);

            var thread = new Thread(new ThreadStart(ScanSystemOnDemand));
            thread.Start();
        }

        public void StopOnDemandScan(CancellationToken cancellationToken)
        {
            if (OnDemandScanStatus != OnDemandScanStatuses.Scanning)
                throw new OnDemandScanNotRunningException(
                    ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.OnDemandScanNotRunning]);

            _onDemandScanningTaskCancellationTokenSource.Cancel();
            OnDemandScanStatus = OnDemandScanStatuses.StoppedByUser;
        }

        #region Event Emitters

        protected virtual void OnOnDemandStatusChangeEvent(StatusEventArgs e)
            => AntivirusOnDemandStatusChangeEvent?.Invoke(this, e);


        protected virtual void OnThreatDetectedEvent(ThreatDetectedEventArgs e)
            => ThreatDetectedEvent?.Invoke(this, e);

        #endregion

        #region Private Methods

        private void ScanSystemOnDemand()
            => OnDemandScanningTask = GenerateFiles();

        // probability weight of 0.01f means 1.0%
        // probability weight of 0.9f means 90.0%
        // probability weight of 1.0f or higher means 100.0%

        // default is set to 5.0% probability of a file being a threat
        private async Task GenerateFiles(float threatProbability = 0.05f)
        {
            OnDemandScanStatus = OnDemandScanStatuses.Scanning;

            const int oneSecondDelay = 1000;

            // random number of seconds between 10 and 30 equal to the amount of time the scan will take
            // also number of files to be generated, one per second
            var scanTimeInSeconds = _faker.Random.Int(10, 30);

            for (var i = 0; i < scanTimeInSeconds; i++)
            {
                await Task.Delay(oneSecondDelay);
                GenerateFile(threatProbability, out var isThreat, out var file);
                DetectThreat(isThreat, file);
            }

            OnDemandScanStatus = OnDemandScanStatuses.ScanFinished;
        }

        private void DetectThreat(bool isThreat, AntivirusDetectionResult file)
        {
            if (isThreat)
                OnThreatDetectedEvent(new ThreatDetectedEventArgs(file));
        }

        private void GenerateFile(float threatProbability, out bool isThreat, out AntivirusDetectionResult file)
        {
            var path = string.Concat(_faker.Random.Enum<Drives>(), ':', _faker.System.FilePath());

            isThreat = IsThreat(threatProbability);
            file = new AntivirusDetectionResult(
                path,
                isThreat
                    ? _faker.Random.Enum(SecurityThreatNames.None, SecurityThreatNames.All) // remove None and All security threats
                    : SecurityThreatNames.None);
        }

        private void ChangeOnDemandStatus(OnDemandScanStatuses newStatus)
        {
            OnOnDemandStatusChangeEvent(new StatusEventArgs(DateTime.Now, newStatus, OnDemandScanStatus));
            Antivirus.SetOnDemandScanStatus(newStatus);
        }

        private bool IsThreat(float threatProbability)
            => _faker.Random.Bool(threatProbability);

        private enum Drives
        {
            C,
            D,
            E
        }

        #endregion Private Methods
    }

    public class AntivirusDetectionResult
    {
        public string Path { get; private set; }
        public string ThreatName => ThreatNameEnum.ToString();
        private SecurityThreatNames ThreatNameEnum { get; }

        public AntivirusDetectionResult(string path, SecurityThreatNames threatName)
        {
            Path = path;
            ThreatNameEnum = threatName;
        }

        public bool IsThreat()
            => !ThreatNameEnum.HasFlag(SecurityThreatNames.None);
    }
}