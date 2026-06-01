using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class ModifiyEmployeeRecognitionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "EmployeeRecognitions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "EmployeeRecognitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
