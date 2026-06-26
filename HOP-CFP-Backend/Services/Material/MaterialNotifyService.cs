using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class MaterialNotifyService : _StandardService<MaterialNotify, MaterialNotifyModel, MaterialNotifySearchViewModel, MaterialNotifyListViewModel, MaterialNotifyListDataModel>
    {
        public MaterialNotifyService(BaseServiceArgument argument) : base(argument) { }

        public override string GetListQueryString_MainSQL()
        {
            return $@"
                 SELECT main.*, 
                       Manager.Name AS UpdateUser, 
                       Supplier.Name AS SupplierName,
                       (
                           SELECT STRING_AGG(MG.Name, '、') WITHIN GROUP (ORDER BY MG.CreateDate ASC)
                           FROM ManyToMany MTM
                           INNER JOIN MaterialGroup MG ON MTM.SourceId = MG.Id
                           WHERE MTM.TargetId = main.Id 
                             AND MTM.TargetTable = 'Material'
                       ) AS MaterialGroupName
                  FROM Material AS main WITH(NOLOCK)
                  LEFT JOIN Manager WITH(NOLOCK) ON main.UpdateUserId = Manager.Id
                  LEFT JOIN Supplier ON main.SupplierId = Supplier.Id
                ";
        }

        public override (string, string, string) GetListQueryString(MaterialNotifySearchViewModel searchModel)
        {
            (string mainSql, string baseWhere, string sqlWhere) = base.GetListQueryString(searchModel);

            if (searchModel.UpdateDateFrom.HasValue)
                sqlWhere += $" and main.UpdateDate >= @UpdateDateFrom ";

            if (searchModel.UpdateDateTo.HasValue)
            {
                searchModel.UpdateDateTo = NormalizeEndDateForExclusiveUpperBound(searchModel.UpdateDateTo.Value);
                sqlWhere += $" and main.UpdateDate < @UpdateDateTo ";
            }

            if (!string.IsNullOrEmpty(searchModel.MaterialGroupName))
                    sqlWhere += @" and exists (
                        select 1 from ManyToMany MTM2
                        inner join MaterialGroup MG2 on MTM2.SourceId = MG2.Id
                        where MTM2.TargetId = main.Id
                          and MTM2.TargetTable = 'Material'
                          and MG2.Name LIKE '%' + @MaterialGroupName + '%'
                    )";

            if (!string.IsNullOrEmpty(searchModel.ProductModel))
                sqlWhere += $" and main.ProductModel LIKE '%' + @ProductModel + '%' ";

            if (!string.IsNullOrEmpty(searchModel.SupplierName))
                sqlWhere += $" and Supplier.Name LIKE '%' + @SupplierName + '%' ";

            return (mainSql, baseWhere, sqlWhere);
        }

        public async Task AddNotify(List<Guid> ids)
        {
            List<Task> tasks = new List<Task>();
            foreach (Guid id in ids)
            {
                tasks.AddTask(async () =>
                {
                    string email = await QueryFirstAsync<string>($@"
                        select Supplier.Email from Material
                          left join Supplier on Material.SupplierId = Supplier.Id
                         where Material.Id = @MaterialId"
                        , new
                        {
                            MaterialId = id
                        });
                    email = email.Trim();

                    if (!string.IsNullOrEmpty(email)) 
                    {
                        _ = _mailSender.SentAsync(email, $@"[測試]供應商平台", $@"
請至 <a href='{WebUrl}/login'>供應商平台</a> 更新您的資料");
                    }
                });

                tasks.AddTask(async () => 
                {
                    MaterialNotifyModel model = new MaterialNotifyModel()
                    {
                        MaterialId = id,
                        IsSend = true
                    };
                    await Insert(model);
                });
            }
            await Task.WhenAll(tasks);
        }

    }
}
