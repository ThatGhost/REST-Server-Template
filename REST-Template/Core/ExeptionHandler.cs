namespace BREADAPI.core
{
    public class ExeptionHandler
    {
        private readonly ILogger<ExeptionHandler> logger;
        private readonly RequestDelegate _next;

        public ExeptionHandler(ILogger<ExeptionHandler> logger, RequestDelegate next)
        {
            this.logger = logger;
            _next = next;   
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(UnauthorizedAccessException))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("{ \"message\": \"" + ex.Message + "\" }");
                }
                else if (ex.GetType() == typeof(NotFoundExeption))
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("{ \"message\": \"" + ex.Message + "\" }");
                }
                else if (ex.GetType() == typeof(BadRequestExeption))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("{ \"message\": \"" + ex.Message + "\" }");
                }
                else if (ex.GetType() == typeof(ForbiddenExeption))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("{ \"message\": \"" + ex.Message + "\" }");
                }
                else
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("{ \"message\": \"" + ex.Message + "\" }");
                }
            }
        }
    }
}
