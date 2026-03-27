using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDitari.Migrations
{
    /// <inheritdoc />
    public partial class FixMissingStudentParent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Table already exists in database, so skipping creation.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
