using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniCare.Modules.Agenda.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWaitlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaitlistEntries",
                schema: "agenda",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PractitionerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AppointmentTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RequestedFrom = table.Column<long>(type: "INTEGER", nullable: false),
                    RequestedTo = table.Column<long>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    JoinedOn = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitlistEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaitlistEntries_PractitionerId_Status_JoinedOn",
                schema: "agenda",
                table: "WaitlistEntries",
                columns: new[] { "PractitionerId", "Status", "JoinedOn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaitlistEntries",
                schema: "agenda");
        }
    }
}
