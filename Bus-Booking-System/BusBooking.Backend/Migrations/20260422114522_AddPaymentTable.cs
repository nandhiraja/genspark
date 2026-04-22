using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusBooking.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:booking_status", "pending,confirmed,cancelled")
                .Annotation("Npgsql:Enum:bus_status", "active,inactive")
                .Annotation("Npgsql:Enum:operator_status", "pending,approved,rejected")
                .Annotation("Npgsql:Enum:payment_status", "initiated,success,failed")
                .Annotation("Npgsql:Enum:user_role", "user,operator,admin")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,confirmed,cancelled")
                .OldAnnotation("Npgsql:Enum:bus_status", "active,inactive")
                .OldAnnotation("Npgsql:Enum:operator_status", "pending,approved,rejected")
                .OldAnnotation("Npgsql:Enum:user_role", "user,operator,admin");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:booking_status", "pending,confirmed,cancelled")
                .Annotation("Npgsql:Enum:bus_status", "active,inactive")
                .Annotation("Npgsql:Enum:operator_status", "pending,approved,rejected")
                .Annotation("Npgsql:Enum:user_role", "user,operator,admin")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,confirmed,cancelled")
                .OldAnnotation("Npgsql:Enum:bus_status", "active,inactive")
                .OldAnnotation("Npgsql:Enum:operator_status", "pending,approved,rejected")
                .OldAnnotation("Npgsql:Enum:payment_status", "initiated,success,failed")
                .OldAnnotation("Npgsql:Enum:user_role", "user,operator,admin");
        }
    }
}
