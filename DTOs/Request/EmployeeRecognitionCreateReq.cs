namespace AttendanceManagementApp.DTOs.Request;

public class EmployeeRecognitionCreateReq
{
    public string? Email { get; set; }
    public float[] EmbeddingImage { get; set; }
    
}