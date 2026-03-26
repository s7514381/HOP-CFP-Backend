using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

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

    }
}
