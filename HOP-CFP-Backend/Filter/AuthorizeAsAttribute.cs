using System;

namespace HOP_CFP_Backend.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeAsAttribute : Attribute
    {
        public string AsAction { get; set; }
        public string AsController { get; set; }

        public AuthorizeAsAttribute(string actionName)
        {
            AsAction = actionName;
        }
        public AuthorizeAsAttribute(string actionName, string controllerName)
        {
            AsAction = actionName;
            AsController = controllerName;
        }
    }
}
