using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class MaterialGroupService : _StandardService<MaterialGroup, MaterialGroupModel, MaterialGroupSearchViewModel, MaterialGroupListViewModel, MaterialGroupListDataModel>
    {
        public MaterialGroupService(BaseServiceArgument argument) : base(argument) { }

        protected override async Task SetModel(MaterialGroupModel viewModel) 
        {
            await base.SetModel(viewModel);

            viewModel.MaterialList = (await _lazy.MaterialService.Value.GetMTMModelList(viewModel.Id)).ToList();
        }

        protected override async Task ModelSave(MaterialGroupModel viewModel) 
        {
            await base.ModelSave(viewModel);

            await _lazy.ManyToManyService.Value.SaveById(viewModel, viewModel.MaterialList.Select(x => x.Id), CommonUtility.GetTableAttribute<Material>());
        }

    }
}
