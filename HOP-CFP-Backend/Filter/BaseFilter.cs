using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Text;

namespace HOP_CFP_Backend.Filter
{
    public class BaseFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public T GetSession<T>(ActionExecutingContext context, string sessionName)
        {
            byte[] sessionBytes = null;
            if (context.HttpContext?.Session.TryGetValue(sessionName, out sessionBytes) == false) {
                return default;
            }

            var jsonSession = Encoding.UTF8.GetString(sessionBytes);
            T sessionModel = JsonConvert.DeserializeObject<T>(jsonSession);

            return sessionModel;
        }

    }
}
