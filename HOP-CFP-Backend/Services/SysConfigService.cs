using HOP_CFP_Backend.Library.Models.System;
using HOP_CFP_Backend.Library.Repositories;

namespace HOP_CFP_Backend.Services
{
    // 因為會與 TestVariable 造成循環相依性，所以不繼承 BaseService
    public class SysConfigService
    {
        public IDapperRepository Repository;

        public SysConfigService(IDapperRepository repository)
        {
            Repository = repository;
        }

        public Task<IEnumerable<SysConfig>> GetAllAsync()
        {
            return Repository.QueryAsync<SysConfig>("SELECT * FROM [SysConfig] WITH (NOLOCK)");
        }

        public Task<IEnumerable<SysConfig>> GetByTypeAsync(string typeName)
        {
            return Repository.QueryAsync<SysConfig>(@"
                SELECT * FROM [SysConfig] WITH (NOLOCK)
                WHERE [TypeName] = @typeName
            ", new { typeName });
        }

        public Task<SysConfig> GetAsync(string id)
        {
            return Repository.QueryFirstAsync<SysConfig>(@"
                SELECT TOP (1) *
                FROM [SysConfig] WITH (NOLOCK)
                WHERE [Key] = @id
            ", new { id });
        }

        public Task CreateAsync(params SysConfig[] sysConfigs)
        {
            var tasks = new List<Task<int>>();

            foreach (var dto in sysConfigs)
            {
                tasks.Add(Repository.ExecuteAsync(@"
                    INSERT INTO [SysConfig]
                    VALUES (@Id, @Value, @Note, @TypeName)
                ", dto));
            }

            return Task.WhenAll(tasks);
        }

        public Task UpdateAsync(params SysConfig[] sysConfigs)
        {
            var tasks = new List<Task<int>>();

            foreach (var dto in sysConfigs)
            {
                tasks.Add(Repository.ExecuteAsync(@"
                    UPDATE [SysConfig]
                    SET [Value] = @Value,
                        [Note] = @Note,
                        [TypeName] = @TypeName
                    WHERE [Id] = @Id
                ", dto));
            }

            return Task.WhenAll(tasks);
        }

        public Task Delete(string id)
        {
            return Repository.ExecuteAsync("DELETE FROM [SysConfig] WHERE [Id] = @id", new { id });
        }
    }
}