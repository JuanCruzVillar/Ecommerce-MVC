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
            return View(); // Vista Admin
        }

        // -------------------------------
        // LOGIN ADMIN (POST)
        // -------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string contrasena, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
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
            var result = hasher.VerifyHashedPassword(usuario, usuario.Contraseña, contrasena);
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
        public async Task<IActionResult> LoginCliente(string correo, string contrasena)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
                return RedirectToAction("Index", "Catalogo", new { area = "Negocio" });

            var usuario = await _usuarioService.GetByCorreoAsync(correo);
            if (usuario == null || !usuario.Activo || usuario.Rol != "Cliente")
                return RedirectToAction("Index", "Catalogo", new { area = "Negocio" });

            var hasher = new PasswordHasher<Usuario>();
            if (hasher.VerifyHashedPassword(usuario, usuario.Contraseña, contrasena) == PasswordVerificationResult.Failed)
                return RedirectToAction("Index", "Catalogo", new { area = "Negocio" });

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

            // Redirige al catálogo del área negocio
            return RedirectToAction("Index", "Catalogo", new { area = "Negocio" });
        }


        // -------------------------------
        // LOGOUT
        // -------------------------------
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
                return RedirectToAction("Login", "Auth");

            return RedirectToAction("Index", "Catalogo", new { area = "Negocio" });
        }
    }
}
