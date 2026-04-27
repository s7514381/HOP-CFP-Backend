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
    public class AdminMenuController : StandardController<AdminMenu, AdminMenuModel, AdminMenuSearchViewModel, AdminMenuListViewModel, AdminMenuListDataModel>
    {
        private AdminMenuService _adminMenuService => _lazy.AdminMenuService.Value;

        public AdminMenuController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.AdminMenuService.Value) { }


        [HttpPost]
        [IgnoreAuthorize]
        public async Task<IActionResult> GetAdminMenus()
        {
            ApiResult<IEnumerable<FullAdminMenuModel>> result = new();
            result.SetSuccess(await _adminMenuService.GetAdminMenus());
            return Json(result);
        }

    }
}
