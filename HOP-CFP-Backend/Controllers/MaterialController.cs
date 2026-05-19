using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Filter;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    public class MaterialController : StandardController<Material, MaterialModel, MaterialSearchViewModel, MaterialListViewModel, MaterialListDataModel>
    {
        private MaterialService _materialService => _lazy.MaterialService.Value;

        public MaterialController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.MaterialService.Value) { }

        [HttpPost]
        [IgnoreAuthorize]
        public async Task<IActionResult> GetSelectListItems(string? keyword)
        {
            ApiResult<IEnumerable<SelectListItem>> result = new();
            result.SetSuccess(await _materialService.GetSelectListItems(keyword));
            return Json(result);
        }


        [HttpPost]
        [IgnoreAuthorize]
        public async Task<IActionResult> GetKeywordSelectListItems(string? keyword)
        {
            ApiResult<IEnumerable<SelectListItem>> result = new();
            result.SetSuccess(await _materialService.GetKeywordSelectListItems(keyword));
            return Json(result);
        }

        [HttpGet]
        [IgnoreAuthorize]
        public IActionResult DownloadImportTemplate()
        {
            byte[] fileBytes = _materialService.BuildImportTemplate();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MaterialImportTemplate.xlsx");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [IgnoreAuthorize]
        public async Task<IActionResult> Import([FromForm] ImportFormFileModel form)
        {
            ApiResult<MaterialImportResult> result = new();

            if (form.file == null || form.file.Length == 0)
            {
                result.SetError("請選擇要匯入的 xlsx 檔案。");
                return Json(result);
            }

            result.SetSuccess(await _materialService.ImportFromCsv(form.file, form.ignoreErrors));
            return Json(result);
        }

    }
}
