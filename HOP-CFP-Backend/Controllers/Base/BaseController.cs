using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Controllers;
using HOP_CFP_Backend.Library.Repositories;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;
using HOP_CFP_Backend.Models;
using Newtonsoft.Json.Linq;
using HOP_CFP_Backend.Filter;
using System.Reflection;

namespace HOP_CFP_Backend.Controllers
{
    /// <summary>
    /// 基礎 Controller 類別
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected readonly IWebHostEnvironment _hostingEnvironment;
        protected readonly IConfiguration _configuration;
        protected readonly IMailSender _mailSender;
        protected readonly IDapperRepository _repository;
        protected ILogger _logger;
        protected readonly BaseService _baseService;
        protected readonly LazyServiceArgument _lazy;
        protected readonly UploadFileService _uploadFileService;

        protected string WebRootPath => _hostingEnvironment.WebRootPath;

        // 請將所以有要注入的參數整合進 BaseControllerArgument
        public BaseController(BaseControllerArgument argument)
        {
            _hostingEnvironment = argument.HostingEnvironment;
            _configuration = argument.Configuration;
            _mailSender = argument.MailSender;
            _repository = argument.Repository;
            _baseService = argument.BaseService;
            _logger = argument.Logger;
            _lazy = argument.LazyServiceArgument;
            _uploadFileService = _lazy.UploadFileService.Value;
        }

        /// <summary>
        /// Session 中儲存的使用者資料。
        /// <para>
        ///     若要更新 Session 中的值，需直接呼叫 setter，
        ///     如 SessionManagerInfo = newInfo。
        /// </para>
        /// </summary>
        protected SessionManagerInfo SessionManagerInfo
        {
            get
            {
                if (HttpContext.Session.GetString("ManagerInfo") == null)
                    return null;
                else
                    return JsonSerializer.Deserialize<SessionManagerInfo>(HttpContext.Session.GetString("ManagerInfo"));
            }
            set
            {
                HttpContext.Session.SetString("ManagerInfo", JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = false
                }));
            }
        }

        /// <summary>
        /// 當前的 Controller 名稱。
        /// </summary>
        protected string CurrentController
        {
            get
            {
                return ControllerContext.RouteData.Values["controller"]?.ToString();
            }
        }

        /// <summary>
        /// 當前的 Action 名稱。
        /// </summary>
        protected string CurrentAction
        {
            get
            {
                return ControllerContext.RouteData.Values["action"]?.ToString();
            }
        }

        protected string CurrentControllerPrefix
        {
            get { return CurrentController.Replace("Controller", ""); }
        }


        /// <summary>
        /// 紀錄例外狀況與當下的參數。
        /// </summary>
        /// <param name="ex">發生的例外狀況</param>
        /// <param name="level">例外狀況的嚴重程度(若為 Critical 則忽略 Line Notify 發送限制)</param>
        protected void LogActionError(Exception ex, LogLevel level = LogLevel.Error)
        {
            _logger.Log(level, ex,
                "\r\n【請求路徑】：[{0}] {1}" +
                "\r\n【異常類型】：{2}" +
                "\r\n【異常訊息】：{3}" +
                "\r\n【參數】：{4}" +
                "\r\n【StackTrace】：{5}\r\n",
                Request.Method,
                Request.Path,
                ex.GetType().Name,
                ex.Message,
                Request.Method != "POST" ? "" :
                    Request.ContentType == "application/json" ? "" :
                    Request?.Form != null ?
                    JsonSerializer.Serialize(
                        Request.Form.Where(x => x.Key != "__RequestVerificationToken"),
                        new JsonSerializerOptions() { WriteIndented = true }) : "",
                ex.StackTrace);
        }

        protected string FormComponent(string formAction = "")
        {
            if (string.IsNullOrEmpty(formAction)) { formAction = CurrentAction; }
            return $"~/Views/Shared/FormComponents/{formAction}.cshtml";
        }

        /// <summary>
        /// 取得錯誤的 ModelState。
        /// </summary>
        protected IEnumerable<KeyValuePair<string, ModelStateEntry>> GetInvalidModelStateEntry()
        {
            return ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).ToArray();
        }

        protected List<ControllerAction> GetControllerActionList()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var controlleractionlist = asm.GetTypes()
                    .Where(type => typeof(Controller).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Select(x => new ControllerAction
                    {
                        Controller = x.DeclaringType.Name,
                        Action = x.Name,
                        CustomAttributes = x.CustomAttributes.ToList(),
                    }).ToList();
            return controlleractionlist;
        }

        //所有執行動作包交易
        protected virtual async Task<(bool, string)> TransactionFunc(Func<Task?> func, Func<Task?> exceptionFunc = null)
        {
            (bool isSuccess, Exception ex) = await _baseService.TransactionFunc(func, exceptionFunc);

            if (!isSuccess)
            {
                LogActionError(ex);
            }
            return (isSuccess, ex?.Message);
        }

        protected IActionResult AlertView(string message, string href = "")
        {
            JObject jobj = JObject.FromObject(new
            {
                Message = message,
                Href = href,
            });
            return View($"~/Views/Shared/Alert.cshtml", jobj);
        }

    }
}

namespace HOP_CFP_Backend.Argument 
{
    /// <summary>
    /// BaseController 的依賴聚合類別，
    /// 以解決新增 BaseController 依賴項時需要修改每個子類別建構子的問題。
    /// </summary>
    public class BaseControllerArgument
    {
        public IWebHostEnvironment HostingEnvironment { get; set; }
        public IConfiguration Configuration { get; set; }
        public IMailSender MailSender { get; set; }
        public IDapperRepository Repository { get; set; }
        public ILogger Logger { get; set; }
        public BaseService BaseService { get; set; }
        public LazyServiceArgument LazyServiceArgument { get; set; }

        public BaseControllerArgument(
            BaseService baseService,
            IWebHostEnvironment hostingEnvironment,
            IConfiguration configuration,
            IMailSender mailSender,
            IDapperRepository repository,
            ILogger<BaseController> logger,
            LazyServiceArgument lazyServiceArgument)
        {
            HostingEnvironment = hostingEnvironment;
            Configuration = configuration;
            MailSender = mailSender;
            Repository = repository;
            BaseService = baseService;
            Logger = logger;
            LazyServiceArgument = lazyServiceArgument;
        }
    }
}
