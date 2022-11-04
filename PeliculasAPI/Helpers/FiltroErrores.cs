using Microsoft.AspNetCore.Mvc.Filters;

namespace PeliculasAPI.Helpers
{
    public class FiltroErrores : ExceptionFilterAttribute
    {
        private readonly ILogger<FiltroErrores> _logger;

        public FiltroErrores(ILogger<FiltroErrores> logger)
        {
            _logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
