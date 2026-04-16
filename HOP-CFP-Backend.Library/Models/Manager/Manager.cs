using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HOP_CFP_Backend.Library.Attributes;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;
using Microsoft.EntityFrameworkCore;

namespace HOP_CFP_Backend.Library.Models.Manager
{
    /// <summary>
    /// 使用者
    /// </summary>
    [Table("Manager")]
    public class Manager : IdModelBase
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Display(Name = "帳號")]
        public string? Account { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Display(Name = "密碼")]
        public byte[]? PasswordHash { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        [Display(Name = "使用者名稱")]
        public string? Name { get; set; }

        [Display(Name = "統編")]
        public string? TaxID { get; set; }

        /// <summary>
        /// 是否已通過電子郵件驗證
        /// </summary>
        public bool EmailConfirm { get; set; }

        /// <summary>
        /// 狀態被改為停用的時間
        /// </summary>
        [Display(Name = "停權時間")]
        public DateTime? PauseDate { get; set; }

        /// <summary>
        /// 最後一次更改密碼的時間
        /// </summary>
        public DateTime LastPasswordChangeDate { get; set; }

        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manager>()
                 .HasIndex(b => b.TaxID);
        }
    }
}