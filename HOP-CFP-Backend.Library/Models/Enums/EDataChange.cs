using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.Library.Models
{
    /// <summary>
    /// 枚舉資料異動的行為種類
    /// </summary>
    public enum EDataChange : short
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Display(Name = "新增")]
        Create,

        /// <summary>
        /// 編輯
        /// </summary>
        [Display(Name = "編輯")]
        Edit,

        /// <summary>
        /// 刪除
        /// </summary>
        [Display(Name = "刪除")]
        Delete
    }
}
