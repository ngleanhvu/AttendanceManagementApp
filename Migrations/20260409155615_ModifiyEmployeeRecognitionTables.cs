using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class ModifiyEmployeeRecognitionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FaceEmbedding",
                table: "EmployeeRecognitions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "FaceEmbedding",
                table: "EmployeeRecognitions",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
