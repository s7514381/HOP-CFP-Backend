using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.Library.Models
{
    /// <summary>
    /// 枚舉資料紀錄的狀態
    /// </summary>
    public enum EStatus : short
    {
        /// <summary>
        /// 刪除
        /// </summary>
        [Display(Name = "刪除")]
        Deleted = -1,

        /// <summary>
        /// 停用
        /// </summary>
        [Display(Name = "停用")]
        Disable = 0,

        /// <summary>
        /// 啟用
        /// </summary>
        [Display(Name = "啟用")]
        Enable = 1,
    }
}
