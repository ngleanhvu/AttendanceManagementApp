namespace AttendanceManagementApp.Middlewares
{
    using AttendanceManagementApp.Exception;
    using AttendanceManagementApp.Exceptions;
    using AttendanceManagementApp.Utils;
    using System.Net;
    using System.Text.Json;

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (IOException ex)
            {
                await HandleException(context, ex);
            }
            }

        private async Task HandleException(HttpContext context, IOException ex)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string errorCode;
            string message;

            switch (ex)
            {
                case AppException appEx:
                    statusCode = appEx.StatusCode;
                    errorCode = appEx.ErrorCode;
                    message = appEx.Message;
                    break;

                case NotFoundException:
                    statusCode = 404;
                    errorCode = "NOT_FOUND";
                    message = ex.Message;
                    break;

                case UnauthorizedException:
                    statusCode = 401;
                    errorCode = "UNAUTHORIZED";
                    message = ex.Message;
                    break;

                case BadRequestException:
                    statusCode = 400;
                    errorCode = "BAD_REQUEST";
                    message = ex.Message;
                    break;

                default:
                    statusCode = 500;
                    errorCode = "INTERNAL_SERVER_ERROR";
                    message = "Đã xảy ra lỗi hệ thống";
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new ErrorResponse
            {
                Message = message,
                ErrorCode = errorCode,
                StatusCode = statusCode
            };

            var apiResponse = ApiResponse<ErrorResponse>.ErrorResponse(message, response);

            var json = JsonSerializer.Serialize(apiResponse);

            await context.Response.WriteAsync(json);
        }
    }
}
