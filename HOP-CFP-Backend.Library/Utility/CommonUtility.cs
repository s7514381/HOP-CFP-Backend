using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HOP_CFP_Backend.Library.Utility
{
    public static class CommonUtility
    {
        public static T CastBy<T>(this object source)
        {
            if (source == null)
            {
                return default;
            }

            // 創建序列化設置來處理循環引用
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            string json = JsonConvert.SerializeObject(source, settings);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static void Map<T>(this object source, T target)
        {
            if (source == null) { return; }

            var settings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(target, settings);
            JsonConvert.PopulateObject(json, source, settings);
        }

        public static T DeepCopy<T>(this T source)
        {
            if (source == null)
            {
                return default;
            }

            // 創建序列化設置來處理循環引用
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Auto
            };

            string json = JsonConvert.SerializeObject(source, settings);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// 取得父Model與子Model的關聯鍵，或任意關聯欄位，設定例: [ForeignKey(nameof(Professional))]
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TParent"></typeparam>
        /// <returns></returns>
        public static string GetForeignKeyField<TModel, TParent>()
        {
            string result = "";
            PropertyInfo[] props = typeof(TModel).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attributes = prop.GetCustomAttributes(typeof(ForeignKeyAttribute), false);
                if (attributes.Length > 0)
                {
                    ForeignKeyAttribute foreignKeyAttribute = attributes[0] as ForeignKeyAttribute;
                    if (foreignKeyAttribute == null) { continue; }

                    //ViewModel或DBModel都可使用
                    if (typeof(TParent).Name == foreignKeyAttribute.Name
                        || typeof(TParent).BaseType.Name == foreignKeyAttribute.Name) { result = prop.Name; break; }
                }
            }
            return result;
        }

        public static void SetFieldValue<T>(this T model, string field, object val)
        {
            PropertyInfo Info = typeof(T).GetProperty(field);

            try { Info?.SetValue(model, val); }
            catch (Exception ex) {  }
        }

        public static string GetTableAttribute<T>()
        {
            PropertyInfo[] props = typeof(T).GetProperties();
            object[] attributes = null;
            foreach (PropertyInfo prop in props)
            {
                Type propType = prop.ReflectedType;
                attributes = propType.GetCustomAttributes(typeof(Dapper.Contrib.Extensions.TableAttribute), false);
                if (attributes.Length > 0) 
                {
                    var attribute = attributes[0] as Dapper.Contrib.Extensions.TableAttribute;
                    if (attribute != null) { return attribute.Name; }
                }

                attributes = propType.BaseType.GetCustomAttributes(typeof(Dapper.Contrib.Extensions.TableAttribute), false);
                if (attributes.Length > 0)
                {
                    var attribute = attributes[0] as Dapper.Contrib.Extensions.TableAttribute;
                    if (attribute != null) { return attribute.Name; }
                }
            }
            return "";
        }

        public static string GetKeyAttribute<T>()
        {
            string result = "";
            PropertyInfo[] props = typeof(T).GetProperties();
            object[] attributes = null;
            foreach (PropertyInfo prop in props)
            {
                attributes = prop.GetCustomAttributes(typeof(KeyAttribute), false);
                if (attributes != null && attributes.Length > 0) { result = prop.Name; break; }

                attributes = prop.GetCustomAttributes(typeof(Dapper.Contrib.Extensions.ExplicitKeyAttribute), false);
                if (attributes != null && attributes.Length > 0) { result = prop.Name; break; }
            }
            return result;
        }

        public static List<T> GetEnumListByString<T>(this string str) where T : struct
        {
            List<T> result = new List<T>();
            if (!string.IsNullOrEmpty(str))
            {
                foreach (var item in str.Split(','))
                {
                    if (Enum.TryParse(item, out T category)) { result.Add(category); }
                }
            }
            return result;
        }

        public static List<TEnum> GetEnumList<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .ToList();
        }

        public static void AddTask(this List<Task> tasks, Func<Task> func)
        {
            tasks.Add(Task.Run(async () => {
                await func();
            }));
        }

        public static async Task Run(this List<Func<Task>> funcs)
        {
            List<Task> tasks = new();

            foreach (var func in funcs) {
                tasks.Add(Task.Run(async () => {
                    await func();
                }));
            }
            await Task.WhenAll(tasks);
        }

        public static async Task RunTasks(List<Func<Task>> funcs)
        {
            await funcs.Run();
        }

        public static object? GetFieldValue(this object pObject, string pField)
        {
            PropertyInfo? Info = pObject.GetType().GetProperty(pField);
            return Info?.GetValue(pObject);
        }


    }
}
