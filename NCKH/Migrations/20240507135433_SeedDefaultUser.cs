using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NCKH.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AllUser",
                columns: new[] { "User_ID", "Dob", "Email", "Gender", "Password", "Phone", "Type", "UserName" },
                values: new object[] { 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@utc.com", "male", "admin", "0123456789", 0, "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AllUser",
                keyColumn: "User_ID",
                keyValue: 1);
        }
    }
}
