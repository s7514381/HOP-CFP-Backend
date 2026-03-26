using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HOP_CFP_Backend.Library.Repositories
{
    public interface IDapperRepository : IDisposable
    {
        IDbConnection Connection { get; }

        event EventHandler Disposed;

        /// <summary>
        /// 開始一個新的 Transaction
        /// </summary>
        /// <returns>包裝後的 Transaction 物件，會連動所屬的 IDapperRepository</returns>
        ManagedTransaction OpenTransaction();

        IEnumerable<T> Query<T>(string sql, object param = null);

        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null);

        T QueryFirst<T>(string sql, object param = null);

        Task<T> QueryFirstAsync<T>(string sql, object param = null);

        T QueryFirstOrDefault<T>(string sql, object param = null);

        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null);

        //2025.12.26 Ren added : 新增 QuerySingel 查詢出來的資料, 一定要有資料
        T QuerySingle<T>(string sql, object param = null);

        Task<T> QuerySingleAsync<T>(string sql, object param = null);

        int Execute(string sql, object param = null);

        Task<int> ExecuteAsync(string sql, object param = null);

        long Insert<T>(T entityToInsert) where T : class;

        Task<int> InsertAsync<T>(T entityToInsert) where T : class;

        Task<bool> UpdateAsync<T>(T entityToInsert) where T : class;

        SqlMapper.GridReader QueryMultiple(string sql, object param = null);

        Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object param = null);
    }
}