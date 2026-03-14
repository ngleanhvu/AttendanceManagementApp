using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Mappings
{
    public class PositionMapping
    {
        public PositionRes ToPositionRes(Position position)
        {
            return new PositionRes
            {
                Id = position.Id,
                Name = position.Name,
                Description = position.Description
            };
        }
    }
}
