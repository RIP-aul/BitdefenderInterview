using AvMock;
using AvMock.Exceptions;
using AvMock.Interfaces;
using BitdefenderInterview.Controllers.Commons;
using Microsoft.AspNetCore.Mvc;

namespace BitdefenderInterview.Controllers
{
    [ApiController]
    [Route("real-time-scan")]
    public class RealTimeScanController : BaseController
    {
        public RealTimeScanController(IAntivirusEventHandler antivirusEventHandler, IAntivirusService antivirusService)
            : base(antivirusEventHandler, antivirusService) { }

        /// <summary>
        /// Activates the real-time scan antivirus scan.
        /// </summary>
        /// <returns></returns>
        [HttpPost("activate")]
        public IActionResult ActivateRealTimeScan()
        {
            try
            {
                AntivirusService.ActivateRealTimeScan();
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
                AntivirusService.DeactivateRealTimeScan(realTimeScanDisableOptions);
            }
            catch (RealTimeScanAlreadyDisabledException exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok();
        }
    }
}