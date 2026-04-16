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
    [Table(nameof(MaterialSpec))]
    public class MaterialSpec : IdModelBase
    {
        [ForeignKey(nameof(MaterialCompare))]
        public Guid? MaterialCompareId { get; set; }

        [ForeignKey(nameof(Material))]
        public Guid? MaterialId { get; set; }

        [Display(Name = "規格編號")]
        public string? SpecNumber { get; set; }

        [Display(Name = "名稱")]
        public string? Name { get; set; }

        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MaterialSpec>()
                 .HasIndex(b => b.MaterialCompareId);

            modelBuilder.Entity<MaterialSpec>()
                 .HasIndex(b => b.MaterialId);
        }
    }
}