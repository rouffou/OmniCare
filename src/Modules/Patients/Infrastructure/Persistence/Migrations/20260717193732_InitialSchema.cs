using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniCare.Modules.Patients.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "patients");

            migrationBuilder.CreateTable(
                name: "ClinicalRecords",
                schema: "patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Profession = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    OpenedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                schema: "patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Niss = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Contact_Email = table.Column<string>(type: "TEXT", nullable: true),
                    Contact_Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Contact_AddressLine = table.Column<string>(type: "TEXT", nullable: true),
                    Contact_PostalCode = table.Column<string>(type: "TEXT", nullable: true),
                    Contact_City = table.Column<string>(type: "TEXT", nullable: true),
                    Mutuality_MutualityCode = table.Column<string>(type: "TEXT", nullable: true),
                    Mutuality_MemberNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Mutuality_HasPreferentialRate = table.Column<bool>(type: "INTEGER", nullable: true),
                    InsurabilityState = table.Column<int>(type: "INTEGER", nullable: false),
                    InsurabilityCheckedOn = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    TreatingPhysicianName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EmergencyContact = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ReferentPractitionerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RegisteredOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClinicalEntries",
                schema: "patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClinicalRecordId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorPractitionerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecordedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicalEntries_ClinicalRecords_ClinicalRecordId",
                        column: x => x.ClinicalRecordId,
                        principalSchema: "patients",
                        principalTable: "ClinicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                schema: "patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClinicalRecordId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PrescriberName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PrescribedOn = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    SessionsPrescribed = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionsConsumed = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescriptions_ClinicalRecords_ClinicalRecordId",
                        column: x => x.ClinicalRecordId,
                        principalSchema: "patients",
                        principalTable: "ClinicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consents",
                schema: "patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    GrantedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    RevokedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consents_Patients_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "patients",
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalEntries_ClinicalRecordId",
                schema: "patients",
                table: "ClinicalEntries",
                column: "ClinicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalRecords_PatientId_Profession",
                schema: "patients",
                table: "ClinicalRecords",
                columns: new[] { "PatientId", "Profession" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consents_PatientId",
                schema: "patients",
                table: "Consents",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Niss",
                schema: "patients",
                table: "Patients",
                column: "Niss",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_ClinicalRecordId",
                schema: "patients",
                table: "Prescriptions",
                column: "ClinicalRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClinicalEntries",
                schema: "patients");

            migrationBuilder.DropTable(
                name: "Consents",
                schema: "patients");

            migrationBuilder.DropTable(
                name: "Prescriptions",
                schema: "patients");

            migrationBuilder.DropTable(
                name: "Patients",
                schema: "patients");

            migrationBuilder.DropTable(
                name: "ClinicalRecords",
                schema: "patients");
        }
    }
}
