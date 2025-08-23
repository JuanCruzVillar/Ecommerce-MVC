using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eCommerceMVC.Models;
using eCommerceMVC.ViewModels;

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
        public async Task<IActionResult> Create(ProductoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var producto = new Productos
                {
                    Nombre = vm.Nombre,
                    Descripcion = vm.Descripcion,
                    IdMarca = vm.IdMarca,
                    IdCategoria = vm.IdCategoria,
                    Precio = vm.Precio,
                    Stock = vm.Stock,
                    Activo = true,
                    FechaRegistro = DateTime.Now
                };

                // Guardar imagen si se subió
                if (vm.ImagenArchivo != null && vm.ImagenArchivo.Length > 0)
                {
                    var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

                    var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImagenArchivo.FileName);
                    var rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);

                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await vm.ImagenArchivo.CopyToAsync(stream);
                    }

                    producto.RutaImagen = "/images/" + nombreArchivo;
                    producto.NombreImagen = vm.ImagenArchivo.FileName;
                }

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarDropdowns(vm);
            return View(vm);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            var vm = new ProductoViewModel
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                IdMarca = producto.IdMarca,
                IdCategoria = producto.IdCategoria,
                Precio = producto.Precio,
                Stock = producto.Stock,
                Activo = producto.Activo ?? true, // si es null, setear true
                FechaRegistro = producto.FechaRegistro,
                RutaImagen = producto.RutaImagen,
                NombreImagen = producto.NombreImagen
            };


            CargarDropdowns(vm);
            return View(vm);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoViewModel vm)
        {
            if (id != vm.IdProducto) return NotFound();

            if (ModelState.IsValid)
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null) return NotFound();

                producto.Nombre = vm.Nombre;
                producto.Descripcion = vm.Descripcion;
                producto.IdMarca = vm.IdMarca;
                producto.IdCategoria = vm.IdCategoria;
                producto.Precio = vm.Precio;
                producto.Stock = vm.Stock;
                producto.Activo = vm.Activo;

                // Guardar nueva imagen si se subió
                if (vm.ImagenArchivo != null && vm.ImagenArchivo.Length > 0)
                {
                    var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

                    var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImagenArchivo.FileName);
                    var rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);

                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await vm.ImagenArchivo.CopyToAsync(stream);
                    }

                    producto.RutaImagen = "/images/" + nombreArchivo;
                    producto.NombreImagen = vm.ImagenArchivo.FileName;
                }

                _context.Update(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarDropdowns(vm);
            return View(vm);
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

        private void CargarDropdowns(ProductoViewModel? vm = null)
        {
            var marcas = _context.Marcas.ToList();
            var categorias = _context.Categorias.ToList();

            ViewBag.IdMarca = new SelectList(marcas, "IdMarca", "Descripcion", vm?.IdMarca);
            ViewBag.IdCategoria = new SelectList(categorias, "IdCategoria", "Descripcion", vm?.IdCategoria);
        }

        private bool ProductosExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }
    }
}
