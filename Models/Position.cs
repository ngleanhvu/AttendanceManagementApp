namespace AttendanceManagementApp.Models
{
    public class Position: BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
