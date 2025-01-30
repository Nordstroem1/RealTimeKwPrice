using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KiloWattPrice");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ElectricityPrices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Loggers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    WhatWentWrong = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Function = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loggers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElectricityPrices_UserId",
                table: "ElectricityPrices",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElectricityPrices_AspNetUsers_UserId",
                table: "ElectricityPrices",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElectricityPrices_AspNetUsers_UserId",
                table: "ElectricityPrices");

            migrationBuilder.DropTable(
                name: "Loggers");

            migrationBuilder.DropIndex(
                name: "IX_ElectricityPrices_UserId",
                table: "ElectricityPrices");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ElectricityPrices");

            migrationBuilder.CreateTable(
                name: "KiloWattPrice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    TimeEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KiloWattPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KiloWattPrice_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KiloWattPrice_UserId",
                table: "KiloWattPrice",
                column: "UserId");
        }
    }
}
