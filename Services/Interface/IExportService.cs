namespace AttendanceManagementApp.Services.Interface;

public interface IExportService
{
    Task<byte[]> ExportPayrollPdfAsync(int payrollId);
}