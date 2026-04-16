using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HDP_CFP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Tim_20260402 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AlterColumn<string>(
                name: "TaxID",
                table: "Manager",
                type: "nvarchar(450)",
                nullable: true,
                comment: "統編",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "統編");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialCompare_MaterialNumber",
                table: "MaterialCompare",
                column: "MaterialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Manager_TaxID",
                table: "Manager",
                column: "TaxID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaterialCompare_MaterialNumber",
                table: "MaterialCompare");

            migrationBuilder.DropIndex(
                name: "IX_Manager_TaxID",
                table: "Manager");

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

            migrationBuilder.AlterColumn<string>(
                name: "TaxID",
                table: "Manager",
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
