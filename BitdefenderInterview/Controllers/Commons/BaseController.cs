using AvMock.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BitdefenderInterview.Controllers.Commons
{
    public class BaseController : ControllerBase
    {
        protected IAntivirusEventHandler AntivirusEventHandler { get; init; }
        protected IAntivirusService AntivirusService { get; init; }

        public BaseController(IAntivirusEventHandler antivirusEventHandler, IAntivirusService antivirusService)
        {
            AntivirusEventHandler = antivirusEventHandler;
            AntivirusService = antivirusService;

            AntivirusService.AntivirusOnDemandStatusChangeEvent += AntivirusEventHandler.OnStatusChangedEvent;
            AntivirusService.ThreatDetectedEvent += AntivirusEventHandler.OnThreatDetectedEvent;
        }

        [HttpGet("scan-events")]
        public IActionResult GetScanEvents()
        {
            // Shared logic for this endpoint
            return Ok(AntivirusEventHandler.Events);
        }

        ~BaseController()
        {
            AntivirusService.AntivirusOnDemandStatusChangeEvent -= AntivirusEventHandler.OnStatusChangedEvent;
            AntivirusService.ThreatDetectedEvent -= AntivirusEventHandler.OnThreatDetectedEvent;
        }
    }
}
