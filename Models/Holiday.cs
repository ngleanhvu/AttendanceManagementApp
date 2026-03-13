using Microsoft.Identity.Client;

namespace AttendanceManagementApp.Models
{
    public class Holiday: BaseEntity
    {
        public string Name { get; set; }

        public int Month { get; set; }

        public string Description { get; set; }
    }
}
