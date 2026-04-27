using HOP_CFP_Backend.Library.Models.Manager;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class AdminMenuModel : AdminMenu
    {
        //public AdminFunctionModel? AdminFunction { get; set; }
        public List<AdminMenuModel> ChildList { get; set; } = new List<AdminMenuModel>();
    }

    public class AdminMenuSearchViewModel : BaseSearchViewModel
    {
    }

    public class AdminMenuListViewModel : PagingViewModel<AdminMenuListDataModel>
    {
    }

    public class AdminMenuListDataModel : BaseListDataModel
    {
        [Display(Name = "¥\¯à¦WºÙ")]
        public string? Title { get; set; }
    }

    public class FullAdminMenuModel : AdminMenu
    {
        public AdminFunctionModel? AdminFunction { get; set; }
        public List<FullAdminMenuModel> ChildList { get; set; } = new List<FullAdminMenuModel>();
    }
}
