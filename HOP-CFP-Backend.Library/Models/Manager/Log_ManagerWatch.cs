using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models.Manager
{
    /// <summary>
    /// 管理者使用觀察模式紀錄
    /// </summary>
    [Table("Log_ManagerWatch")]
    public class Log_ManagerWatch : IdModelBase
    {
        /// <summary>
        /// 管理者 Id
        /// </summary>
        [ForeignKey(nameof(Manager))]
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// 觀察對象 Id
        /// </summary>
        public Guid? WatchingId { get; set; }
    }
}
