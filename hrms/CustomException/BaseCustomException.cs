using System.Net;

namespace hrms.CustomException
{
    public class BaseCustomException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        protected BaseCustomException(String message,HttpStatusCode statusCode) : base(message) 
        {
            this.StatusCode = statusCode;
        }
    }
    public class NotFoundCustomException : BaseCustomException
    {
        public NotFoundCustomException(String message) : base(message, HttpStatusCode.BadRequest) { }
    }

    public class InvalidOperationCustomException : BaseCustomException
    {
        public InvalidOperationCustomException(String message) : base(message, HttpStatusCode.BadRequest) { }
    }
    public class BadRequestCustomException : BaseCustomException
    {
        public BadRequestCustomException(String message) : base(message, HttpStatusCode.BadRequest) { }
    }
    public class ExistsCustomException : BaseCustomException
    {
        public ExistsCustomException(String message) : base(message, HttpStatusCode.BadRequest) { }
    }

    public class UnauthorizedCustomException : BaseCustomException
    {
        public UnauthorizedCustomException(string message) : base(message, HttpStatusCode.Unauthorized) { }
    }
}
