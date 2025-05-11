using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpacePirates.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCelestialObjectStarOOP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StarId",
                table: "SolarSystems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "X",
                table: "Planets",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Y",
                table: "Planets",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Stars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    X = table.Column<double>(type: "double", nullable: false),
                    Y = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stars", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SolarSystems_StarId",
                table: "SolarSystems",
                column: "StarId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolarSystems_Stars_StarId",
                table: "SolarSystems",
                column: "StarId",
                principalTable: "Stars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolarSystems_Stars_StarId",
                table: "SolarSystems");

            migrationBuilder.DropTable(
                name: "Stars");

            migrationBuilder.DropIndex(
                name: "IX_SolarSystems_StarId",
                table: "SolarSystems");

            migrationBuilder.DropColumn(
                name: "StarId",
                table: "SolarSystems");

            migrationBuilder.DropColumn(
                name: "X",
                table: "Planets");

            migrationBuilder.DropColumn(
                name: "Y",
                table: "Planets");
        }
    }
}
