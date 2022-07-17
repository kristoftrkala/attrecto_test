using Newtonsoft.Json;
using Serilog;

namespace Attrecto.Exceptions
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate requestDelegate;

        public ExceptionHandler(RequestDelegate requestDelegate)
        {
            this.requestDelegate = requestDelegate;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await requestDelegate.Invoke(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            Log.Error(e, e.Message);
            context.Response.ContentType = "application/json";
            if (e is ArgumentException || e is InvalidOperationException)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiErrorDetails
                {
                    StatusCode = 400,
                    ErrorMessage = "Bad Request",
                    ErrorDetail = e.Message + " | " + e?.InnerException?.Message
                }));
            }
            if (e is UnauthorizedAccessException)
            {
                context.Response.StatusCode = 403;
                return context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiErrorDetails
                {
                    StatusCode = 401,
                    ErrorMessage = "Unauthorized operation",
                    ErrorDetail = e.Message + " | " + e?.InnerException?.Message
                }));
            }
            if (e is KeyNotFoundException)
            {
                context.Response.StatusCode = 404;
                return context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiErrorDetails
                {
                    StatusCode = 404,
                    ErrorMessage = "Not found",
                    ErrorDetail = e.Message + " | " + e?.InnerException?.Message
                }));
            }
            if (e is NotImplementedException)
            {
                context.Response.StatusCode = 501;
                return context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiErrorDetails
                {
                    StatusCode = 501,
                    ErrorMessage = "Not Implemented",
                    ErrorDetail = e.Message + " | " + e?.InnerException?.Message
                }));
            }
            context.Response.StatusCode = 500;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiErrorDetails
            {
                StatusCode = 500,
                ErrorMessage = "Internal Server Error",
                ErrorDetail = e.Message + " | " + e?.InnerException?.Message
            }));
        }
    }
}
