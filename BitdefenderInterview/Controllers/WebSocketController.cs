using AvMock.Interfaces;
using BitdefenderInterview.Controllers.Commons;
using Microsoft.AspNetCore.Mvc;

namespace BitdefenderInterview.Controllers
{
    [ApiController]
    public class WebSocketController : BaseController
    {
        public WebSocketController(IAntivirusEventHandler antivirusEventHandler, IAntivirusService antivirusService)
            : base(antivirusEventHandler, antivirusService) { }

        [HttpGet("scan-events")]
        public IActionResult GetScanEvents()
            => AntivirusEventHandler.Events.Any()
                ? Ok(AntivirusEventHandler.Events)
                : NotFound("No Events found!");
    }
}
