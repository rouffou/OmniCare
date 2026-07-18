using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniCare.Modules.Agenda.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                schema: "agenda",
                table: "Appointments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rooms",
                schema: "agenda",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_RoomId",
                schema: "agenda",
                table: "Appointments",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Name",
                schema: "agenda",
                table: "Rooms",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Rooms_RoomId",
                schema: "agenda",
                table: "Appointments",
                column: "RoomId",
                principalSchema: "agenda",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Rooms_RoomId",
                schema: "agenda",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "Rooms",
                schema: "agenda");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_RoomId",
                schema: "agenda",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "RoomId",
                schema: "agenda",
                table: "Appointments");
        }
    }
}
