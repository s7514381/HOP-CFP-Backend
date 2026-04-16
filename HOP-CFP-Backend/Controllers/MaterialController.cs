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


        [HttpPost]
        public async Task<IActionResult> GetBuyerCompareModel(Guid? id)
        {
            if (!id.HasValue) { return Json(null); }

            ApiResult<BuyerCompareModel> result = new();
            result.SetSuccess(await _materialService.GetBuyerCompareModel(id.Value));
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditBuyerCompareModel(IFormCollection form, BuyerCompareModel viewModel)
        {
            ApiResult<object> result = new();

            (bool isSuccess, string errorMessage) = await TransactionFunc(async () =>
            {
                await _materialService.EditBuyerCompareModel(viewModel);
                result.SetSuccess(null);
            }, async () =>
            {
                result.SetError("處理失敗，請聯絡系統管理人員。");
            });
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetBuyerMaterialList(Guid? id)
        {
            ApiResult<IEnumerable<BuyerMaterialCompare>> result = new();

            if (!id.HasValue) { result.SetError("取得資料失敗"); }
            else
            {
                result.SetSuccess(await _materialService.GetBuyerMaterialList(id.Value));
            }
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetSellerCompareModel(Guid? id)
        {
            if (!id.HasValue) { return Json(null); }

            ApiResult<SellerCompareModel> result = new();
            result.SetSuccess(await _materialService.GetSellerCompareModel(id.Value));
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditSellerCompareModel(IFormCollection form, SellerCompareModel viewModel)
        {
            ApiResult<object> result = new();

            (bool isSuccess, string errorMessage) = await TransactionFunc(async () =>
            {
                await _materialService.EditSellerCompareModel(viewModel);
                result.SetSuccess(null);
            }, async () =>
            {
                result.SetError("處理失敗，請聯絡系統管理人員。");
            });
            return Json(result);
        }

    }
}
