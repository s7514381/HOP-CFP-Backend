using Dapper.Contrib.Extensions;
using SunshineHeros.Library.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using HOP_CFP_Backend.Library.Models;
using Microsoft.EntityFrameworkCore;

namespace HOP_CFP_Backend.Library.Models
{
    [Table(nameof(ManyToMany))]
    public class ManyToMany : IdModelBase
    {
        public string? SourceTable { get; set; }

        public Guid? SourceId { get; set; }

        [StringLength(100)]
        public string? TargetTable { get; set; }

        public Guid? TargetId { get; set; }

        public int? RelationType { get; set; }

        public string? Params { get; set; }

        public override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<ManyToMany>()
                 .HasIndex(b => new { b.SourceId, b.TargetTable, b.RelationType });

            modelBuilder.Entity<ManyToMany>()
                 .HasIndex(b => b.TargetId);
        }
    }

}
