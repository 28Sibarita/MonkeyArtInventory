using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonkeyArtInventory.Data.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Type = table.Column<int>(type: "INTEGER", nullable: false),
                Barcode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                StockMinimo = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ImagePath = table.Column<string>(type: "TEXT", maxLength: 260, nullable: true),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Movements",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Type = table.Column<int>(type: "INTEGER", nullable: false),
                OccurredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Movements", x => x.Id);
                table.ForeignKey(
                    name: "FK_Movements_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Movements_ProductId",
            table: "Movements",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_Products_Barcode",
            table: "Products",
            column: "Barcode",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Movements");

        migrationBuilder.DropTable(
            name: "Products");
    }
}
