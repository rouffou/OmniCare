using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniCare.Modules.Agenda.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentReminderSentOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReminderSentOn",
                schema: "agenda",
                table: "Appointments",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderSentOn",
                schema: "agenda",
                table: "Appointments");
        }
    }
}
