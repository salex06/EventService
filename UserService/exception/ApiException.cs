namespace ClientService.exception
{
    public abstract class ApiException : Exception
    {
        public int StatusCode { get; }

        protected ApiException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : ApiException
    {
        public NotFoundException(string message)
            : base(message, 404)
        {
        }
    }

    public class BadRequestException : ApiException
    {
        public BadRequestException(string message)
            : base(message, 400)
        {
        }
    }
}
