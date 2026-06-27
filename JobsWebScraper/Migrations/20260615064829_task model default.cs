using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManager.Migrations
{
    /// <inheritdoc />
    public partial class taskmodeldefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MyTask",
                columns: new[] { "Id", "Description", "State", "Title" },
                values: new object[,]
                {
                    { 1, "Head First Design Patterns", 0, "Read Book" },
                    { 2, "Cracking the coding Interview", 0, "Read Book" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MyTask",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MyTask",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
