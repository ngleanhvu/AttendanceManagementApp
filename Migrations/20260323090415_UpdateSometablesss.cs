using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSometablesss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignedBy",
                table: "Contracts");

            migrationBuilder.AddColumn<int>(
                name: "LeaveRequestType",
                table: "LeaveRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaveRequestType",
                table: "LeaveRequests");

            migrationBuilder.AddColumn<string>(
                name: "SignedBy",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
