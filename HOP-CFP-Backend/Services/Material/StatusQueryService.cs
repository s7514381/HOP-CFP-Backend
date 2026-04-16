using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class StatusQueryService : _StandardService<MaterialNotify, StatusQueryModel, StatusQuerySearchViewModel, StatusQueryListViewModel, StatusQueryListDataModel>
    {
        public StatusQueryService(BaseServiceArgument argument) : base(argument) { }


        public override string GetListQueryString_MainSQL()
        {
            return $@"
                 SELECT main.*, Manager.Name AS UpdateUser, M.MaterialNumber, M.ProductModel, M.ProductName, S.Name as SupplierName
                   FROM MaterialNotify main
                   LEFT JOIN Manager with(NOLOCK) ON main.UpdateUserId = Manager.Id
                   LEFT JOIN Material M on main.MaterialId = M.Id
                   LEFT JOIN Supplier S on M.SupplierId = S.Id
                ";
        }

        public override (string, string, string) GetListQueryString(StatusQuerySearchViewModel searchModel)
        {
            (string mainSql, string baseWhere, string sqlWhere) = base.GetListQueryString(searchModel);

            if (searchModel.IsSend.HasValue)
                sqlWhere += $" and main.IsSend = @IsSend ";

            if (searchModel.IsUpdate.HasValue)
                sqlWhere += $" and main.IsUpdate = @IsUpdate ";

            if (searchModel.UpdateDateFrom.HasValue)
                sqlWhere += $" and main.UpdateDate >= @UpdateDateFrom ";

            if (searchModel.UpdateDateTo.HasValue)
                sqlWhere += $" and main.UpdateDate <= @UpdateDateTo ";

            return (mainSql, baseWhere, sqlWhere);
        }

        public override string GetOrderSql<T>(StatusQuerySearchViewModel searchModel) where T : class
        {
            return "ORDER BY CreateDate DESC, UpdateDate DESC";
        }

        public override async Task<List<StatusQueryListDataModel>> HandleList(List<StatusQueryListDataModel> list)
        {
            foreach (StatusQueryListDataModel item in list)
            {
                if (item.IsSend == true)
                {
                    item.strCreateDate = item.CreateDate?.ToString("yyyy/MM/dd HH:mm:ss");
                }

                if (item.IsUpdate == true)
                {
                    item.strUpdateDate = item.UpdateDate?.ToString("yyyy/MM/dd HH:mm:ss");
                }
            }

            return list;
        }

    }
}
