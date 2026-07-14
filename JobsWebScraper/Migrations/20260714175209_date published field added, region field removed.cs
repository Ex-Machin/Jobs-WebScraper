using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobsWebScraper.Migrations
{
    /// <inheritdoc />
    public partial class datepublishedfieldaddedregionfieldremoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterviewRound",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Job");

            migrationBuilder.AddColumn<DateTime>(
                name: "DatePublished",
                table: "Job",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatePublished",
                table: "Job");

            migrationBuilder.AddColumn<int>(
                name: "InterviewRound",
                table: "Job",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Job",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Job",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
