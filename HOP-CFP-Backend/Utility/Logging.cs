using HOP_CFP_Backend.Library.Repositories;
using HOP_CFP_Backend.ViewModels;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace HOP_CFP_Backend.Utility
{
    public class Logging
    {
        private readonly RequestDelegate _next;
        protected IDbConnectionFactory _factory;
        private readonly ConcurrentDictionary<string, Type> _controllerList;

        public Logging(RequestDelegate next,
            IConfiguration configuration,
            IDbConnectionFactory dbConnectionFactory)
        {
            _factory = dbConnectionFactory;
            _next = next;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(x =>
                    x.Namespace != null &&
                    (x.Namespace.StartsWith("HOP_CFP_Backend.Controllers")) &&
                    x.IsClass && x.IsPublic && !x.IsAbstract);

            _controllerList = new ConcurrentDictionary<string, Type>();

            foreach (var type in types)
            {
                string key = type.Name.EndsWith("Controller")
                             ? type.Name.Substring(0, type.Name.LastIndexOf("Controller"))
                             : type.Name;
                _controllerList.TryAdd(key, type);
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 確保 HTTP Request 可以多次讀取
            context.Request.EnableBuffering();

            var body = "";

            using (var bodyReader = new StreamReader(stream: context.Request.Body,
                                              encoding: Encoding.UTF8,
                                              detectEncodingFromByteOrderMarks: false,
                                              bufferSize: 1024,
                                              leaveOpen: true))
            {
                //body = await bodyReader.ReadToEndAsync();
            }

            // 將 HTTP Request 的 Stream 起始位置歸零
            context.Request.Body.Position = 0;
            string managerId = GetManagerId(context)?.ToString();

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                await _next(context).ConfigureAwait(false);
                sw.Stop();

                var controllerName = context.Request.RouteValues["controller"]?.ToString();
                if (!string.IsNullOrEmpty(controllerName) && _controllerList.TryGetValue(controllerName, out var controllerType))
                {
                    //var entity = new TableEntity(nameof(Logging), Guid.NewGuid().ToString())
                    //    {
                    //        { "ManagerId", managerId },
                    //        { "QueryString", context.Request.QueryString.ToString() },
                    //        { "Path", context.Request.Path.ToString() },
                    //        { "Body", body },
                    //        { "IPAddress", context.Connection.RemoteIpAddress?.ToString() },
                    //        { "LoadTime", sw.ElapsedMilliseconds },
                    //        { "LocalTimestamp", SystemVariable.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffffff") },
                    //    };
                    //_ = _azureStoreService.Upsert(TableClientType.Log, entity);
                }
            }
            catch (Exception ex)
            {
                //var entity = new TableEntity("Error", Guid.NewGuid().ToString())
                //{
                //    { "ErrorMessage", ex.ToString() },
                //    { "ManagerId", managerId },
                //    { "QueryString", context.Request.QueryString.ToString() },
                //    { "Path", context.Request.Path.ToString() },
                //    { "IPAddress", context.Connection.RemoteIpAddress?.ToString() },
                //    { "LocalTimestamp", SystemVariable.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffffff") },
                //};
                //_ = _azureStoreService.Upsert(TableClientType.Log, entity);
            }
        }

        private Guid? GetManagerId(HttpContext context)
        {
            Guid? managerId;

            if (context.Session.GetString("ManagerInfo") == null)
                managerId = null;
            else
                managerId = JsonConvert.DeserializeObject<SessionManagerInfo>(context.Session.GetString("ManagerInfo")).RealManagerId;

            return managerId;
        }
    }

}
