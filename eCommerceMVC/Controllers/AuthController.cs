using eCommerce.Entities;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eCommerceMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // -------------------------------
        // LOGIN ADMIN (GET)
        // -------------------------------
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(); // Vista: Views/Auth/Login.cshtml usando _LayoutAdmin.cshtml
        }

        // -------------------------------
        // LOGIN ADMIN (POST)
        // -------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string contraseña, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contraseña))
            {
                ViewBag.Error = "Debés completar todos los campos";
                return View();
            }

            var usuario = await _usuarioService.GetByCorreoAsync(correo);
            if (usuario == null || !usuario.Activo || usuario.Rol != "Admin")
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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nombres),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                          new ClaimsPrincipal(claimsIdentity),
                                          new AuthenticationProperties { IsPersistent = true });

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        // -------------------------------
        // LOGIN CLIENTE (POST desde modal)
        // -------------------------------
        [HttpPost]
        public async Task<IActionResult> LoginCliente([FromForm] string correo, [FromForm] string contraseña)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contraseña))
            {
                return Json(new { success = false, message = "Debés completar todos los campos." });
            }

            var usuario = await _usuarioService.GetByCorreoAsync(correo);
            if (usuario == null || !usuario.Activo || usuario.Rol != "Cliente")
            {
                return Json(new { success = false, message = "Usuario o contraseña incorrectos." });
            }

            var hasher = new PasswordHasher<Usuario>();
            var result = hasher.VerifyHashedPassword(usuario, usuario.Contraseña, contraseña);
            if (result == PasswordVerificationResult.Failed)
            {
                return Json(new { success = false, message = "Usuario o contraseña incorrectos." });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nombres),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                          new ClaimsPrincipal(claimsIdentity),
                                          new AuthenticationProperties { IsPersistent = true });

            return Json(new { success = true, message = "Bienvenido " + usuario.Nombres });
        }

        // -------------------------------
        // LOGOUT
        // -------------------------------
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Si viene vía AJAX (modal cliente) retorna JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            // Redirige según rol si está logueado (fallback: catalogo)
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
                return RedirectToAction("Login", "Auth");

            return RedirectToAction("Index", "Catalogo", new { area = "Negocio" });
        }
    }
}
