using Dapper;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

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

        public override async Task<IEnumerable<SelectListItem>> GetSelectListItems(string? keyword)
        {
            var result = await QueryAsync<(Guid Value, string Text)>($@"
                SELECT top 100 main.{_keyField} AS Value, MaterialNumber + ' - ' + ProductName AS Text
                FROM {_tableName} as main
                left join Manager on main.CreateUserId = Manager.Id
                WHERE main.Status != -1
                  AND Manager.TaxID = @TaxID
                  AND (MaterialNumber LIKE @Keyword OR ProductName LIKE @Keyword ) ", new { Keyword = $"%{keyword}%", _currentManager?.TaxID });
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

        public async Task<IEnumerable<SelectListItem>> GetKeywordSelectListItems(string? keyword)
        {
            // 1. 如果沒輸入關鍵字，回傳空或預設前 50 筆
            if (string.IsNullOrWhiteSpace(keyword))
            {
                // 這裡可以依需求決定回傳空列表，或是執行不帶關鍵字的 TOP 50 查詢
                return Enumerable.Empty<SelectListItem>();
            }

            // 2. 拆分關鍵字 (例如 "4230 1838" -> ["4230", "1838"])
            var keywords = keyword.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var sqlBuilder = new StringBuilder($@"
        SELECT TOP 50 M.Id AS Value, S.[Name] + '(' + S.TaxID + ') - ' + M.MaterialNumber AS Text
          FROM Material M
          LEFT JOIN Manager on M.UpdateUserId = Manager.Id
          LEFT JOIN Supplier S ON Manager.TaxID = S.TaxID AND S.[Status] = 1
         WHERE M.[Status] = 1 ");

            var parameters = new DynamicParameters();
            var allConditions = new List<string>();

            // 3. 針對每一個拆分出來的字，建立一組括號內的 OR 條件
            for (int i = 0; i < keywords.Length; i++)
            {
                string paramName = $"@p{i}";
                // 這裡的邏輯是：(該字出現在 TaxID 或 Name 或 MaterialNumber)
                allConditions.Add($"(S.TaxID LIKE {paramName} OR S.[Name] LIKE {paramName} OR M.MaterialNumber LIKE {paramName})");
                parameters.Add(paramName, $"%{keywords[i]}%");
            }

            // 4. 將這些「組」用 AND 連接起來
            if (allConditions.Any())
            {
                sqlBuilder.Append(" AND " + string.Join(" AND ", allConditions));
            }

            // 5. 執行 SQL
            var result = await QueryAsync<(Guid Value, string Text)>(sqlBuilder.ToString(), parameters);

            return result.Select(x => new SelectListItem(x.Text, x.Value.ToString()));
        }


    }
}
