using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Filter;
using HOP_CFP_Backend.Library.Models.Manager;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Models;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    public class RoleController : StandardController<Role, RoleModel, RoleSearchViewModel, RoleListViewModel, RoleListDataModel>
    {
        private RoleService _roleService => _lazy.RoleService.Value;

        public RoleController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.RoleService.Value) { }

        [HttpPost]
        [IgnoreAuthorize]
        public JsonResult GetRoleItems()
        {
            return Json(BaseFunction.GetSelectListItem<RoleType>());
        }

        [HttpPost]
        [IgnoreAuthorize]
        public async Task<IActionResult> GetSelectListItems()
        {
            ApiResult<IEnumerable<SelectListItem>> result = new();
            result.SetSuccess(await _roleService.GetSelectListItems());
            return Json(result);
        }

    }
}
