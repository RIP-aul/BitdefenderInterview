using AvMock.Enums;
using AvMock.Exceptions;
using AvMock.Interfaces;
using BitdefenderInterview.Commons.Interfaces;
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
        /// <returns>Returns a 200 response if the real-time scan activation is successful and a 400 otherwise.</returns>
        [HttpPost]
        [Route("activate")]
        public IActionResult ActivateRealTimeScan()
        {
            try
            {
                AntivirusService.ActivateRealTimeScan();
                return Ok(new { Message = ResponseMessages.RealTimeScanActivatedSuccessfully });
            }
            catch (RealTimeScanAlreadyEnabledException exception)
            {
                return BadRequest(new { exception.Message });
            }
        }

        /// <summary>
        /// Deactivates the real-time scan antivirus scan.
        /// </summary>
        /// <param name="realTimeScanDisableOptions">An option for pausing the real-time scan, or stopping it completely.</param>
        /// <returns>Returns a 200 response if the real-time scan deactivation is successful and a 400 otherwise.</returns>
        [HttpPost]
        [Route("deactivate/{realTimeScanDisableOptions}")]
        public IActionResult DeactivateRealTimeScan(TemporaryRealTimeScanDisableOptions realTimeScanDisableOptions)
        {
            try
            {
                AntivirusService.DeactivateRealTimeScan(realTimeScanDisableOptions);
                return Ok(new { Message = ResponseMessages.RealTimeScanDectivatedSuccessfully });
            }
            catch (RealTimeScanAlreadyDisabledException exception)
            {
                return BadRequest(new { exception.Message });
            }
        }
    }
}
