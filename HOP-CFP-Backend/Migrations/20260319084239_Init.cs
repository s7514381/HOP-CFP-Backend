using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HDP_CFP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminFunction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Controller = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionFunctionSN = table.Column<short>(type: "smallint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminFunction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminMenu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminFunctionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IconClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminMenu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminMenuByRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AdminMenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionFunctionAssembly = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminMenuByRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataChange",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ModelClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrimaryKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Action = table.Column<short>(type: "smallint", nullable: false, comment: "(0)新增 (1)編輯 (2)刪除"),
                    DifferenceJSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataChange", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeyValueSetting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Type = table.Column<int>(type: "int", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Group = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyValueSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Log_BackendPageRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueryString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoadTime = table.Column<long>(type: "bigint", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log_BackendPageRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Log_ManagerLogin",
                columns: table => new
                {
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActionType = table.Column<short>(type: "smallint", nullable: false, comment: "(1)登入 (2)登出"),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()")
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Log_ManagerLoginFail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log_ManagerLoginFail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Log_ManagerWatch",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WatchingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log_ManagerWatch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "帳號"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Email"),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true, comment: "密碼"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "使用者名稱"),
                    EmailConfirm = table.Column<bool>(type: "bit", nullable: false),
                    PauseDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "停權時間"),
                    LastPasswordChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manager", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManyToMany",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    SourceTable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetTable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelationType = table.Column<int>(type: "int", nullable: true),
                    Params = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManyToMany", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "角色名稱"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Value = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysConfig", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeyValueSetting_Type_Group_Key",
                table: "KeyValueSetting",
                columns: new[] { "Type", "Group", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_ManyToMany_SourceId_TargetTable_RelationType",
                table: "ManyToMany",
                columns: new[] { "SourceId", "TargetTable", "RelationType" });

            migrationBuilder.CreateIndex(
                name: "IX_ManyToMany_TargetId",
                table: "ManyToMany",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_UpdateDate",
                table: "Role",
                column: "UpdateDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminFunction");

            migrationBuilder.DropTable(
                name: "AdminMenu");

            migrationBuilder.DropTable(
                name: "AdminMenuByRole");

            migrationBuilder.DropTable(
                name: "DataChange");

            migrationBuilder.DropTable(
                name: "KeyValueSetting");

            migrationBuilder.DropTable(
                name: "Log_BackendPageRequest");

            migrationBuilder.DropTable(
                name: "Log_ManagerLogin");

            migrationBuilder.DropTable(
                name: "Log_ManagerLoginFail");

            migrationBuilder.DropTable(
                name: "Log_ManagerWatch");

            migrationBuilder.DropTable(
                name: "Manager");

            migrationBuilder.DropTable(
                name: "ManyToMany");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "SysConfig");
        }
    }
}
