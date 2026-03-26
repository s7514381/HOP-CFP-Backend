using Dapper;
using System;
using HOP_CFP_Backend.Library.Repositories;

namespace HOP_CFP_Backend.Library.Attributes
{
    /// <summary>
    /// 標註一個資料欄位，代表其顯示內容需要透過資料庫查詢取得。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class QueryDisplayValueAttribute : Attribute
    {
        /// <summary>
        /// 查詢資料庫要使用的 SQL 語法(帶入參數為 @value)
        /// </summary>
        public string SqlCommand { get; set; }

        /// <param name="sqlCommand">查詢資料庫要使用的 SQL 語法(帶入參數為 @value)</param>
        public QueryDisplayValueAttribute(string sqlCommand)
        {
            SqlCommand = sqlCommand;
        }

        /// <summary>
        /// 取得顯示內容
        /// </summary>
        /// <param name="repository">資料庫物件</param>
        /// <param name="value">要帶入的值</param>
        /// <returns></returns>
        public string GetDisplayValue(IDapperRepository repository, object value)
        {
            return repository.QueryFirstOrDefault<string>(SqlCommand, new { value });
        }
    }
}
