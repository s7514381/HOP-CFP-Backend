using HOP_CFP_Backend.Argument;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Services;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartExpoIoT.ViewModels.Api;

namespace HOP_CFP_Backend.Controllers
{
    public class SellerCompareController : StandardController<Material, SellerCompareModel, SellerCompareSearchModel, SellerCompareListViewModel, SellerCompareListDataModel>
    {
        private SellerCompareService _sellerCompareService => _lazy.SellerCompareService.Value;

        public SellerCompareController(BaseControllerArgument argument) : base(argument, argument.LazyServiceArgument.SellerCompareService.Value) { }

    }
}
