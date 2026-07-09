using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobsWebScraper.Migrations
{
    /// <inheritdoc />
    public partial class linkfieldaddedtojobmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Job",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Job",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Link",
                table: "Job");

            migrationBuilder.InsertData(
                table: "Job",
                columns: new[] { "Id", "City", "Company", "Department", "InterviewRound", "Region", "Status", "Title" },
                values: new object[] { 1, "dfs", "EPAM", "asd", 1, "sda", 0, "Software Developer" });
        }
    }
}
