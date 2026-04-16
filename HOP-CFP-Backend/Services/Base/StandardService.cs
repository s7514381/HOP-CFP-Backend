using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Models;
using HOP_CFP_Backend.ViewModels;
using static Dapper.SqlMapper;

namespace HOP_CFP_Backend.Services
{
    /// <summary>
    /// 標準CRUD Service
    /// </summary>
    /// <typeparam name="DBModel"></typeparam>
    /// <typeparam name="ViewModel"></typeparam>
    /// <typeparam name="SearchViewModel"></typeparam>
    /// <typeparam name="ListViewModel"></typeparam>
    /// <typeparam name="ListDataModel"></typeparam>
    public abstract class _StandardService<DBModel, ViewModel, SearchViewModel, ListViewModel, ListDataModel> : _ModelService<DBModel, ViewModel>
        where DBModel : IdModelBase, new()
        where ViewModel : DBModel, new()
        where SearchViewModel : BaseSearchViewModel
        where ListViewModel : PagingViewModel<ListDataModel>, new()
        where ListDataModel : BaseListDataModel
    {
        public _StandardService(BaseServiceArgument argument) : base(argument) { }

        public virtual string GetBaseWhere()
        {
            string result = "";
            result = $@" WHERE main.[Status] != -1 
                           and Manager.Id = @ManagerId";

            return result;
        }

        public virtual (string, string, string) GetListQueryString(SearchViewModel searchModel) 
        {
            string mainSql = GetListQueryString_MainSQL();

            string baseWhere = GetBaseWhere();

            string sqlWhere = "";

            if (searchModel.Status.HasValue)
                sqlWhere += $" and main.[Status] = @Status ";

            //範例: 在繼承的地方override 用 base.GetListQueryString 取得這三個參數
            return (mainSql, baseWhere, sqlWhere);
        }

        public virtual string GetListQueryString_MainSQL()
        {
            return $@"
                SELECT main.*, Manager.Name AS UpdateUser
                       {AdditionalSqlSelect()}
                FROM {_tableName} AS main with(NOLOCK)
                LEFT JOIN Manager with(NOLOCK) ON main.CreateUserId = Manager.Id
                ";
        }

        public virtual string AdditionalSqlSelect() 
        {
            return "";
        }

        public virtual async Task<ListViewModel> GetList(SearchViewModel searchModel)
        {
            searchModel.ManagerId = _currentManager?.ManagerId;

            //主要資料語法
            (string mainSql, string baseWhere, string sqlWhere) = GetListQueryString(searchModel);

            //總數量(容易有效能問題，先不用)
            string totalRecordsSql = $"SELECT COUNT(*) FROM ({mainSql} {baseWhere}) AS A";

            //篩選後數量
            string filteredSql = $"SELECT COUNT(*) FROM ({mainSql} {baseWhere + sqlWhere}) AS A";

            //完整語法
            string completeSql = @$"
                SELECT * FROM ({mainSql} {baseWhere + sqlWhere}) AS A
                {GetOrderSql<ListDataModel>(searchModel)}
                OFFSET {searchModel.start} ROWS FETCH NEXT {searchModel.length} ROWS ONLY 
                ";

            //QueryMultiple減少DB Request次數
            string multiSql = $"{filteredSql} {completeSql}";
            GridReader multiList = await QueryMultipleAsync(multiSql, searchModel);

            int filteredCount = multiList.Read<int>().FirstOrDefault();
            var list = multiList.Read<ListDataModel>().ToList();
            list = await HandleList(list);

            ListViewModel model = new ListViewModel
            {
                draw = searchModel.draw + 1,
                recordsTotal = filteredCount,
                recordsFiltered = filteredCount,
                data = list,
            };
            return model;
        }

        public virtual string GetOrderSql<T>(SearchViewModel searchModel) where T : class
        {
            return BaseFunction.GetOrderSql<T>(searchModel);
        }


        /// <summary>
        /// 處理列表資料
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual async Task<List<ListDataModel>> HandleList(List<ListDataModel> list) 
        {
            return list;
        }

        public enum FrontendColor {
            success, danger, warning, info, primary, secondary, dark, light
        }

        public virtual string BadgeHtml(Guid id, string content, FrontendColor color, string addClass = "") 
        {
            return $@"<span data-id='{id.ToString()}' class='badge rounded-pill fs-0 badge-soft-{color} {addClass}'>{content}</span>";
        }

        public virtual async Task SwitchStatus(Guid id)
        {
            await ExecuteAsync(
               $@"update {_tableName}
                      set [Status] = 1 - [Status]
                    where Id = @Id", new { Id = id });
        }

    }
}