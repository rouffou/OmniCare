using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniCare.Modules.Billing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThirdPartyPayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MutualityShare",
                schema: "billing",
                table: "Invoices",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PatientShare",
                schema: "billing",
                table: "Invoices",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "PreferentialRateAtIssue",
                schema: "billing",
                table: "Invoices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ThirdPartyPayer",
                schema: "billing",
                table: "Invoices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MutualityShare",
                schema: "billing",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PatientShare",
                schema: "billing",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PreferentialRateAtIssue",
                schema: "billing",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ThirdPartyPayer",
                schema: "billing",
                table: "Invoices");
        }
    }
}
