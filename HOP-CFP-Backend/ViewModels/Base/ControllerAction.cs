using System.Collections.Generic;
using System.Reflection;

namespace HOP_CFP_Backend.ViewModels
{
    public class ControllerAction
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public List<CustomAttributeData> CustomAttributes { get; set; }
    }



}
