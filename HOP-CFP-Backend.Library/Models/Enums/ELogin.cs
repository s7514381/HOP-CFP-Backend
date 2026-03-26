using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.Library.Models
{
    /// <summary>
    /// 枚舉登入的行為種類
    /// </summary>
    public enum ELogin : short
    {
        /// <summary>
        /// 登入
        /// </summary>
        [Display(Name = "登入")]
        Login = 1,

        /// <summary>
        /// 登出
        /// </summary>
        [Display(Name = "登出")]
        Logout = 2
    }
}
