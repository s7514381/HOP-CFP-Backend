using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models
{
    /// <summary>
    /// 資料表: 活動清單
    /// </summary>
    [Table(nameof(Material))]
    public class Material : IdModelBase
    {
        [ForeignKey(nameof(Supplier))]
        public Guid? SupplierId { get; set; }

        [Display(Name = "料號")]
        public string? MaterialNumber { get; set; }

        [Display(Name = "產品型號")]
        public string? ProductModel { get; set; }

        [Display(Name = "產品名稱")]
        public string? ProductName { get; set; }

        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Material>()
                 .HasIndex(b => b.MaterialNumber);

            modelBuilder.Entity<Material>()
                 .HasIndex(b => b.SupplierId);
        }
    }
}