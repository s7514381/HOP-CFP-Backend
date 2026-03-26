using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace HOP_CFP_Backend.Library.Repositories
{
    /// <summary>
    /// 資料庫列舉
    /// </summary>
    public enum DbConnectionType
    {
        Default
    }

    public interface IDbConnectionFactory
    {
        void Connect(Action<IDbConnection> action, DbConnectionType connectionType = DbConnectionType.Default);
        Task ConnectAsync(Func<IDbConnection, Task> func, DbConnectionType connectionType = DbConnectionType.Default);
        void TranConnect(Action<IDbConnection, IDbTransaction> action, DbConnectionType connectionType = DbConnectionType.Default);
        Task TranConnectAsync(Func<IDbConnection, IDbTransaction, Task> func, DbConnectionType connectionType = DbConnectionType.Default);
    }

    public class SqlConnectionFactoryConfig
    {
        public string ConnectionString { get; set; }
    }

    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly SqlConnectionFactoryConfig _config;

        public SqlConnectionFactory(SqlConnectionFactoryConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 取得對應資料庫連線字串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetDbConnectionString(DbConnectionType type)
        {
            switch (type)
            {
                case DbConnectionType.Default:
                    return _config.ConnectionString;
                default:
                    return "";
            }
        }

        #region 基礎方法

        public void Connect(Action<IDbConnection> action, DbConnectionType connectionType = DbConnectionType.Default)
        {
            using (var connection = new SqlConnection(GetDbConnectionString(connectionType)))
            {
                connection.Open();
                action(connection);
            }
        }

        public async Task ConnectAsync(Func<IDbConnection, Task> func, DbConnectionType connectionType = DbConnectionType.Default)
        {
            using (var connection = new SqlConnection(GetDbConnectionString(connectionType)))
            {
                await connection.OpenAsync();
                await func(connection);
            }
        }

        public void TranConnect(Action<IDbConnection, IDbTransaction> action, DbConnectionType connectionType = DbConnectionType.Default)
        {
            using (var connection = new SqlConnection(GetDbConnectionString(connectionType)))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        action(connection, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public async Task TranConnectAsync(Func<IDbConnection, IDbTransaction, Task> func, DbConnectionType connectionType = DbConnectionType.Default)
        {
            using (var connection = new SqlConnection(GetDbConnectionString(connectionType)))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await func(connection, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        #endregion


    }


}
