using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Models.DataTables;
using HOP_CFP_Backend.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace HOP_CFP_Backend.Models
{
    public static class BaseFunction
    {
        public static PropertyInfo[] GetDisplayField(object pObject, bool pIsDisplayAbstract = false)
        {
            PropertyInfo[] returnVal = null;

            PropertyInfo[] propertyInfos = pObject.GetType().GetProperties();
            foreach (var info in propertyInfos)
            {

                if (!pIsDisplayAbstract)
                    if (info.GetMethod.IsAbstract)
                    {
                        propertyInfos = propertyInfos.Where(x => x.Name != info.Name).ToArray();
                    }
            }
            returnVal = propertyInfos;

            return returnVal;
        }

        public static IEnumerable<SelectListItem> GetStatusItems()
        {
            return Enum.GetValues(typeof(EStatus)).Cast<EStatus>().Select(v => new SelectListItem
            {
                Text = v.GetDisplayName(),
                Value = ((int)v).ToString()
            }).OrderByDescending(p => p.Value).Where(p => p.Text != EStatus.Deleted.GetDisplayName());
        }

        public static List<string> GetModelField(object model, bool isInherit)
        {
            List<string> rtValue = new List<string>();
            PropertyInfo[] propertyInfos = model.GetType().GetProperties();

            foreach (var info in propertyInfos)
            {
                if (isInherit) { rtValue.Add(info.Name); }
                else { if (!IsInheritInfo(info)) { rtValue.Add(info.Name); } }
            }
            return rtValue;
        }

        public static List<string> GetModelField(object model)
        {
            List<string> rtValue = new List<string>();
            PropertyInfo[] propertyInfos = model.GetType().GetProperties();

            foreach (var info in propertyInfos)
            {
                rtValue.Add(info.Name);
            }
            return rtValue;
        }

        public static IEnumerable<SelectListItem> GetSelectListItem<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select(v => new SelectListItem
            {
                Text = v.GetDisplayName(),
                Value = Convert.ToInt32(v).ToString()
            });
        }

        /// <summary>
        /// 判斷是否為繼承欄位
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool IsInheritInfo(PropertyInfo info)
        {
            string inheritClass = info.DeclaringType.Name; //繼承class
            string currentClass = info.ReflectedType.Name; //當前class
            return inheritClass != currentClass;
        }

        public static int GetSplitLength(string pArray, char pSplit = ',')
        {
            int val = 0;
            if (!string.IsNullOrEmpty(pArray))
            {
                val = pArray.Split(pSplit).Length;
            }
            return val;
        }

        public static object GetObjectValue(object pObject, string pField)
        {
            object val = null;
            PropertyInfo Info = pObject.GetType().GetProperty(pField);
            if (Info != null)
                if (Info.GetValue(pObject) != null)
                {
                    val = Info.GetValue(pObject);
                }
            return val;
        }

        public static string GetDisplayName(object pObject, string pField)
        {
            PropertyInfo Info = pObject.GetType().GetProperty(pField);

            object[] attributes = Info.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                var displayName = (DisplayAttribute)attributes[0];
                return displayName.Name;
            }
            else
            {
                return pField;
            }
        }

        public static string GetDisplayName(PropertyInfo Info)
        {
            object[] attributes = Info.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                var displayName = (DisplayAttribute)attributes[0];
                return displayName.Name;
            }
            else
            {
                return Info.Name;
            }
        }

        public static string GetEnumDisplayName(Enum enumValue)
        {
            string rtValue = enumValue.ToString();
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString());
            if (memberInfo.Count() > 0)
            {
                rtValue = memberInfo.First().GetCustomAttribute<DisplayAttribute>().GetName();
            }
            return rtValue;
        }

        public static bool CheckInfoCanSetValue(object pObject, string infoName)
        {
            PropertyInfo info = pObject.GetType().GetProperty(infoName);

            if (info == null) { return false; }
            if (info.GetValue(pObject) == null) { return false; }
            if (!info.CanWrite || !info.CanRead) { return false; }

            return true;
        }

        public static T ModelParse<T>(object pObject) where T : class, new()
        {
            T model = new T();
            PropertyInfo[] propertyInfos = model.GetType().GetProperties();

            foreach (var info in propertyInfos)
            {
                PropertyInfo targetInfo = pObject.GetType().GetProperty(info.Name);

                if (CheckInfoCanSetValue(pObject, info.Name)) { info.SetValue(model, targetInfo.GetValue(pObject)); }
            }
            return model;
        }

        public static JObject ParseModelToJObject(object model)
        {
            JObject rtValue;
            string modelJsonString = JsonConvert.SerializeObject(model);
            rtValue = JsonConvert.DeserializeObject<JObject>(modelJsonString);

            return rtValue;
        }

        public static string GetKey<T>(T obj)
        {
            string result = "";
            PropertyInfo[] props = obj.GetType().GetProperties();
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

        public static string GetTableAttribute<T>(T obj)
        {
            PropertyInfo[] props = obj.GetType().GetProperties();
            object[] attributes = null;
            foreach (PropertyInfo prop in props)
            {
                attributes = prop.ReflectedType.GetCustomAttributes(typeof(Dapper.Contrib.Extensions.TableAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    return GetObjectValue(attributes[0], "Name").ToString();
                }
            }
            return "";
        }

        public static bool IsNumeric(string String)
        {
            try
            {
                decimal.Parse(String);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static int GetNumericOrDefault(string String, int defaultValue = 0)
        {
            try
            {
                return int.Parse(String);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string GetDateString(string dateTime, string format)
        {
            if (DateTime.TryParse(dateTime, out DateTime result))
            {
                return result.ToString(format);
            }

            return dateTime;
        }

        public static string ByteArrayToHexString(byte[] bytearray)
        {
            StringBuilder sb = new StringBuilder("0x");
            for (int i = 0; i < bytearray.Length; i++)
            {
                sb.Append(string.Format("{0:x2}", bytearray[i]));
            }
            return sb.ToString();
        }

        public static byte[] ZipData(Dictionary<string, byte[]> data)
        {
            using (var zipStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update))
                {
                    foreach (var fileName in data.Keys)
                    {
                        var entry = zipArchive.CreateEntry(fileName);
                        using (var entryStream = entry.Open())
                        {
                            var buff = data[fileName];
                            entryStream.Write(buff, 0, buff.Length);
                        }
                    }
                }
                return zipStream.ToArray();
            }
        }

        public static string GetRowValueByDataType(string dataType, object rowValue)
        {
            string value = "";
            switch (dataType)
            {
                case "System.Byte[]":
                    value = ByteArrayToHexString((byte[])rowValue).ToUpper();
                    break;
                case "System.DateTime":
                    value = "CAST(N'" + ((DateTime)rowValue).ToString("yyyy-MM-ddTHH:mm:ss") + "' AS DateTime2)";
                    break;
                default:
                    value = "N'" + rowValue.ToString().Replace("'", "''") + "'";
                    break;
            }
            return value;
        }

        public static string GenerateTableInsert(DataTable dt, string tableName)
        {
            string sqlAll = "";
            foreach (DataRow rV in dt.Rows)
            {
                string sqlField = "";
                string sqlValue = "";

                foreach (DataColumn col in dt.Columns)
                {
                    string colName = col.Caption;
                    string value = "";

                    if (!string.IsNullOrEmpty(sqlField)) { sqlField += ","; sqlValue += ","; }

                    if (rV.IsNull(colName)) { value = "NULL"; }
                    else { value = GetRowValueByDataType(col.DataType.FullName, rV[colName]); }

                    sqlField += "[" + colName + "]";
                    sqlValue += value;
                }
                sqlAll += "Insert " + tableName + "(" + sqlField + ") Values (" + sqlValue + ") \n";
            }
            return sqlAll;
        }

        public static string GetArrayStringField(string arrayString, int arrayIndex, char split = ',')
        {
            string result = "";
            string[] Array = arrayString.Split(split);

            if (Array.Length >= arrayIndex + 1)
            {
                result = Array[arrayIndex];
            }
            return result;
        }

        public static DateTime GetChDateTime(DateTime d)
        {
            try
            {
                return d.AddYears(-1911);
            }
            catch
            {
                return d;
            }
        }

        public static string GetDateTimeCHString(DateTime dateTime, bool IsConvertToChDateTime = false)
        {
            if (IsConvertToChDateTime) { dateTime = GetChDateTime(dateTime); }
            return dateTime.Year.ToString() + "年" + dateTime.Month.ToString() + "月" + dateTime.Day.ToString() + "日";
        }

        public static string GetDateTimeCHString(DateTime? nullableDateTime, bool IsConvertToChDateTime = false)
        {
            string result = "";
            if (nullableDateTime.HasValue)
            {
                DateTime dateTime = (DateTime)nullableDateTime;
                result = GetDateTimeCHString(dateTime, IsConvertToChDateTime);
            }
            return result;
        }

        public static string GetWeekDay(DayOfWeek dayOfWeek)
        {
            string WeekDay = "";
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    WeekDay = "星期一";
                    break;
                case DayOfWeek.Friday:
                    WeekDay = "星期五";
                    break;
                case DayOfWeek.Saturday:
                    WeekDay = "星期六";
                    break;
                case DayOfWeek.Sunday:
                    WeekDay = "星期日";
                    break;
                case DayOfWeek.Thursday:
                    WeekDay = "星期四";
                    break;
                case DayOfWeek.Tuesday:
                    WeekDay = "星期二";
                    break;
                case DayOfWeek.Wednesday:
                    WeekDay = "星期三";
                    break;
            }
            return WeekDay;
        }

        public static string StringFill(string paraString, int digits, string fillChar)
        {
            while (paraString.Length < digits) { paraString = fillChar + paraString; }
            return paraString;
        }

        public static List<Column> GetDataTableColumns<T>() where T : class
        {
            List<Column> columns = new List<Column>();
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();

            foreach (var info in propertyInfos)
            {
                DisplayAttribute displayAttribute = info.GetCustomAttribute<DisplayAttribute>();
                Column column = new Column
                {
                    data = info.Name,
                    title = displayAttribute != null ? displayAttribute.Name : info.Name,
                    visible = displayAttribute != null
                };
                columns.Add(column);
            }
            return columns;
        }

        public static string GetOrderSql<T>(BaseSearchViewModel searchModel) where T : class
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            int columnIndex = 0;
            string orderDir = "asc";

            if (searchModel?.order?.Count > 0)
            {
                DataTablesOrder firstOrder = searchModel.order[0];
                columnIndex = firstOrder.column;
                orderDir = firstOrder.dir;
            }

            string orderField = propertyInfos[columnIndex].Name;
            string sql = $@"order by {orderField} {orderDir}";

            return sql;
        }

        public static IEnumerable<SelectListItem> GetSelectListItem<T>(IEnumerable<T> list, bool displayName = false) where T : Enum
        {
            return list.Select(v => new SelectListItem
            {
                Text = displayName ? v.GetDisplayName() : v.ToString(),
                Value = Convert.ToInt32(v).ToString()
            });
        }

        public static IEnumerable<SelectListItem> GetSelectListItem<T>(bool displayName = false) where T : Enum
        {
            return GetSelectListItem(CommonUtility.GetEnumList<T>(), displayName);
        }

    }
}
