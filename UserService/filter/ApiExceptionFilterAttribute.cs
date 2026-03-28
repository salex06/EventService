using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ClientService.exception;

namespace ClientService.filter
{
    [AttributeUsage(AttributeTargets.All)]
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {

        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            var response = new ErrorResponse();

            switch (exception)
            {
                case NotFoundException notFound:
                    response.StatusCode = notFound.StatusCode;
                    response.Message = notFound.Message;
                    break;

                case BadRequestException badRequest:
                    response.StatusCode = badRequest.StatusCode;
                    response.Message = badRequest.Message;
                    break;

                default:
                    response.StatusCode = 500;
                    response.Message = $"Внутренняя ошибка сервера: {exception.Message}";
                    break;
            }

            context.HttpContext.Response.StatusCode = response.StatusCode;
            context.HttpContext.Response.ContentType = "application/json";

            context.Result = new JsonResult(response);
            context.ExceptionHandled = true;
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
