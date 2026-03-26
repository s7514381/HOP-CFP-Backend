using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.Library.Models
{
    /// <summary>
    /// 枚舉資料紀錄的狀態
    /// </summary>
    public enum EEventRole
    {
        簽約公司 = 1,
        管理者 = 2,
        服務台 = 3,
        小幫手 = 4,
        民眾 = 5,
        系統管理員 = 99,
    }

    public static class EventRoleExtension
    {
        public static string Shortname(this EEventRole eventRole)
        {
            switch (eventRole)
            {
                case EEventRole.管理者:
                    return "mg";
                case EEventRole.服務台:
                    return "sd";
                case EEventRole.小幫手:
                    return "help";
                case EEventRole.簽約公司:
                case EEventRole.民眾:
                default:
                    return "";
            }
        }
    }
}
