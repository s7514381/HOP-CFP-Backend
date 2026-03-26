using System.Collections.Generic;
using System.Threading.Tasks;
using HOP_CFP_Backend.Services;

namespace HOP_CFP_Backend.Utility
{
    public interface IMailSender
    {
        Task SetConfigAsync(SysConfigService sysConfigService);
        Task SentAsync(string toEmail, string subject, string message);
        Task SentAsync(string toEmail, IEnumerable<string> ccEmail, string subject, string message);
    }
}
