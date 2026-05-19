using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class BuyerCompareService : _StandardService<Material, BuyerCompareModel, BuyerCompareSearchModel, BuyerCompareListViewModel, BuyerCompareListDataModel>
    {
        public BuyerCompareService(BaseServiceArgument argument) : base(argument) { }


        public override (string, string, string) GetListQueryString(BuyerCompareSearchModel searchModel)
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
                SELECT main.*, 
                       Manager.Name AS UpdateUser, 
                       Supplier.Name AS SupplierName,
                       (
                           SELECT COUNT(1)
                               FROM MaterialSpec MS
                           WHERE MS.MaterialId = main.Id
                               AND MS.[Status] = 1
                       ) AS SpecCount,
                       (
                            SELECT COUNT(1)
                              FROM MaterialCompare MC
                             INNER JOIN Material M ON MC.BuyerMaterialId = M.Id AND MC.[Status] = 1
                             WHERE M.Id = main.Id
                               AND MC.[Status] = 1
                               AND NOT EXISTS 
                                   (
                                     SELECT 1
                                       FROM MaterialSpec MS
                                      WHERE MS.MaterialCompareId = MC.Id
                                        AND MS.[Status] = 1
                                    )
                       ) AS NotCompareCount,
                       (
                           SELECT STRING_AGG(S.Name, '、') 
                             FROM MaterialCompare MC
		                     left join Material M on MC.BuyerMaterialId = M.Id
		                     left join Supplier S on M.SupplierId = S.id
		                    where MC.MaterialId = main.Id
                              and MC.[Status] = 1
                       ) AS BuyerName
                  FROM Material AS main WITH(NOLOCK)
                  LEFT JOIN Manager WITH(NOLOCK) ON main.UpdateUserId = Manager.Id
                  LEFT JOIN Supplier ON main.SupplierId = Supplier.Id
                ";
        }

        public override async Task<BuyerCompareModel> GetModel(Guid id)
        {
            string sql = SqlData(select: "SELECT main.*, Supplier.Name as SupplierName",
                                   join: "left join Supplier on main.SupplierId = Supplier.Id");

            BuyerCompareModel viewModel = await QueryFirstAsync<BuyerCompareModel>(sql, new { Id = id });

            viewModel.MaterialSpecList = await _lazy.MaterialSpecService.Value.GetModelListByParent<Material>(id);

            return viewModel;
        }

        public override async Task Update(BuyerCompareModel viewModel) 
        {
            List<Task> tasks = new();

            tasks.Add(_lazy.MaterialSpecService.Value.UpdateDetail(viewModel, viewModel.MaterialSpecList));

            foreach (var item in viewModel.DeleteMaterialCompareIdList) 
            {
                tasks.Add(_lazy.MaterialCompareService.Value.Delete(item));
            }
            await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<BuyerMaterialCompare>> GetBuyerMaterialList(Guid id)
        {
            string sql = $@"
select MC.*, M.ProductModel, M.ProductName, BM.MaterialNumber as SellerMaterialNumber, BM.ProductName as SellerProductName
  from Material M
 inner join MaterialCompare MC on MC.BuyerMaterialId = M.Id and MC.[Status] = 1
  left join Material as BM on MC.MaterialId = BM.Id
  left join Supplier S on BM.SupplierId = S.Id
 where M.Id = @MaterialId";

            IEnumerable<BuyerMaterialCompare> list = await QueryAsync<BuyerMaterialCompare>(sql, new { ManagerId = _currentManager?.Id, MaterialId = id });

            return list;
        }

    }
}
