using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HDP_CFP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Tim_20260416_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MaterialNumber",
                table: "Material",
                type: "nvarchar(450)",
                nullable: true,
                comment: "料號",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "料號");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialNotify_MaterialId",
                table: "MaterialNotify",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_MaterialNumber",
                table: "Material",
                column: "MaterialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Material_SupplierId",
                table: "Material",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaterialNotify_MaterialId",
                table: "MaterialNotify");

            migrationBuilder.DropIndex(
                name: "IX_Material_MaterialNumber",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_Material_SupplierId",
                table: "Material");

            migrationBuilder.AlterColumn<string>(
                name: "MaterialNumber",
                table: "Material",
                type: "nvarchar(max)",
                nullable: true,
                comment: "料號",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true,
                oldComment: "料號");
        }
    }
}
