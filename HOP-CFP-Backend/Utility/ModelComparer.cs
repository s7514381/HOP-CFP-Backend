using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using HOP_CFP_Backend.Library.Attributes;
using HOP_CFP_Backend.Library.Repositories;
using HOP_CFP_Backend.Library.Attributes;
using HOP_CFP_Backend.Library.Repositories;

namespace HOP_CFP_Backend.Utility
{
    /// <summary>
    /// 提供比對 Model 差異的功能，並產出差異報告
    /// </summary>
    public static class ModelComparer
    {
        private delegate bool EqualsDelegate(object obj);
        private delegate string ToStringDelegate();

        /// <summary>
        /// 比對兩個 Model 的資料是否有差異(略過有 NoHistoryAttribute 的 Property)
        /// </summary>
        /// <param name="difference">中文比對結果(JSON格式)</param>
        /// <param name="repository">針對中文內容需要關聯性查找的屬性，提供資料庫連線</param>
        public static bool HasDifference<T>(
            T originalModel, T newModel,
            out string difference,
            IDapperRepository repository = null)
        {
            var result = HasDifferenceJArray(originalModel, newModel, out JArray jArray, repository);
            difference = jArray.ToString(Formatting.None);
            return result;
        }

        /// <summary>
        /// 比對兩個 Model 的資料是否有差異(略過有 NoHistoryAttribute 的)
        /// </summary>
        /// <param name="difference">比對結果</param>
        /// <param name="queryDisplayValueAttribute">針對中文內容需要關聯性查找的屬性，挾帶相關的資訊</param>
        /// <param name="repository">針對中文內容需要關聯性查找的屬性，提供資料庫連線</param>
        private static bool HasDifferenceJArray<T>(
            T originalModel, T newModel,
            out JArray difference,
            IDapperRepository repository = null,
            QueryDisplayValueAttribute queryDisplayValueAttribute = null)
        {
            difference = new JArray();

            // 區域函式
            JArray getDifference()
            {
                return new JArray
                {
                    new JObject
                    {
                        {"Type", "difference" },
                        {"Before", PrintObject(originalModel, queryDisplayValueAttribute, repository) },
                        {"After", PrintObject(newModel, queryDisplayValueAttribute, repository) }
                    }
                };
            }

            if (originalModel == null && newModel == null) return false;
            else if (originalModel == null || newModel == null)
            {
                difference = getDifference();
                return true;
            }

            // 有實作 Equals 方法的用 Equals 解決
            EqualsDelegate func = originalModel.Equals;
            if (func.Method.DeclaringType == originalModel.GetType())
            {
                if (!originalModel.Equals(newModel))
                {
                    difference = getDifference();
                    return true;
                }
                return false;
            }

            // 基礎類別直接比對
            var modelType = originalModel.GetType();
            if (modelType.IsPrimitive || modelType.IsEnum || originalModel is string)
            {
                if (!originalModel.Equals(newModel))
                {
                    difference = getDifference();
                    return true;
                }
                return false;
            }

            // 如果是集合類
            if (originalModel is ICollection collectionA)
            {
                var enumerableA = collectionA.GetEnumerator();
                var enumerableB = (newModel as ICollection).GetEnumerator();

                // 檢查集合內每個元素(照順序)是否都相同
                var ANotEnd = enumerableA.MoveNext();
                var BNotEnd = enumerableB.MoveNext();
                while (ANotEnd && BNotEnd)
                {
                    if (HasDifference(enumerableA.Current, enumerableB.Current, out var _, repository))
                    {
                        difference = getDifference();
                        return true;
                    }

                    ANotEnd = enumerableA.MoveNext();
                    BNotEnd = enumerableB.MoveNext();
                }

                // 檢查元素數量是否相同
                if (ANotEnd || BNotEnd)
                {
                    difference = getDifference();
                    return true;
                }
                return false;
            }

            // 沒有實作 Equals 的 Struct 之類的東西
            var properties = modelType.GetProperties();
            if (properties.Length == 0)
                throw new NotImplementedException($"請為 {modelType.Name} 覆寫 Equals 函式");

            // 檢查每個 Property 是否都一樣
            bool isDifferent = false;
            foreach (PropertyInfo property in properties)
            {
                // 靜態 Property 一定會一樣就不檢查
                if (property.GetAccessors(true)[0].IsStatic)
                    continue;

                // 有 NoHistoryAttribute 的不檢查
                if (property.GetCustomAttribute(typeof(NoHistoryAttribute), true) is NoHistoryAttribute)
                {
                    continue;
                }

                // 有 NoHistoryAttribute 的不檢查
                string name = property.Name;
                if (property.GetCustomAttribute(typeof(DisplayAttribute), true) is DisplayAttribute display)
                {
                    name = display.Name;
                }

                var queryAttribute = property.GetCustomAttribute(typeof(QueryDisplayValueAttribute)) as QueryDisplayValueAttribute;
                if (HasDifferenceJArray(property.GetValue(originalModel), property.GetValue(newModel), out JArray diff, repository, queryAttribute))
                {
                    difference.Add(new JObject
                        {
                            {"Type", "field" },
                            {"Name", name},
                            {"Content", diff }
                        });
                    isDifferent = true;
                }
            }
            return isDifferent;
        }

        /// <summary>
        /// 打印物件中文內容(略過有 NoHistoryAttribute 的)
        /// </summary>
        /// <param name="obj">要打印的物件</param>
        /// <param name="queryDisplayValueAttribute">挾帶的關聯性資訊</param>
        /// <param name="repository">供關聯性查詢的資料庫連線</param>
        private static JToken PrintObject(object obj,
            QueryDisplayValueAttribute queryDisplayValueAttribute = null,
            IDapperRepository repository = null)
        {
            if (obj == null) return JValue.CreateNull();

            // 如果帶有 QueryDisplayValueAttribute，回傳查詢結果
            // 再遞迴一次可以確保字串輸出格式正確
            // 集合類留待後面處理
            if (queryDisplayValueAttribute != null && !(obj is ICollection))
            {
                return PrintObject(queryDisplayValueAttribute.GetDisplayValue(repository, obj));
            }

            // 如果子類別有覆寫 ToString，直接回傳
            ToStringDelegate func = obj.ToString;
            if (func.Method.DeclaringType == obj.GetType())
            {
                return new JValue(obj.ToString());
            }

            // 如果是基礎類別或 enum
            var modelType = obj.GetType();
            if (modelType.IsPrimitive)
            {
                return new JValue(obj.ToString());
            }
            else if (modelType.IsEnum)
            {
                if (modelType.GetField(obj.ToString()).GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute display)
                {
                    return new JValue(display.Name);
                }
                return new JValue(obj.ToString());
            }

            // 如果是集合類，遍歷印出內容
            if (obj is ICollection collection)
            {
                var jArray = new JArray();
                if (queryDisplayValueAttribute != null)
                {
                    foreach (var element in collection)
                    {
                        jArray.Add(PrintObject(queryDisplayValueAttribute.GetDisplayValue(repository, element)));
                    }
                }
                else
                {
                    foreach (var element in collection)
                    {
                        jArray.Add(PrintObject(element));
                    }
                }
                return jArray;
            }

            // 沒有實作 Equals 的 Struct 之類的東西
            var properties = modelType.GetProperties();
            if (properties.Length == 0)
                throw new NotImplementedException($"請為 {modelType.Name} 覆寫 ToString 函式");

            // 如果是類別
            else
            {
                var jObject = new JObject();
                foreach (PropertyInfo property in properties)
                {
                    // 有 NoHistoryAttribute 的不打印
                    if (property.GetCustomAttribute(typeof(NoHistoryAttribute), true) is NoHistoryAttribute)
                    {
                        continue;
                    }

                    string name = property.Name;
                    if (property.GetCustomAttribute(typeof(DisplayAttribute), true) is DisplayAttribute display)
                    {
                        name = display.Name;
                    }
                    jObject.Add(name, PrintObject(property.GetValue(obj)));
                }
                return jObject;
            }
        }
    }
}
