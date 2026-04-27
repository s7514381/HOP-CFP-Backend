using HOP_CFP_Backend.Library.Models.Manager;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class RoleModel : Role
    {
        public List<Guid> SelectedAdminFunctionIds { get; set; } = new List<Guid>();
        public List<Guid> SelectedAdminMenuIds { get; set; } = new List<Guid>();
    }

    public class RoleSearchViewModel : BaseSearchViewModel
    {
    }

    public class RoleListViewModel : PagingViewModel<RoleListDataModel>
    {
    }

    public class RoleListDataModel : BaseListDataModel
    {
        [Display(Name = "角色名稱")]
        public string? Name { get; set; }

        [Display(Name = "功能名稱")]
        public string? AdminMenuName { get; set; }
    }
}
