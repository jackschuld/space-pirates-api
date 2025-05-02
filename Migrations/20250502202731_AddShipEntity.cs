using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpacePirates.API.Migrations
{
    /// <inheritdoc />
    public partial class AddShipEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CaptainName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    X = table.Column<double>(type: "double", nullable: false),
                    Y = table.Column<double>(type: "double", nullable: false),
                    MaxHullIntegrity = table.Column<int>(type: "int", nullable: false),
                    CurrentHullIntegrity = table.Column<int>(type: "int", nullable: false),
                    MaxShieldCapacity = table.Column<int>(type: "int", nullable: false),
                    CurrentShieldCapacity = table.Column<int>(type: "int", nullable: false),
                    EngineLevel = table.Column<int>(type: "int", nullable: false),
                    MaxSpeed = table.Column<double>(type: "double", nullable: false),
                    Maneuverability = table.Column<double>(type: "double", nullable: false),
                    MaxFuelCapacity = table.Column<int>(type: "int", nullable: false),
                    CurrentFuel = table.Column<int>(type: "int", nullable: false),
                    FuelEfficiency = table.Column<double>(type: "double", nullable: false),
                    MaxCargoCapacity = table.Column<int>(type: "int", nullable: false),
                    CurrentCargoLoad = table.Column<int>(type: "int", nullable: false),
                    WeaponLevel = table.Column<int>(type: "int", nullable: false),
                    DefenseLevel = table.Column<int>(type: "int", nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    LastDocked = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsInCombat = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_Name",
                table: "Ships",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ships");
        }
    }
}
