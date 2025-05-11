using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpacePirates.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDiscoveredAndPlanetLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDiscovered",
                table: "Stars",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscovered",
                table: "Planets",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDiscovered",
                table: "Stars");

            migrationBuilder.DropColumn(
                name: "IsDiscovered",
                table: "Planets");
        }
    }
}
