using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace HOP_CFP_Backend.Library.Repositories
{
    public class DapperDBContext
    {
        private readonly string _connectionString;

        public SQLType SQLType { get; set; } = SQLType.SQLSERVER;

        public DapperDBContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 修改點：取得連線物件，但不立即開啟連線
        /// 這樣可以讓連線在真正要用時才由 Repository 開啟，並交由連線池管理
        /// </summary>
        public IDbConnection GetConnection() // 改名為 GetConnection，移除 Open 字樣
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("資料庫連線字串尚未設定。");
            }

            IDbConnection conn = null;

            switch (SQLType)
            {
                case SQLType.SQLSERVER:
                    conn = new SqlConnection(_connectionString);
                    break;
                default:
                    throw new NotSupportedException($"不支援的資料庫類型: {SQLType}");
            }

            // 回傳一個「Ready to use」但尚未開啟的連線物件。
            return conn;
        }

        /// <summary>
        /// 重新取得一個乾淨的連線物件
        /// </summary>
        public IDbConnection ReConnection(IDbConnection conn)
        {
            if (conn != null)
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Dispose();
            }
            return GetConnection();
        }
    }

    public enum SQLType
    {
        SQLSERVER
    }
}