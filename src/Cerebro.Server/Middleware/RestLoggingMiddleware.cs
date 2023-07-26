namespace Cerebro.Server.Middleware
{
    public class RestLoggingMiddleware
    {
        private readonly ILogger<RestLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public RestLoggingMiddleware(ILogger<RestLoggingMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var isFromFirstParty = httpContext.Request.Headers["x-called-by"].ToString();
            if (isFromFirstParty != "")
                _logger.LogInformation($"HTTP Request: {isFromFirstParty} {httpContext.Request.Method} '{httpContext.Request.Path}' is called");
            else
                _logger.LogInformation($"HTTP Request: {httpContext.Request.Method} '{httpContext.Request.Path}' is called");

            // Call the next middleware delegate in the pipeline 
            await _next.Invoke(httpContext);
        }
    }
}
