using HOP_CFP_Backend.Library.Models;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class SupplierModel : Supplier
    {
    }

    public class SupplierSearchViewModel : BaseSearchViewModel
    {
        [Display(Name = "名稱")]
        public string? Name { get; set; }

        [Display(Name = "統編")]
        public string? TaxID { get; set; }
    }

    public class SupplierListViewModel : PagingViewModel<SupplierListDataModel>
    {
    }

    public class SupplierListDataModel : BaseListDataModel
    {
        [Display(Name = "名稱")]
        public string? Name { get; set; }

        [Display(Name = "統編")]
        public string? TaxID { get; set; }

        [Display(Name = "聯絡窗口")]
        public string? ContactName { get; set; }

        [Display(Name = "聯絡電話")]
        public string? ContactPhone { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }
    }

}
