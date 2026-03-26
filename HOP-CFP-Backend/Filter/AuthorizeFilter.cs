using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;


namespace HOP_CFP_Backend.Filter
{
    public class AuthorizeFilter : BaseFilter
    {
        private readonly ManagerService _managerService;

        public AuthorizeFilter(ManagerService managerService)
        {
            _managerService = managerService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //AllowAnonymous標籤不須登入
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any()) { await next(); return; }

            //檢查是否有登入
            SessionManagerInfo managerInfo = GetSession<SessionManagerInfo>(context, "ManagerInfo");

            if (managerInfo == null)
            {
                //context.Result = new RedirectToRouteResult(
                //    new RouteValueDictionary { { "controller", "Manager" }, { "action", "Login" } });

                context.Result = new ContentResult { Content = "" };
            }
            else
            {
                //檢查Manager的所有Role，有改到Role的就重新讀取權限
                DateTime? roleUpdateDate = await _managerService.CheckManagerRoleUpdateDate(managerInfo.RealManagerId);
                if (roleUpdateDate.HasValue)
                    if (roleUpdateDate.Value > managerInfo.LastApplyRoleDate)
                    {
                        await _managerService.SetSessionManagerInfo(managerInfo.RealManagerId);
                    }

                //IgnoreAuthorize標籤不須權限，例:首頁
                if (context.ActionDescriptor.EndpointMetadata.OfType<IgnoreAuthorizeAttribute>().Any()) { await next(); return; }

                //取得前往頁面
                IDictionary<string, string> routeValues = context.ActionDescriptor.RouteValues;
                string currentController = routeValues["Controller"];
                string currentAction = routeValues["Action"];

                //如果有AuthorizeAs標籤，則權限等同於參數的效力
                var authorizeAsAttribute = context.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAsAttribute>().FirstOrDefault();
                if (authorizeAsAttribute != null)
                {
                    currentAction = authorizeAsAttribute.AsAction;
                    if (!string.IsNullOrEmpty(authorizeAsAttribute.AsController)) { currentController = authorizeAsAttribute.AsController; }
                }

                bool hasPermission = false;
                //foreach (AdminMenuByRoleViewModel roleMenu in managerInfo.RoleMenuList)
                //{
                //    //選單本身功能或是子功能其一符合則有權限
                //    if (roleMenu.AdminFunction?.Controller == currentController && roleMenu.AdminFunction?.Action == currentAction)
                //    {
                //        hasPermission = true; break;
                //    }
                //    if (roleMenu.AdminFunctionList.Where(x => x.Controller == currentController && x.Action == currentAction).Any())
                //    {
                //        hasPermission = true; break;
                //    }
                //}

                if (!hasPermission) { context.Result = new ContentResult { Content = "權限不足" }; }
                else { await next(); }
            }
        }

        /// <summary>
        /// OnActionExecuted – after Action execute
        /// </summary>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        //OnResultExecuting – 在執行 Action Result 之前執行
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {

        }

        ////OnResultExecuted – 在執行 Action Result 之後執行
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {

        }

    }

}
