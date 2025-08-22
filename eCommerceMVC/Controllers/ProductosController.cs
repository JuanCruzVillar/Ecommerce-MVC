using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eCommerceMVC.Models;

namespace eCommerceMVC.Controllers
{
    public class ProductosController : Controller
    {
        private readonly DbecommerceContext _context;

        public ProductosController(DbecommerceContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var productos = _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.IdMarcaNavigation);
            return View(await productos.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.IdMarcaNavigation)
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            CargarDropdowns();
            return View();
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Productos producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Add(producto);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Si hay error, volver a cargar los dropdowns
            CargarDropdowns(producto);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var producto = _context.Productos.Find(id);
            if (producto == null) return NotFound();

            CargarDropdowns(producto);
            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Productos producto)
        {
            if (id != producto.IdProducto) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(producto);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            CargarDropdowns(producto);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.IdMarcaNavigation)
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void CargarDropdowns(Productos producto = null)
        {
            var marcas = _context.Marcas.ToList();
            var categorias = _context.Categorias.ToList();

            ViewBag.IdMarca = new SelectList(marcas, "IdMarca", "Descripcion", producto?.IdMarca);
            ViewBag.IdCategoria = new SelectList(categorias, "IdCategoria", "Descripcion", producto?.IdCategoria);
        }

        private bool ProductosExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }
    }
}
