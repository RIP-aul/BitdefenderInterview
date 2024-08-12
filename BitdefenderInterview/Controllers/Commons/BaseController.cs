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

            AntivirusService.ThreatsDetectedEvent += AntivirusEventHandler.OnThreatsDetectedEvent;
            AntivirusService.StatusChangedEvent += AntivirusEventHandler.OnStatusChangedEvent;
        }

        ~BaseController()
        {
            AntivirusService.ThreatsDetectedEvent -= AntivirusEventHandler.OnThreatsDetectedEvent;
            AntivirusService.StatusChangedEvent -= AntivirusEventHandler.OnStatusChangedEvent;
        }
    }
}
