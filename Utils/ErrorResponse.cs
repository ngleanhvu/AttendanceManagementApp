namespace AttendanceManagementApp.Utils
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; }
    }
}
