using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HDP_CFP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Tim_20260422 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaterialCompare_MaterialNumber",
                table: "MaterialCompare");

            migrationBuilder.DropIndex(
                name: "IX_MaterialCompare_SupplierId",
                table: "MaterialCompare");

            migrationBuilder.AlterColumn<string>(
                name: "MaterialNumber",
                table: "MaterialCompare",
                type: "nvarchar(max)",
                nullable: true,
                comment: "供應商料號",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true,
                oldComment: "供應商料號");

            migrationBuilder.AddColumn<Guid>(
                name: "BuyerMaterialId",
                table: "MaterialCompare",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialCompare_BuyerMaterialId",
                table: "MaterialCompare",
                column: "BuyerMaterialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaterialCompare_BuyerMaterialId",
                table: "MaterialCompare");

            migrationBuilder.DropColumn(
                name: "BuyerMaterialId",
                table: "MaterialCompare");

            migrationBuilder.AlterColumn<string>(
                name: "MaterialNumber",
                table: "MaterialCompare",
                type: "nvarchar(450)",
                nullable: true,
                comment: "供應商料號",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "供應商料號");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialCompare_MaterialNumber",
                table: "MaterialCompare",
                column: "MaterialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialCompare_SupplierId",
                table: "MaterialCompare",
                column: "SupplierId");
        }
    }
}
