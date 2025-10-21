using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace eCommerceMVC.Middleware
{
    
    /// Middleware para manejo de errores
    
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log del error
            _logger.LogError(exception,
                "Error no controlado: {Message}. Path: {Path}",
                exception.Message,
                context.Request.Path);

            // Determinar el código de estado
            var statusCode = exception switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            // Si es una petición AJAX, devolver JSON
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                        ? exception.Message
                        : "Ha ocurrido un error. Por favor, intente nuevamente."
                });
                await context.Response.WriteAsync(result);
            }
            else
            {
                // Redirigir a página de error
                context.Response.Redirect($"/Error?statusCode={context.Response.StatusCode}");
            }
        }
    }

    
    // Extension method para registrar el middleware
    
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}