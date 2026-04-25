using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusBooking.Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixShadowProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "BusId1",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "OperatorId1",
                table: "Buses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
