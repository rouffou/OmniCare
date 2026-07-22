using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniCare.Modules.Patients.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicalDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClinicalDocuments",
                schema: "patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClinicalRecordId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    StorageKey = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    UploadedByPractitionerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UploadedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicalDocuments_ClinicalRecords_ClinicalRecordId",
                        column: x => x.ClinicalRecordId,
                        principalSchema: "patients",
                        principalTable: "ClinicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalDocuments_ClinicalRecordId",
                schema: "patients",
                table: "ClinicalDocuments",
                column: "ClinicalRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClinicalDocuments",
                schema: "patients");
        }
    }
}
