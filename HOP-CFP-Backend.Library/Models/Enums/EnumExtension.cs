using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace HOP_CFP_Backend.Library.Models
{
    /// <summary>
    /// 提供 Enum 使用的擴展方法
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 取得 Enum 的顯示名稱
        /// </summary>
        /// <returns>DisplayAttribute 標註的 Name</returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            var memberInfo = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault(); // 使用 FirstOrDefault 以避免在找不到成員時拋出例外

            if (memberInfo == null)
            {
                // 理論上不應該發生，但作為防禦性編程
                return enumValue.ToString();
            }

            var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();

            // 如果 displayAttribute 不為 null，則回傳 Name；否則回傳 enumValue.ToString()
            return displayAttribute?.Name ?? enumValue.ToString();
        }
    }
}
