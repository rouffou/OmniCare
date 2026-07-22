using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniCare.Modules.Practitioners.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "practitioners");

            migrationBuilder.CreateTable(
                name: "Cabinets",
                schema: "practitioners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    BceNumber = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    AddressLine = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    RegisteredOn = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cabinets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Practitioners",
                schema: "practitioners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Profession = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    InamiNumber = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    CabinetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RegisteredOn = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Practitioners", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cabinets_BceNumber",
                schema: "practitioners",
                table: "Cabinets",
                column: "BceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Practitioners_CabinetId",
                schema: "practitioners",
                table: "Practitioners",
                column: "CabinetId");

            migrationBuilder.CreateIndex(
                name: "IX_Practitioners_InamiNumber",
                schema: "practitioners",
                table: "Practitioners",
                column: "InamiNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cabinets",
                schema: "practitioners");

            migrationBuilder.DropTable(
                name: "Practitioners",
                schema: "practitioners");
        }
    }
}
