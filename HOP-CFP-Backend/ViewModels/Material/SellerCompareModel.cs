using HOP_CFP_Backend.Library.Models;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class SellerCompareModel : Material
    {
        public List<MaterialCompareModel> MaterialCompareList { get; set; } = new();
    }

    public class SellerCompareSearchModel : BaseSearchViewModel
    {
        [Display(Name = "供應商")]
        public string? SupplierName { get; set; }

        [Display(Name = "料號")]
        public string? MaterialNumber { get; set; }
    }

    public class SellerCompareListViewModel : PagingViewModel<SellerCompareListDataModel>
    {
    }

    public class SellerCompareListDataModel : BaseListDataModel
    {
        [Display(Name = "料號")]
        public string? MaterialNumber { get; set; }

        [Display(Name = "供應商")]
        public string? SupplierName { get; set; }

        [Display(Name = "產品型號")]
        public string? ProductModel { get; set; }

        [Display(Name = "產品名稱")]
        public string? ProductName { get; set; }

        [Display(Name = "買方")]
        public string? BuyerName { get; set; }
    }

    public class SellerCompareImportResult
    {
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
