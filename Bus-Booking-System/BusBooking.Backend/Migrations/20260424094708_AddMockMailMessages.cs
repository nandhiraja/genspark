using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusBooking.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMockMailMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MockMailMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ToEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Subject = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    Body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ParentMessageId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockMailMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MockMailMessages_MockMailMessages_ParentMessageId",
                        column: x => x.ParentMessageId,
                        principalTable: "MockMailMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MockMailMessages_ParentMessageId",
                table: "MockMailMessages",
                column: "ParentMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MockMailMessages_ToEmail_CreatedAt",
                table: "MockMailMessages",
                columns: new[] { "ToEmail", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MockMailMessages_ToEmail_IsRead",
                table: "MockMailMessages",
                columns: new[] { "ToEmail", "IsRead" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MockMailMessages");
        }
    }
}
