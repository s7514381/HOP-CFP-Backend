using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HDP_CFP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Tim_20260401 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TaxID",
                table: "Supplier",
                type: "nvarchar(450)",
                nullable: true,
                comment: "統編",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "統編");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_TaxID",
                table: "Supplier",
                column: "TaxID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialCompare_MaterialId",
                table: "MaterialCompare",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialCompare_SupplierId",
                table: "MaterialCompare",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Supplier_TaxID",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_MaterialCompare_MaterialId",
                table: "MaterialCompare");

            migrationBuilder.DropIndex(
                name: "IX_MaterialCompare_SupplierId",
                table: "MaterialCompare");

            migrationBuilder.AlterColumn<string>(
                name: "TaxID",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: true,
                comment: "統編",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true,
                oldComment: "統編");
        }
    }
}
