﻿using AvMock.Exceptions.ExceptionMessages;
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

        /// <summary>
        /// Get the scan event log including status changes and threats detected.
        /// </summary>
        /// <returns>Returns a 200 response code containing a list of events if any exist or a 404 otherwise.</returns>
        [HttpGet]
        [Route("event-log")]
        public IActionResult GetScanEventsLog()
            => AntivirusEventHandler.EventsLog.Any()
                ? Ok(AntivirusEventHandler.EventsLog)
                : NotFound(new { Message = ExceptionMessageDictionary.ErrorCodeDictionary[ErrorCodes.EventsNotFound] });
    }
}
