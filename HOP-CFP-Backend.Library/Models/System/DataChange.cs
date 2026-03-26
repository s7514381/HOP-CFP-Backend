using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.Library.Models.System
{
    /// <summary>
    /// 資料異動紀錄
    /// </summary>
    [Table("DataChange")]
    public class DataChange : IdModelBase
    {
        /// <summary>
        /// 異動資料所屬的 Model 類別 (以 ViewModel 為主)
        /// </summary>
        [StringLength(50)]
        public string? ModelClass { get; set; }

        /// <summary>
        /// 異動資料的主鍵
        /// </summary>
        [StringLength(256)]
        public string? PrimaryKey { get; set; }

        /// <summary>
        /// 行為種類
        /// </summary>
        public EDataChange Action { get; set; }

        /// <summary>
        /// 異動細節 (JSON 格式)
        /// </summary>
        public string? DifferenceJSON { get; set; }
    }
}
