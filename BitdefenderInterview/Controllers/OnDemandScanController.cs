using AvMock.Exceptions;
using AvMock.Interfaces;
using BitdefenderInterview.Commons.Interfaces;
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
        /// <returns>Returns a 200 response if the on-demand scan start is successful and a 400 otherwise.</returns> 
        [HttpGet]
        [Route("start")]
        public IActionResult StartOnDemandScan()
        {
            try
            {
                AntivirusService.StartOnDemandScan();
                return Ok(new { message = ResponseMessages.OnDemandScanStartedSuccessfully });
            }
            catch (OnDemandScanAlreadyRunningException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Stop the antivirus on-demand scan.
        /// </summary>
        /// <returns>Returns a 200 response if the on-demand scan stop is successful and a 400 otherwise.</returns> 
        [HttpGet]
        [Route("stop")]
        public IActionResult StopOnDemandScan()
        {
            try
            {
                AntivirusService.StopOnDemandScan(new CancellationToken(true));
                return Ok(ResponseMessages.OnDemandScanStoppedSuccessfully);
            }
            catch (OnDemandScanNotRunningException exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
