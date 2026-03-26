using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HOP_CFP_Backend.Library.Models
{
    public enum UpdateAwardReceiveStatusResult
    {
        [Display(Name = "成功更新")]
        Success = 1,

        [Display(Name = "該領獎者已領過獎")]
        AlreadyReceived = 2,

        [Display(Name = "活動沒有該位領獎者")]
        WinnerNotFound = 3,

        [Display(Name = "保持未領獎狀態")]
        MaintainNotReceived = 4
    }

    public enum AddWinnerResult
    {
        [Display(Name = "成功增加")]
        Success = 1,

        [Display(Name = "獎項已抽滿")]
        AwardIsFull = 2,

        [Display(Name = "得獎者已存在")]
        WinnerExists = 3,

        [Display(Name = "未知錯誤")]
        UnknownError = 4
    }

    public enum DeleteWinnerResult
    {
        [Display(Name = "成功刪除")]
        Success = 1,

        [Display(Name = "得獎者不存在")]
        WinnerNotFound = 2,

        [Display(Name = "未知錯誤")]
        UnknownError = 3
    }
}
