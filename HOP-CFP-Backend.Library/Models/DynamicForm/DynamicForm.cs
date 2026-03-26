using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models
{
    [Table(nameof(DynamicForm))]
    public class DynamicForm : _DynamicForm { }

    public abstract class _DynamicForm : IdModelBase
    {
        [Display(Name = "表單代號")]
        public string Code { get; set; }

        [Display(Name = "標題")]
        public string Name { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }

        [Display(Name = "發佈時間")]
        public DateTime? StartTime { get; set; }

        [Display(Name = "截止時間")]
        public DateTime? EndTime { get; set; }

        public bool? InUse { get; set; }

        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DynamicForm>()
                 .HasIndex(b => b.Code);
        }
    }

    [Table(nameof(DynamicFormRecord))]
    public class DynamicFormRecord : _DynamicForm
    {
        [ForeignKey(nameof(DynamicForm))]
        public Guid? DynamicFormId { get; set; }

        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DynamicFormRecord>()
                 .HasIndex(b => b.DynamicFormId);
        }
    }

}
