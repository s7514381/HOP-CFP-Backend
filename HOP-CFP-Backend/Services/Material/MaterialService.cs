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
                SELECT {_keyField} AS Value, MaterialNumber + ' - ' + ProductName AS Text
                FROM {_tableName}
                WHERE Status != -1
                  AND (MaterialNumber LIKE @Keyword OR ProductName LIKE @Keyword ) ", new { Keyword = $"%{keyword}%" });
            return result.Select(x => new SelectListItem(x.Text, x.Value.ToString()));
        }

    }
}
