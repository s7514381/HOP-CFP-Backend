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

            if (viewModel.BuyerMaterialId.HasValue)
            {
                (viewModel.BuyerMaterialNumber, viewModel.SupplierName, viewModel.SupplierTaxID, viewModel.BuyerSpecNumber) = 
                    await QueryFirstAsync<(string, string, string, string)>(
                        $@"select M.MaterialNumber, Manager.[Name] as SupplierName, Manager.TaxID as SupplierTaxID, MS.SpecNumber as BuyerSpecNumber  
                             from Material M
                             left join Manager on M.UpdateUserId = Manager.Id
                             left join MaterialCompare MC on MC.BuyerMaterialId = M.Id and MC.[Status] = 1
                             left join MaterialSpec MS on MS.MaterialCompareId = MC.Id
                            where M.Id = @Id", new { Id = viewModel.BuyerMaterialId.Value });
            }
        }
    }
}
