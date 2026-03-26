using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models.Manager
{
    /// <summary>
    /// 使用者登入失敗紀錄
    /// </summary>
    [Table("Log_ManagerLoginFail")]
    public class Log_ManagerLoginFail : IdModelBase
    {
        /// <summary>
        /// 嘗試帳號
        /// </summary>
        public string? Account { get; set; }

        /// <summary>
        /// IP 位址
        /// </summary>
        public string? IPAddress { get; set; }
    }
}