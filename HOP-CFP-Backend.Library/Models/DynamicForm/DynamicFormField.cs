using Dapper.Contrib.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models
{
    [Table(nameof(DynamicFormField))]
    public class DynamicFormField : _DynamicFormField { }

    public abstract class _DynamicFormField : IdModelBase
    {
        [ForeignKey(nameof(DynamicForm))]
        public Guid? DynamicFormId { get; set; }

        [Display(Name = "標題")]
        public string Name { get; set; }

        [Display(Name = "類型")]
        public EDynamicFormFieldType Type { get; set; }

        [Display(Name = "是否必填")]
        public bool IsRequired { get; set; }

        [Display(Name = "是否可查詢")]
        public bool CanSearch { get; set; }

        [Display(Name = "字數限制")]
        public short WordLimit { get; set; } = 100;

        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DynamicFormField>()
                 .HasIndex(b => b.DynamicFormId);
        }
    }

    [Table(nameof(DynamicFormFieldRecord))]
    public class DynamicFormFieldRecord : _DynamicFormField 
    {
        [ForeignKey(nameof(DynamicFormField))]
        public Guid? DynamicFormFieldId { get; set; }

        [ForeignKey(nameof(DynamicFormRecord))]
        public Guid? DynamicFormRecordId { get; set; }

        public Guid? SelectedOptionId { get; set; } 

        public string Value { get; set; }

        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DynamicFormFieldRecord>()
                 .HasIndex(b => b.DynamicFormFieldId);
        }
    }

}
