using AvMock.Exceptions;
using AvMock.Exceptions.ExceptionMessages;
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
        public Task<IEnumerable<AntivirusDetectionResult>> ScanningTask { get; set; } = Task.FromResult(Enumerable.Empty<AntivirusDetectionResult>());

        public OnDemandScanStatuses OnDemandScanStatus
        {
            get => Antivirus.OnDemandScanStatus;
            set => Antivirus.SetOnDemandScanStatus(value);
        }

        public delegate void AntivirusStatusChangeHandler(object source, StatusEventArgs args);
        public event AntivirusStatusChangeHandler? AntivirusStatusChangeEvent;

        private Faker _faker { get; } = new("en_US");

        public AntivirusService(IAntivirus antivirus)
        {
            Antivirus = antivirus;
        }

        public void StartOnDemandScan()
        {
            if (OnDemandScanStatus == OnDemandScanStatuses.Scanning)
                throw new OnDemandScanAlreadyRunningException(
                    ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.OnDemandScanAlreadyRunning]);

            var thread = new Thread(new ThreadStart(ScanSystem));
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

        protected virtual void OnStatusChangeEvent(StatusEventArgs e)
        {
            AntivirusStatusChangeEvent?.Invoke(this, e);
        }

        #region Private Methods

        private async void ScanSystem()
        {
            OnDemandScanStatus = OnDemandScanStatuses.Scanning;
            ScanningTask = MockScan();

            var result = await ScanningTask;
            OnDemandScanStatus = OnDemandScanStatuses.ScanFinished;
        }

        private async Task<IEnumerable<AntivirusDetectionResult>> MockScan()
            => (await GenerateFiles()).Where(t => t.IsThreat());

        private async Task<IEnumerable<AntivirusDetectionResult>> GenerateFiles()
        {
            var files = new List<AntivirusDetectionResult>();

            // probability weight of 0.01f means 1.0%
            // probability weight of 0.9f means 90.0%
            // probability weight of 1.0f or higher means 100.0%

            // 5.0% probability of a file being a threat
            const float threatProbability = 0.05f;

            // random number of seconds between 10 and 30 equal to the amount of time the scan will take
            // also number of files to be generated, one per second
            var scanTimeInSeconds = _faker.Random.Int(10, 30);

            for (var i = 0; i < scanTimeInSeconds; i++)
            {
                await Task.Delay(1000);

                var path = string.Concat(_faker.Random.Enum<Drives>(), ';', _faker.System.FilePath());

                var file = new AntivirusDetectionResult(
                    path,
                    IsThreat(threatProbability)
                        ? _faker.Random.Enum(SecurityThreatNames.None, SecurityThreatNames.All) // remove None and All security threats
                        : SecurityThreatNames.None);

                files.Add(file);
            }

            return files;
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

    public class EventHandlerClass : IEventHandlerClass
    {
        private IAntivirusService _antivirusService { get; set; }

        public EventHandlerClass(IAntivirusService antivirusService)
        {
            _antivirusService = antivirusService;
            _antivirusService.AntivirusStatusChangeEvent += OnStatusChangedEvent;
        }

        public void OnStatusChangedEvent(object sender, StatusEventArgs args)
        {
            Console.WriteLine("PLM");
        }
    }

    public interface IEventHandlerClass
    {
        void OnStatusChangedEvent(object sender, StatusEventArgs args);
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

    public enum OnDemandScanStatuses
    {
        StandingBy = 1,
        StoppedByUser = 2,
        ScanFinished = 4,
        Scanning = 6,
    }

    public enum RealTimeScanStatuses
    {
        Disabled = 0,
        Enabled = 1,
        Paused = 2
    }
}