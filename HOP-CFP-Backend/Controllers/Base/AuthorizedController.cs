using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Filter;
using System;

namespace HOP_CFP_Backend.Controllers
{
    //[ServiceFilter(typeof(AuthorizeFilter))]
    [ServiceFilter(typeof(ApiFilter))]
    public class AuthorizedController : BaseController
    {
        public AuthorizedController(BaseControllerArgument argument) : base(argument) { }

        /// <summary>
        /// 檢查是否可以進去活動頁面
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> AccessEvent(Guid? eventId, Func<Task<IActionResult>> func)
        {
            if (!eventId.HasValue) { return AlertView("參數錯誤"); }

            return await func();
        }

        protected virtual async Task<IActionResult> AccessEvent(string eventIdStr, Func<Task<IActionResult>> func)
        {
            Guid? eventId = null;
            if (Guid.TryParse(eventIdStr, out Guid parsedEventId))
            {
                eventId = parsedEventId;
            }
            return await AccessEvent(eventId, func);
        }

    }
}
