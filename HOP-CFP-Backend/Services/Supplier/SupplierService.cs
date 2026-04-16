using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HOP_CFP_Backend.Services
{
    public class SupplierService : _StandardService<Supplier, SupplierModel, SupplierSearchViewModel, SupplierListViewModel, SupplierListDataModel>
    {
        public SupplierService(BaseServiceArgument argument) : base(argument) { }

        public override (string, string, string) GetListQueryString(SupplierSearchViewModel searchModel)
        {
            (string mainSql, string baseWhere, string sqlWhere) = base.GetListQueryString(searchModel);

            if (!string.IsNullOrEmpty(searchModel.Name))
                sqlWhere += $" and main.Name LIKE '%' + @Name + '%' ";

            if (!string.IsNullOrEmpty(searchModel.TaxID))
                sqlWhere += $" and main.TaxID LIKE '%' + @TaxID + '%' ";

            return (mainSql, baseWhere, sqlWhere);
        }

        public override async Task<IEnumerable<SelectListItem>> GetSelectListItems(string? keyword)
        {
            var result = await QueryAsync<(Guid Value, string Text)>($@"
                SELECT top 100 {_keyField} AS Value, TaxID + ' - ' + Name AS Text
                FROM {_tableName}
                WHERE Status != -1
                  AND {_tableName}.CreateUserId = @CreateUserId
                  AND (TaxID LIKE @Keyword OR Name LIKE @Keyword ) ", new { Keyword = $"%{keyword}%", CreateUserId = _currentManager?.ManagerId });
            return result.Select(x => new SelectListItem(x.Text, x.Value.ToString()));
        }

    }
}
