using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class SellerCompareService : _StandardService<Material, SellerCompareModel, SellerCompareSearchModel, SellerCompareListViewModel, SellerCompareListDataModel>
    {
        public SellerCompareService(BaseServiceArgument argument) : base(argument) { }


        public override (string, string, string) GetListQueryString(SellerCompareSearchModel searchModel)
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

        protected override async Task SetModel(SellerCompareModel viewModel) 
        {
            await base.SetModel(viewModel);
            viewModel.MaterialCompareList = await _lazy.MaterialCompareService.Value.GetModelListByParent<Material>(viewModel.Id);
        }

        public override async Task Update(SellerCompareModel viewModel) 
        {
            await _lazy.MaterialCompareService.Value.UpdateDetail(viewModel, viewModel.MaterialCompareList);
        }

    }
}
