using AvMock.Exceptions;
using AvMock.Interfaces;
using BitdefenderInterview.Controllers.Commons;
using Microsoft.AspNetCore.Mvc;

namespace BitdefenderInterview.Controllers
{
    [ApiController]
    [Route("on-demand-scan")]
    public class OnDemandScanController : BaseController
    {
        public OnDemandScanController(IAntivirusEventHandler antivirusEventHandler, IAntivirusService antivirusService)
            : base(antivirusEventHandler, antivirusService) { }

        /// <summary>
        /// Start the antivirus on-demand scan.
        /// </summary>
        /// <returns></returns>
        [HttpPost("start")]
        public IActionResult StartOnDemandScan()
        {
            try
            {
                AntivirusService.StartOnDemandScan();
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
            AntivirusService.StopOnDemandScan(new CancellationToken(true));

            return Ok();
        }
    }
}
