using AvMock;
using AvMock.Exceptions;
using AvMock.Interfaces;
using BitdefenderInterview.Controllers.Commons;
using Microsoft.AspNetCore.Mvc;

namespace BitdefenderInterview.Controllers
{
    [ApiController]
    [Route("real-time-scan")]
    public class RealTimeScanController : ControllerBase
    {
        private IAntivirusService _antivirusService { get; init; }
        private IAntivirusEventHandler _eventHandler { get; set; }

        public RealTimeScanController(IAntivirusService antivirusService, IAntivirusEventHandler eventHandler)
        {
            _antivirusService = antivirusService;
            _eventHandler = eventHandler;

            _antivirusService.AntivirusOnDemandStatusChangeEvent += _eventHandler.OnStatusChangedEvent;
            _antivirusService.ThreatDetectedEvent += _eventHandler.OnThreatDetectedEvent;
        }

        /// <summary>
        /// Activates the real-time scan antivirus scan.
        /// </summary>
        /// <returns></returns>
        [HttpPost("activate")]
        public IActionResult ActivateRealTimeScan()
        {
            try
            {
                _antivirusService.ActivateRealTimeScan();
            }
            catch (RealTimeScanAlreadyEnabledException exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Deactivates the real-time scan antivirus scan.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("deactivate/{realTimeScanDisableOptions}")]
        public IActionResult DeactivateRealTimeScan(TemporaryRealTimeScanDisableOptions realTimeScanDisableOptions)
        {
            try
            {
                _antivirusService.DeactivateRealTimeScan(realTimeScanDisableOptions);
            }
            catch (RealTimeScanAlreadyDisabledException exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok();
        }
    }
}