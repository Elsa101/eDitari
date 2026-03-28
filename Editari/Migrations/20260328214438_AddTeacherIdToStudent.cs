using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDitari.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherIdToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "Students",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Students");
        }
    }
}
