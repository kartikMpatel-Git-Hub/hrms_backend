using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

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
            Console.WriteLine("EXCEPTION HERE : " + context.Exception);

            if(context.Exception is AggregateException agr)
            {
                var flat = agr.Flatten();
                if(flat.InnerExceptions.Count == 1)
                {
                    context.Exception = flat.InnerExceptions[0];
                }
                else
                {
                    context.Exception = flat;
                }
            }

            if (context.Exception is NotFoundCustomException notFoundException)
            {
                statusCode = notFoundException.StatusCode;
                message = notFoundException.Message;
            }
            else if (context.Exception is InvalidOperationCustomException invalidOperationException)
            {
                statusCode = invalidOperationException.StatusCode;
                message = invalidOperationException.Message;
            }
            else if (context.Exception is ExistsCustomException existsCustomException)
            {
                statusCode = existsCustomException.StatusCode;
                message = existsCustomException.Message;
            }
            else if (context.Exception is UnauthorizedCustomException unauthorizedCustom)
            {
                statusCode = unauthorizedCustom.StatusCode;
                message = unauthorizedCustom.Message;
            }
            _logger.LogError(context.Exception, "An unhandled exception occurred: {Message}", context.Exception.Message);

            var result = new ObjectResult(new
            {
                error = new { details = message }
            })
            {
                StatusCode = (int)statusCode
            };

            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}
