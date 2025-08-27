using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eCommerceMVC.Models;

namespace eCommerceMVC.Controllers
{
    public class MarcasController : Controller
    {
        private readonly DbecommerceContext _context;

        public MarcasController(DbecommerceContext context)
        {
            _context = context;
        }

        // GET: Marcas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Marcas.ToListAsync());
        }

        // GET: Marcas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var marca = await _context.Marcas
                .FirstOrDefaultAsync(m => m.IdMarca == id);
            if (marca == null) return NotFound();

            return View(marca);
        }

        // GET: Marcas/Create
        public IActionResult Create() => View();

        // POST: Marcas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Marca marca)
        {
            if (!ModelState.IsValid) return View(marca);

            marca.FechaRegistro = DateTime.Now; // Fecha automática
            _context.Add(marca);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Marcas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null) return NotFound();

            return View(marca);
        }

        // POST: Marcas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Marca marca)
        {
            if (id != marca.IdMarca) return NotFound();

            if (!ModelState.IsValid) return View(marca);

            var marcaExistente = await _context.Marcas.FindAsync(id);
            if (marcaExistente == null) return NotFound();

            // 🔹 Solo actualizamos lo que corresponde
            marcaExistente.Descripcion = marca.Descripcion;
            marcaExistente.Activo = marca.Activo;
            // ❌ No tocamos FechaRegistro

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Marcas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var marca = await _context.Marcas
                .FirstOrDefaultAsync(m => m.IdMarca == id);
            if (marca == null) return NotFound();

            return View(marca);
        }

        // POST: Marcas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);

            if (marca == null)
            {
                return NotFound();
            }

            // Verificar si la marca tiene productos asignados
            bool tieneProductos = await _context.Productos.AnyAsync(p => p.IdMarca == id);

            if (tieneProductos)
            {
                // Mensaje de error para que no se elimine la marca
                TempData["ErrorMessage"] = "No se puede eliminar la marca porque tiene productos asignados.";
                return RedirectToAction(nameof(Index));
            }

            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool MarcaExists(int id)
        {
            return _context.Marcas.Any(e => e.IdMarca == id);
        }
    }
}
