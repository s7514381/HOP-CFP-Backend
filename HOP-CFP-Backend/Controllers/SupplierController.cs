using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    public class SupplierController : StandardController<Supplier, SupplierModel, SupplierSearchViewModel, SupplierListViewModel, SupplierListDataModel>
    {
        private SupplierService _supplierService => _lazy.SupplierService.Value;

        public SupplierController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.SupplierService.Value) { }

        [HttpPost]
        public async Task<IActionResult> GetSelectListItems(string? keyword)
        {
            ApiResult<IEnumerable<SelectListItem>> result = new();
            result.SetSuccess(await _supplierService.GetSelectListItems(keyword));
            return Json(result);
        }

        [HttpGet]
        public IActionResult test() {
            return Json(new { success = true });
        }



    }
}
