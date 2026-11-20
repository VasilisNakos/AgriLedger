using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriLedger.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Productions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CaseQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productions_Date",
                table: "Productions",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Productions");
        }
    }
}
