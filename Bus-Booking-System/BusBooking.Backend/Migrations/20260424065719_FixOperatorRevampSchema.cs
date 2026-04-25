using System;
using BusBooking.Backend.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusBooking.Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixOperatorRevampSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:booking_status", "pending,confirmed,cancelled")
                .Annotation("Npgsql:Enum:bus_status", "active,inactive,pending_approval")
                .Annotation("Npgsql:Enum:operator_status", "pending,approved,rejected")
                .Annotation("Npgsql:Enum:payment_status", "initiated,success,failed")
                .Annotation("Npgsql:Enum:request_status", "pending,approved,rejected")
                .Annotation("Npgsql:Enum:user_role", "user,operator,admin")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,confirmed,cancelled")
                .OldAnnotation("Npgsql:Enum:bus_status", "active,inactive,pending_approval")
                .OldAnnotation("Npgsql:Enum:operator_status", "pending,approved,rejected")
                .OldAnnotation("Npgsql:Enum:payment_status", "initiated,success,failed")
                .OldAnnotation("Npgsql:Enum:user_role", "user,operator,admin");

            migrationBuilder.AddColumn<bool>(
                name: "AdminDeactivated",
                table: "Buses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationBoardingPointId",
                table: "Buses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HiddenFromSearch",
                table: "Buses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceBoardingPointId",
                table: "Buses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusReactivationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusId = table.Column<Guid>(type: "uuid", nullable: false),
                    OperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    OperatorReason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    AdminReason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Status = table.Column<RequestStatus>(type: "request_status", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusReactivationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusReactivationRequests_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusReactivationRequests_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusRouteChangeRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusId = table.Column<Guid>(type: "uuid", nullable: false),
                    OperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldRouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewRouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldSourceBoardingPointId = table.Column<Guid>(type: "uuid", nullable: true),
                    OldDestinationBoardingPointId = table.Column<Guid>(type: "uuid", nullable: true),
                    NewSourceBoardingPointId = table.Column<Guid>(type: "uuid", nullable: true),
                    NewDestinationBoardingPointId = table.Column<Guid>(type: "uuid", nullable: true),
                    OperatorReason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    AdminReason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Status = table.Column<RequestStatus>(type: "request_status", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusRouteChangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusRouteChangeRequests_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusRouteChangeRequests_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperatorBoardingPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    AddressLine = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorBoardingPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperatorBoardingPoints_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperatorBoardingPoints_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buses_DestinationBoardingPointId",
                table: "Buses",
                column: "DestinationBoardingPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Buses_SourceBoardingPointId",
                table: "Buses",
                column: "SourceBoardingPointId");

            migrationBuilder.CreateIndex(
                name: "IX_BusReactivationRequests_BusId_Status",
                table: "BusReactivationRequests",
                columns: new[] { "BusId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BusReactivationRequests_OperatorId",
                table: "BusReactivationRequests",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_BusRouteChangeRequests_BusId_Status",
                table: "BusRouteChangeRequests",
                columns: new[] { "BusId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BusRouteChangeRequests_OperatorId",
                table: "BusRouteChangeRequests",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name",
                table: "Cities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperatorBoardingPoints_CityId",
                table: "OperatorBoardingPoints",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_OperatorBoardingPoints_OperatorId_CityId_Label",
                table: "OperatorBoardingPoints",
                columns: new[] { "OperatorId", "CityId", "Label" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Buses_OperatorBoardingPoints_DestinationBoardingPointId",
                table: "Buses",
                column: "DestinationBoardingPointId",
                principalTable: "OperatorBoardingPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Buses_OperatorBoardingPoints_SourceBoardingPointId",
                table: "Buses",
                column: "SourceBoardingPointId",
                principalTable: "OperatorBoardingPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buses_OperatorBoardingPoints_DestinationBoardingPointId",
                table: "Buses");

            migrationBuilder.DropForeignKey(
                name: "FK_Buses_OperatorBoardingPoints_SourceBoardingPointId",
                table: "Buses");

            migrationBuilder.DropTable(
                name: "BusReactivationRequests");

            migrationBuilder.DropTable(
                name: "BusRouteChangeRequests");

            migrationBuilder.DropTable(
                name: "OperatorBoardingPoints");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Buses_DestinationBoardingPointId",
                table: "Buses");

            migrationBuilder.DropIndex(
                name: "IX_Buses_SourceBoardingPointId",
                table: "Buses");

            migrationBuilder.DropColumn(
                name: "AdminDeactivated",
                table: "Buses");

            migrationBuilder.DropColumn(
                name: "DestinationBoardingPointId",
                table: "Buses");

            migrationBuilder.DropColumn(
                name: "HiddenFromSearch",
                table: "Buses");

            migrationBuilder.DropColumn(
                name: "SourceBoardingPointId",
                table: "Buses");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:booking_status", "pending,confirmed,cancelled")
                .Annotation("Npgsql:Enum:bus_status", "active,inactive,pending_approval")
                .Annotation("Npgsql:Enum:operator_status", "pending,approved,rejected")
                .Annotation("Npgsql:Enum:payment_status", "initiated,success,failed")
                .Annotation("Npgsql:Enum:user_role", "user,operator,admin")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,confirmed,cancelled")
                .OldAnnotation("Npgsql:Enum:bus_status", "active,inactive,pending_approval")
                .OldAnnotation("Npgsql:Enum:operator_status", "pending,approved,rejected")
                .OldAnnotation("Npgsql:Enum:payment_status", "initiated,success,failed")
                .OldAnnotation("Npgsql:Enum:request_status", "pending,approved,rejected")
                .OldAnnotation("Npgsql:Enum:user_role", "user,operator,admin");
        }
    }
}
