using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;

namespace HOP_CFP_Backend.Filter
{
    public class GlobalExceptionsFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionsFilter> _logger;

        public GlobalExceptionsFilter(ILogger<GlobalExceptionsFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
           _logger.LogError(context.Exception,
                "\r\n【請求路徑】：[{0}] {1}\r\n【異常類型】：{2}\r\n【異常訊息】：{3}\r\n【參數】：{4}\r\n【StackTrace】：{5}\r\n",
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path,
                context.Exception.GetType().Name,
                context.Exception.Message,
                context.HttpContext.Request.Method == "POST" ?
                    JsonSerializer.Serialize(
                        context.HttpContext.Request.Form.AsEnumerable().Where(x => x.Key != "__RequestVerificationToken"),
                        new JsonSerializerOptions() { WriteIndented = true }) : "",
                context.Exception.StackTrace);
        }
    }
}
