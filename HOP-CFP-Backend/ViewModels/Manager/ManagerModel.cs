using HOP_CFP_Backend.Library.Models.Manager;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class ManagerModel : Manager
    {
        public Guid? RoleId { get; set; }
    }

    public class ManagerLoginViewModel
    {
        [Required(ErrorMessage = "帳號為必填")]
        [Display(Name = "帳號")]
        public string Account { get; set; }

        [Required(ErrorMessage = "密碼為必填")]
        [Display(Name = "密碼")]
        public string Password { get; set; }
    }

    public class ManagerRegisterViewModel
    {
        [Required(ErrorMessage = "帳號為必填")]
        [Display(Name = "帳號")]
        public string? Account { get; set; }

        [Required(ErrorMessage = "Email為必填")]
        [EmailAddress(ErrorMessage = "Email格式不正確")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "使用者名稱為必填")]
        [Display(Name = "使用者名稱")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "統編為必填")]
        [Display(Name = "統編")]
        public string? TaxID { get; set; }

        [Required(ErrorMessage = "密碼為必填")]
        [Display(Name = "密碼")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "確認密碼為必填")]
        [Compare(nameof(Password), ErrorMessage = "密碼與確認密碼不一致")]
        [Display(Name = "確認密碼")]
        public string? ConfirmPassword { get; set; }
    }

    public class ManagerSearchViewModel : BaseSearchViewModel
    {
        [Display(Name = "帳號")]
        public string? Account { get; set; }
    }

    public class ManagerListViewModel : PagingViewModel<ManagerListDataModel>
    {
    }

    public class ManagerListDataModel : BaseListDataModel
    {
        [Display(Name = "使用者名稱")]
        public string? Name { get; set; }

        [Display(Name = "帳號")]
        public string? Account { get; set; }

        [Display(Name = "統編")]
        public string? TaxID { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

    }

    /// <summary>
    /// 快取於 IMemoryCache 的登入資訊，以 Token (Guid) 為 Key
    /// </summary>
    public class ManagerSessionModel : Manager
    {
        public List<FullAdminMenuModel> AdminMenus { get; set; } = new();
    }

    public class LoginInfoModel
    {
        public Guid? Token { get; set; }
        public List<FullAdminMenuModel> AdminMenus { get; set; } = new();
    }
}
