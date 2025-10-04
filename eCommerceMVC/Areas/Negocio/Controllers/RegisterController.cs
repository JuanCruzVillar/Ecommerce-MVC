using eCommerce.Data;
using eCommerce.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eCommerceMVC.Areas.Negocio.Controllers
{
    [Area("Negocio")]
    public class RegisterController : Controller
    {
        private readonly DbecommerceContext _context;

        public RegisterController(DbecommerceContext context)
        {
            _context = context;
        }

        // GET: Negocio/Register
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: Negocio/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string nombres, string apellidos, string correo, string contrasena, string confirmarContrasena)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(nombres) || string.IsNullOrWhiteSpace(apellidos) ||
                    string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
                {
                    ViewBag.Error = "Todos los campos son obligatorios";
                    return View();
                }

                if (contrasena != confirmarContrasena)
                {
                    ViewBag.Error = "Las contraseñas no coinciden";
                    return View();
                }

                if (contrasena.Length < 6)
                {
                    ViewBag.Error = "La contraseña debe tener al menos 6 caracteres";
                    return View();
                }

                // Verificar si el correo ya existe
                var correoExiste = await _context.Clientes
                    .AnyAsync(c => c.Correo.ToLower() == correo.ToLower());

                if (correoExiste)
                {
                    ViewBag.Error = "Ya existe una cuenta con este correo electrónico";
                    return View();
                }

                var usuarioExiste = await _context.Usuarios
                    .AnyAsync(u => u.Correo.ToLower() == correo.ToLower());

                if (usuarioExiste)
                {
                    ViewBag.Error = "Ya existe una cuenta con este correo electrónico";
                    return View();
                }

                // Crear el Cliente
                var hasher = new PasswordHasher<Cliente>();
                var nuevoCliente = new Cliente
                {
                    Nombres = nombres.Trim(),
                    Apellidos = apellidos.Trim(),
                    Correo = correo.Trim().ToLower(),
                    FechaRegistro = DateTime.Now,
                    Restablecer = false // AGREGAR ESTO
                };

                nuevoCliente.Contraseña = hasher.HashPassword(nuevoCliente, contrasena);

                _context.Clientes.Add(nuevoCliente);
                await _context.SaveChangesAsync();

                // Crear el Usuario asociado
                var hasherUsuario = new PasswordHasher<Usuario>();
                var nuevoUsuario = new Usuario
                {
                    Nombres = nombres.Trim(),
                    Apellidos = apellidos.Trim(),
                    Correo = correo.Trim().ToLower(),
                    Rol = "Cliente",
                    Activo = true,
                    FechaRegistro = DateTime.Now,
                    IdCliente = nuevoCliente.IdCliente,
                    Restablecer = false // AGREGAR ESTO
                };

                nuevoUsuario.Contraseña = hasherUsuario.HashPassword(nuevoUsuario, contrasena);

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                // Iniciar sesión automáticamente
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, nuevoUsuario.Nombres),
            new Claim(ClaimTypes.Email, nuevoUsuario.Correo),
            new Claim(ClaimTypes.Role, nuevoUsuario.Rol),
            new Claim("IdCliente", nuevoUsuario.IdCliente.Value.ToString()),
            new Claim("IdUsuario", nuevoUsuario.IdUsuario.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties { IsPersistent = true }
                );

                TempData["Success"] = "¡Cuenta creada exitosamente! Bienvenido/a.";
                return RedirectToAction("Index", "Catalogo", new { area = "Negocio" });
            }
            catch (Exception ex)
            {
                // CAMBIAR ESTO para ver el error real
                var errorCompleto = ex.Message;
                if (ex.InnerException != null)
                {
                    errorCompleto += " | Inner: " + ex.InnerException.Message;
                }
                ViewBag.Error = $"Error detallado: {errorCompleto}";

                // También escribir en la consola de debug
                System.Diagnostics.Debug.WriteLine($"ERROR REGISTRO: {errorCompleto}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                return View();
            }
        }
    }
    }
