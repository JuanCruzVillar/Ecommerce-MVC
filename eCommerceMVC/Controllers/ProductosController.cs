using eCommerce.Entities;
using eCommerce.Services.Interfaces;
using eCommerce.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Threading.Tasks;

namespace eCommerceMVC.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IMarcaService _marcaService;

        public ProductosController(
            IProductoService productoService,
            ICategoriaService categoriaService,
            IMarcaService marcaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _marcaService = marcaService;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.GetAllAsync();
            return View(productos);
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _productoService.GetByIdAsync(id.Value);
            if (producto == null) return NotFound();

            return View(producto);
        }

        // GET: Productos/Create
        public async Task<IActionResult> Create()
        {
            await CargarDropdowns();
            return View();
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await CargarDropdowns(vm);
                return View(vm);
            }

            var producto = new Producto
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

            if (vm.ImagenArchivo != null && vm.ImagenArchivo.Length > 0)
            {
                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(vm.ImagenArchivo.FileName);
                var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);
                var rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);

                using var stream = new FileStream(rutaArchivo, FileMode.Create);
                await vm.ImagenArchivo.CopyToAsync(stream);

                producto.RutaImagen = "/images/" + nombreArchivo;
                producto.NombreImagen = vm.ImagenArchivo.FileName;
            }

            await _productoService.CreateAsync(producto);
            TempData["Success"] = "Producto creado correctamente";
            return RedirectToAction(nameof(Index));

        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _productoService.GetByIdAsync(id.Value);
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
                Activo = producto.Activo == true,
                FechaRegistro = producto.FechaRegistro,
                RutaImagen = producto.RutaImagen,
                NombreImagen = producto.NombreImagen
            };

            await CargarDropdowns(vm);
            return View(vm);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoViewModel vm)
        {
            if (id != vm.IdProducto) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarDropdowns(vm);
                return View(vm);
            }

            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null) return NotFound();

            producto.Nombre = vm.Nombre;
            producto.Descripcion = vm.Descripcion;
            producto.IdMarca = vm.IdMarca;
            producto.IdCategoria = vm.IdCategoria;
            producto.Precio = vm.Precio;
            producto.Stock = vm.Stock;
            producto.Activo = vm.Activo;

            if (vm.ImagenArchivo != null && vm.ImagenArchivo.Length > 0)
            {
                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(vm.ImagenArchivo.FileName);
                var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);
                var rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);

                using var stream = new FileStream(rutaArchivo, FileMode.Create);
                await vm.ImagenArchivo.CopyToAsync(stream);

                producto.RutaImagen = "/images/" + nombreArchivo;
                producto.NombreImagen = vm.ImagenArchivo.FileName;
            }

            await _productoService.UpdateAsync(producto);
            TempData["Success"] = "Producto modificado correctamente";
            return RedirectToAction(nameof(Index));

        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _productoService.GetByIdAsync(id.Value);
            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _productoService.DeleteAsync(id);
            if (!result)
                TempData["Error"] = "No se puede eliminar el producto.";
            else
                TempData["Success"] = "Producto eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarDropdowns(ProductoViewModel? vm = null)
        {
            var marcas = await _marcaService.GetAllAsync();
            var categorias = await _categoriaService.GetAllAsync();

            ViewBag.IdMarca = new SelectList(marcas, "IdMarca", "Descripcion", vm?.IdMarca);
            ViewBag.IdCategoria = new SelectList(categorias, "IdCategoria", "Descripcion", vm?.IdCategoria);
        }
    }
}
