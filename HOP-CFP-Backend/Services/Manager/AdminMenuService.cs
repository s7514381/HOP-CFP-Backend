using HOP_CFP_Backend.Library.Models.Manager;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HOP_CFP_Backend.Services
{
    public class AdminMenuService : _StandardService<AdminMenu, AdminMenuModel, AdminMenuSearchViewModel, AdminMenuListViewModel, AdminMenuListDataModel>
    {
        public AdminMenuService(BaseServiceArgument argument) : base(argument) { }

        public override string GetBaseWhere()
        {
            string result = base.GetBaseWhere();
            result += $@" and ParentId is null";

            return result;
        }

        protected override async Task SetModel(AdminMenuModel viewModel)
        {
            List<Task> tasks = new List<Task>();

            tasks.Add(base.SetModel(viewModel));

            tasks.AddTask(async () => {
                viewModel.ChildList = await GetModelListByParent<AdminMenu>(viewModel.Id);
            });
            await Task.WhenAll(tasks);
        }

        protected override async Task ModelSave(AdminMenuModel viewModel)
        {
            await base.ModelSave(viewModel);

            await UpdateDetail(viewModel, viewModel.ChildList);
        }

        public async Task<IEnumerable<FullAdminMenuModel>> GetAdminMenus()
        {
            List<Task> tasks = new List<Task>();
            List<FullAdminMenuModel> result = new();

            string sql = Sql();
            IEnumerable<FullAdminMenuModel> list = await QueryAsync<FullAdminMenuModel>(sql);
            list = list.Where(x => x.Status == Library.Models.EStatus.Enable);

            foreach (FullAdminMenuModel model in list) 
            {
                if (model.AdminFunctionId.HasValue) 
                {
                    tasks.AddTask(async () => {
                        model.AdminFunction = await _lazy.AdminFunctionService.Value.GetModel(model.AdminFunctionId.Value);
                    });
                }
            }
            await Task.WhenAll(tasks);

            result.AddRange(list.Where(x => !x.ParentId.HasValue));

            foreach (FullAdminMenuModel model in result) 
            {
                model.ChildList = list.Where(x => x.ParentId == model.Id).ToList();
            }   

            return result;
        }

    }
}
