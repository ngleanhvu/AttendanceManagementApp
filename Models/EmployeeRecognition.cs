namespace AttendanceManagementApp.Models;

public class EmployeeRecognition: BaseEntity
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public float[] FaceEmbedding { get; set; }
}