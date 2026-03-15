using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSomeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AllowedEarlyLeaveMinutes",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AllowedLateMinutes",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "BreakEndTime",
                table: "Shifts",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "BreakStartTime",
                table: "Shifts",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Shifts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Shifts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOvernight",
                table: "Shifts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ShiftType",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ToDate",
                table: "EmployeeShifts",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FromDate",
                table: "EmployeeShifts",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "EmployeeShifts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "AssignedBy",
                table: "EmployeeShifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "EmployeeShifts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "EmployeeShifts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedEarlyLeaveMinutes",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "AllowedLateMinutes",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "BreakEndTime",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "BreakStartTime",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "IsOvernight",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "ShiftType",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "EmployeeShifts");

            migrationBuilder.DropColumn(
                name: "AssignedBy",
                table: "EmployeeShifts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "EmployeeShifts");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "EmployeeShifts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ToDate",
                table: "EmployeeShifts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FromDate",
                table: "EmployeeShifts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
