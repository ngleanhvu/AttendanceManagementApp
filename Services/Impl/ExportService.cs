using AttendanceManagementApp.Configs;
using AttendanceManagementApp.Exception;
using AttendanceManagementApp.Services.Interface;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;

namespace AttendanceManagementApp.Services.Impl;

public class ExportService : IExportService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ExportService(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<byte[]> ExportPayrollPdfAsync(int payrollId)
    {
        var payroll = await _context.Payrolls
            .Include(p => p.Employee)
                .ThenInclude(e => e.EmployeeDetail)
                    .ThenInclude(d => d.Department)
            .Include(p => p.Employee)
                .ThenInclude(e => e.EmployeeDetail)
                    .ThenInclude(d => d.Position)
            .Include(p => p.Employee.Contracts)
            .Include(p => p.PayrollDetails)
            .FirstOrDefaultAsync(p => p.Id == payrollId);

        if (payroll == null)
            throw new NotFoundException("Không tìm thấy bảng lương");

        var emp = payroll.Employee;
        var detail = emp.EmployeeDetail;
        var contract = emp.Contracts
            .OrderByDescending(c => c.StartDate)
            .FirstOrDefault();

        var leaveDays = payroll.TotalWorkingDaysInMonth - payroll.ActualWorkingDays;

        using var ms = new MemoryStream();
        var writer = new PdfWriter(ms);
        var pdf = new PdfDocument(writer);
        var doc = new Document(pdf);

        // ===== FONT (TIẾNG VIỆT) =====
        var fontPath = Path.Combine(_env.ContentRootPath, "Resources", "Fonts", "DejaVuSans.ttf");
        var boldFontPath = Path.Combine(_env.ContentRootPath, "Resources", "Fonts", "DejaVuSans-Bold.ttf");

        var normalFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
        var boldFont = PdfFontFactory.CreateFont(boldFontPath, PdfEncodings.IDENTITY_H);

        // ===== HEADER =====
        doc.Add(new Paragraph("PHIẾU LƯƠNG (PAYSLIP)")
            .SetFont(boldFont)
            .SetFontSize(18)
            .SetTextAlignment(TextAlignment.CENTER));

        doc.Add(new Paragraph($"Kỳ lương: Tháng {payroll.Month}/{payroll.Year}")
            .SetFont(normalFont)
            .SetTextAlignment(TextAlignment.CENTER));

        doc.Add(new Paragraph("\n"));

        // ===== EMPLOYEE =====
        doc.Add(Title("THÔNG TIN NHÂN VIÊN", boldFont));

        var empTable = NewTable(4);
        AddCell(empTable, "Mã nhân viên", emp.Code, boldFont, normalFont);
        AddCell(empTable, "Họ và tên", emp.Fullname, boldFont, normalFont);
        AddCell(empTable, "Email", emp.Email, boldFont, normalFont);
        AddCell(empTable, "Phòng ban", detail?.Department?.Name, boldFont, normalFont);
        AddCell(empTable, "Chức vụ", detail?.Position?.Name, boldFont, normalFont);
        AddCell(empTable, "Ngày vào làm", detail?.HireDate.ToString("dd/MM/yyyy"), boldFont, normalFont);
        AddCell(empTable, "Số điện thoại", detail?.Phone, boldFont, normalFont);

        doc.Add(empTable);
        doc.Add(Spacer());

        // ===== WORK =====
        doc.Add(Title("THÔNG TIN CHẤM CÔNG", boldFont));

        var workTable = NewTable(4);
        AddCell(workTable, "Ngày công chuẩn", payroll.TotalWorkingDaysInMonth.ToString(), boldFont, normalFont);
        AddCell(workTable, "Ngày công thực tế", payroll.ActualWorkingDays.ToString(), boldFont, normalFont);
        AddCell(workTable, "Ngày nghỉ", leaveDays.ToString(), boldFont, normalFont);
        AddCell(workTable, "Tổng giờ làm", payroll.TotalHours.ToString(), boldFont, normalFont);
        AddCell(workTable, "Giờ làm thêm", payroll.OvertimeHours.ToString(), boldFont, normalFont);

        doc.Add(workTable);
        doc.Add(Spacer());

        // ===== CONTRACT =====
        doc.Add(Title("THÔNG TIN HỢP ĐỒNG", boldFont));

        var contractTable = NewTable(4);
        AddCell(contractTable, "Lương cơ bản", FormatCurrency(contract?.BaseSalary), boldFont, normalFont, true);
        AddCell(contractTable, "Phụ cấp ăn trưa", FormatCurrency(contract?.AllowanceLunchBreak), boldFont, normalFont, true);
        AddCell(contractTable, "Phụ cấp gửi xe", FormatCurrency(contract?.AllowancePark), boldFont, normalFont, true);
        AddCell(contractTable, "Hệ số làm thêm", contract?.OverTimeRate.ToString(), boldFont, normalFont);
        AddCell(contractTable, "Ngày phép/tháng", contract?.TotalLeavingsPerMonth.ToString(), boldFont, normalFont);

        doc.Add(contractTable);
        doc.Add(Spacer());

        // ===== SALARY =====
        doc.Add(Title("THU NHẬP & KHẤU TRỪ", boldFont));

        var salaryTable = NewTable(2);
        AddCell(salaryTable, "Lương cơ bản", FormatCurrency(payroll.BasicSalary), boldFont, normalFont, true);
        AddCell(salaryTable, "Phụ cấp", FormatCurrency(payroll.Allowance), boldFont, normalFont, true);
        AddCell(salaryTable, "Thưởng", FormatCurrency(payroll.Bonus), boldFont, normalFont, true);
        AddCell(salaryTable, "Khấu trừ", FormatCurrency(payroll.Deduction), boldFont, normalFont, true);
        AddCell(salaryTable, "Thuế TNCN", FormatCurrency(payroll.Tax), boldFont, normalFont, true);
        AddCell(salaryTable, "Bảo hiểm", FormatCurrency(payroll.Insurance), boldFont, normalFont, true);

        doc.Add(salaryTable);
        doc.Add(Spacer());

        // ===== DETAIL =====
        doc.Add(Title("CHI TIẾT PHÁT SINH", boldFont));

        var detailTable = NewTable(3);

        detailTable.AddHeaderCell(HeaderCell("Loại", boldFont));
        detailTable.AddHeaderCell(HeaderCell("Mô tả", boldFont));
        detailTable.AddHeaderCell(HeaderCell("Số tiền", boldFont));

        foreach (var d in payroll.PayrollDetails)
        {
            detailTable.AddCell(new Paragraph(d.Type.ToString()).SetFont(normalFont));
            detailTable.AddCell(new Paragraph(d.Description ?? "").SetFont(normalFont));
            detailTable.AddCell(new Paragraph(FormatCurrency(d.Amount))
                .SetFont(normalFont)
                .SetTextAlignment(TextAlignment.RIGHT));
        }

        doc.Add(detailTable);
        doc.Add(Spacer());

        // ===== TOTAL =====
        doc.Add(Title("TỔNG KẾT", boldFont));

        var totalTable = NewTable(2);
        AddCell(totalTable, "Tổng thu nhập (Gross)", FormatCurrency(payroll.GrossSalary), boldFont, normalFont, true);
        AddCell(totalTable, "Thực nhận (Net)", FormatCurrency(payroll.NetSalary), boldFont, normalFont, true);

        doc.Add(totalTable);

        doc.Add(new Paragraph("\n"));

        // ===== SIGN =====
        var signTable = NewTable(2);

        signTable.AddCell(new Paragraph("Phòng nhân sự\n\n\n(Ký, ghi rõ họ tên)").SetFont(normalFont));
        signTable.AddCell(new Paragraph("Nhân viên\n\n\n(Ký, ghi rõ họ tên)").SetFont(normalFont));

        doc.Add(signTable);

        // ===== FOOTER =====
        doc.Add(new Paragraph("\nGhi chú: Phiếu lương được tạo tự động từ hệ thống.")
            .SetFont(normalFont)
            .SetFontSize(9));

        doc.Close();
        return ms.ToArray();
    }

    // ===== HELPER =====

    private Table NewTable(int cols)
        => new Table(cols).UseAllAvailableWidth();

    private Paragraph Title(string text, PdfFont font)
        => new Paragraph(text).SetFont(font).SetFontSize(12);

    private Paragraph Spacer()
        => new Paragraph("\n");

    private Cell HeaderCell(string text, PdfFont font)
        => new Cell().Add(new Paragraph(text).SetFont(font));

    private void AddCell(Table table, string label, string? value, PdfFont bold, PdfFont normal, bool isMoney = false)
    {
        table.AddCell(new Cell().Add(new Paragraph(label).SetFont(bold)));

        var p = new Paragraph(value ?? "").SetFont(normal);
        if (isMoney)
            p.SetTextAlignment(TextAlignment.RIGHT);

        table.AddCell(new Cell().Add(p));
    }

    private string FormatCurrency(decimal? amount)
    {
        if (amount == null) return "";
        return string.Format("{0:N0} VNĐ", amount);
    }
}