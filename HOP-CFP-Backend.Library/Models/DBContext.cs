using HOP_CFP_Backend.Library.Models.Manager;
using HOP_CFP_Backend.Library.Models.System;
using HOP_CFP_Backend.Library.Utility;
using Microsoft.EntityFrameworkCore;


namespace HOP_CFP_Backend.Library.Models
{
    public class DBContext : DbContext
    {

        public DBContext(DbContextOptions options) : base(options)
        {
        }

        #region Manager
        public virtual DbSet<AdminMenu> AdminMenu { get; set; }
        public virtual DbSet<AdminFunction> AdminFunction { get; set; }
        public virtual DbSet<Log_ManagerLogin> Log_ManagerLogin { get; set; }
        public virtual DbSet<Log_ManagerLoginFail> Log_ManagerLoginFail { get; set; }
        public virtual DbSet<Manager.Manager> Manager { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Log_ManagerWatch> Log_ManagerWatch { get; set; }
        #endregion

        #region System
        public virtual DbSet<Log_BackendPageRequest> Log_BackendPageRequest { get; set; }
        public virtual DbSet<SysConfig> SysConfig { get; set; }
        public virtual DbSet<DataChange> DataChange { get; set; }
        #endregion

        public virtual DbSet<ManyToMany> ManyToMany { get; set; }
        public virtual DbSet<KeyValueSetting> KeyValueSetting { get; set; }

        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<Material> Material { get; set; }
        public virtual DbSet<MaterialGroup> MaterialGroup { get; set; }
        public virtual DbSet<MaterialNotify> MaterialNotify { get; set; }
        public virtual DbSet<MaterialCompare> MaterialCompare { get; set; }
        public virtual DbSet<MaterialSpec> MaterialSpec { get; set; }
        //

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log_ManagerLogin>(eb => { eb.HasNoKey(); });

            modelBuilder.Entity<Role>()
                 .HasIndex(b => b.UpdateDate);


            foreach (var tableType in CommentGenerator.GetTableTypes(this.GetType()))
            {
                if (CommentGenerator.NeedGenerateComment(tableType, out string tableComment))
                {
                    modelBuilder.Entity(tableType, eb => eb.HasComment(tableComment));
                }

                foreach (var field in tableType.GetProperties())
                {
                    if (CommentGenerator.NeedGenerateComment(field, out string comment))
                    {
                        modelBuilder.Entity(tableType, eb => eb.Property(field.Name).HasComment(comment));
                        continue;
                    }
                }
            }

            //尋覽DbSet裡面的Table
            foreach (var setProperty in this.GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType &&
                            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)))
            {
                var entityType = setProperty.PropertyType.GetGenericArguments()[0];

                // 為 Id 屬性設定 NEWID() 預設值 (假設是 GUID 類型)
                var idProperty = entityType.GetProperty("Id");
                if (idProperty != null && idProperty.PropertyType == typeof(Guid))
                {
                    modelBuilder.Entity(entityType)
                        .Property("Id")
                        .HasDefaultValueSql("NEWID()");
                }

                //在每個IdModelBase去覆寫OnModelCreating來設定額外的ModelBuilder設定(請參考ManyToMany)
                if (typeof(IdModelBase).IsAssignableFrom(entityType))
                {
                    try
                    {
                        var instance = Activator.CreateInstance(entityType) as IdModelBase;
                        if (instance != null)
                        {
                            instance.OnModelCreating(modelBuilder);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"無法建立 {entityType.Name} 的實例: {ex.Message}");
                    }
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
