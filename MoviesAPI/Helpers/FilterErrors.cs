using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MoviesAPI.Helpers
{
    public class FilterErrors : ExceptionFilterAttribute
    {
        private readonly ILogger<FilterErrors> logger;

        public FilterErrors(ILogger<FilterErrors> logger)
        {
            this.logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
