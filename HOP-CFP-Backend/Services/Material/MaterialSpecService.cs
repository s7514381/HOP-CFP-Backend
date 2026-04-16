using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class MaterialSpecService : _ModelService<MaterialSpec, MaterialSpecModel>
    {
        public MaterialSpecService(BaseServiceArgument argument) : base(argument) { }

        protected override async Task SetModel(MaterialSpecModel viewModel)
        {
            await base.SetModel(viewModel);
        }
    }
}
