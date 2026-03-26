using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Filter;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Models;
using HOP_CFP_Backend.Models.DataTables;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using System.Linq;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    /// <summary>
    /// 標準CRUD Controller => StandardController => AuthorizedController => BaseController
    /// </summary>
    /// <typeparam name="DBModel"></typeparam>
    /// <typeparam name="ViewModel"></typeparam>
    /// <typeparam name="SearchViewModel"></typeparam>
    /// <typeparam name="ListViewModel"></typeparam>
    /// <typeparam name="ListDataModel"></typeparam>
    [ApiController]
    [Route("[controller]/[action]")]
    public abstract class StandardController<DBModel, ViewModel, SearchViewModel, ListViewModel, ListDataModel> : AuthorizedController
        where DBModel : IdModelBase, new()
        where ViewModel : DBModel, new()
        where SearchViewModel : BaseSearchViewModel
        where ListViewModel : PagingViewModel<ListDataModel>, new()
        where ListDataModel : BaseListDataModel
    {
        public readonly _StandardService<DBModel, ViewModel, SearchViewModel, ListViewModel, ListDataModel> _service;
        private ManagerService _managerService => _lazy.ManagerService.Value;
        public string _tableName;
        public string _keyField;

        public StandardController(
            BaseControllerArgument argument,
            _StandardService<DBModel, ViewModel, SearchViewModel, ListViewModel, ListDataModel> standardService
        ) : base(argument)
        {
            _service = standardService;
            _tableName = _service._tableName;
            _keyField = _service._keyField;
        }

        [HttpPost]
        [AuthorizeAs(nameof(Index))]
        public virtual async Task<IActionResult> GetList(SearchViewModel searchModel)
        {
            ApiResult<ListViewModel> result = new();
            result.SetSuccess(await _service.GetList(searchModel));
            return Json(result);
        }

        protected virtual List<Column> GetTableColumnsModel()
        {
            List<Column> cols = BaseFunction.GetDataTableColumns<ListDataModel>();
            return cols;
        }


        [HttpPost]
        [AuthorizeAs(nameof(Index))]
        public virtual JsonResult GetTableColumns()
        {
            return Json(GetTableColumnsModel());
        }

        [HttpPost]
        [AuthorizeAs("Detail")]
        public virtual async Task<IActionResult> GetDetailModel(Guid? id)
        {
            if (!id.HasValue) { return Json(null); }

            ApiResult<ViewModel> result = new();
            result.SetSuccess(await _service.GetModel(id.Value));
            return Json(result);
        }

        [HttpPost]
        [AuthorizeAs(nameof(Create))]
        public virtual async Task<JsonResult> GetNewModel()
        {
            return Json(await _service.CreateModel());
        }

        [HttpPost]
        [AuthorizeAs(nameof(Copy))]
        public virtual async Task<JsonResult> GetCopyModel(Guid? id)
        {
            if (!id.HasValue) { return Json(null); }

            return Json(await _service.GetCopyModel(id.Value));
        }

        [HttpPost]
        [AuthorizeAs(nameof(Edit))]
        public virtual async Task<JsonResult> GetModel(Guid? id)
        {
            if (!id.HasValue) { return Json(null); }

            ApiResult<ViewModel> result = new();
            result.SetSuccess(await _service.GetModel(id.Value));
            return Json(result);
        }

        protected virtual async Task ModelValidation(ViewModel viewModel, IFormCollection form = null) {

        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(IFormCollection form, ViewModel viewModel)
        {
            ApiResult<IActionResult> result = new();

            result.SetSuccess(await ModelValidationFunc(viewModel, form, async () =>
            {
                await _service.Insert(viewModel);
            }));
            return Json(result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit(IFormCollection form, ViewModel viewModel)
        {
            ApiResult<IActionResult> result = new();

            result.SetSuccess(await ModelValidationFunc(viewModel, form, async () =>
            {
                await _service.Update(viewModel);
            }));
            return Json(result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Copy(IFormCollection form, ViewModel viewModel)
        {
            return await ModelValidationFunc(viewModel, form, async () =>
            {
                await _service.Copy(viewModel);
            });
        }

        [HttpPost]
        [AuthorizeAs(nameof(Edit))]
        public virtual async Task<IActionResult> Save(IFormCollection form, ViewModel viewModel)
        {
            return await ModelValidationFunc(viewModel, async () =>
            {
                await _service.Save(viewModel);
            });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            (bool isSuccess, string message) = await TransactionFunc(async () =>  
            {
                await _service.Delete(id);
            });
            if (!isSuccess) { message = "系統錯誤，請聯絡系統管理人員。"; }

            return new JsonResult(new { success = isSuccess, message = message });
        }

        protected virtual async Task<IActionResult> ModelValidationFunc(ViewModel viewModel, Func<Task?> func)
        {
            // 通用資料驗證
            await ModelValidation(viewModel);
            if (!ModelState.IsValid)
            {
                return Json(GetInvalidModelStateEntry());
            }

            (bool isSuccess, string errorMessage)  = await TransactionFunc(func, async () => {
                ModelState.AddModelError("", "處理失敗，請聯絡系統管理人員。");
            });

            // 回傳錯誤的 ModelState
            // 若沒有任何錯誤，前端就會知道成功了
            return Json(GetInvalidModelStateEntry());
        }

        protected virtual async Task<IActionResult> ModelValidationFunc(ViewModel viewModel, IFormCollection form, Func<Task?> func)
        {
            //限制檔案不能超過5M
            foreach (var file in form.Files) {
                if (file.Length > 5242880) { ModelState.AddModelError("", $"上傳檔案: {file.FileName} 檔案大小超過5M，請重新確認。"); }
            }
            if (!ModelState.IsValid) { return Json(GetInvalidModelStateEntry()); }

            //先把上傳檔案放到欄位裡，檢查是否有上傳檔案，沒有真的上傳
            await _uploadFileService.FilesSyncModel(viewModel, form.Files, CurrentControllerPrefix);

            // 通用資料驗證
            await ModelValidation(viewModel, form);
            if (!ModelState.IsValid) { return Json(GetInvalidModelStateEntry()); }

            //上傳檔案
            if (form.Files.Count > 0) { await _uploadFileService.UploadFileAndSyncModel(viewModel, form.Files, CurrentControllerPrefix); }

            (bool isSuccess, string errorMessage) = await TransactionFunc(func, async () => {
                ModelState.AddModelError("", "處理失敗，請聯絡系統管理人員。");
            });

            // 回傳錯誤的 ModelState
            // 若沒有任何錯誤，前端就會知道成功了
            return Json(GetInvalidModelStateEntry());
        }

        [HttpPost]
        [AuthorizeAs(nameof(Index))]
        public JsonResult GetStatusItems()
        {
            return Json(BaseFunction.GetStatusItems());
        }

        protected virtual JsonResult GetJsonResult(bool success)
        {
            return GetJsonResult(success, "");
        }
        protected virtual JsonResult GetJsonResult(bool success, string message) 
        {
            return new JsonResult(new { success = success, message = message });
        }
        protected virtual JsonResult GetJsonResult(object? value)
        {
            return new JsonResult(value);
        }

    }
}