using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContractTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalWorkDays",
                table: "Payrolls");

            migrationBuilder.RenameColumn(
                name: "Allowance",
                table: "Contracts",
                newName: "Tax");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalHours",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Payrolls",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<decimal>(
                name: "OvertimeHours",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "NetSalary",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "GrossSalary",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualWorkingDays",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Allowance",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BasicSalary",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Bonus",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Deduction",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Holidays",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Insurance",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Payrolls",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidDate",
                table: "Payrolls",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalWorkingDaysInMonth",
                table: "Payrolls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "PayrollDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PayrollDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ReferenceId",
                table: "PayrollDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AllowanceLunchBreak",
                table: "Contracts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AllowancePark",
                table: "Contracts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalLeavingsPerMonth",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualWorkingDays",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "Allowance",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "BasicSalary",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "Bonus",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "Deduction",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "Holidays",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "Insurance",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "PaidDate",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "TotalWorkingDaysInMonth",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "AllowanceLunchBreak",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "AllowancePark",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "TotalLeavingsPerMonth",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "Tax",
                table: "Contracts",
                newName: "Allowance");

            migrationBuilder.AlterColumn<float>(
                name: "TotalHours",
                table: "Payrolls",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Payrolls",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "OvertimeHours",
                table: "Payrolls",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<float>(
                name: "NetSalary",
                table: "Payrolls",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<float>(
                name: "GrossSalary",
                table: "Payrolls",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<float>(
                name: "TotalWorkDays",
                table: "Payrolls",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "PayrollDetails",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
