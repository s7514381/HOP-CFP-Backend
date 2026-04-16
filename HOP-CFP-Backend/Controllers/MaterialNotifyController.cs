using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    public class MaterialNotifyController : StandardController<MaterialNotify, MaterialNotifyModel, MaterialNotifySearchViewModel, MaterialNotifyListViewModel, MaterialNotifyListDataModel>
    {
        private MaterialNotifyService _materialNotifyService => _lazy.MaterialNotifyService.Value;

        public MaterialNotifyController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.MaterialNotifyService.Value) { }

        [HttpPost]
        public async Task<IActionResult> AddNotify([FromBody] List<Guid> ids)
        {
            ApiResult<object> result = new();

            await _materialNotifyService.AddNotify(ids);

            result.SetSuccess(ids.Count);
            return Json(result);
        }

        [HttpGet]
        public IActionResult test()
        {
            _mailSender.SentAsync("s7514381@gmail.com", "123", "31");
            return Content("");
        }

    }
}
