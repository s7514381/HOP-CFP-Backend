using System;

namespace HOP_CFP_Backend.Library.Attributes
{
    /// <summary>
    /// 標註一個欄位不該被算做資料異動
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NoHistoryAttribute : Attribute
    {

    }
}
