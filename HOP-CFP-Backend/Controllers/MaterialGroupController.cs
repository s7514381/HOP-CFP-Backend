using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HOP_CFP_Backend.Controllers
{
    public class MaterialGroupController : StandardController<MaterialGroup, MaterialGroupModel, MaterialGroupSearchViewModel, MaterialGroupListViewModel, MaterialGroupListDataModel>
    {
        private MaterialGroupService _materialGroupService => _lazy.MaterialGroupService.Value;

        public MaterialGroupController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.MaterialGroupService.Value) { }

    }
}
