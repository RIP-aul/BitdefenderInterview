using AvMock.Exceptions;
using AvMock.Interfaces;
using BitdefenderInterview.Controllers.Commons;
using Microsoft.AspNetCore.Mvc;

namespace BitdefenderInterview.Controllers
{
    [ApiController]
    [Route("on-demand-scan")]
    public class OnDemandScanController : ControllerBase
    {
        private IAntivirusService _antivirusService { get; init; }
        private IAntivirusEventHandler _eventHandler { get; set; }

        public OnDemandScanController(IAntivirusService antivirusService, IAntivirusEventHandler eventHandler)
        {
            _antivirusService = antivirusService;
            _eventHandler = eventHandler;

            _antivirusService.AntivirusOnDemandStatusChangeEvent += _eventHandler.OnStatusChangedEvent;
            _antivirusService.ThreatDetectedEvent += _eventHandler.OnThreatDetectedEvent;
        }

        /// <summary>
        /// Start the antivirus on-demand scan.
        /// </summary>
        /// <returns></returns>
        [HttpPost("start")]
        public IActionResult StartOnDemandScan()
        {
            try
            {
                _antivirusService.StartOnDemandScan();
            }
            catch (OnDemandScanAlreadyRunningException exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Stop the antivirus on-demand scan.
        /// </summary>
        /// <returns></returns>
        [HttpPost("stop")]
        public IActionResult StopOnDemandScan()
        {
            try
            {
                _antivirusService.StopOnDemandScan(new CancellationToken(true));
            }
            catch (OnDemandScanNotRunningException exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok();
        }
    }
}
