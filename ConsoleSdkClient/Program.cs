using AvMock;
using AvMock.Enums;
using AvMock.Exceptions;
using AvMock.Interfaces;
using AvMock.Services;
using BitdefenderInterview.Commons;
using BitdefenderInterview.Commons.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using AntivirusSdk = AntivirusSDK.AntivirusSDK;

namespace ConsoleAppSdkClient
{
    public class Program
    {
        public static ISdkHandler _sdkHandler { get; set; }
        public static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IAntivirus, Antivirus>()
                .AddSingleton<IAntivirusService, AntivirusService>()
                .AddSingleton<IOnDemandScanService, OnDemandScanService>()
                .AddSingleton<IRealTimeScanService, RealTimeScanService>()
                .AddSingleton<IAntivirusEventHandler, AntivirusEventHandler>()
                .AddSingleton<ISdkHandler, SdkHandler>()
                .BuildServiceProvider();

            _sdkHandler = serviceProvider.GetService<ISdkHandler>();

            if (_sdkHandler == null)
            {
                Console.WriteLine("Failed to resolve ISdkHandler.");
                return;
            }

            var userInput = AvailableUserInputs.None;

            do
            {
                DisplayOptions();
                Console.WriteLine();
                Console.WriteLine("User Input: ");

                if (int.TryParse(Console.ReadLine(), out var result)
                    && result.TryConvertEnum(out userInput))
                {
                    HandleUserInput(userInput);
                }
                else
                {
                    Console.WriteLine("Invalid input!");
                    continue;
                }
            } while (userInput != AvailableUserInputs.None);
        }

        private static void DisplayOptions()
        {
            Console.WriteLine();
            Console.WriteLine("1. Start on-demand scan");
            Console.WriteLine("2. Stop on-demand scan");
            Console.WriteLine("3. Activate real-time scan");
            Console.WriteLine("4. Deactivate real-time scan");
            Console.WriteLine("5. Get event log");
            Console.WriteLine();
            Console.WriteLine("0. Exit");
            Console.WriteLine();
        }

        public static void HandleUserInput(AvailableUserInputs userInput)
        {
            switch (userInput)
            {
                case AvailableUserInputs.StartOnDemandScan:
                    Console.WriteLine("\nSelectedOption: \"1. Start on-demand scan\"");
                    _sdkHandler.AntivirusSdk.StartOnDemandScan();

                    break;

                case AvailableUserInputs.StopOnDemandScan:
                    Console.WriteLine("\nSelectedOption: \"2. Stop on-demand scan\"");
                    _sdkHandler.AntivirusSdk.StopOnDemandScan(new CancellationToken(true));

                    break;

                case AvailableUserInputs.ActivateRealTimeScan:
                    Console.WriteLine("\nSelectedOption: \"3. Activate real-time scan\"");
                    _sdkHandler.AntivirusSdk.ActivateRealTimeScan();

                    break;

                case AvailableUserInputs.DeactivateRealTimeScan:
                    Console.WriteLine("\nSelectedOption: \"4. Deactivate real-time scan\"");

                    TemporaryRealTimeScanDisableOptions pauseOption;

                    var isValidInput = false;

                    do
                    {
                        ProvideTemporaryDisableOptions();

                        if (int.TryParse(Console.ReadLine(), out var result)
                            && result.TryConvertEnum(out pauseOption))
                        {
                            _sdkHandler.AntivirusSdk.DeactivateRealTimeScan(pauseOption);
                            isValidInput = true;
                        }

                        else
                            Console.WriteLine("Invalid input!");

                    } while (!isValidInput);

                    break;

                case AvailableUserInputs.GetEventLog:
                    Console.WriteLine("\nSelectedOption: \"5. Get event log\"");
                    try
                    {
                        var events = _sdkHandler.AntivirusSdk.GetEventLog();

                        foreach (var item in events)
                            Console.WriteLine(item);
                    }
                    catch (NoLogsFoundException exception)
                    {
                        Console.WriteLine(exception.Message);
                    }

                    break;

                case AvailableUserInputs.None:
                    Console.WriteLine("\nSelectedOption: \"0. Exit\"");
                    Console.WriteLine("Application exited.");
                    break;

                default:
                    Console.WriteLine();
                    break;
            }
        }

        private static void ProvideTemporaryDisableOptions()
        {
            Console.WriteLine();
            Console.WriteLine("Please Select a deactivation option:");
            Console.WriteLine();
            Console.WriteLine("0. Disable permanently");
            Console.WriteLine("1. Disable for 1 minute");
            Console.WriteLine("5. Disable for 5 minutes");
            Console.WriteLine("10. Disable for 10 minutes");
            Console.WriteLine("15. Disable for 15 minutes");
            Console.WriteLine("30. Disable for 30 minutes");
            Console.WriteLine("60. Disable for 60 minutes");
            Console.WriteLine();
            Console.WriteLine("User Input:");
        }
    }

    public interface ISdkHandler
    {
        AntivirusSdk AntivirusSdk { get; init; }
    }

    public class SdkHandler : ISdkHandler
    {
        public AntivirusSdk AntivirusSdk { get; init; }
        public SdkHandler(IAntivirusService antivirusService, IAntivirusEventHandler eventHandler)
        {
            AntivirusSdk = new AntivirusSdk(antivirusService, eventHandler);
        }
    }

    public enum AvailableUserInputs
    {
        None = 0,

        StartOnDemandScan = 1,
        StopOnDemandScan = 2,

        ActivateRealTimeScan = 3,
        DeactivateRealTimeScan = 4,

        GetEventLog = 5
    }

    public static class ExtensionMethods
    {
        public static bool TryConvertEnum<T>(this int intInput, out T result) where T : struct
        {
            if (Enum.TryParse(intInput.ToString(), out result))
                return true;

            return false;
        }
    }
}
