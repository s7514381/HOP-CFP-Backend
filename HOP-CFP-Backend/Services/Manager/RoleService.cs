using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Models.Manager;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Services
{
    public class RoleService : _StandardService<Role, RoleModel, RoleSearchViewModel, RoleListViewModel, RoleListDataModel>
    {
        public RoleService(BaseServiceArgument argument) : base(argument) { }

        public override string GetBaseWhere()
        {
            string result = base.GetBaseWhere();
            result += $@" and [Type] = 3";

            return result;
        }

        public override string GetListQueryString_MainSQL()
        {
            return $@"
                 SELECT main.* ,  
                        (
                             SELECT STRING_AGG(AM.Title, '¡B') WITHIN GROUP (ORDER BY AM.[Sequence] ASC)
                               FROM AdminMenu AM
	                           LEFT JOIN ManyToMany MTM on AM.Id = MTM.TargetId
	                           WHERE MTM.SourceId = main.Id
	                           AND AM.ParentId is not null
                         ) AS AdminMenuName
                   FROM [Role] as main
                   LEFT JOIN Manager WITH(NOLOCK) ON main.UpdateUserId = Manager.Id
                ";
        }


        protected override async Task SetModel(RoleModel viewModel)
        {
            List<Task> tasks = new List<Task>();

            tasks.AddTask(async () => {
                await base.SetModel(viewModel);
            });
            tasks.AddTask(async () => {
                viewModel.SelectedAdminMenuIds = (await _lazy.ManyToManyService.Value.GetIdsBySource(viewModel.Id, nameof(AdminMenu))).ToList();
            });
            tasks.AddTask(async () => {
                viewModel.SelectedAdminFunctionIds = (await _lazy.ManyToManyService.Value.GetIdsBySource(viewModel.Id, nameof(AdminFunction))).ToList();
            });
            await Task.WhenAll(tasks);
        }

        protected override async Task ModelSave(RoleModel viewModel)
        {
            List<Task> tasks = new List<Task>();

            if (!viewModel.Type.HasValue) { viewModel.Type = RoleType.¤@¯ë­û¤u;  }

            await base.ModelSave(viewModel);

            tasks.Add(_lazy.ManyToManyService.Value.SaveById(viewModel, viewModel.SelectedAdminMenuIds, nameof(AdminMenu)));
            tasks.Add(_lazy.ManyToManyService.Value.SaveById(viewModel, viewModel.SelectedAdminFunctionIds, nameof(AdminFunction)));

            await Task.WhenAll(tasks);
        }

        public async Task<Role> GetDataByType(RoleType type) 
        {
            string sql = Sql(subWhere: " and main.[Type] = @Type");
            Role result = await QueryFirstAsync<Role>(sql, new { Type = type });
            return result;
        }

        public async Task<IEnumerable<SelectListItem>> GetSelectListItems()
        {
            var result = await QueryAsync<(Guid Value, string Text)>($@"
                SELECT top 100 main.{_keyField} AS Value, main.Name AS Text
                  FROM {_tableName} as main
                  LEFT JOIN Manager on main.CreateUserId = Manager.Id
                 WHERE main.Status != -1
                   AND main.[Type] in (2, 3, 4)
                   AND Manager.TaxID = @TaxID", new { _currentManager?.TaxID });
            return result.Select(x => new SelectListItem(x.Text, x.Value.ToString()));
        }

        public async Task<IEnumerable<FullAdminMenuModel>?> GetRoleAdminMenus(Guid managerId) 
        {
            List<Task> tasks = new List<Task>();
            List<Guid> selectedAdminMenuIds = new List<Guid>();
            List<Guid> selectedAdminFunctionIds = new List<Guid>();
            IEnumerable<FullAdminMenuModel>? fullAdminMenus = new List<FullAdminMenuModel>();
            List<FullAdminMenuModel> result = new();

            Role role = await _lazy.RoleService.Value.GetMTMData(managerId);

            tasks.AddTask(async () => {
                selectedAdminMenuIds = (await _lazy.ManyToManyService.Value.GetIdsBySource(role.Id, nameof(AdminMenu))).ToList();
            });
            tasks.AddTask(async () => {
                selectedAdminFunctionIds = (await _lazy.ManyToManyService.Value.GetIdsBySource(role.Id, nameof(AdminFunction))).ToList();
            });
            tasks.AddTask(async () => {
                fullAdminMenus = await QueryAsync<FullAdminMenuModel>(_lazy.AdminMenuService.Value.Sql());
            });
            await Task.WhenAll(tasks); tasks.Clear();

            fullAdminMenus = fullAdminMenus?.Where(x => selectedAdminMenuIds.Contains(x.Id) && x.Status == EStatus.Enable);
            if(fullAdminMenus == null
               || !fullAdminMenus.Any()) { return new List<FullAdminMenuModel>(); }

            foreach (FullAdminMenuModel model in fullAdminMenus)
            {
                if (model.AdminFunctionId.HasValue
                    && selectedAdminFunctionIds.Contains(model.AdminFunctionId.Value))
                {
                    tasks.AddTask(async () => {
                        model.AdminFunction = await _lazy.AdminFunctionService.Value.GetModel(model.AdminFunctionId.Value);
                    });
                }
            }
            await Task.WhenAll(tasks);

            result.AddRange(fullAdminMenus.Where(x => !x.ParentId.HasValue));

            foreach (FullAdminMenuModel model in result)
            {
                model.ChildList = fullAdminMenus.Where(x => x.ParentId == model.Id).ToList();
            }

            return result;
        }

    }
}
