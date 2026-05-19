using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Filter;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    public class SellerCompareController : StandardController<Material, SellerCompareModel, SellerCompareSearchModel, SellerCompareListViewModel, SellerCompareListDataModel>
    {
        private SellerCompareService _sellerCompareService => _lazy.SellerCompareService.Value;

        public SellerCompareController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.SellerCompareService.Value) { }

        [HttpGet]
        [IgnoreAuthorize]
        public IActionResult DownloadImportTemplate()
        {
            byte[] fileBytes = _sellerCompareService.BuildImportTemplate();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SellerCompareImportTemplate.xlsx");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [IgnoreAuthorize]
        public async Task<IActionResult> Import([FromForm] ImportFormFileModel form)
        {
            ApiResult<SellerCompareImportResult> result = new();

            if (form.file == null || form.file.Length == 0)
            {
                result.SetError("請選擇要匯入的 xlsx 檔案。");
                return Json(result);
            }

            result.SetSuccess(await _sellerCompareService.ImportFromCsv(form.file, form.ignoreErrors));
            return Json(result);
        }

    }
}
