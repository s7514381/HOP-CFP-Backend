using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class MaterialGroupService : _StandardService<MaterialGroup, MaterialGroupModel, MaterialGroupSearchViewModel, MaterialGroupListViewModel, MaterialGroupListDataModel>
    {
        public MaterialGroupService(BaseServiceArgument argument) : base(argument) { }



    }
}
