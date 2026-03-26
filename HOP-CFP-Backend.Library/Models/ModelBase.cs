using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using HOP_CFP_Backend.Library.Attributes;
using Microsoft.EntityFrameworkCore;

namespace HOP_CFP_Backend.Library.Models
{
    public class ModelBase
    {
        [NoHistory]
        public virtual DateTime? CreateDate { get; set; } = DateTime.Now;

        [NoHistory]
        public virtual Guid? CreateUserId { get; set; }

        [Display(Name = "更新時間")]
        public virtual DateTime? UpdateDate { get; set; }

        [Display(Name = "修改人員")]
        public virtual Guid? UpdateUserId { get; set; }

        [Display(Name = "狀態")]
        public virtual EStatus? Status { get; set; } = EStatus.Enable;

        public int? Sequence { get; set; }
    }

    public class IdModelBase : ModelBase
    {
        [ExplicitKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        public virtual void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}