using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HDP_CFP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Tim_20260319 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaterialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "料號"),
                    ProductModel = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "產品型號"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "產品名稱"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "群組代號"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "群組名稱"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialNotify",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsSend = table.Column<bool>(type: "bit", nullable: false, comment: "是否發送"),
                    IsUpdate = table.Column<bool>(type: "bit", nullable: false, comment: "是否更新資料"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialNotify", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "名稱"),
                    TaxID = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "統編"),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "聯絡窗口"),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "聯絡電話"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Email"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Material");

            migrationBuilder.DropTable(
                name: "MaterialGroup");

            migrationBuilder.DropTable(
                name: "MaterialNotify");

            migrationBuilder.DropTable(
                name: "Supplier");
        }
    }
}
