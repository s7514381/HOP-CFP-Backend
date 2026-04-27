using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    public class BuyerCompareController : StandardController<Material, BuyerCompareModel, BuyerCompareSearchModel, BuyerCompareListViewModel, BuyerCompareListDataModel>
    {
        public BuyerCompareController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.BuyerCompareService.Value) { }

        [HttpPost]
        public async Task<IActionResult> GetBuyerMaterialList(Guid? id)
        {
            ApiResult<IEnumerable<BuyerMaterialCompare>> result = new();

            if (!id.HasValue) { result.SetError("取得資料失敗"); }
            else
            {
                result.SetSuccess(await _lazy.BuyerCompareService.Value.GetBuyerMaterialList(id.Value));
            }
            return Json(result);
        }

    }
}
