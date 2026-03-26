using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Filter;
using HOP_CFP_Backend.Library.Models.Manager;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    public class ManagerController : StandardController<Manager, ManagerModel, ManagerSearchViewModel, ManagerListViewModel, ManagerListDataModel>
    {
        private ManagerService _managerService => _lazy.ManagerService.Value;

        public ManagerController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.ManagerService.Value) { }

        [HttpPost]
        [IgnoreAuthorize]
        public async Task<JsonResult> GetManagerSession()
        {
            return Json(SessionManagerInfo);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(ManagerLoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return Json(GetInvalidModelStateEntry());

            ApiResult<Guid?> result = new();
            var (isSuccess, _) = await TransactionFunc(async () =>
            {
                result = await _managerService.Login(viewModel);
            });

            if (!isSuccess)
                return Json(new ApiResult<object>().SetError("系統錯誤，請聯絡系統管理人員。"));

            return Json(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(ManagerRegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return Json(GetInvalidModelStateEntry());

            ApiResult<object> result = null;
            var (isSuccess, _) = await TransactionFunc(async () =>
            {
                result = await _managerService.Register(viewModel);
            });

            if (!isSuccess)
                return Json(new ApiResult<object>().SetError("系統錯誤，請聯絡系統管理人員。"));

            return Json(result);
        }

    }
}
