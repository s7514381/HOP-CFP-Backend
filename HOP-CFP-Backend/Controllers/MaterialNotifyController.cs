using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HOP_CFP_Backend.Controllers
{
    public class MaterialNotifyController : StandardController<MaterialNotify, MaterialNotifyModel, MaterialNotifySearchViewModel, MaterialNotifyListViewModel, MaterialNotifyListDataModel>
    {
        private MaterialNotifyService _materialNotifyService => _lazy.MaterialNotifyService.Value;

        public MaterialNotifyController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.MaterialNotifyService.Value) { }

    }
}
