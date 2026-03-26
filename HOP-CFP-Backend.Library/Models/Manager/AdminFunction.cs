using Dapper.Contrib.Extensions;
using System;

namespace HOP_CFP_Backend.Library.Models.Manager
{
    [Table("AdminFunction")]
    public class AdminFunction : IdModelBase
    {
        public Guid? ParentId { get; set; }

        public string? Title { get; set; }

        public string? Controller { get; set; }

        public string? Action { get; set; }

        public string? Parameter { get; set; }

        public short? ActionFunctionSN { get; set; }

    }
}
