using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Filter;
using HOP_CFP_Backend.Library.Models.Manager;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    public class AdminFunctionController : StandardController<AdminFunction, AdminFunctionModel, AdminFunctionSearchViewModel, AdminFunctionListViewModel, AdminFunctionListDataModel>
    {
        private AdminFunctionService _adminFunctionService => _lazy.AdminFunctionService.Value;

        public AdminFunctionController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.AdminFunctionService.Value) { }

        [HttpPost]
        [IgnoreAuthorize]
        public async Task<IActionResult> GetSelectListItems()
        {
            ApiResult<IEnumerable<SelectListItem>> result = new();
            result.SetSuccess(await _adminFunctionService.GetSelectListItems());
            return Json(result);
        }

    }
}
