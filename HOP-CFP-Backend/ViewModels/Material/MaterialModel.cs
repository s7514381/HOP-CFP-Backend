using HOP_CFP_Backend.Library.Models;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class MaterialModel : Material
    {
    }

    public class MaterialSearchViewModel : BaseSearchViewModel
    {
        [Display(Name = "供應商")]
        public string? SupplierName { get; set; }

        [Display(Name = "料號")]
        public string? MaterialNumber { get; set; }
    }

    public class MaterialListViewModel : PagingViewModel<MaterialListDataModel>
    {
    }

    public class MaterialListDataModel : BaseListDataModel
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

        [Display(Name = "是否可銷售")]
        public string? strCanSell { get; set; }
        public bool? CanSell { get; set; }
    }

    public class MaterialImportResult
    {
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
