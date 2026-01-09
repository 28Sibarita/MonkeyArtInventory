using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonkeyArtInventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClientFieldsToMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "Movements",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ClientPrice",
                table: "Movements",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConsignment",
                table: "Movements",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ClientNote",
                table: "Movements",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "ClientPrice",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "IsConsignment",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "ClientNote",
                table: "Movements");
        }
    }
}
