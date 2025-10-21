using eCommerceMVC.Middleware;

namespace eCommerceMVC.Extensions
{
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandling(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // página detallada de errores
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // middleware personalizado
                app.UseErrorHandlingMiddleware();

                // Status code pages para errores HTTP
                app.UseStatusCodePagesWithReExecute("/Error/{0}");

                // HSTS para seguridad
                app.UseHsts();
            }

            return app;
        }
    }
}