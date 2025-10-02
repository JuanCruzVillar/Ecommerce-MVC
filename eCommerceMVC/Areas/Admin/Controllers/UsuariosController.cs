using eCommerce.Entities;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace eCommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IClienteService _clienteService;

        public UsuariosController(IUsuarioService usuarioService, IClienteService clienteService)
        {
            _usuarioService = usuarioService;
            _clienteService = clienteService;
        }


        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _usuarioService.GetByIdAsync(id.Value);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // GET: Usuarios/Create
        public async Task<IActionResult> Create()
        {
            await CargarClientes();
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                await CargarClientes();
                return View(usuario);
            }

            // Hash de la contraseña
            var hasher = new PasswordHasher<Usuario>();
            usuario.Contraseña = hasher.HashPassword(usuario, usuario.Contraseña);
            usuario.FechaRegistro = DateTime.Now;
            usuario.Activo = true;

            var result = await _usuarioService.CreateAsync(usuario);
            if (!result)
            {
                TempData["Error"] = "Ya existe un usuario con ese correo.";
                await CargarClientes();
                return View(usuario);
            }

            TempData["Success"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _usuarioService.GetByIdAsync(id.Value);
            if (usuario == null) return NotFound();

            await CargarClientes(usuario.IdCliente);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario, string NuevaContrasena)
        {
            if (id != usuario.IdUsuario) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarClientes(usuario.IdCliente);
                return View(usuario);
            }

            var usuarioDb = await _usuarioService.GetByIdAsync(id);
            if (usuarioDb == null) return NotFound();

            usuarioDb.Nombres = usuario.Nombres;
            usuarioDb.Apellidos = usuario.Apellidos;
            usuarioDb.Correo = usuario.Correo;
            usuarioDb.IdCliente = usuario.IdCliente;
            usuarioDb.Activo = usuario.Activo;

            if (!string.IsNullOrEmpty(NuevaContrasena))
            {
                var hasher = new PasswordHasher<Usuario>();
                usuarioDb.Contraseña = hasher.HashPassword(usuarioDb, NuevaContrasena);
            }

            await _usuarioService.UpdateAsync(usuarioDb);

            // NUEVO: Sincronizar con el Cliente si existe
            if (usuario.IdCliente.HasValue)
            {
                var cliente = await _clienteService.GetByIdAsync(usuario.IdCliente.Value);
                if (cliente != null)
                {
                    cliente.Nombres = usuario.Nombres;
                    cliente.Apellidos = usuario.Apellidos;
                    cliente.Correo = usuario.Correo;
                    await _clienteService.UpdateAsync(cliente);
                }
            }

            TempData["Success"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _usuarioService.GetByIdAsync(id.Value);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _usuarioService.DeleteAsync(id);
            if (!result)
                TempData["Error"] = "No se puede eliminar este usuario.";
            else
                TempData["Success"] = "Usuario eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarClientes(int? selectedId = null)
        {
            var clientes = await _clienteService.GetAllAsync();
            ViewBag.IdCliente = new SelectList(clientes, "IdCliente", "Nombres", selectedId);
        }
    }
}
