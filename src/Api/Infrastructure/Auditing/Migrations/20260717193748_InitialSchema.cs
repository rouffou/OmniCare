using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniCare.Api.Infrastructure.Auditing.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditTrail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RequestType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TargetId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OccurredOn = table.Column<long>(type: "INTEGER", nullable: false),
                    Succeeded = table.Column<bool>(type: "INTEGER", nullable: false),
                    FailureReason = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrail", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrail_OccurredOn",
                table: "AuditTrail",
                column: "OccurredOn");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrail_TargetId",
                table: "AuditTrail",
                column: "TargetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrail");
        }
    }
}
