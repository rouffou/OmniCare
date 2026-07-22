using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniCare.Modules.Patients.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientPortalUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PortalUserId",
                schema: "patients",
                table: "Patients",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PortalUserId",
                schema: "patients",
                table: "Patients",
                column: "PortalUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_PortalUserId",
                schema: "patients",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PortalUserId",
                schema: "patients",
                table: "Patients");
        }
    }
}
