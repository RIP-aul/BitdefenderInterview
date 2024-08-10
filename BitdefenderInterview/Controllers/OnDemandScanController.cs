using AvMock;
using AvMock.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BitdefenderInterview.Controllers
{
    [ApiController]
    [Route("on-demand-scan")]
    public class OnDemandScanController : ControllerBase
    {
        private IAntivirusService _antivirusService { get; init; }
        private IEventHandlerClass _eventHandlerClass { get; set; }

        public OnDemandScanController(IAntivirusService antivirusService, IEventHandlerClass eventHandlerClass)
        {
            _antivirusService = antivirusService;
            _eventHandlerClass = eventHandlerClass;
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
