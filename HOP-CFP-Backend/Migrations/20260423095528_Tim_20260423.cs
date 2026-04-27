using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HDP_CFP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Tim_20260423 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Role",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ActionFunctionSN",
                table: "AdminFunction",
                type: "int",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Role");

            migrationBuilder.AlterColumn<short>(
                name: "ActionFunctionSN",
                table: "AdminFunction",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
