using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models.System
{
    [Table("Log_BackendPageRequest")]
    public class Log_BackendPageRequest : IdModelBase
    {
        [ForeignKey(nameof(Manager))]
        public Guid? ManagerId { get; set; }

        public string? Path { get; set; }

        public string? QueryString { get; set; }

        public string? Body { get; set; }

        public string? IPAddress { get; set; }

        public long LoadTime { get; set; }

        public DateTime RequestTime { get; set; }
    }
}