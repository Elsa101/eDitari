using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eDitari.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Staff",
                columns: new[] { "StaffId", "ClassId", "Name", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, null, "Admin 1", "$2y$12$K7v156yY1WqF7A0kR8zUvOhVvG.sEqf5LHAyKgTBTuWbvku30Zc4jb", "Admin", "admin1@editari.com" },
                    { 2, null, "Admin 2", "$2y$12$K7v156yY1WqF7A0kR8zUvOhVvG.sEqf5LHAyKgTBTuWbvku30Zc4jb", "Admin", "admin2@editari.com" },
                    { 3, null, "Admin 3", "$2y$12$K7v156yY1WqF7A0kR8zUvOhVvG.sEqf5LHAyKgTBTuWbvku30Zc4jb", "Admin", "admin3@editari.com" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "StaffId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "StaffId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "StaffId",
                keyValue: 3);
        }
    }
}
