using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HDP_CFP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Tim_20260504 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminMenuByRole");

            migrationBuilder.AddColumn<bool>(
                name: "CanSell",
                table: "Material",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanSell",
                table: "Material");

            migrationBuilder.CreateTable(
                name: "AdminMenuByRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ActionFunctionAssembly = table.Column<int>(type: "int", nullable: false),
                    AdminMenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminMenuByRole", x => x.Id);
                });
        }
    }
}
