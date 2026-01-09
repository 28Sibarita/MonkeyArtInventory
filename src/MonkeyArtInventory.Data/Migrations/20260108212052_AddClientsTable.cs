using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonkeyArtInventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClientsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Movements",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Destination",
                table: "Movements",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ContactPhone = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ContactEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsConsignmentClient = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DefaultDiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movements_ClientId",
                table: "Movements",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Name",
                table: "Clients",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Clients_ClientId",
                table: "Movements",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Clients_ClientId",
                table: "Movements");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Movements_ClientId",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "Movements");
        }
    }
}
