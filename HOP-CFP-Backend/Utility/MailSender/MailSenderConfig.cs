namespace HOP_CFP_Backend.Utility
{
    public class MailSenderConfig
    {
        /// <summary>
        /// SMTP 伺服器類型 (Google、Other)
        /// </summary>
        public string SMTP_ServerType { get; set; }

        /// <summary>
        /// SMTP 位置
        /// </summary>
        public string SMTP_Server { get; set; }

        /// <summary>
        /// SMTP Port
        /// </summary>
        public int SMTP_Port { get; set; }

        /// <summary>
        /// SMTP 帳號
        /// </summary>
        public string SMTP_Account { get; set; }

        /// <summary>
        /// SMTP 密碼
        /// </summary>
        public string SMTP_Password { get; set; }

        /// <summary>
        /// 寄件者信箱
        /// </summary>
        public string SMTP_Sender { get; set; }
    }
}