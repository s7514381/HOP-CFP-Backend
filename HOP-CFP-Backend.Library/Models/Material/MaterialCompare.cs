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
    [Table(nameof(MaterialCompare))]
    public class MaterialCompare : IdModelBase
    {
        [ForeignKey(nameof(Material))]
        public Guid? MaterialId { get; set; }

        [ForeignKey(nameof(Supplier))]
        public Guid? SupplierId { get; set; }

        [Display(Name = "供應商料號")]
        public string? MaterialNumber { get; set; }

        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MaterialCompare>()
                 .HasIndex(b => b.MaterialId);

            modelBuilder.Entity<MaterialCompare>()
                 .HasIndex(b => b.SupplierId);

            modelBuilder.Entity<MaterialCompare>()
                .HasIndex(b => b.MaterialNumber);
        }
    }
}