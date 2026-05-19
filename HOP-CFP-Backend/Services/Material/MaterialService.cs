using Dapper;
using Microsoft.AspNetCore.Http;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace HOP_CFP_Backend.Services
{
    public class MaterialService : _StandardService<Material, MaterialModel, MaterialSearchViewModel, MaterialListViewModel, MaterialListDataModel>
    {
        private static readonly string[] ImportHeaders = ["供應商統編", "料號", "產品型號", "產品名稱", "是否可銷售"];

        static MaterialService()
        {
            ExcelPackage.License.SetNonCommercialOrganization("HOP-CFP");
        }

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
                SELECT TOP 50 M.Id AS Value, Manager.[Name] + '(' + Manager.TaxID + ') - ' + M.MaterialNumber AS Text
                  FROM Material M
                  LEFT JOIN Manager on M.UpdateUserId = Manager.Id
                 WHERE M.[Status] = 1 ");

            var parameters = new DynamicParameters();
            var allConditions = new List<string>();

            // 3. 針對每一個拆分出來的字，建立一組括號內的 OR 條件
            for (int i = 0; i < keywords.Length; i++)
            {
                string paramName = $"@p{i}";
                // 這裡的邏輯是：(該字出現在 TaxID 或 Name 或 MaterialNumber)
                allConditions.Add($"(Manager.TaxID LIKE {paramName} OR Manager.[Name] LIKE {paramName} OR M.MaterialNumber LIKE {paramName})");
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

        public byte[] BuildImportTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("MaterialImportTemplate");

            for (int column = 0; column < ImportHeaders.Length; column++)
            {
                worksheet.Cells[1, column + 1].Value = ImportHeaders[column];
                worksheet.Cells[1, column + 1].Style.Font.Bold = true;
            }

            worksheet.Cells[2, 1].Value = "12345678";
            worksheet.Cells[2, 2].Value = "MAT-0001";
            worksheet.Cells[2, 3].Value = "MODEL-001";
            worksheet.Cells[2, 4].Value = "範例產品";
            worksheet.Cells[2, 5].Value = "是";

            worksheet.Cells.AutoFitColumns();

            using var stream = new MemoryStream();
            package.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<MaterialImportResult> ImportFromCsv(IFormFile file, bool ignoreErrors = true)
        {
            if (file == null || file.Length == 0)
            {
                return new MaterialImportResult
                {
                    TotalCount = 0,
                    FailureCount = 1,
                    Errors = ["沒有可匯入的檔案。"]
                };
            }

            var lines = await ImportFileUtility.ReadImportLinesAsync(file);

            if (lines.Count < 2)
            {
                return new MaterialImportResult
                {
                    TotalCount = 0,
                    FailureCount = 1,
                    Errors = new List<string> { "匯入檔案沒有可處理的資料。" }
                };
            }

            var headers = ImportFileUtility.ParseCsvLine(lines[0]).Select(x => x.Trim()).ToList();
            int supplierIndex = ImportFileUtility.GetHeaderIndex(headers, new[] { "供應商統編", "供應商名稱", "供應商" });
            int materialNumberIndex = ImportFileUtility.GetHeaderIndex(headers, new[] { "料號" });
            int productModelIndex = ImportFileUtility.GetHeaderIndex(headers, new[] { "產品型號" });
            int productNameIndex = ImportFileUtility.GetHeaderIndex(headers, new[] { "產品名稱" });
            int canSellIndex = ImportFileUtility.GetHeaderIndex(headers, new[] { "是否可銷售", "可銷售", "是/否" });

            if (new[] { supplierIndex, materialNumberIndex, productModelIndex, productNameIndex }.Any(x => x < 0))
            {
                return new MaterialImportResult
                {
                    TotalCount = 0,
                    FailureCount = 1,
                    Errors = new List<string> { "匯入格式不正確，請先下載範本。" }
                };
            }

            var result = new MaterialImportResult();
            foreach (var line in lines.Skip(1))
            {
                var values = ImportFileUtility.ParseCsvLine(line);
                if (values.Count == 0)
                {
                    continue;
                }

                result.TotalCount += 1;

                string supplierText = ImportFileUtility.GetValue(values, supplierIndex);
                string materialNumber = ImportFileUtility.GetValue(values, materialNumberIndex);
                string productModel = ImportFileUtility.GetValue(values, productModelIndex);
                string productName = ImportFileUtility.GetValue(values, productNameIndex);
                string canSellText = canSellIndex >= 0 ? ImportFileUtility.GetValue(values, canSellIndex) : "0";

                if (string.IsNullOrWhiteSpace(supplierText) || string.IsNullOrWhiteSpace(materialNumber) || string.IsNullOrWhiteSpace(productModel) || string.IsNullOrWhiteSpace(productName))
                {
                    AddImportError(result, materialNumber, "必填欄位缺漏", ignoreErrors);
                    if (!ignoreErrors) { break; }
                    continue;
                }

                var supplier = await FindSupplierAsync(supplierText);
                if (supplier == null)
                {
                    AddImportError(result, materialNumber, $"找不到供應商「{supplierText}」", ignoreErrors);
                    if (!ignoreErrors) { break; }
                    continue;
                }

                try
                {
                    await Insert(new MaterialModel
                    {
                        SupplierId = supplier.Id,
                        MaterialNumber = materialNumber,
                        ProductModel = productModel,
                        ProductName = productName,
                        CanSell = ParseCanSell(canSellText)
                    });
                    result.SuccessCount += 1;
                }
                catch (Exception ex)
                {
                    AddImportError(result, materialNumber, ex.Message, ignoreErrors);
                    if (!ignoreErrors) { break; }
                }
            }

            result.FailureCount = result.TotalCount - result.SuccessCount;
            return result;
        }

        private async Task<Supplier?> FindSupplierAsync(string supplierText)
        {
            string normalized = NormalizeText(supplierText);
            var suppliers = await QueryAsync<Supplier>($@"
                SELECT TOP 500 *
                  FROM Supplier WITH(NOLOCK)
                 WHERE [Status] != -1
                   AND CreateUserId = @CreateUserId
                 ORDER BY CreateDate DESC", new { CreateUserId = _currentManager?.Id });

            return suppliers.FirstOrDefault(s => MatchesSupplier(s, normalized));
        }

        private static bool MatchesSupplier(Supplier supplier, string normalizedText)
        {
            if (supplier == null) { return false; }

            var candidates = new[]
            {
                supplier.Name,
                supplier.TaxID,
                $"{supplier.TaxID} - {supplier.Name}",
                $"{supplier.Name} ({supplier.TaxID})"
            };

            return candidates.Any(candidate => NormalizeText(candidate) == normalizedText);
        }

        private static bool ParseCanSell(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) { return false; }

            string normalized = value.Trim();
            return normalized switch
            {
                "1" => true,
                "是" => true,
                "可" => true,
                _ when normalized.Equals("true", StringComparison.OrdinalIgnoreCase) => true,
                _ when normalized.Equals("yes", StringComparison.OrdinalIgnoreCase) => true,
                _ when normalized.Equals("y", StringComparison.OrdinalIgnoreCase) => true,
                "0" => false,
                "否" => false,
                _ when normalized.Equals("false", StringComparison.OrdinalIgnoreCase) => false,
                _ when normalized.Equals("no", StringComparison.OrdinalIgnoreCase) => false,
                _ when normalized.Equals("n", StringComparison.OrdinalIgnoreCase) => false,
                _ => false,
            };
        }

        private static string NormalizeText(string value)
        {
            return new string(value.Where(ch => !char.IsWhiteSpace(ch)).ToArray()).ToLowerInvariant();
        }

        private static void AddImportError(MaterialImportResult result, string materialNumber, string message, bool ignoreErrors)
        {
            result.Errors.Add($"{(string.IsNullOrWhiteSpace(materialNumber) ? "未填料號" : materialNumber)}：{message}");
        }


    }
}
