using HOP_CFP_Backend.Library.Models;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class NotifyStatusReportModel : MaterialNotify
    {
    }

    public class NotifyStatusReportSearchModel : BaseSearchViewModel
    {
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public string? SupplierName { get; set; }

    }

    public class NotifyStatusReportListViewModel : PagingViewModel<NotifyStatusReportListDataModel>
    {
    }

    public class NotifyStatusReportListDataModel : BaseListDataModel
    {
        [Display(Name = "日期")]
        public string? strDate { get; set; }

        [Display(Name = "供應商")]
        public string? SupplierName { get; set; }

        [Display(Name = "收到筆數")]
        public string? SentCount { get; set; }

        [Display(Name = "更新筆數")]
        public string? UpdateCount { get; set; }

        public DateTime? Date { get; set; }
    }
}
