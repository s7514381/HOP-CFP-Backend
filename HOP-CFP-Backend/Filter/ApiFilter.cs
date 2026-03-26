using Dapper;
using HOP_CFP_Backend.Library.Repositories;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Filter
{
    public class ApiFilter(ManagerService managerService, IMemoryCache cache, IDbConnectionFactory factory) : BaseFilter
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any()) { await next(); return; }

            Guid? token = GetToken(context.HttpContext);
#if DEBUG
            if (!token.HasValue)
            {
                await factory.ConnectAsync(async (conn) => {
                    token = await conn.QueryFirstOrDefaultAsync<Guid?>($@"select top 1 Id from Log_ManagerLogin order by CreateDate desc");
                });

                if (!cache.TryGetValue(token.Value, out ManagerSessionModel? wmodel)) 
                {
                    ApiResult<Guid?> result = await managerService.Login(new ManagerLoginViewModel { Account = "string", Password = "string" });
                    token = result.Data;
                }
            }
            
#endif

            if (token == null) { reject(); return; }

            if (!cache.TryGetValue(token.Value, out ManagerSessionModel? model))
            {
                reject();
                return;
            }
            else 
            {
                cache.Set(token.Value, model, new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(20)
                });

                managerService.CurrentManager = model;
            }
            
            await next().ConfigureAwait(false);

            void reject()
            {
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    Content = "身份驗證失敗",
                    ContentType = "text/plain"
                };
            }
        }

        public static Guid? GetToken(HttpContext context)
        {
            var headers = context.Request.Headers;
            string sToken = headers["Authorization"].ToString();

            return Guid.TryParse(sToken, out Guid token) ? token : null;
        }
    }
}
