using AvMock;
using Microsoft.AspNetCore.Mvc;

namespace BitdefenderInterview.Controllers
{
    [ApiController]
    [Route("real-time-scan")]
    public class RealTimeScanController : ControllerBase
    {
        private IAntivirusService _antivirusService { get; init; }
        private IEventHandlerClass _eventHandlerClass { get; set; }
        
        public RealTimeScanController(IAntivirusService antivirusService, IEventHandlerClass eventHandlerClass)
        {
            _antivirusService = antivirusService;
            _eventHandlerClass = eventHandlerClass;
        }

        /// <summary>
        /// Activates the real-time scan antivirus scan.
        /// </summary>
        /// <returns></returns>
        [HttpPost("activate")]
        public async Task<IActionResult> ActivateRealTimeScan()
        {


            await Task.FromResult(0);
            return Ok();
        }

        /// <summary>
        /// Deactivates the real-time scan antivirus scan.
        /// </summary>
        /// <returns></returns>
        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateRealTimeScan()
        {
            _antivirusService.StopOnDemandScan(new CancellationToken(true));
            await Task.FromResult(0);
            return Ok();
        }
    }
}