using Dapper.Contrib.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models
{
    [Table(nameof(DynamicFormFieldOption))]
    public class DynamicFormFieldOption : _DynamicFormFieldOption { }

    public abstract class _DynamicFormFieldOption : IdModelBase
    {
        [ForeignKey(nameof(DynamicFormField))]
        public Guid? DynamicFormFieldId { get; set; }

        [Display(Name = "選項內容")]
        public string Name { get; set; }

        [Display(Name = "是否可輸入內容")]
        public bool CanInput { get; set; }

        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DynamicFormFieldOption>()
                 .HasIndex(b => b.DynamicFormFieldId);
        }
    }

    [Table(nameof(DynamicFormFieldOptionRecord))]
    public class DynamicFormFieldOptionRecord : _DynamicFormFieldOption 
    {
        [ForeignKey(nameof(DynamicFormFieldOption))]
        public Guid? DynamicFormFieldOptionId { get; set; }

        [ForeignKey(nameof(DynamicFormFieldRecord))]
        public Guid? DynamicFormFieldRecordId { get; set; }

        public bool? Selected { get; set; }

        //可以輸入內容的話就會有Value，例: 其他
        public string Value { get; set; } 

        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DynamicFormFieldOptionRecord>()
                 .HasIndex(b => b.DynamicFormFieldOptionId);
        }
    }

}
