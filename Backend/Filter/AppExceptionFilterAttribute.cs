using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Filter
{
    public class AppExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<AppExceptionFilterAttribute> _logger;

        public AppExceptionFilterAttribute(
            ILogger<AppExceptionFilterAttribute> logger
            )
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.StackTrace);

            context.HttpContext.Response.StatusCode = 500;

            context.Result = new JsonResult(new Dictionary<string, object>
            {
                { "Error" , context.Exception.Message },
                { "StackTrace", context.Exception.StackTrace }
            });

            // base.OnException(context);
        }
    }
}
