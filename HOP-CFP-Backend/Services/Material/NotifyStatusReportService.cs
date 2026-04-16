using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class NotifyStatusReportService : _StandardService<MaterialNotify, NotifyStatusReportModel, NotifyStatusReportSearchModel, NotifyStatusReportListViewModel, NotifyStatusReportListDataModel>
    {
        public NotifyStatusReportService(BaseServiceArgument argument) : base(argument) { }


        public override string GetListQueryString_MainSQL()
        {
            return $@"
                 SELECT main.* FROM (
                   SELECT CAST(MN.[CreateDate] AS DATE) AS [Date]
                          , S.Name as [SupplierName]          
                          , SUM(CASE WHEN [IsSend] = 1 THEN 1 ELSE 0 END) AS [SentCount]   
                          , SUM(CASE WHEN [IsUpdate] = 1 THEN 1 ELSE 0 END) AS [UpdateCount] 
		                  , MN.UpdateUserId
                          , MN.[Status]
                     FROM MaterialNotify AS MN
                     LEFT JOIN Material AS M ON MN.[MaterialId] = M.[Id]
                     LEFT JOIN Supplier S on M.SupplierId = S.Id
                    GROUP BY MN.UpdateUserId, CAST(MN.[CreateDate] AS DATE), S.Name, MN.[Status]
                  ) main
                  LEFT JOIN Manager with(NOLOCK) ON main.UpdateUserId = Manager.Id
                ";
        }

        public override (string, string, string) GetListQueryString(NotifyStatusReportSearchModel searchModel)
        {
            (string mainSql, string baseWhere, string sqlWhere) = base.GetListQueryString(searchModel);

            if (searchModel.CreateDateFrom.HasValue)
                sqlWhere += $" and Date >= @CreateDateFrom ";

            if (searchModel.CreateDateTo.HasValue)
                sqlWhere += $" and Date <= @CreateDateTo ";

            if (!string.IsNullOrEmpty(searchModel.SupplierName))
                sqlWhere += $" and SupplierName  LIKE '%' + @SupplierName + '%' ";

            return (mainSql, baseWhere, sqlWhere);
        }

        public override string GetOrderSql<T>(NotifyStatusReportSearchModel searchModel) where T : class
        {
            return "ORDER BY Date DESC";
        }

        public override async Task<List<NotifyStatusReportListDataModel>> HandleList(List<NotifyStatusReportListDataModel> list)
        {
            foreach (var item in list)
            {
                item.strDate = item.Date?.ToString("yyyy/MM/dd");
            }

            return list;
        }

    }
}
