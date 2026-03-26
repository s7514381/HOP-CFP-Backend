using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.Library.Attributes
{
    /// <summary>
    /// 標註一個集合類資料欄位，驗證其元素數量在指定的區間。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class HasValueAttribute : RequiredAttribute
    {
        /// <summary>
        /// 最小元素數量(含)。
        /// </summary>
        public int MinCount { get; set; } = 1;

        /// <summary>
        /// 最大元素數量(含)。
        /// </summary>
        public int MaxCount { get; set; }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (!(value is ICollection collection))
            {
                throw new ValidationException("HasValueAttribute 不能用於非 ICollection 之類別");
            }

            var result = collection.Count >= MinCount;
            if (MaxCount != default)
            {
                result = result && (collection.Count <= MaxCount);
            }

            return result;
        }
    }
}
