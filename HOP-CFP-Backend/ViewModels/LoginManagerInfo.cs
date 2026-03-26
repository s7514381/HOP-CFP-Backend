using System;
using System.Collections.Generic;
using HOP_CFP_Backend.Library.Models;

namespace HOP_CFP_Backend.ViewModels
{
    public class LoginManagerInfo
    {
        public Guid ManagerId { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public EStatus Status { get; set; }
        public DateTime LastPasswordChangeDate { get; set; }
        public Guid? CompanyId { get; set; }
    }

    /// <summary>
    /// 儲存於 Session 的使用者資料格式
    /// </summary>
    public class SessionManagerInfo
    {
        /// <summary>
        /// 使用者 Id
        /// <para>注意!! 若使用者正在觀察模式，則此變數會儲存觀察對象的 Id，若要保證取得原帳號 Id，請使用 RealManagerId。</para>
        /// </summary>
        public Guid ManagerId { get; set; }

        /// <summary>
        /// 使用者名稱
        /// <para>注意!! 若使用者正在觀察模式，則此變數會儲存觀察對象的 Name，若要保證取得原帳號 Name，請使用 RealName。</para>
        /// </summary>
        public string Name { get; set; }

        //public List<AdminMenuByRoleViewModel> RoleMenuList { get; set; }
        
        //public List<AdminMenuViewModel> AdminMenuList { get; set; }
        
        public DateTime? LastApplyRoleDate { get; set; }

        public DateTime LastPasswordChangeDate { get; set; }

        #region 觀察模式
        /// <summary>
        /// 是否正在觀察模式
        /// </summary>
        public bool Watching
        {
            get
            {
                return NullableRealManagerId.HasValue || !string.IsNullOrEmpty(NullableRealName);
            }
        }

        /// <summary>
        /// 觀察模式時用於儲存真實 ManagerId
        /// </summary>
        public Guid? NullableRealManagerId { get; set; }

        /// <summary>
        /// 觀察模式時用於儲存真實 Name
        /// </summary>
        public string NullableRealName { get; set; }

        /// <summary>
        /// 保證取得原帳號 ManagerId
        /// </summary>
        public Guid RealManagerId
        {
            get
            {
                return NullableRealManagerId ?? ManagerId;
            }
        }

        /// <summary>
        /// 保證取得原帳號 Name
        /// </summary>
        public string RealName
        {
            get
            {
                return NullableRealName ?? Name;
            }
        }
        #endregion

        /// <summary>
        /// 取得屬性變更過後的 SessionManagerInfo，方便覆寫
        /// </summary>
        /// <param name="propertyName">要變更的屬性名稱</param>
        /// <param name="value">要變更的值</param>
        /// <returns>新的 SessionManagerInfo 物件</returns>
        public SessionManagerInfo UpdateSessionManagerInfo(string propertyName, object value)
        {
            var result = MemberwiseClone() as SessionManagerInfo;
            var property = typeof(SessionManagerInfo).GetProperty(propertyName);
            if (property == null)
            {
                throw new ArgumentException($"SessionManagerInfo 類別找不到欄位 {propertyName}", "propertyName");
            }

            property.SetValue(result, value);
            return result;
        }

    }
}