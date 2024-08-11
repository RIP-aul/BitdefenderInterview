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
                    const float threatProbability = 0.5f;

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
            await Task.Delay(TimeSpan.FromSeconds(10));

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

            Task.FromCanceled(cancellationToken);
            OnDemandScanStatus = OnDemandScanStatuses.StoppedByUser;
        }

        protected virtual void OnOnDemandStatusChangeEvent(StatusEventArgs e)
        {
            AntivirusOnDemandStatusChangeEvent?.Invoke(this, e);
        }

        protected virtual void OnThreatDetectedEvent(ThreatDetectedEventArgs e)
        {
            ThreatDetectedEvent?.Invoke(this, e);
        }

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

    public class StatusEventArgs : EventArgs
    {
        public DateTime? TimeOfEvent { get; set; }
        public OnDemandScanStatuses NewStatus { get; set; }
        public OnDemandScanStatuses OldStatus { get; set; }

        public StatusEventArgs(DateTime timeOfEvent)
            => TimeOfEvent = timeOfEvent;

        public StatusEventArgs(DateTime timeOfEvent, OnDemandScanStatuses newStatus, OnDemandScanStatuses oldStatus)
        {
            TimeOfEvent = timeOfEvent;
            NewStatus = newStatus;
            OldStatus = oldStatus;
        }
    }

    public class ThreatDetectedEventArgs : EventArgs
    {
        public DateTime? TimeOfEvent { get; set; }
        public AntivirusDetectionResult? AntivirusDetectionResult { get; set; }

        public ThreatDetectedEventArgs(AntivirusDetectionResult antivirusDetectionResult)
        {
            TimeOfEvent = DateTime.Now;
            AntivirusDetectionResult = antivirusDetectionResult;
        }
    }

    [Flags]
    public enum SecurityThreatNames
    {
        None = 0,

        Virus = 1 << 0,
        Worm = 1 << 1,
        Trojan = 1 << 2,
        Ransomware = 1 << 3,
        Spyware = 1 << 4,

        Adware = 1 << 5,
        PotentiallyUnwantedPrograms = 1 << 6,

        Phishing = 1 << 7,
        ManInTheMiddle = 1 << 8,
        DDoS = 1 << 9,

        Rootkit = 1 << 10,
        Keylogger = 1 << 11,
        Backdoor = 1 << 12,

        Malware = Virus | Worm | Trojan | Ransomware | Spyware,
        NetworkThreats = Phishing | ManInTheMiddle | DDoS,
        OtherThreats = Rootkit | Keylogger | Backdoor,

        All = Malware | Adware | PotentiallyUnwantedPrograms | NetworkThreats | OtherThreats
    }

    public enum TemporaryRealTimeScanDisableOptions
    {
        None = 0,

        OneMinute = 1,
        FiveMinutes = 5,
        TenMinutes = 10,
        FifteenMinutes = 15,
        ThirtyMinutes = 30,
        SixtyMinutes = 60,
    }

    public enum OnDemandScanStatuses
    {
        StandingBy = 1,
        StoppedByUser = 2,
        ScanFinished = 3,
        Scanning = 4,
    }

    public enum RealTimeScanStatuses
    {
        Disabled = 0,
        Enabled = 1,
        Paused = 2
    }
}