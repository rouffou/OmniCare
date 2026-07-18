using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniCare.Modules.Billing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddActCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActCatalogEntries",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Profession = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    InamiCode = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    Label = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DefaultTariff = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActCatalogEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActCatalogEntries_Profession_InamiCode",
                schema: "billing",
                table: "ActCatalogEntries",
                columns: new[] { "Profession", "InamiCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActCatalogEntries",
                schema: "billing");
        }
    }
}
