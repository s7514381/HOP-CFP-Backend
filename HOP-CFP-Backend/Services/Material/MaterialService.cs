using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HOP_CFP_Backend.Services
{
    public class MaterialService : _StandardService<Material, MaterialModel, MaterialSearchViewModel, MaterialListViewModel, MaterialListDataModel>
    {
        public MaterialService(BaseServiceArgument argument) : base(argument) { }

        public override (string, string, string) GetListQueryString(MaterialSearchViewModel searchModel)
        {
            (string mainSql, string baseWhere, string sqlWhere) = base.GetListQueryString(searchModel);

            if (!string.IsNullOrEmpty(searchModel.MaterialNumber))
                sqlWhere += $" and main.MaterialNumber LIKE '%' + @MaterialNumber + '%' ";

            if (!string.IsNullOrEmpty(searchModel.SupplierName))
                sqlWhere += $" and Supplier.Name LIKE '%' + @SupplierName + '%' ";

            return (mainSql, baseWhere, sqlWhere);
        }


        public override string GetListQueryString_MainSQL()
        {
            return $@"
                SELECT main.*, Manager.Name AS UpdateUser, Supplier.Name as SupplierName
                FROM {_tableName} AS main with(NOLOCK)
                LEFT JOIN Manager with(NOLOCK) ON main.UpdateUserId = Manager.Id
                LEFT JOIN Supplier ON main.SupplierId = Supplier.Id
                ";
        }

        public virtual async Task<IEnumerable<SelectListItem>> GetSelectListItems(string? keyword)
        {
            var result = await QueryAsync<(Guid Value, string Text)>($@"
                SELECT top 100 {_keyField} AS Value, MaterialNumber + ' - ' + ProductName AS Text
                FROM {_tableName}
                WHERE Status != -1
                  AND {_tableName}.CreateUserId = @CreateUserId
                  AND (MaterialNumber LIKE @Keyword OR ProductName LIKE @Keyword ) ", new { Keyword = $"%{keyword}%", CreateUserId = _currentManager?.ManagerId });
            return result.Select(x => new SelectListItem(x.Text, x.Value.ToString()));
        }

        public override async Task Update(MaterialModel viewModel)
        {
            await base.Update(viewModel);

            await ExecuteAsync($@"
update MaterialNotify
   set IsUpdate = 1
	   , UpdateDate = GETDATE()
  from MaterialNotify MN
  left join Material M on MN.MaterialId = M.Id
  left join Supplier S on M.SupplierId = S.Id
 where M.MaterialNumber = @MaterialNumber
   and S.TaxID = @TaxID
   and IsUpdate != 1", new { viewModel.MaterialNumber, _currentManager?.TaxID });
        }


        public async Task<BuyerCompareModel> GetBuyerCompareModel(Guid id)
        {
            string sql = SqlData(select: "SELECT main.*, Supplier.Name as SupplierName",
                                   join: "left join Supplier on main.SupplierId = Supplier.Id");

            BuyerCompareModel viewModel = await QueryFirstAsync<BuyerCompareModel>(sql, new { Id = id });

            viewModel.MaterialSpecList = await _lazy.MaterialSpecService.Value.GetModelListByParent<Material>(id);

            return viewModel;
        }

        public async Task EditBuyerCompareModel(BuyerCompareModel model)
        {
            await _lazy.MaterialSpecService.Value.UpdateDetail(model, model.MaterialSpecList);
        }

        public async Task<IEnumerable<BuyerMaterialCompare>> GetBuyerMaterialList(Guid id)
        {
            string sql = $@"
select MC.*, M.ProductModel, M.ProductName, SM.MaterialNumber as SellerMaterialNumber, SM.ProductName as SellerProductName
  from Material M
 inner join MaterialCompare MC on MC.MaterialNumber = M.MaterialNumber and MC.[Status] = 1
  left join Supplier S on MC.SupplierId = S.Id
  left join Manager on Manager.TaxID = S.TaxID
  left join Material as SM on MC.MaterialId = SM.Id
 where Manager.Id = @ManagerId
   and M.Id = @MaterialId";

            IEnumerable<BuyerMaterialCompare> list = await QueryAsync<BuyerMaterialCompare>(sql, new { ManagerId = _currentManager?.ManagerId, MaterialId = id });

            return list;
        }

        public async Task<SellerCompareModel> GetSellerCompareModel(Guid id)
        {
            SellerCompareModel viewModel = await QueryFirstAsync<SellerCompareModel>(SqlModelData(), new { Id = id });
            viewModel.MaterialCompareList = await _lazy.MaterialCompareService.Value.GetModelListByParent<Material>(id);

            return viewModel;
        }

        public async Task EditSellerCompareModel(SellerCompareModel model)
        {
            await _lazy.MaterialCompareService.Value.UpdateDetail(model, model.MaterialCompareList);
        }



    }
}
