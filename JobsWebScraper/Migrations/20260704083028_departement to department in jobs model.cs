using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobsWebScraper.Migrations
{
    /// <inheritdoc />
    public partial class departementtodepartmentinjobsmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Departement",
                table: "Job",
                newName: "Department");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Department",
                table: "Job",
                newName: "Departement");
        }
    }
}
