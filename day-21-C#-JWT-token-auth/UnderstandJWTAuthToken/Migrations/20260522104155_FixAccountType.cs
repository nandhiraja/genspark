using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnderstandJWTAuthToken.Migrations
{
    /// <inheritdoc />
    public partial class FixAccountType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountNumber",
                keyValue: "000999887711");

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountNumber", "AccountType", "Balance", "CustomerId", "Status" },
                values: new object[] { "000999887711", "Account", 12343.4f, 101, "Active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountNumber",
                keyValue: "000999887711");

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountNumber", "AccountType", "Balance", "CustomerId", "Status" },
                values: new object[] { "000999887711", "", 12343.4f, 101, "Active" });
        }
    }
}
