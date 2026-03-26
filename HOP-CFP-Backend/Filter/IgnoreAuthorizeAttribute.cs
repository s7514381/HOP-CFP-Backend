using System;

namespace HOP_CFP_Backend.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class IgnoreAuthorizeAttribute : Attribute
    {

        public IgnoreAuthorizeAttribute()
        {
        }
    }
}
