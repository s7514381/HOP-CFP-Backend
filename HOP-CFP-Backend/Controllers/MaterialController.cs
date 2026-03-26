using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    public class MaterialController : StandardController<Material, MaterialModel, MaterialSearchViewModel, MaterialListViewModel, MaterialListDataModel>
    {
        private MaterialService _materialService => _lazy.MaterialService.Value;

        public MaterialController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.MaterialService.Value) { }

        [HttpPost]
        public async Task<IActionResult> GetSelectListItems(string? keyword)
        {
            ApiResult<IEnumerable<SelectListItem>> result = new();
            result.SetSuccess(await _materialService.GetSelectListItems(keyword));
            return Json(result);
        }

    }
}
