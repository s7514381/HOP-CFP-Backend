using HOP_CFP_Backend.Library.Models;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class BuyerCompareModel : Material
    {
        [Display(Name = "供應商名稱")]
        public string? SupplierName { get; set; }

        public List<MaterialSpecModel> MaterialSpecList { get; set; } = new();

        public List<Guid> DeleteMaterialCompareIdList { get; set; } = new();
    }

    public class BuyerCompareSearchModel : BaseSearchViewModel
    {
        [Display(Name = "供應商")]
        public string? SupplierName { get; set; }

        [Display(Name = "料號")]
        public string? MaterialNumber { get; set; }
    }

    public class BuyerCompareListViewModel : PagingViewModel<BuyerCompareListDataModel>
    {
    }

    public class BuyerCompareListDataModel : BaseListDataModel
    {
        [Display(Name = "料號")]
        public string? MaterialNumber { get; set; }

        [Display(Name = "供應商")]
        public string? SupplierName { get; set; }

        [Display(Name = "產品型號")]
        public string? ProductModel { get; set; }

        [Display(Name = "產品名稱")]
        public string? ProductName { get; set; }

        [Display(Name = "規格碼筆數")]
        public int? SpecCount { get; set; }

        [Display(Name = "未對照筆數")]
        public int? NotCompareCount { get; set; }
    }

    public class BuyerMaterialCompare : MaterialCompare
    {
        [Display(Name = "產品型號")]
        public string? ProductModel { get; set; }

        [Display(Name = "產品名稱")]
        public string? ProductName { get; set; }

        [Display(Name = "賣方料號")]
        public string? SellerMaterialNumber { get; set; }

        [Display(Name = "賣方產品名稱")]
        public string? SellerProductName { get; set; }
    }

}
