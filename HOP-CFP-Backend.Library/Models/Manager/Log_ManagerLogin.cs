using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models.Manager
{
    /// <summary>
    /// 使用者登入紀錄
    /// </summary>
    [Table("Log_ManagerLogin")]
    public class Log_ManagerLogin : IdModelBase
    {
        /// <summary>
        /// 使用者 Id
        /// </summary>
        [ForeignKey(nameof(Manager))]
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// 登入行為種類
        /// </summary>
        public ELogin ActionType { get; set; }

        /// <summary>
        /// IP 位址
        /// </summary>
        public string? IPAddress { get; set; }

        /// <summary>
        /// 是否登出
        /// </summary>
        public bool IsLogout { get; set; }

    }
}