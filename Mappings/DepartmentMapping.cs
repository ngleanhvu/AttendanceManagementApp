using AttendanceManagementApp.DTOs.Response;

namespace AttendanceManagementApp.Mappings
{
    public class DepartmentMapping
    {
        public DepartmentRes ToDepartmentRes(Models.Department department)
        {
            if (department == null) return null;
            return new DepartmentRes
            {
                Name = department.Name,
                Description = department.Description
            };
        }
    }
}
