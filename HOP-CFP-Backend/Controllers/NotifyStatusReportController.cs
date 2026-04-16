using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    public class NotifyStatusReportController : StandardController<MaterialNotify, NotifyStatusReportModel, NotifyStatusReportSearchModel, NotifyStatusReportListViewModel, NotifyStatusReportListDataModel>
    {
        private NotifyStatusReportService notifyStatusReportService => _lazy.NotifyStatusReportService.Value;

        public NotifyStatusReportController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.NotifyStatusReportService.Value) { }


    }
}
