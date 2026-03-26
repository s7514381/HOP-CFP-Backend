using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models
{
    /// <summary>
    /// 資料表: 活動清單
    /// </summary>
    [Table(nameof(MaterialGroup))]
    public class MaterialGroup : IdModelBase
    {
        [Display(Name = "群組代號")]
        public string? Code { get; set; }

        [Display(Name = "群組名稱")]
        public string? Name { get; set; }
    }
}