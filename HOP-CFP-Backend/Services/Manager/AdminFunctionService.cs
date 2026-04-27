using HOP_CFP_Backend.Library.Models.Manager;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HOP_CFP_Backend.Services
{
    public class AdminFunctionService : _StandardService<AdminFunction, AdminFunctionModel, AdminFunctionSearchViewModel, AdminFunctionListViewModel, AdminFunctionListDataModel>
    {
        public AdminFunctionService(BaseServiceArgument argument) : base(argument) { }

        public override string GetBaseWhere()
        {
            string result = base.GetBaseWhere();
            result += $@" and ParentId is null";

            return result;
        }

        protected override async Task SetModel(AdminFunctionModel viewModel) 
        {
            await base.SetModel(viewModel);

            viewModel.ChildList = await GetModelListByParent<AdminFunction>(viewModel.Id);
        }

        protected override async Task ModelSave(AdminFunctionModel viewModel) 
        {
            await base.ModelSave(viewModel);

            await UpdateDetail(viewModel, viewModel.ChildList);
        }

        public async Task<IEnumerable<SelectListItem>> GetSelectListItems()
        {
            var result = await QueryAsync<(Guid Value, string Text)>($@"
                SELECT top 100 {_keyField} AS Value, Title AS Text
                FROM {_tableName}
                WHERE Status != -1
                  AND ParentId is null");
            return result.Select(x => new SelectListItem(x.Text, x.Value.ToString()));
        }

    }
}
