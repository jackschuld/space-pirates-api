using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpacePirates.API.Migrations
{
    /// <inheritdoc />
    public partial class SeparateComponentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentCargoLoad",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "CurrentFuel",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "CurrentHullIntegrity",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "CurrentShieldCapacity",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "DefenseLevel",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "EngineLevel",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "FuelEfficiency",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "Maneuverability",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "MaxCargoCapacity",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "MaxFuelCapacity",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "MaxHullIntegrity",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "MaxShieldCapacity",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "MaxSpeed",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "WeaponLevel",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "X",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "Y",
                table: "Ships");

            migrationBuilder.CreateTable(
                name: "CargoSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CurrentLoad = table.Column<int>(type: "int", nullable: false),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false),
                    MaxLevel = table.Column<int>(type: "int", nullable: false),
                    LastUpgraded = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ShipId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoSystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargoSystems_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Engines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false),
                    MaxLevel = table.Column<int>(type: "int", nullable: false),
                    LastUpgraded = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ShipId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Engines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Engines_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FuelSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CurrentFuel = table.Column<int>(type: "int", nullable: false),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false),
                    MaxLevel = table.Column<int>(type: "int", nullable: false),
                    LastUpgraded = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ShipId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelSystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelSystems_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Hulls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CurrentIntegrity = table.Column<int>(type: "int", nullable: false),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false),
                    MaxLevel = table.Column<int>(type: "int", nullable: false),
                    LastUpgraded = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ShipId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hulls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hulls_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    X = table.Column<double>(type: "double", nullable: false),
                    Y = table.Column<double>(type: "double", nullable: false),
                    ShipId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Shields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CurrentIntegrity = table.Column<int>(type: "int", nullable: false),
                    LastRechargeTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false),
                    MaxLevel = table.Column<int>(type: "int", nullable: false),
                    LastUpgraded = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ShipId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shields_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WeaponSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false),
                    MaxLevel = table.Column<int>(type: "int", nullable: false),
                    LastUpgraded = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ShipId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponSystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeaponSystems_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CargoSystems_ShipId",
                table: "CargoSystems",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Engines_ShipId",
                table: "Engines",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuelSystems_ShipId",
                table: "FuelSystems",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hulls_ShipId",
                table: "Hulls",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_ShipId",
                table: "Positions",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shields_ShipId",
                table: "Shields",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeaponSystems_ShipId",
                table: "WeaponSystems",
                column: "ShipId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CargoSystems");

            migrationBuilder.DropTable(
                name: "Engines");

            migrationBuilder.DropTable(
                name: "FuelSystems");

            migrationBuilder.DropTable(
                name: "Hulls");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Shields");

            migrationBuilder.DropTable(
                name: "WeaponSystems");

            migrationBuilder.AddColumn<int>(
                name: "CurrentCargoLoad",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentFuel",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentHullIntegrity",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentShieldCapacity",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefenseLevel",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EngineLevel",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "FuelEfficiency",
                table: "Ships",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Maneuverability",
                table: "Ships",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "MaxCargoCapacity",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxFuelCapacity",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxHullIntegrity",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxShieldCapacity",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "MaxSpeed",
                table: "Ships",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "WeaponLevel",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "X",
                table: "Ships",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Y",
                table: "Ships",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
