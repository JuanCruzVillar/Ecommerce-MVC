using eCommerce.Entities;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eCommerceMVC.Areas.Negocio.Controllers
{
    [Area("Negocio")]
    public class AuthController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: Negocio/Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(); 
        }

        // POST: Negocio/Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                ViewBag.Error = "Debés completar todos los campos";
                return View();
            }

            var usuario = await _usuarioService.GetByCorreoAsync(correo);
            if (usuario == null || usuario.Rol != "Cliente" || !usuario.Activo)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }

            var hasher = new PasswordHasher<Usuario>();
            if (hasher.VerifyHashedPassword(usuario, usuario.Contraseña, contrasena) == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }

            var claims = new List<Claim>
            {
             new Claim(ClaimTypes.Name, usuario.Nombres),
             new Claim(ClaimTypes.Email, usuario.Correo),
             new Claim(ClaimTypes.Role, usuario.Rol),
              new Claim("IdCliente", usuario.IdCliente.ToString()),
            new Claim("IdUsuario", usuario.IdUsuario.ToString())
             };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true }
            );

            return RedirectToAction("Index", "Catalogo", new { area = "Negocio" });
        }

        // GET: Negocio/Auth/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Catalogo", new { area = "Negocio" });

        }
    }
}
