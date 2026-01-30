using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDitari.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Baseline migration – database already exists
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No rollback for baseline
        }
    }
}
