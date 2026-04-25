using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusBooking.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddBusAdminDeactivationReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminDeactivationReason",
                table: "Buses",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminDeactivationReason",
                table: "Buses");
        }
    }
}
