using eCommerceMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceMVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DbecommerceContext _context;

        public UsuariosController(DbecommerceContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var dbecommerceContext = _context.Usuarios.Include(u => u.IdClienteNavigation);
            return View(await dbecommerceContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdClienteNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Hash de la contraseña
                var hasher = new PasswordHasher<Usuario>();
                var hashedPassword = hasher.HashPassword(usuario, usuario.Contraseña);

                // Ejecutar procedimiento almacenado
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC RegistrarUsuario @Nombres, @Apellidos, @Correo, @Contrasena",
                    new SqlParameter("@Nombres", usuario.Nombres),
                    new SqlParameter("@Apellidos", usuario.Apellidos),
                    new SqlParameter("@Correo", usuario.Correo),
                    new SqlParameter("@Contrasena", hashedPassword)
                );

                TempData["SuccessMessage"] = "Usuario creado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }




        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", usuario.IdCliente);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario, string NuevaContrasena)
        {
            if (id != usuario.IdUsuario)
                return NotFound();

            if (ModelState.IsValid)
            {
                var userDb = await _context.Usuarios.FindAsync(id);
                if (userDb == null)
                    return NotFound();

                userDb.Nombres = usuario.Nombres;
                userDb.Apellidos = usuario.Apellidos;
                userDb.Correo = usuario.Correo;
               

                if (!string.IsNullOrEmpty(NuevaContrasena))
                {
                    var hasher = new PasswordHasher<Usuario>();
                    userDb.Contraseña = hasher.HashPassword(userDb, NuevaContrasena);
                }

                _context.Update(userDb);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores de validación, vuelve a mostrar la vista
            return View(usuario);
        }






        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdClienteNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                TempData["DangerMessage"] = "Usuario eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }


        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}
