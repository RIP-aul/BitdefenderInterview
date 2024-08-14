using AvMock.Exceptions.ExceptionMessages;
using AvMock.Interfaces;
using BitdefenderInterview.Commons.Interfaces;
using BitdefenderInterview.Controllers.Commons;
using Microsoft.AspNetCore.Mvc;

namespace BitdefenderInterview.Controllers
{
    [ApiController]
    public class EventRetrievalController : BaseController
    {
        public EventRetrievalController(IAntivirusEventHandler antivirusEventHandler, IAntivirusService antivirusService)
            : base(antivirusEventHandler, antivirusService) { }

        [HttpGet]
        [Route("event-log")]
        public IActionResult GetScanEventsLog()
            => AntivirusEventHandler.EventsLog.Any()
                ? Ok(AntivirusEventHandler.EventsLog)
                : NotFound(ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.EventsNotFound]);
    }
}
