using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    public class StatusQueryController : StandardController<MaterialNotify, StatusQueryModel, StatusQuerySearchViewModel, StatusQueryListViewModel, StatusQueryListDataModel>
    {
        private StatusQueryService statusQueryService => _lazy.StatusQueryService.Value;

        public StatusQueryController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.StatusQueryService.Value) { }


    }
}
