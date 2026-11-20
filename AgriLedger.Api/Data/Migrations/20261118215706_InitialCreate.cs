using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriLedger.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SaleType = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    EggSize = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    CaseQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    PricePerCase = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PaymentStatus = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Supplier = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DocumentNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ExpenseCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "Id", "Code", "CreatedAt", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "FEED", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Ζωοτροφές", 1 },
                    { 2, "ELECTRICITY", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Ρεύμα / Ενέργεια", 2 },
                    { 3, "PAYROLL", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Μισθοδοσία", 3 },
                    { 4, "PURCHASES", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Αγορές πουλάδων / εξοπλισμού", 4 },
                    { 5, "LOGISTICS", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Καύσιμα / Μεταφορικά", 5 },
                    { 6, "OTHER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Λοιπά", 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_Code",
                table: "ExpenseCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CategoryId_Date",
                table: "Expenses",
                columns: new[] { "CategoryId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Date",
                table: "Expenses",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_Date",
                table: "Sales",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PaymentStatus_Date",
                table: "Sales",
                columns: new[] { "PaymentStatus", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");
        }
    }
}
