using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace HOP_CFP_Backend.Services
{
    public class SellerCompareService : _StandardService<Material, SellerCompareModel, SellerCompareSearchModel, SellerCompareListViewModel, SellerCompareListDataModel>
    {
        private static readonly string[] ImportHeaders = ["料號", "對照供應商統編", "對照料號"];

        static SellerCompareService()
        {
            ExcelPackage.License.SetNonCommercialOrganization("HOP-CFP");
        }

        public SellerCompareService(BaseServiceArgument argument) : base(argument) { }

        public override string GetBaseWhere()
        {
            return base.GetBaseWhere() + " and main.CanSell = 1";
        }


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
                          SELECT STRING_AGG(BuyerManager.Name + '(' + BuyerManager.TaxID + ') - ' + BuyerMaterial.MaterialNumber + '-' + ISNULL(BuyerSpec.SpecNumber, '未對照'), N'、') 
                            FROM MaterialCompare MC
                            LEFT JOIN Material BuyerMaterial on MC.BuyerMaterialId = BuyerMaterial.Id
                            LEFT JOIN MaterialSpec BuyerSpec on BuyerSpec.MaterialCompareId = MC.Id
		                    LEFT JOIN Manager BuyerManager on BuyerManager.Id = BuyerMaterial.UpdateUserId
                           WHERE MC.MaterialId = main.Id
                             AND MC.[Status] = 1
                       ) AS BuyerName
                  FROM Material AS main WITH(NOLOCK)
                  LEFT JOIN Manager WITH(NOLOCK) ON main.UpdateUserId = Manager.Id
                  LEFT JOIN Supplier ON main.SupplierId = Supplier.Id
                ";
        }

        public override async Task<SellerCompareModel> GetModel(Guid id)
        {
            SellerCompareModel viewModel = await base.GetModel(id);
            viewModel.MaterialCompareList = await _lazy.MaterialCompareService.Value.GetModelListByParent<Material>(viewModel.Id);
            return viewModel;
        }

        protected override async Task SetModel(SellerCompareModel viewModel)
        {
            await base.SetModel(viewModel);
        }

        public override async Task Update(SellerCompareModel viewModel)
        {
            await _lazy.MaterialCompareService.Value.UpdateDetail(viewModel, viewModel.MaterialCompareList);
        }

        public byte[] BuildImportTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("SellerCompareImportTemplate");

            for (int column = 0; column < ImportHeaders.Length; column++)
            {
                worksheet.Cells[1, column + 1].Value = ImportHeaders[column];
                worksheet.Cells[1, column + 1].Style.Font.Bold = true;
            }

            worksheet.Cells[2, 1].Value = "MAT-0001";
            worksheet.Cells[2, 2].Value = "12345678";
            worksheet.Cells[2, 3].Value = "CMP-0001";

            worksheet.Cells.AutoFitColumns();

            using var stream = new MemoryStream();
            package.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<SellerCompareImportResult> ImportFromCsv(IFormFile file, bool ignoreErrors = true)
        {
            if (file == null || file.Length == 0)
            {
                return new SellerCompareImportResult
                {
                    TotalCount = 0,
                    FailureCount = 1,
                    Errors = ["沒有可匯入的檔案。"]
                };
            }

            var lines = await ImportFileUtility.ReadImportLinesAsync(file);

            if (lines.Count < 2)
            {
                return new SellerCompareImportResult
                {
                    TotalCount = 0,
                    FailureCount = 1,
                    Errors = ["匯入檔案沒有可處理的資料。"]
                };
            }

            var headers = ImportFileUtility.ParseCsvLine(lines[0]).Select(x => x.Trim()).ToList();
            int materialNumberIndex = ImportFileUtility.GetHeaderIndex(headers, ["料號"]);
            int supplierTaxIdIndex = ImportFileUtility.GetHeaderIndex(headers, ["對照供應商統編"]);
            int buyerMaterialNumberIndex = ImportFileUtility.GetHeaderIndex(headers, ["對照料號"]);

            if (new[] { materialNumberIndex, supplierTaxIdIndex, buyerMaterialNumberIndex }.Any(x => x < 0))
            {
                return new SellerCompareImportResult
                {
                    TotalCount = 0,
                    FailureCount = 1,
                    Errors = ["匯入格式不正確，請先下載範本。"]
                };
            }

            var result = new SellerCompareImportResult();
            foreach (var line in lines.Skip(1))
            {
                var values = ImportFileUtility.ParseCsvLine(line);
                if (values.Count == 0)
                {
                    continue;
                }

                result.TotalCount += 1;

                string materialNumber = ImportFileUtility.GetValue(values, materialNumberIndex);
                string supplierTaxId = ImportFileUtility.GetValue(values, supplierTaxIdIndex);
                string buyerMaterialNumber = ImportFileUtility.GetValue(values, buyerMaterialNumberIndex);

                if (string.IsNullOrWhiteSpace(materialNumber) || string.IsNullOrWhiteSpace(supplierTaxId) || string.IsNullOrWhiteSpace(buyerMaterialNumber))
                {
                    AddImportError(result, materialNumber, "必填欄位缺漏", ignoreErrors);
                    if (!ignoreErrors) { break; }
                    continue;
                }

                var sellerMaterial = await FindSellerMaterialAsync(materialNumber);
                if (sellerMaterial == null)
                {
                    AddImportError(result, materialNumber, $"找不到可銷售料號「{materialNumber}」", ignoreErrors);
                    if (!ignoreErrors) { break; }
                    continue;
                }

                if (!IsCanSell(sellerMaterial))
                {
                    AddImportError(result, materialNumber, $"料號「{materialNumber}」不是可銷售料號", ignoreErrors);
                    if (!ignoreErrors) { break; }
                    continue;
                }

                var buyerMaterial = await FindBuyerMaterialAsync(supplierTaxId, buyerMaterialNumber);
                if (buyerMaterial == null)
                {
                    AddImportError(result, buyerMaterialNumber, $"找不到對照料號「{buyerMaterialNumber}」或供應商統編「{supplierTaxId}」", ignoreErrors);
                    if (!ignoreErrors) { break; }
                    continue;
                }

                if (!IsCanSell(buyerMaterial))
                {
                    AddImportError(result, buyerMaterialNumber, $"對照料號「{buyerMaterialNumber}」不是可銷售料號", ignoreErrors);
                    if (!ignoreErrors) { break; }
                    continue;
                }

                if (await ExistsCompareByBusinessKeyAsync(sellerMaterial.Id, supplierTaxId, buyerMaterialNumber))
                {
                    AddImportError(result, materialNumber, $"料號「{materialNumber}」與對照料號「{buyerMaterialNumber}」已存在對照，已跳過", ignoreErrors);
                    continue;
                }

                try
                {
                    await UpsertCompareAsync(sellerMaterial.Id, buyerMaterial.Id);
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

        private async Task UpsertCompareAsync(Guid sellerMaterialId, Guid buyerMaterialId)
        {
            var existing = await QueryFirstAsync<MaterialCompare>(@"
                SELECT TOP 1 *
                  FROM MaterialCompare
                 WHERE MaterialId = @MaterialId
                   AND BuyerMaterialId = @BuyerMaterialId", new { MaterialId = sellerMaterialId, BuyerMaterialId = buyerMaterialId });

            if (existing == null || existing.Id == Guid.Empty)
            {
                await InsertAsync(new MaterialCompare
                {
                    MaterialId = sellerMaterialId,
                    BuyerMaterialId = buyerMaterialId,
                    Status = EStatus.Enable
                });
                return;
            }

            existing.MaterialId = sellerMaterialId;
            existing.BuyerMaterialId = buyerMaterialId;
            existing.Status = EStatus.Enable;
            await UpdateAsync(existing);
        }

        private async Task<bool> ExistsCompareByBusinessKeyAsync(Guid sellerMaterialId, string supplierTaxId, string buyerMaterialNumber)
        {
            var existing = await QueryFirstAsync<MaterialCompare>(@"
                                SELECT TOP 1 MC.*
                                    FROM MaterialCompare MC
                                    LEFT JOIN Material BM ON MC.BuyerMaterialId = BM.Id
                                    LEFT JOIN Supplier S ON BM.SupplierId = S.Id
                                 WHERE MC.MaterialId = @MaterialId
                                     AND MC.[Status] = 1
                                     AND BM.[Status] != -1
                                     AND S.TaxID = @TaxID
                                     AND BM.MaterialNumber = @MaterialNumber", new
            {
                MaterialId = sellerMaterialId,
                TaxID = supplierTaxId,
                MaterialNumber = buyerMaterialNumber
            });

            return existing != null && existing.Id != Guid.Empty;
        }

        private async Task<Material?> FindSellerMaterialAsync(string materialNumber)
        {
            return await QueryFirstAsync<Material>(@"
                SELECT TOP 1 main.*
                  FROM Material main
                  LEFT JOIN Manager ON main.CreateUserId = Manager.Id
                 WHERE main.[Status] != -1
                   AND main.CanSell = 1
                   AND Manager.TaxID = @TaxID
                   AND main.MaterialNumber = @MaterialNumber", new { TaxID = _currentManager?.TaxID, MaterialNumber = materialNumber });
        }

        private async Task<Material?> FindBuyerMaterialAsync(string supplierTaxId, string materialNumber)
        {
            return await QueryFirstAsync<Material>(@"
                SELECT TOP 1 main.*
                  FROM Material main
                  LEFT JOIN Supplier ON main.SupplierId = Supplier.Id
                 WHERE main.[Status] != -1
                   AND main.CanSell = 1
                   AND Supplier.TaxID = @TaxID
                   AND main.MaterialNumber = @MaterialNumber", new { TaxID = supplierTaxId, MaterialNumber = materialNumber });
        }

        private static bool IsCanSell(Material? material)
        {
            if (material == null) { return false; }

            return material.CanSell == true;
        }

        private static void AddImportError(SellerCompareImportResult result, string materialNumber, string message, bool ignoreErrors)
        {
            result.Errors.Add($"{(string.IsNullOrWhiteSpace(materialNumber) ? "未填料號" : materialNumber)}：{message}");
        }

    }
}
