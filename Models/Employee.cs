using System.Diagnostics.Contracts;

namespace AttendanceManagementApp.Models
{
    public class Employee: BaseEntity
    {
        public string Code { get; set; }

        public string Fullname { get; set; }

        public bool Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; }

        public Department Department { get; set; }

        public Position Position { get; set; }

        public ICollection<Contract> Contracts { get; set; }

        public ICollection<Attendance> Attendances { get; set; }

        public ICollection<EmployeeShift> EmployeeShifts { get; set; }
    }
}
