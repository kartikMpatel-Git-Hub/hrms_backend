using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using static hrms.CustomException.BaseCustomException;

namespace hrms.CustomException
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "An internal server error occurred.";

            if (context.Exception is NotFoundException notFoundException)
            {
                statusCode = notFoundException.StatusCode;
                message = notFoundException.Message;
            }
            else if (context.Exception is InvalidOperationCustomException invalidOperationException)
            {
                statusCode = invalidOperationException.StatusCode;
                message = invalidOperationException.Message;
            }
            _logger.LogError(context.Exception, "An unhandled exception occurred: {Message}", context.Exception.Message);

            var result = new ObjectResult(new
            {
                error = new { message = message, details = context.Exception.Message }
            })
            {
                StatusCode = (int)statusCode
            };

            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}
