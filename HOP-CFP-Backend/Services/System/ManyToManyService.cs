using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class ManyToManyService : _ModelService<ManyToMany, ManyToManyModel>
    {
        public ManyToManyService(BaseServiceArgument argument) : base(argument) { }

        public async Task<IEnumerable<Guid>> GetIdsBySource(Guid id, string targetTable, int? relationType = null)
        {
            string sqlWhere = "";
            if (relationType.HasValue)
                sqlWhere += "AND RelationType = @RelationType";

            string sql = Sql(select: $"SELECT TargetId ",
                             subWhere: $@"AND SourceId = @SourceId
                                          AND TargetTable = @TargetTable
                                        {sqlWhere}");
            return await QueryAsync<Guid>(sql, new { SourceId = id, TargetTable = targetTable, RelationType = relationType });
        }
        //簡化方法
        public async Task<IEnumerable<Guid>> GetTargetIds<TTarget>(Guid id, int? relationType = null)
        {
            return await GetIdsBySource(id, CommonUtility.GetTableAttribute<TTarget>(), relationType);
        }
        //取得單一SourceId
        public async Task<Guid?> GetTargetId<TTarget>(Guid id, int? relationType = null)
        {
            Guid? result = null;

            IEnumerable<Guid> list = await GetTargetIds<TTarget>(id, relationType);
            if (list.Count() > 0) { result = list.FirstOrDefault(); }

            return result;
        }

        /// <summary>
        /// 取得SourceId列表
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="targetTable"></param>
        /// <param name="relationType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Guid>> GetSourceIds(Guid targetId, string targetTable, int? relationType = null)
        {
            string sqlWhere = "";
            if (relationType.HasValue)
                sqlWhere += "AND RelationType = @RelationType";

            string sql = Sql(select: $"SELECT SourceId ",
                             subWhere: $@"AND TargetId = @TargetId
                                          AND TargetTable = @TargetTable
                                        {sqlWhere}");
            return await QueryAsync<Guid>(sql, new { TargetId = targetId, TargetTable = targetTable, RelationType = relationType });
        }
        //簡化方法
        public async Task<IEnumerable<Guid>> GetSourceIds<TTarget>(Guid targetId, int? relationType = null)
        {
           return await GetSourceIds(targetId, CommonUtility.GetTableAttribute<TTarget>(), relationType);
        }
        //取得單一SourceId
        public async Task<Guid?> GetSourceId<TTarget>(Guid targetId, int? relationType = null)
        {
            Guid? result = null;

            IEnumerable<Guid> list = await GetSourceIds<TTarget>(targetId, relationType);
            if (list.Count() > 0) { result = list.FirstOrDefault(); }

            return result;
        }

        public async Task SaveById<TParent, TModel>(TParent parent, IEnumerable<TModel> models)
            where TParent : IdModelBase
            where TModel : IdModelBase
        {
            await SaveById(parent, models.Select(x => x.Id).ToList(), CommonUtility.GetTableAttribute<TModel>());
        }

        public async Task SaveById<TParent>(TParent parent, IEnumerable<Guid> ids, string targetTable) where TParent : IdModelBase
        {
            List<Task> tasks = new();
            string sql;
            Guid parentId = parent.Id;
            HashSet<Guid> hashIds = [.. ids];

            IEnumerable<Guid>? existIds = await GetIdsBySource(parentId, targetTable);
            HashSet<Guid> hashExistIds = [.. existIds];

            //檢查是否有被刪除的
            foreach (var item in hashExistIds)
            {
                tasks.AddTask(async () =>
                {
                    if (!hashIds.Contains(item))
                    {
                        sql = $@"delete {_tableName} where TargetId = @Id";
                        await ExecuteAsync(sql, new { Id = item });
                    }
                });
            }
            //檢查是否有新增的
            foreach (var item in hashIds)
            {
                tasks.AddTask(async () =>
                {
                    if (!hashExistIds.Contains(item))
                    {
                        ManyToMany many = new()
                        {
                            SourceTable = CommonUtility.GetTableAttribute<TParent>(),
                            SourceId = parentId,
                            TargetTable = targetTable,
                            TargetId = item,
                        };
                        await InsertAsync(many);
                    }
                });
            }
            await Task.WhenAll(tasks);
        }

        public override async Task Delete(Guid id)
        {
            //刪除ManyToMany.SourceId
            string sql = $@"update ManyToMany
                               set [Status] = -1
                                 , UpdateDate = @UpdateDate
                             where Status != -1 
                               and (SourceId = @Id
								    or TargetId = @Id) ";
            await ExecuteAsync(sql, new
            {
                Id = id,
                UpdateDate = SystemVariable.Now
            });
        }

    }
}
