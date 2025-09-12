using eCommerce.Data;
using eCommerce.Repositories;
using eCommerce.Repositories.Implementations;
using eCommerce.Repositories.Interfaces;
using eCommerce.Services.Implementations;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
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

// <-- HttpContextAccessor necesario para Session -->
builder.Services.AddHttpContextAccessor();

// CarritoService (soporta usuario logueado o Session)
builder.Services.AddScoped<ICarritoService, CarritoService>();

// Autenticación y autorización usando scheme por defecto "Cookies"
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Negocio/Auth/Login"; // redirige al login de clientes
        options.AccessDeniedPath = "/Negocio/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

builder.Services.AddAuthorization();

// <-- Agregamos Session -->
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1); // Duración de la sesión
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // ✅ SOLO AQUÍ

app.UseAuthentication();
app.UseAuthorization();

// Rutas por áreas (Admin / Negocio)
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

// Redirigir raíz al catálogo en área Negocio
app.MapGet("/", context =>
{
    context.Response.Redirect("/Negocio/Catalogo/Index");
    return Task.CompletedTask;
});

app.Run();
