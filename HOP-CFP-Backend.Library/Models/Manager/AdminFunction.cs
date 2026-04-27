using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models.Manager
{
    [Table("AdminFunction")]
    public class AdminFunction : IdModelBase
    {
        [ForeignKey(nameof(AdminFunction))]
        public Guid? ParentId { get; set; }

        public string? Title { get; set; }

        public string? Controller { get; set; }

        public string? Action { get; set; }

        public string? Parameter { get; set; }

        public int? ActionFunctionSN { get; set; }

    }
}
