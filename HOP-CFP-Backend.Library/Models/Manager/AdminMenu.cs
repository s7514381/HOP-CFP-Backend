using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models.Manager
{
    [Table("AdminMenu")]
    public class AdminMenu : IdModelBase
    {
        public Guid? ParentId { get; set; }

        public string? Title { get; set; }

        [ForeignKey(nameof(AdminFunction))]
        public Guid? AdminFunctionId { get; set; }

        public string? IconClass { get; set; }

        public string? Url { get; set; }
    }
}
