using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class MaterialNotifyService : _StandardService<MaterialNotify, MaterialNotifyModel, MaterialNotifySearchViewModel, MaterialNotifyListViewModel, MaterialNotifyListDataModel>
    {
        public MaterialNotifyService(BaseServiceArgument argument) : base(argument) { }



    }
}
