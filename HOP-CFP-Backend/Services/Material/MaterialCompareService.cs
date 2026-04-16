using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class MaterialCompareService : _ModelService<MaterialCompare, MaterialCompareModel>
    {
        public MaterialCompareService(BaseServiceArgument argument) : base(argument) { }

        protected override async Task SetModel(MaterialCompareModel viewModel)
        {
            await base.SetModel(viewModel);

            if (viewModel.SupplierId.HasValue)
            {
                var supplier = await _lazy.SupplierService.Value.GetFieldData(viewModel.SupplierId.Value, "Name, TaxID");
                if (supplier != null) 
                {
                    viewModel.SupplierName = supplier.Name;
                    viewModel.SupplierTaxID = supplier.TaxID;
                }
            }
        }
    }
}
