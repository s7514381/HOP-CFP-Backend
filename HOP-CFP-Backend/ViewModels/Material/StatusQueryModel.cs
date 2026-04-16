using HOP_CFP_Backend.Library.Models;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class StatusQueryModel : MaterialNotify
    {
    }

    public class StatusQuerySearchViewModel : BaseSearchViewModel
    {
        public bool? IsSend { get; set; }
        public bool? IsUpdate { get; set; }
        public DateTime? UpdateDateFrom { get; set; }
        public DateTime? UpdateDateTo { get; set; }
    }

    public class StatusQueryListViewModel : PagingViewModel<StatusQueryListDataModel>
    {
    }

    public class StatusQueryListDataModel : BaseListDataModel
    {
        [Display(Name = "是否發送")]
        public bool? IsSend { get; set; }

        [Display(Name = "是否更新資料")]
        public bool? IsUpdate { get; set; }

        [Display(Name = "寄送時間")]
        public string? strCreateDate { get; set; }

        [Display(Name = "更新時間")]
        public string? strUpdateDate { get; set; }

        [Display(Name = "供應商")]
        public string? SupplierName { get; set; }

        [Display(Name = "料號")]
        public string? MaterialNumber { get; set; }

        [Display(Name = "產品型號")]
        public string? ProductModel { get; set; }

        [Display(Name = "產品名稱")]
        public string? ProductName { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
