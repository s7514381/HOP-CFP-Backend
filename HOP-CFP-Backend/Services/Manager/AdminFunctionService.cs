using HOP_CFP_Backend.Library.Models.Manager;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class AdminFunctionService : _StandardService<AdminFunction, AdminFunctionModel, AdminFunctionSearchViewModel, AdminFunctionListViewModel, AdminFunctionListDataModel>
    {
        public AdminFunctionService(BaseServiceArgument argument) : base(argument) { }

        protected virtual async Task SetModel(AdminFunctionModel viewModel) 
        {
            await base.SetModel(viewModel);

            viewModel.ChildList = await GetModelListByParent("ParentId", viewModel.Id);
        }

        protected override async Task ModelSave(AdminFunctionModel viewModel) 
        {
            await base.ModelSave(viewModel);

            await UpdateDetail(viewModel.ChildList, "ParentId", viewModel.Id);
        }

    }
}
