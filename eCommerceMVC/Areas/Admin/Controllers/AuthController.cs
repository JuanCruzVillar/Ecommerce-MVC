using eCommerce.Entities;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eCommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: Admin/Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin/Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string contraseña)
        {
            try
            {
                if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contraseña))
                {
                    ViewBag.Error = "Debe completar todos los campos";
                    return View();
                }

                var usuario = await _usuarioService.GetByCorreoAsync(correo);
                if (usuario == null || usuario.Rol != "Admin" || !usuario.Activo)
                {
                    ViewBag.Error = "Usuario o contraseña incorrectos";
                    return View();
                }

                var hasher = new PasswordHasher<Usuario>();
                var result = hasher.VerifyHashedPassword(usuario, usuario.Contraseña, contraseña);
                if (result == PasswordVerificationResult.Failed)
                {
                    ViewBag.Error = "Usuario o contraseña incorrectos";
                    return View();
                }

                // CORREGIDO: Agregar IdUsuario para consistencia
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombres),
                    new Claim(ClaimTypes.Email, usuario.Correo),
                    new Claim(ClaimTypes.Role, usuario.Rol),
                    new Claim("IdUsuario", usuario.IdUsuario.ToString()) // AGREGADO
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties { IsPersistent = true }
                );

                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            catch (System.Exception ex)
            {
                // Log del error (en producción usar ILogger)
                ViewBag.Error = "Error interno del servidor. Intente nuevamente.";
                // En desarrollo, podrías mostrar: ex.Message
                return View();
            }
        }

        // GET: Admin/Auth/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}