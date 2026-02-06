using System.Net;

namespace hrms.CustomException
{
    public class BaseCustomException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        protected BaseCustomException(String message,HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
        }
    }
    public class NotFoundException : BaseCustomException
    {
        public NotFoundException(String message, HttpStatusCode statusCode) : base(message, statusCode) { }
    }

    public class InvalidOperationCustomException : BaseCustomException
    {
        public InvalidOperationCustomException(String message) : base(message, HttpStatusCode.BadRequest) { }
    }
    public class BadRequestCustomException : BaseCustomException
    {
        public BadRequestCustomException(String message) : base(message, HttpStatusCode.BadRequest) { }
    }
}
