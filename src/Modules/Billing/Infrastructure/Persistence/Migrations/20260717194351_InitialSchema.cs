using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniCare.Modules.Billing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "billing");

            migrationBuilder.CreateTable(
                name: "Invoices",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PractitionerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InamiCode = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    InsuredAtIssue = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    IssuedOn = table.Column<long>(type: "INTEGER", nullable: false),
                    PaidOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CancellationReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PatientId",
                schema: "billing",
                table: "Invoices",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PractitionerId_IssuedOn",
                schema: "billing",
                table: "Invoices",
                columns: new[] { "PractitionerId", "IssuedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Status",
                schema: "billing",
                table: "Invoices",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices",
                schema: "billing");
        }
    }
}
