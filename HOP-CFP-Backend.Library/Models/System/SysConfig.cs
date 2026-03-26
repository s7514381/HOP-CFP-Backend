using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models.System
{
    /// <summary>
    /// 系統參數
    /// </summary>
    [Table("SysConfig")]
    public class SysConfig : IdModelBase
    {
        /// <summary>
        /// 對應Value
        /// </summary>
        [MaxLength(50, ErrorMessage = "長度最大為 {1} .")]
        [Required(ErrorMessage = "此為必填欄位")]
        public string? Value { get; set; }

        /// <summary>
        /// 說明
        /// </summary>
        [MaxLength(200)]
        public string? Note { get; set; }

        /// <summary>
        /// 備用1
        /// </summary>
        [MaxLength(200)]
        public string? TypeName { get; set; }
    }
}