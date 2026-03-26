using HOP_CFP_Backend.Services;

namespace HOP_CFP_Backend.Utility
{
    public class MailSender : IMailSender
    {
        public const string _smtpServerType = "smtp.ServerType";
        public const string _smtpServer = "smtp.Server";
        public const string _smtpPort = "smtp.Port";
        public const string _smtpAccount = "smtp.Account";
        public const string _smtpPwName = "smtp.Password";
        public const string _smtpSender = "smtp.Sender";

        public MailSenderConfig _globalConfig;
        protected IConfiguration _configuration;

        public MailSender(MailSenderConfig globalConfig, IConfiguration configuration)
        {
            _globalConfig = globalConfig;
            _configuration = configuration;
        }

        public async Task SetConfigAsync(SysConfigService sysConfigService)
        {
            var portAsync = sysConfigService.GetAsync(_smtpPort);
            var serverTypeAsync = sysConfigService.GetAsync(_smtpServerType);
            var serverAsync = sysConfigService.GetAsync(_smtpServer);
            var accountAsync = sysConfigService.GetAsync(_smtpAccount);
            var pwNameAsync = sysConfigService.GetAsync(_smtpPwName);
            var sender = sysConfigService.GetAsync(_smtpSender);

            _globalConfig.SMTP_Port = Convert.ToInt32((await portAsync).Value);
            _globalConfig.SMTP_ServerType = (await serverTypeAsync).Value;
            _globalConfig.SMTP_Server = (await serverAsync).Value;
            _globalConfig.SMTP_Account = (await accountAsync).Value;
            _globalConfig.SMTP_Password = (await pwNameAsync).Value;
            _globalConfig.SMTP_Sender = (await sender).Value;
        }

        public Task SentAsync(string toEmail, string subject, string message)
        {
            return SentAsync(toEmail, null, subject, message);
        }

        public async Task SentAsync(string toEmail, IEnumerable<string> ccEmail, string subject, string message)
        {
            //MimeMessage Message = new MimeMessage();
            //Message.From.Add(new MailboxAddress(_configuration.GetValue<string>("SiteSettings:ApplicationName"), _globalConfig.SMTP_Sender));
            //Message.To.Add(MailboxAddress.Parse(toEmail));
            //
            //if (ccEmail != null)
            //{
            //    foreach (var email in ccEmail)
            //    {
            //        if (!string.IsNullOrEmpty(email))
            //        {
            //            Message.Cc.Add(MailboxAddress.Parse(email));
            //        }
            //    }
            //}
            //
            //Message.Subject = subject;
            //Message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            //{
            //    Text = message
            //};
            //
            //using var client = new SmtpClient();
            //client.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            //client.Timeout = 3 * 1000;
            //
            //if (_globalConfig.SMTP_ServerType == "Google")
            //{
            //    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            //    client.AuthenticationMechanisms.Remove("XOAUTH2");
            //    await client.ConnectAsync(_globalConfig.SMTP_Server, _globalConfig.SMTP_Port, false);
            //    await client.AuthenticateAsync(_globalConfig.SMTP_Account, _globalConfig.SMTP_Password);
            //    await client.SendAsync(Message);
            //    client.Disconnect(true);
            //}
            //else if (_globalConfig.SMTP_ServerType == "Other")
            //{
            //    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            //    client.CheckCertificateRevocation = false;
            //    await client.ConnectAsync(_globalConfig.SMTP_Server, _globalConfig.SMTP_Port, MailKit.Security.SecureSocketOptions.Auto);
            //    client.AuthenticationMechanisms.Remove("XOAUTH2");
            //
            //    if (!string.IsNullOrEmpty(_globalConfig.SMTP_Account) && !string.IsNullOrEmpty(_globalConfig.SMTP_Password))
            //        client.Authenticate(_globalConfig.SMTP_Account, _globalConfig.SMTP_Password);
            //
            //    await client.SendAsync(Message);
            //    client.Disconnect(true);
            //}
        }
    }
}