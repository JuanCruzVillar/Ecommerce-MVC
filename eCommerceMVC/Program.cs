using eCommerce.Data;
using eCommerce.Repositories;
using eCommerce.Repositories.Implementations;
using eCommerce.Repositories.Interfaces;
using eCommerce.Services.Implementations;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<DbecommerceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EcommerceContext"))
);

// Repositories
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

builder.Services.AddHttpContextAccessor();

// CORREGIDO: Autenticación y autorización con manejo dinámico de login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;

        // MEJORADO: Manejo dinámico de rutas de login basado en el área
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
    });

builder.Services.AddAuthorization();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Rutas por áreas 
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// Ruta por defecto fuera de áreas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalogo}/{action=Index}/{id?}",
    defaults: new { area = "Negocio" }
);

// MEJORADO: Redireccionamiento inicial
app.MapGet("/", context =>
{
    context.Response.Redirect("/Negocio/Catalogo/Index");
    return Task.CompletedTask;
});

app.Run();