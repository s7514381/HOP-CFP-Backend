using HOP_CFP_Backend.Library.Models;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class MaterialNotifyModel : MaterialNotify
    {
    }

    public class MaterialNotifySearchViewModel : BaseSearchViewModel
    {
        public DateTime? UpdateDateFrom { get; set; }
        public DateTime? UpdateDateTo { get; set; }
        public string? MaterialGroupName { get; set; }
        public string? ProductModel { get; set; }
        public string? SupplierName { get; set; }
    }

    public class MaterialNotifyListViewModel : PagingViewModel<MaterialNotifyListDataModel>
    {
    }

    public class MaterialNotifyListDataModel : BaseListDataModel
    {
        [Display(Name = "群組")]
        public string? MaterialGroupName { get; set; }

        [Display(Name = "供應商")]
        public string? SupplierName { get; set; }

        [Display(Name = "料號")]
        public string? MaterialNumber { get; set; }

        [Display(Name = "產品型號")]
        public string? ProductModel { get; set; }

        [Display(Name = "產品名稱")]
        public string? ProductName { get; set; }
    }
}
