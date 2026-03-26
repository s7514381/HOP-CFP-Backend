using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models.Manager
{
    [Table("Role")]
    public class Role : IdModelBase
    {
        [Display(Name = "角色名稱")]
        public string? Name { get; set; }
    }
}