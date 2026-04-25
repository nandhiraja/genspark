using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusBooking.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:booking_status", "pending,confirmed,cancelled")
                .Annotation("Npgsql:Enum:bus_status", "active,inactive,pending_approval")
                .Annotation("Npgsql:Enum:operator_status", "pending,approved,rejected")
                .Annotation("Npgsql:Enum:payment_status", "initiated,success,failed")
                .Annotation("Npgsql:Enum:user_role", "user,operator,admin")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,confirmed,cancelled")
                .OldAnnotation("Npgsql:Enum:bus_status", "active,inactive")
                .OldAnnotation("Npgsql:Enum:operator_status", "pending,approved,rejected")
                .OldAnnotation("Npgsql:Enum:payment_status", "initiated,success,failed")
                .OldAnnotation("Npgsql:Enum:user_role", "user,operator,admin");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BusId1",
                table: "Seats",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OperatorId1",
                table: "Buses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_BusId1",
                table: "Seats",
                column: "BusId1");

            migrationBuilder.CreateIndex(
                name: "IX_Buses_OperatorId1",
                table: "Buses",
                column: "OperatorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Buses_Operators_OperatorId1",
                table: "Buses",
                column: "OperatorId1",
                principalTable: "Operators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Buses_BusId1",
                table: "Seats",
                column: "BusId1",
                principalTable: "Buses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buses_Operators_OperatorId1",
                table: "Buses");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Buses_BusId1",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_BusId1",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Buses_OperatorId1",
                table: "Buses");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "BusId1",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "OperatorId1",
                table: "Buses");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:booking_status", "pending,confirmed,cancelled")
                .Annotation("Npgsql:Enum:bus_status", "active,inactive")
                .Annotation("Npgsql:Enum:operator_status", "pending,approved,rejected")
                .Annotation("Npgsql:Enum:payment_status", "initiated,success,failed")
                .Annotation("Npgsql:Enum:user_role", "user,operator,admin")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,confirmed,cancelled")
                .OldAnnotation("Npgsql:Enum:bus_status", "active,inactive,pending_approval")
                .OldAnnotation("Npgsql:Enum:operator_status", "pending,approved,rejected")
                .OldAnnotation("Npgsql:Enum:payment_status", "initiated,success,failed")
                .OldAnnotation("Npgsql:Enum:user_role", "user,operator,admin");
        }
    }
}
