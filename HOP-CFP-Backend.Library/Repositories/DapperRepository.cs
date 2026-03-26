using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HOP_CFP_Backend.Library.Repositories
{
    // 務必確保介面有繼承 IDisposable
    public class DapperRepository : IDapperRepository
    {
        private readonly DapperDBContext _dbContext;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private bool _disposed = false;
        public event EventHandler Disposed;

        // 修改點 1：Connection 改為內部管理，不對外隨意存取，確保狀態受控
        public IDbConnection Connection
        {
            get
            {
                EnsureConnectionOpen();
                return _connection;
            }
        }

        public IDbTransaction Transaction => _transaction;

        public DapperRepository(DapperDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _connection = _dbContext.GetConnection();
        }

        // 修改點 3：新增一個內部機制，確保只有在真的要執行 SQL 時才開啟連線
        private void EnsureConnectionOpen()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                Console.WriteLine($"開啟連線");
                _connection.Open();
            }
        }

        public ManagedTransaction OpenTransaction()
        {
            EnsureConnectionOpen();
            _transaction = _connection.BeginTransaction();
            return new ManagedTransaction(_transaction, this);
        }

        public void ClearTransaction()
        {
            _transaction?.Dispose();
            _transaction = null;
        }

        // 修改點 4：所有的執行方法，透過 _connection 屬性確保已開啟，並傳入 _transaction
        public IEnumerable<T> Query<T>(string sql, object param = null)
            => Connection.Query<T>(sql, param, _transaction);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
            => Connection.QueryAsync<T>(sql, param, _transaction);

        public T QueryFirst<T>(string sql, object param = null)
            => Connection.QueryFirst<T>(sql, param, _transaction);

        public Task<T> QueryFirstAsync<T>(string sql, object param = null)
            => Connection.QueryFirstAsync<T>(sql, param, _transaction);

        public T QueryFirstOrDefault<T>(string sql, object param = null)
            => Connection.QueryFirstOrDefault<T>(sql, param, _transaction);

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null)
            => Connection.QueryFirstOrDefaultAsync<T>(sql, param, _transaction);

        public T QuerySingle<T>(string sql, object param = null)
            => Connection.QuerySingle<T>(sql, param, _transaction);

        public Task<T> QuerySingleAsync<T>(string sql, object param = null)
            => Connection.QuerySingleAsync<T>(sql, param, _transaction);

        public int Execute(string sql, object param = null)
            => Connection.Execute(sql, param, _transaction);

        public Task<int> ExecuteAsync(string sql, object param = null)
            => Connection.ExecuteAsync(sql, param, _transaction);

        public long Insert<T>(T entityToInsert) where T : class
            => Connection.Insert(entityToInsert, _transaction);

        public Task<int> InsertAsync<T>(T entityToInsert) where T : class
            => Connection.InsertAsync(entityToInsert, _transaction);

        public Task<bool> UpdateAsync<T>(T entityToInsert) where T : class
            => Connection.UpdateAsync(entityToInsert, _transaction);

        public SqlMapper.GridReader QueryMultiple(string sql, object param = null)
            => Connection.QueryMultiple(sql, param, _transaction);

        public Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object param = null)
            => Connection.QueryMultipleAsync(sql, param, _transaction);

        // 修改點 5：完善的 Dispose 模式，確保連線一定會歸還給連線池
        public void Dispose()
        {
            Console.WriteLine($"關閉連線");

            Disposed?.Invoke(this, EventArgs.Empty);
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }

                    if (_connection != null)
                    {
                        if (_connection.State != ConnectionState.Closed)
                            _connection.Close();
                        _connection.Dispose();
                    }
                }
                _disposed = true;
            }
        }
    }
}