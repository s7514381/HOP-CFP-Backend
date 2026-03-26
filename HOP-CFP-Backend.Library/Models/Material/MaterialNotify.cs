using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models
{
    /// <summary>
    /// 資料表: 活動清單
    /// </summary>
    [Table(nameof(MaterialNotify))]
    public class MaterialNotify : IdModelBase
    {
        [ForeignKey(nameof(Material))]
        public Guid? MaterialId { get; set; }

        [Display(Name = "是否發送")]
        public bool IsSend { get; set; }

        [Display(Name = "是否更新資料")]
        public bool IsUpdate { get; set; }
    }
}