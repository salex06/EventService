using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MS_Lab.exception;

namespace MS_Lab.filter
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var response = new BadRequestException("Переданы некорректные данные");

                context.Result = new BadRequestObjectResult(response);
            }
        }
    }
}
