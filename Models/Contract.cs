namespace AttendanceManagementApp.Models
{
    public class Contract: BaseEntity
    {
        public string ContractType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal BaseSalary { get; set; }

        public Employee Employee { get; set; }
    }
}
