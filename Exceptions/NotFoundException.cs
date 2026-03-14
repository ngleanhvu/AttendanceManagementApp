namespace AttendanceManagementApp.Exception
{
    public class NotFoundException : IOException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
