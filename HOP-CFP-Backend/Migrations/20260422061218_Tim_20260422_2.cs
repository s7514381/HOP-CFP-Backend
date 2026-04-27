using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HDP_CFP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Tim_20260422_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaterialNumber",
                table: "MaterialCompare");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "MaterialCompare");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaterialNumber",
                table: "MaterialCompare",
                type: "nvarchar(max)",
                nullable: true,
                comment: "供應商料號");

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "MaterialCompare",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
