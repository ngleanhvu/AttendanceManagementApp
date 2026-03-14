namespace AttendanceManagementApp.Middlewares
{
    using AttendanceManagementApp.Exception;
    using AttendanceManagementApp.Utils;
    using System.Net;
    using System.Text.Json;

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, IOException exception)
        {
            HttpStatusCode status;
            string errorCode;

            switch (exception)
            {
                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    errorCode = "NOT_FOUND";
                    break;

                case BadRequestException:
                    status = HttpStatusCode.BadRequest;
                    errorCode = "BAD_REQUEST";
                    break;

                case UnauthorizedException:
                    status = HttpStatusCode.Unauthorized;
                    errorCode = "UNAUTHORIZED";
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    errorCode = "INTERNAL_SERVER_ERROR";
                    break;
            }

            var response = new ErrorResponse
            {
                Message = exception.Message,
                StatusCode = (int)status,
                ErrorCode = errorCode
            };

            var json = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(json);
        }
    }
}
