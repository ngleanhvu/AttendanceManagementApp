namespace AttendanceManagementApp.DTOs.Response
{
    public class FullPayrollDetailRes
    {
        public PayrollRes Payroll {  get; set; }
        public List<PayrollDetailRes> PayrollList { get; set;}
    }
}
