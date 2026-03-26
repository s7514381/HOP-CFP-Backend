using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.Library.Models
{
    public enum EFileType
    {
        [Display(Name = "圖片")]
        Image,

        [Display(Name = "檔案")]
        File,
    }
}
