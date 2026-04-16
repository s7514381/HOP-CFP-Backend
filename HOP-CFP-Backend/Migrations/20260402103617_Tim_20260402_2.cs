using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HDP_CFP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Tim_20260402_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaterialId",
                table: "MaterialSpec",
                newName: "MaterialCompareId");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialSpec_MaterialId",
                table: "MaterialSpec",
                newName: "IX_MaterialSpec_MaterialCompareId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaterialCompareId",
                table: "MaterialSpec",
                newName: "MaterialId");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialSpec_MaterialCompareId",
                table: "MaterialSpec",
                newName: "IX_MaterialSpec_MaterialId");
        }
    }
}
