using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models.Manager
{
    [Table("AdminMenuByRole")]
    public class AdminMenuByRole : IdModelBase
    {
        [ForeignKey(nameof(AdminMenu))]
        public Guid AdminMenuId { get; set; }

        [ForeignKey(nameof(Role))]
        public Guid RoleId { get; set; }

        public int ActionFunctionAssembly { get; set; }
    }
}
