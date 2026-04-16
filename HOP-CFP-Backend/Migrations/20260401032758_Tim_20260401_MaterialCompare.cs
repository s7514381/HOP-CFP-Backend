using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HDP_CFP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Tim_20260401_MaterialCompare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaxID",
                table: "Manager",
                type: "nvarchar(max)",
                nullable: true,
                comment: "統編");

            migrationBuilder.CreateTable(
                name: "MaterialCompare",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaterialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "供應商料號"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialCompare", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialCompare");

            migrationBuilder.DropColumn(
                name: "TaxID",
                table: "Manager");
        }
    }
}
