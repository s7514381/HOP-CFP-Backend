using System;
using System.ComponentModel.DataAnnotations;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models
{
    /// <summary>
    /// 資料表: 活動清單
    /// </summary>
    [Table(nameof(Supplier))]
    public class Supplier : IdModelBase
    {
        [Display(Name = "名稱")]
        public string? Name { get; set; }

        [Display(Name = "統編")]
        public string? TaxID { get; set; }

        [Display(Name = "聯絡窗口")]
        public string? ContactName { get; set; }

        [Display(Name = "聯絡電話")]
        public string? ContactPhone { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }
    }
}