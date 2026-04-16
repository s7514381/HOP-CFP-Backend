using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HDP_CFP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Tim_MaterialSpec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialSpec",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SpecNumber = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "規格編號"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "名稱"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新時間"),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改人員"),
                    Status = table.Column<short>(type: "smallint", nullable: true, comment: "狀態"),
                    Sequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialSpec", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialSpec_MaterialId",
                table: "MaterialSpec",
                column: "MaterialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialSpec");
        }
    }
}
