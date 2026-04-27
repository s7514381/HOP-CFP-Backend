using Microsoft.AspNetCore.Mvc.Rendering;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOP_CFP_Backend.Services
{
    public abstract class _ModelService<DBModel, ViewModel> : BaseService
        where DBModel : IdModelBase, new()
        where ViewModel : DBModel, new()
    {
        public string? _tableName;
        public string? _keyField;
        protected bool softDelete { get; set; } = true;

        public _ModelService(BaseServiceArgument argument) : base(argument)
        {
            _tableName = CommonUtility.GetTableAttribute<DBModel>();

            //新的IdModelBase已經改為Id，舊ModelBase才會是其他的
            _keyField = CommonUtility.GetKeyAttribute<DBModel>();
        }

        public virtual string SqlData(
            string select = "", string from = "", string join = "",
            string where = "", string subWhere = "", string order = "")
        {
            return Sql(select, from, join, where, subWhere: "and main.Id = @Id", order);
        }

        public virtual string SqlModelData()
        {
            return SqlData();
        }

        public virtual async Task<ViewModel> CreateModel()
        {
            return new ViewModel();
        }

        public virtual async Task<DBModel> GetData(Guid id)
        {
            DBModel result = await QueryFirstAsync<DBModel>(SqlData(), new { Id = id });
            return result;
        }

        public virtual async Task<DBModel> GetFieldData(Guid id, string fieldArray)
        {
            string sql = $"SELECT {fieldArray} FROM {_tableName} WHERE {_keyField} = @Id";

            DBModel result = await QueryFirstAsync<DBModel>(sql, new { Id = id });
            return result;
        }

        public virtual async Task<ViewModel> GetModel(Guid id)
        {
            ViewModel viewModel = await QueryFirstAsync<ViewModel>(SqlModelData(), new { Id = id });
            if (viewModel != null) { await SetModel(viewModel); }

            return viewModel;
        }

        public virtual async Task<ViewModel> GetCopyModel(Guid id)
        {
            ViewModel model = await GetModel(id);
            if (model != null) { await CopySetting(model); }
            return model;
        }

        public virtual string Sql(
            string select = "", string from = "", string join = "",
            string where = "", string subWhere = "", string order = "")
        {
            if (string.IsNullOrEmpty(select)) { select = $"SELECT main.* "; }
            if (string.IsNullOrEmpty(from)) { from = $"FROM {_tableName} AS main "; }
            if (string.IsNullOrEmpty(where)) { where = $"WHERE main.[Status] != -1"; }
            if (string.IsNullOrEmpty(order)) { order = $"ORDER BY main.Sequence"; }

            return $@"{select} {from} {join} {where} {subWhere} {order}";
        }

        public virtual async Task<IEnumerable<ViewModel>> GetModelList(IEnumerable<Guid> ids)
        {
            string sql = Sql(subWhere: $"AND Id in @Ids");
            IEnumerable<ViewModel> list = await QueryAsync<ViewModel>(sql, new { Ids = ids });
            return await SetModelList(list);
        }

        public virtual string SqlMTMDataList(string subWhere = "")
        {
            return Sql(join: $@"inner join ManyToMany on main.Id = ManyToMany.TargetId",
                   subWhere: $@"AND ManyToMany.SourceId = @SourceId
                                AND TargetTable = @TargetTable
                                {subWhere}");
        }

        public virtual async Task<IEnumerable<DBModel>> GetMTMDataList(Guid sourceId, int? relationType = null)
        {
            string subWhere = "";
            if (relationType.HasValue)
                subWhere += "AND RelationType = @RelationType";

            string sql = Sql(join: $@"inner join ManyToMany on main.Id = ManyToMany.TargetId",
                           subWhere: $@"AND ManyToMany.SourceId = @SourceId
                                        AND TargetTable = @TargetTable
                                        {subWhere}");

            return await QueryAsync<DBModel>(sql, new { SourceId = sourceId, TargetTable = _tableName, RelationType = relationType });
        }
        public virtual async Task<DBModel> GetMTMData(Guid sourceId, int? relationType = null)
        {
            DBModel result = null;
            IEnumerable<DBModel> list = await GetMTMDataList(sourceId, relationType);
            if (list.Count() > 0) { result = list.FirstOrDefault(); }

            return result;
        }

        /// <summary>
        /// 多對多專用，傳入SourceId(父Id)
        /// </summary>
        /// <param name="subWhere"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ViewModel>> GetMTMModelList(List<Guid> sourceIds, int? relationType = null)
        {
            string subWhere = "";
            if (relationType.HasValue)
                subWhere += "AND RelationType = @RelationType";

            string sql = Sql(select: $"SELECT main.*, ManyToMany.SourceId ",
                               join: $@"inner join ManyToMany on main.Id = ManyToMany.TargetId",
                           subWhere: $@"AND ManyToMany.SourceId in @SourceIds
                                        AND TargetTable = @TargetTable
                                      {subWhere}");
            IEnumerable<ViewModel> list = await QueryAsync<ViewModel>(sql, new { SourceIds = sourceIds, TargetTable = _tableName, RelationType = relationType });
            return await SetModelList(list);
        }
        public virtual async Task<IEnumerable<ViewModel>> GetMTMModelList(Guid sourceId, int? relationType = null)
        {
            return await GetMTMModelList(new List<Guid>() { sourceId }, relationType);
        }
        public virtual async Task<ViewModel> GetMTMModel(Guid sourceId, int? relationType = null)
        {
            ViewModel result = null;
            IEnumerable<ViewModel> list = await GetMTMModelList(sourceId, relationType);
            if (list.Count() > 0) { result = list.FirstOrDefault(); }

            return result;
        }

        public virtual async Task<IEnumerable<ViewModel>> SetModelList(IEnumerable<ViewModel> list)
        {
            if (list == null || list.Count() == 0) { return list; }
            List<Task> tasks = new();

            foreach (ViewModel model in list)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await SetModel(model);
                }));
            }
            if (tasks.Count > 0) { await Task.WhenAll(tasks); }
            return list;
        }

        public virtual string SqlDataListByParent(string foreignKeyField)
        {
            return Sql(subWhere: $"AND {foreignKeyField} = @ParentId ",
                                order: $"ORDER BY Sequence");
        }

        public virtual async Task<List<ViewModel>> GetModelListByParent<TParent>(Guid? parentId)
        {
            string foreignKeyField = CommonUtility.GetForeignKeyField<ViewModel, TParent>();
            if (string.IsNullOrEmpty(foreignKeyField)) { return new(); }

            return await GetModelListByParent(foreignKeyField, parentId);
        }

        public virtual async Task<List<ViewModel>> GetModelListByParent(string foreignKeyField, Guid? parentId)
        {
            string sql = SqlDataListByParent(foreignKeyField);
            IEnumerable<ViewModel> list = await QueryAsync<ViewModel>(sql, new { ParentId = parentId });

            List<Task> tasks = new();
            int seq = 0;
            foreach (ViewModel model in list)
            {
                //如果是子項目，則套用排序
                if (!model.Sequence.HasValue) { model.Sequence = seq + 1; }
                seq = model.Sequence.Value;
            }
            await SetModelList(list);

            return list.ToList();
        }

        /// <summary>
        /// 設定完整Model
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        protected virtual async Task SetModel(ViewModel viewModel) { }

        /// <summary>
        /// 資料是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsExist(Guid id)
        {
            return await QueryFirstAsync<int>($@"
                SELECT COUNT(1) 
                  FROM {_tableName} WITH (NOLOCK)
                 WHERE {_keyField} = @KeyValue", new { KeyValue = id }) > 0;
        }

        /// <summary>
        /// 完整儲存，沒有特殊需求一律呼叫這支
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public virtual async Task Save(ViewModel viewModel)
        {
            List<ViewModel> list = new() { viewModel };
            await Save(list);
        }

        public virtual async Task Save(IEnumerable<ViewModel> list, Func<ViewModel, Task> insertFunc = null, Func<ViewModel, Task> updateFunc = null)
        {
            if (list?.Any() != true) return;

            List<Task> tasks = new();
            IEnumerable<Guid>? existIds = null;

            string sql = Sql(select: $"SELECT main.{_keyField} ", subWhere: $" AND {_keyField} in @IdList");
            existIds = await QueryAsync<Guid>(sql, new { IdList = list.Select(x => x.Id) }, "IsExist");
            HashSet<Guid> hashExistIds = [.. existIds];

            foreach (var item in list)
            {
                if (hashExistIds.Contains(item.Id))
                {
                    tasks.AddTask(async () => { await Update(item); });

                    if (updateFunc != null) 
                    {
                        tasks.AddTask(async () => { await updateFunc(item); });
                    }
                }
                else
                {
                    tasks.AddTask(async () => { await Insert(item); });

                    if (insertFunc != null)
                    {
                        tasks.AddTask(async () => { await insertFunc(item); });
                    }
                }
            }
            await Task.WhenAll(tasks);
        }

        public virtual async Task SaveData(DBModel model)
        {
            List<DBModel> list = new() { model };
            await SaveData(list);
        }

        public virtual async Task SaveData(IEnumerable<DBModel> list)
        {
            if (list?.Any() != true) return;

            List<Task> tasks = new();
            IEnumerable<Guid>? existIds = null;

            string sql = Sql(select: $"SELECT main.{_keyField}", subWhere: $" AND {_keyField} in @IdList");
            existIds = await QueryAsync<Guid>(sql, new { IdList = list.Select(x => x.Id) }, "IsExist");
            HashSet<Guid> hashExistIds = [.. existIds];

            foreach (var item in list)
            {
                tasks.Add(Task.Run(async () =>
                {
                    if (hashExistIds.Contains(item.Id)) { await UpdateAsync(item); }
                    else { await InsertAsync(item); }
                }));
            }
            await Task.WhenAll(tasks);
        }

        public virtual async Task Update(ViewModel viewModel)
        {
            await ModelSave(viewModel);

            viewModel.UpdateDate = SystemVariable.Now;
            viewModel.UpdateUserId = _currentManager?.Id;

            DBModel DBModel = viewModel;
            await UpdateAsync(DBModel);
        }

        public virtual async Task Insert(ViewModel viewModel)
        {
            await ModelSave(viewModel);

            viewModel.CreateDate = SystemVariable.Now;
            viewModel.CreateUserId = _currentManager?.Id;
            viewModel.UpdateDate = SystemVariable.Now;
            viewModel.UpdateUserId = _currentManager?.Id;

            DBModel DBModel = viewModel;
            await InsertAsync(DBModel);
        }

        public virtual async Task Copy(ViewModel viewModel)
        {
            await CopySetting(viewModel);

            await Insert(viewModel);
        }

        /// <summary>
        /// 儲存時更新物件資訊
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        protected virtual async Task ModelSave(ViewModel viewModel) { }

        public virtual async Task CopySetting(ViewModel viewModel)
        {
            viewModel.Id = Guid.NewGuid();
        }

        public virtual string SqlDelete(string subWhere)
        {
            if (string.IsNullOrEmpty(subWhere)) { subWhere = " and 1 = 0 "; }
            string sql;

            if (softDelete) { sql = $@"update {_tableName} set Status = -1 where Status != -1 "; }
            else { sql = $@"delete {_tableName} where Status != -1 "; }
            sql += subWhere;

            return sql;
        }

        protected virtual async Task ModelDelete(ViewModel model) { }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task Delete(ViewModel viewModel)
        {
            List<ViewModel> list = new() { viewModel };
            await DeleteModel(list);
        }

        public virtual async Task Delete(Guid id)
        {
            await CommonUtility.RunTasks(new() {
                async () => { await _lazy.ManyToManyService.Value.Delete(id); },
                async () =>
                {
                   string sql = $@"update {_tableName} 
                                   set Status = -1 
                                     , UpdateDate = @UpdateDate
                                 where Status != -1 
                                   and {_keyField} = @Id";
                    await ExecuteAsync(sql, new { Id = id, UpdateDate = SystemVariable.Now });
                },
            });
        }

        public virtual async Task DeleteByParent<TParent>(Guid parentId)
        {
            List<Task> tasks = new();
            string foreignKeyField = CommonUtility.GetForeignKeyField<DBModel, TParent>();
            if (string.IsNullOrEmpty(foreignKeyField)) { throw new Exception(); }

            string sql = Sql(select: $"SELECT main.Id",
                             subWhere: $"AND {foreignKeyField} = @ParentId ");
            IEnumerable<Guid> ids = await QueryAsync<Guid>(sql, new { ParentId = parentId });

            foreach (var id in ids)
            {
                tasks.AddTask(async () => { await Delete(id); });
            }
            await Task.WhenAll(tasks);
        }

        public virtual async Task DeleteModel(IEnumerable<ViewModel> list)
        {
            if (list == null || list.Count() == 0) { return; }
            List<Task> tasks = new();

            foreach (var item in list)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await ModelDelete(item);
                }));
            }

            tasks.AddTask(async () =>
            {
                string sql = SqlDelete($"and {_keyField} in @IdList");
                await ExecuteAsync(sql, new { IdList = list.Select(x => x.Id) });
            });
            await Task.WhenAll(tasks);
        }

        //public virtual async Task TestSave()
        //{
        //    IEnumerable<ViewModel> list = await GetModelList();
        //    if (list.Count() > 0) { await Save(list.First()); }
        //}

        //public virtual async Task TestSaveList()
        //{
        //    IEnumerable<ViewModel> list = await GetModelList();
        //    foreach (var item in list) {
        //        item.CreateDate = DateTime.Now;
        //    }
        //    await Save(list);
        //}

        //public virtual async Task TestDelete()
        //{
        //    IEnumerable<ViewModel> list = await GetModelList();
        //    if (list.Count() > 2) {
        //        await DeleteModel(list.Take(2));
        //    }
        //}

        /// <summary>
        /// 由父項呼叫，更新明細
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <param name="parent">父model</param>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual async Task UpdateDetail<TParent>(TParent parent, List<ViewModel> list)
        {
            //取得外來鍵
            string foreignKeyField = CommonUtility.GetForeignKeyField<ViewModel, TParent>();
            if (string.IsNullOrEmpty(foreignKeyField)) { throw new Exception(); }

            string parentKeyField = CommonUtility.GetKeyAttribute<TParent>();
            if (string.IsNullOrEmpty(parentKeyField)) { throw new Exception(); }

            object? foreignKeyValue = parent?.GetFieldValue(parentKeyField);
            if (foreignKeyValue == null) { throw new Exception(); }

            await UpdateDetail(list, foreignKeyField, foreignKeyValue);
        }

        /// <summary>
        /// 由父項呼叫，更新明細
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <param name="parent">父model</param>
        /// <param name="list"></param>
        /// <returns></returns>
        protected virtual async Task UpdateDetail(List<ViewModel> list, string foreignKeyField, object foreignKeyValue)
        {
            if (list == null
                || string.IsNullOrEmpty(foreignKeyField)
                || foreignKeyValue == null) { return; }

            List<Guid> idList = new();
            List<Task> tasks = new();

            int seq = 0;
            foreach (ViewModel item in list)
            {
                if (item.GetFieldValue(foreignKeyField) != foreignKeyValue)
                    item.SetFieldValue(foreignKeyField, foreignKeyValue);

                if (!item.Sequence.HasValue) { item.Sequence = seq + 1; }
                seq = item.Sequence.Value;

                //還有在使用的明細不刪除
                idList.Add(item.Id);
            }
            tasks.AddTask(async () => { await Save(list); });

            //刪除沒用到的細項
            string sql = SqlDelete($@"and {foreignKeyField} = @ForeignKeyValue
                                      and {_keyField} not in @IdList");
            tasks.AddTask(async () =>
            {
                await ExecuteAsync(sql, new
                {
                    ForeignKeyValue = foreignKeyValue,
                    IdList = idList,
                });
            });
            await Task.WhenAll(tasks);
        }

        public virtual async Task<IEnumerable<SelectListItem>> GetSelectListItems(string nameField)
        {
            var result = await QueryAsync<(Guid Value, string Text)>($@"
                SELECT {_keyField} AS Value, {nameField} AS Text
                FROM {_tableName}
                WHERE Status != -1");
            return result.Select(x => new SelectListItem(x.Text, x.Value.ToString()));
        }

    }

}