using AvMock.Enums;
using AvMock.Exceptions;
using AvMock.Exceptions.ExceptionMessages;
using AvMock.Interfaces;
using BitdefenderInterview.Commons.Interfaces;

namespace AntivirusSDK
{
    public class AntivirusSDK
    {
        private readonly IAntivirusService _antivirusService;
        private readonly IAntivirusEventHandler _antivirusEventHandler;

        public AntivirusSDK(IAntivirusService antivirusService, IAntivirusEventHandler antivirusEventHandler)
        {
            _antivirusService = antivirusService;
            _antivirusEventHandler = antivirusEventHandler;
        }

        /// <summary>
        /// Activate the real-time scan.
        /// </summary>
        public void ActivateRealTimeScan()
        {
            _antivirusService.ActivateRealTimeScan();
        }

        /// <summary>
        /// Deactivate the real-time scan.
        /// </summary>
        /// <param name="disableOption">Option for real-time scan pausing or stopping.</param>
        public void DeactivateRealTimeScan(TemporaryRealTimeScanDisableOptions disableOption)
        {
            _antivirusService.DeactivateRealTimeScan(disableOption);
        }

        /// <summary>
        /// Start the antivirus on-demand scan.
        /// </summary>
        public void StartOnDemandScan()
        {
            _antivirusService.StartOnDemandScan();
        }

        /// <summary>
        /// Stop the antivirus on-demand scan.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token for stopping the on-demand scan</param>
        public void StopOnDemandScan(CancellationToken cancellationToken)
        {
            _antivirusService.StopOnDemandScan(cancellationToken);
        }

        /// <summary>
        /// Get all the event logs, if any exist.
        /// </summary>
        /// <returns>A list of scan events.</returns>
        /// <exception cref="NoLogsFoundException">An exception in case no events exist.</exception>
        public List<EventArgs> GetEventLog()
        {
            if (_antivirusEventHandler.EventsLog.Any())
                return _antivirusEventHandler.EventsLog;

            throw new NoLogsFoundException(ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.EventsNotFound]);
        }
    }
}
