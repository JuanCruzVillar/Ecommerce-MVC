using eCommerce.Data;
using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using eCommerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using static eCommerce.Repositories.Interfaces.IProductoRepository;

namespace eCommerce.Repositories.Implementations
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly DbecommerceContext _context;

        public ProductoRepository(DbecommerceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.IdMarcaNavigation)
                .ToListAsync();
        }

        public async Task<Producto> GetByIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.IdMarcaNavigation)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        public async Task AddAsync(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var producto = await GetByIdAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Producto>> GetAllWithCategoriasAsync()
        {
            return await _context.Productos
                                 .Include(p => p.IdCategoriaNavigation) 
                                 .ThenInclude(c => c.CategoriaPadre)   
                                 .Include(p => p.IdMarcaNavigation)
                                 .ToListAsync();
        }

        public async Task<Producto> GetByIdWithEspecificacionesAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Especificaciones)
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(p => p.IdProducto == id && p.Activo == true);
        }

        public async Task<Producto> GetByIdWithImagenesAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Imagenes.Where(i => i.Activo == true))
                .Include(p => p.Especificaciones.Where(e => e.Activo == true))
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(p => p.IdProducto == id && p.Activo == true);
        }

        public async Task<bool> AgregarImagenAsync(ProductoImagen imagen)
        {
            try
            {
                await _context.ProductoImagenes.AddAsync(imagen);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EliminarImagenAsync(int idImagen)
        {
            try
            {
                var imagen = await _context.ProductoImagenes.FindAsync(idImagen);
                if (imagen == null) return false;

                _context.ProductoImagenes.Remove(imagen);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarcarImagenPrincipalAsync(int idImagen, int idProducto)
        {
            try
            {
                // Desmarcar todas las imágenes del producto
                var imagenes = await _context.ProductoImagenes
                    .Where(i => i.IdProducto == idProducto)
                    .ToListAsync();

                foreach (var img in imagenes)
                {
                    img.EsPrincipal = (img.IdImagen == idImagen);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ReordenarImagenesAsync(List<OrdenImagenDto> orden)
        {
            try
            {
                foreach (var item in orden)
                {
                    var imagen = await _context.ProductoImagenes.FindAsync(item.IdImagen);
                    if (imagen != null)
                    {
                        imagen.Orden = item.Orden;
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(List<Producto> productos, int total)> BuscarConFiltrosAsync(
    string busqueda,
    int? categoriaId,
    int? marcaId,
    decimal? precioMin,
    decimal? precioMax,
    string ordenamiento)
        {
            var query = _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.IdMarcaNavigation)
                .Where(p => p.Activo == true);

            // Filtro por búsqueda de texto
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.ToLower();
                query = query.Where(p =>
                    p.Nombre.ToLower().Contains(busqueda) ||
                    p.Descripcion.ToLower().Contains(busqueda)
                );
            }

            // Filtro por categoría (incluye subcategorías)
            if (categoriaId.HasValue)
            {
                var categoriasHijas = await _context.Categorias
                    .Where(c => c.IdCategoriaPadre == categoriaId.Value)
                    .Select(c => c.IdCategoria)
                    .ToListAsync();

                categoriasHijas.Add(categoriaId.Value);

                query = query.Where(p => categoriasHijas.Contains(p.IdCategoria ?? 0));
            }

            // Filtro por marca
            if (marcaId.HasValue)
            {
                query = query.Where(p => p.IdMarca == marcaId.Value);
            }

            // Filtro por rango de precio
            if (precioMin.HasValue)
            {
                query = query.Where(p => p.Precio >= precioMin.Value);
            }

            if (precioMax.HasValue)
            {
                query = query.Where(p => p.Precio <= precioMax.Value);
            }

            // Ordenamiento
            query = ordenamiento switch
            {
                "precio-asc" => query.OrderBy(p => p.Precio),
                "precio-desc" => query.OrderByDescending(p => p.Precio),
                "nombre" => query.OrderBy(p => p.Nombre),
                "recientes" => query.OrderByDescending(p => p.FechaRegistro),
                _ => query.OrderBy(p => p.Nombre)
            };

            var total = await query.CountAsync();
            var productos = await query.ToListAsync();

            return (productos, total);
        }

        public async Task<(decimal min, decimal max)> ObtenerRangoPreciosAsync()
        {
            var productos = await _context.Productos
                .Where(p => p.Activo == true && p.Precio.HasValue)
                .ToListAsync();

            if (!productos.Any())
                return (0, 0);

            var min = productos.Min(p => p.Precio ?? 0);
            var max = productos.Max(p => p.Precio ?? 0);

            return (min, max);
        }

        public async Task<List<FiltroConteo>> ObtenerCategoriasConConteoAsync()
        {
            return await _context.Categorias
                .Where(c => c.Activo == true)
                .Select(c => new FiltroConteo
                {
                    Id = c.IdCategoria,
                    Nombre = c.Descripcion,
                    Cantidad = c.Productos.Count(p => p.Activo == true)
                })
                .Where(f => f.Cantidad > 0)
                .OrderBy(f => f.Nombre)
                .ToListAsync();
        }

        public async Task<List<FiltroConteo>> ObtenerMarcasConConteoAsync()
        {
            return await _context.Marcas
                .Where(m => m.Activo == true)
                .Select(m => new FiltroConteo
                {
                    Id = m.IdMarca,
                    Nombre = m.Descripcion,
                    Cantidad = m.Productos.Count(p => p.Activo == true)
                })
                .Where(f => f.Cantidad > 0)
                .OrderBy(f => f.Nombre)
                .ToListAsync();
        }
    }
}
