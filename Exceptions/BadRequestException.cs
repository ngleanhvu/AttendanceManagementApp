namespace AttendanceManagementApp.Exception
{
    public class BadRequestException : IOException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
