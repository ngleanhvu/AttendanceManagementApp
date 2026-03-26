namespace AttendanceManagementApp.Exceptions
{
    public class AppException : IOException
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }

        public AppException(string message, string errorCode, int statusCode = 400)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
