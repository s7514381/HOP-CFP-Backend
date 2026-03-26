using Dapper;
using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Library.Repositories;
using HOP_CFP_Backend.Models;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HOP_CFP_Backend.Services
{
    /// <summary>
    /// 基礎 Service 類別
    /// </summary>
    public class BaseService
    {
        /// <summary>
        /// 資料庫管理物件，所有 Service 共用 (Singleton)。
        /// </summary>
        protected IDapperRepository _repository { get; private set; }
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMailSender _mailSender;
        protected readonly IConfiguration _configuration;
        protected readonly string _uploadFilePath;
        protected readonly IWebHostEnvironment _hostEnvironment;
        protected UploadFileService _uploadFileService => _lazy.UploadFileService.Value;
        protected LazyServiceArgument _lazy { get; private set; }

        protected ManagerSessionModel? _currentManager => _lazy.ManagerService.Value.CurrentManager;

        #region 自動化調整 DB 單次連線查詢次數
        private static int _currentMaxCount = 15;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(_currentMaxCount, _currentMaxCount);
        private static readonly object _lockObject = new object();
        private static readonly int _adjustThreshold = 20;
        private static ConcurrentBag<long> _records = new();
        private static bool _semaphoreChecking = false;
        public static int CurrentCount => _semaphore.CurrentCount;
        private static ConcurrentDictionary<string, SemaphoreSlim> _requestSemaphore = new();
        #endregion

        // 所有要注入的參數都要整合到 BaseServiceArgument
        public BaseService(BaseServiceArgument argument)
        {
            _repository = argument.Repository;
            _httpContextAccessor = argument.HttpContextAccessor;
            _configuration = argument.Configuration;
            _hostEnvironment = argument.WebHostEnvironment;
            _uploadFilePath = StandardizePath(_configuration["UploadFilePath"]);
            _mailSender = argument.MailSender;

            _lazy = argument.LazyServiceArgument;

            _repository.Disposed += OnRepositoryDisposed;
        }

        /// <summary>
        /// 儲存於 Session 裡的使用者資料。
        /// <para>
        ///     要修改時需直接使用 Setter，如 SessionManagerInfo = newInfo
        /// </para>
        /// </summary>
        public SessionManagerInfo SessionManagerInfo
        {
            get
            {
                if (_httpContextAccessor.HttpContext.Session.GetString("ManagerInfo") == null)
                    return null;
                else
                    return JsonConvert.DeserializeObject<SessionManagerInfo>(
                        _httpContextAccessor.HttpContext.Session.GetString("ManagerInfo"));
            }
            set
            {
                _httpContextAccessor.HttpContext.Session.SetString("ManagerInfo", JsonConvert.SerializeObject(value));
            }
        }

        protected string WebUrl
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;
                string scheme = httpContext.Request.Scheme;
                string host = httpContext.Request.Host.Value;
                return $"{scheme}://{host}";
            }
        }

        /// <summary>
        /// 自動覆寫 Create 與 Update 參數 (Date 與 UserId)，並新增至資料庫
        /// </summary>
        public virtual async Task<int> InsertAsync<T>(T model, [CallerMemberName] string propertyName = null) where T : class
        {
            int result = default;

            await SemaphoreExecute(async () =>
            {
                result = await _repository.InsertAsync(model);
            }, propertyName, true);
            return result;
        }

        /// <summary>
        /// 自動覆寫 Update 參數 (Date 與 UserId)，並更新至資料庫
        /// </summary>
        public virtual async Task<bool> UpdateAsync<T>(T model, [CallerMemberName] string propertyName = null) where T : class
        {
            bool result = default;

            await SemaphoreExecute(async () =>
            {
                result = await _repository.UpdateAsync(model);
            }, propertyName, true);
            return result;
        }

        public async Task<T> QueryFirstAsync<T>(string sql, object para = null, [CallerMemberName] string propertyName = null)
        {
            T result = default;

            await SemaphoreExecute(async () =>
            {
                result = await _repository.QueryFirstOrDefaultAsync<T>(sql, para);
            }, propertyName, true);
            return result;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object para = null, [CallerMemberName] string propertyName = null)
        {
            IEnumerable<T> result = null;

            await SemaphoreExecute(async () =>
            {
                result = await _repository.QueryAsync<T>(sql, para);
            }, propertyName, true);
            return result;
        }

        public async Task<int> ExecuteAsync(string sql, object para = null, [CallerMemberName] string propertyName = null)
        {
            int result = 0;

            await SemaphoreExecute(async () =>
            {
                result = await _repository.ExecuteAsync(sql, para);
            }, propertyName, true);
            return result;
        }

        public async Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object para = null, [CallerMemberName] string propertyName = null)
        {
            SqlMapper.GridReader result = null;

            await SemaphoreExecute(async () =>
            {
                result = await _repository.QueryMultipleAsync(sql, para);
            }, propertyName, true);
            return result;
        }

        public string GetOrderField<T>(BaseSearchViewModel baseSearch) where T : class, new()
        {
            string orderField = "";
            T listDataModel = new T();

            List<string> modelFields = BaseFunction.GetModelField(listDataModel, false);

            if (baseSearch.order.Count > 0)
            {
                if (baseSearch.order[0].column == 0)
                {
                    orderField = modelFields.FirstOrDefault(x => x == "UpdateDate") ?? modelFields.First();
                }
                else
                {
                    orderField = modelFields[baseSearch.order[0].column];
                }
            }
            else
            {
                orderField = modelFields[0];
            }
            return orderField;
        }

        private static string StandardizePath(string path)
        {
            return path.Trim('\\', '/').Replace('\\', '/');
        }

        public virtual async Task<(bool, Exception ex)> TransactionFunc(Func<Task?> func, Func<Task?> exceptionFunc = null)
        {
            using var trans = _repository.OpenTransaction();
            try
            {
                await func();
                trans.Commit();
                return (true, null);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                if (exceptionFunc != null) { await exceptionFunc(); }
                return (false, ex);
            }
        }


        public async Task SemaphoreExecute(Func<Task> func, [CallerMemberName] string propertyName = null, bool isCalResponseTime = false)
        {
            Stopwatch stopwatch = new Stopwatch();

            string? requestId = _httpContextAccessor?.HttpContext?.TraceIdentifier;

            if (!string.IsNullOrEmpty(requestId)
                && !_requestSemaphore.ContainsKey(requestId))
            {
                _requestSemaphore.TryAdd(requestId, _semaphore);
                await _semaphore.WaitAsync();
            }

            try
            {
                stopwatch.Start();
                await func();
            }
            finally
            {
                stopwatch.Stop();
                long elapsedMs = stopwatch.ElapsedMilliseconds;
            }
        }

        private static int CalculateOptimalMaxCount(long t)
        {
            if (t >= 300) { return 2; }
            else if (t >= 150) { return 5; }
            else if (t >= 100) { return 10; }
            else if (t >= 70) { return 15; }
            else if (t >= 50) { return 25; }
            else if (t >= 20) { return 50; }
            else { return 50; }
        }

        public static void OptimalDBConnection(long elapsedMs)
        {
            _records.Add(elapsedMs);

            if (_semaphore.CanReset(_currentMaxCount)
                && _records.Count > _adjustThreshold)
            {
                _semaphoreChecking = true;
                lock (_lockObject)
                {
                    if (!_semaphoreChecking) { goto LockEnd; }

                    long average = (long)_records.Average();
                    _records.Clear();

                    Console.WriteLine($"平均響應時間: {average} 毫秒");

                    //計算是否需要調整同時連線數
                    int newMaxCount = CalculateOptimalMaxCount(average);

                    if (newMaxCount != _currentMaxCount
                        && _semaphore.CanReset(_currentMaxCount))
                    {
                        Console.WriteLine($"目前:{_currentMaxCount} 調整成:{newMaxCount}");

                        // 創建一個新的semaphore替換舊的
                        SemaphoreSlim oldSemaphore = _semaphore;
                        var newSemaphore = new SemaphoreSlim(newMaxCount, newMaxCount);
                        _semaphore = newSemaphore;
                        oldSemaphore.Dispose();

                        _currentMaxCount = newMaxCount;
                    }

                LockEnd:
                    _semaphoreChecking = false;
                }
            }
        }

        private void OnRepositoryDisposed(object sender, EventArgs e)
        {
            string? requestId = _httpContextAccessor?.HttpContext?.TraceIdentifier;

            if (!string.IsNullOrEmpty(requestId) 
                && _requestSemaphore.TryRemove(requestId, out _))
            {
                _semaphore.Release();
            }
        }

    }

    public static class SemaphoreSlimExtension
    {
        public static bool CanReset(this SemaphoreSlim semaphore, int maxCount)
        {
            return semaphore.CurrentCount == maxCount
                   && semaphore.AvailableWaitHandle.WaitOne(0);
        }
    }

    /// <summary>
    /// BaseService 的依賴聚合類別，
    /// 以解決新增 BaseService 依賴項時需要修改每個子類別建構子的問題。
    /// </summary>
    public class BaseServiceArgument
    {
        public IHttpContextAccessor HttpContextAccessor { get; set; }
        public IWebHostEnvironment WebHostEnvironment { get; set; }
        public IDapperRepository Repository { get; set; }
        public IConfiguration Configuration { get; set; }
        public IMailSender MailSender { get; set; }
        public LazyServiceArgument LazyServiceArgument { get; set; }

        public BaseServiceArgument(
            IHttpContextAccessor httpContextAccessor,
            IDapperRepository repository,
            IConfiguration configuration,
            IMailSender mailSender,
            IUrlHelperFactory urlHelper,
            IWebHostEnvironment webHostEnvironment,
            LazyServiceArgument lazyServiceArgument)
        {
            HttpContextAccessor = httpContextAccessor;
            Repository = repository;
            LazyServiceArgument = lazyServiceArgument;
            Configuration = configuration;
            MailSender = mailSender;
            WebHostEnvironment = webHostEnvironment;
        }
    }

}
