using eCommerce.Data;
using eCommerce.Repositories.Implementations;
using eCommerce.Repositories.Interfaces;
using eCommerce.Services.Implementations;
using eCommerce.Services.Interfaces;
using eCommerceMVC.Extensions;
using eCommerceMVC.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. CONFIGURACIÓN DE VARIABLES DE ENTORNO
// ============================================
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// ============================================
// 2. LOGGING MEJORADO
// ============================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (builder.Environment.IsProduction())
{
    // En producción: solo warnings y errores
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}

// ============================================
// 3. SERVICIOS
// ============================================
builder.Services.AddControllersWithViews();

// DbContext con validación de connection string
var connectionString = builder.Configuration.GetConnectionString("EcommerceContext");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "⚠️ Connection string 'EcommerceContext' no encontrado. " +
        "Configuralo en appsettings.json o como variable de entorno.");
}

builder.Services.AddDbContext<DbecommerceContext>(options =>
{
    options.UseSqlServer(connectionString);

    // Solo en desarrollo: queries detalladas
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IMarcaRepository, MarcaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

// Services
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMarcaService, MarcaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();
builder.Services.AddScoped<IArmatuPcService, ArmatuPcService>();

builder.Services.AddHttpContextAccessor();

// ============================================
// 4. AUTENTICACIÓN Y SEGURIDAD
// ============================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsProduction()
            ? CookieSecurePolicy.Always
            : CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;

        options.Events.OnRedirectToLogin = context =>
        {
            var request = context.Request;
            var isAdminArea = request.Path.StartsWithSegments("/Admin");

            if (isAdminArea)
            {
                context.Response.Redirect("/Admin/Auth/Login");
            }
            else
            {
                context.Response.Redirect("/Negocio/Auth/Login");
            }
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.Redirect("/Error/AccessDenied");
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

// ============================================
// 5. SESSION
// ============================================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = builder.Environment.IsProduction()
        ? CookieSecurePolicy.Always
        : CookieSecurePolicy.SameAsRequest;
});

// ============================================
// 6. BUILD APP
// ============================================
var app = builder.Build();

// ============================================
// 7. MIDDLEWARE PIPELINE (ORDEN IMPORTANTE)
// ============================================

// Manejo de excepciones personalizado
app.UseCustomExceptionHandling(app.Environment);

// Status code pages para errores HTTP (404, 500, etc)
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ============================================
// 8. RUTAS
// ============================================

// Rutas de áreas (Admin, Negocio)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalogo}/{action=Index}/{id?}",
    defaults: new { area = "Negocio" }
);

// Redirección de raíz
app.MapGet("/", context =>
{
    context.Response.Redirect("/Negocio/Catalogo/Index");
    return Task.CompletedTask;
});

// ============================================
// 9. HEALTH CHECK (útil para deployment)
// ============================================
app.MapGet("/health", async (DbecommerceContext db) =>
{
    try
    {
        await db.Database.CanConnectAsync();
        return Results.Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            environment = app.Environment.EnvironmentName
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 503,
            title: "Database connection failed"
        );
    }
});

// ============================================
// 10. LOG DE INICIO
// ============================================
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("========================================");
logger.LogInformation("🚀 Hardware Store eCommerce iniciado");
logger.LogInformation("📊 Ambiente: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("🔌 Base de datos: {HasConnection}", !string.IsNullOrEmpty(connectionString) ? "Configurada" : "NO configurada");
logger.LogInformation("🌐 URLs: {Urls}", string.Join(", ", builder.WebHost.GetSetting("urls")?.Split(';') ?? new[] { "default" }));
logger.LogInformation("========================================");

app.Run();