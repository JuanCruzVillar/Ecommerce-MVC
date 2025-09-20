using eCommerce.Data;
using eCommerce.Entities;
using eCommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;  
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace eCommerce.Services.Implementations
{
    public class CarritoService : ICarritoService
    {
        private readonly DbecommerceContext _context;

        public CarritoService(DbecommerceContext context)
        {
            _context = context;
        }

        public async Task<List<Carrito>> ObtenerCarritoAsync(int? clienteId)
        {
            if (!clienteId.HasValue) return new List<Carrito>();

            var carritos = await _context.Carritos
                .Where(c => c.IdUsuario == clienteId.Value)
                .Include(c => c.IdProductoNavigation) // incluir productos
                .ToListAsync();

            return carritos;
        }


        public async Task AgregarProductoAsync(int? clienteId, int productoId, int cantidad)
        {
            try
            {
                Console.WriteLine($"1. Iniciando - clienteId: {clienteId}, productoId: {productoId}");

                if (!clienteId.HasValue)
                {
                    Console.WriteLine("2. Error: clienteId es null");
                    return;
                }

                Console.WriteLine("3. Buscando item existente en carrito...");
                var item = await _context.Carritos
                    .FirstOrDefaultAsync(c => c.IdUsuario == clienteId.Value && c.IdProducto == productoId);

                Console.WriteLine($"4. Item encontrado: {item != null}");

                if (item != null)
                {
                    Console.WriteLine($"5a. Actualizando cantidad: {item.Cantidad} + {cantidad}");
                    item.Cantidad += cantidad;
                }
                else
                {
                    Console.WriteLine("5b. Creando nuevo item...");
                    var nuevoItem = new Carrito
                    {
                        IdUsuario = clienteId.Value,
                        IdProducto = productoId,
                        Cantidad = cantidad
                    };
                    _context.Carritos.Add(nuevoItem);
                }

                Console.WriteLine("6. Guardando cambios...");
                await _context.SaveChangesAsync();
                Console.WriteLine("7. ¡Éxito!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"INNER: {ex.InnerException?.Message}");
                throw new Exception($"Error al agregar producto: {ex.Message}", ex);
            }
        }

        public async Task EliminarProductoAsync(int? clienteId, int productoId)
        {
            if (!clienteId.HasValue) return;

            var item = await _context.Carritos
                .FirstOrDefaultAsync(c => c.IdUsuario == clienteId.Value && c.IdProducto == productoId);

            if (item != null)
            {
                _context.Carritos.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task VaciarCarritoAsync(int? clienteId)
        {
            if (!clienteId.HasValue) return;

            var items = _context.Carritos.Where(c => c.IdUsuario == clienteId.Value);
            _context.Carritos.RemoveRange(items);
            await _context.SaveChangesAsync();

        }

        public async Task<List<CarritoItemViewModel>> ObtenerItemsCarritoAsync(int idCliente)
        {
            try
            {
                var items = await _context.Carritos
                    .Include(c => c.IdProductoNavigation)
                        .ThenInclude(p => p.IdMarcaNavigation)
                    .Include(c => c.IdProductoNavigation)
                        .ThenInclude(p => p.IdCategoriaNavigation)
                    .Where(c => c.IdUsuario == idCliente)
                    .Select(c => new CarritoItemViewModel
                    {
                        IdProducto = c.IdProducto ?? 0,
                        NombreProducto = c.IdProductoNavigation.Nombre,
                        Descripcion = c.IdProductoNavigation.Descripcion,
                        Precio = c.IdProductoNavigation.Precio ?? 0,
                        Cantidad = c.Cantidad ?? 0,
                        Subtotal = (c.IdProductoNavigation.Precio ?? 0) * (c.Cantidad ?? 0),
                        RutaImagen = c.IdProductoNavigation.RutaImagen,
                        NombreImagen = c.IdProductoNavigation.NombreImagen,
                        StockDisponible = c.IdProductoNavigation.Stock ?? 0,
                        NombreMarca = c.IdProductoNavigation.IdMarcaNavigation.Descripcion,
                        NombreCategoria = c.IdProductoNavigation.IdCategoriaNavigation.Descripcion
                    })
                    .ToListAsync();

                return items;
            }
            catch (Exception)
            {
                return new List<CarritoItemViewModel>();
            }
        }

        public async Task<int> ObtenerCantidadItemsAsync(int idCliente)
        {
            try
            {
                var cantidadTotal = await _context.Carritos
                    .Where(c => c.IdUsuario == idCliente)
                    .SumAsync(c => c.Cantidad ?? 0);

                return cantidadTotal;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> ObtenerTotalCarritoAsync(int idCliente)
        {
            try
            {
                var total = await _context.Carritos
                    .Include(c => c.IdProductoNavigation)
                    .Where(c => c.IdUsuario == idCliente)
                    .SumAsync(c => (c.IdProductoNavigation.Precio ?? 0) * (c.Cantidad ?? 0));

                return total;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> ValidarStockDisponibleAsync(int idCliente, int idProducto, int cantidad)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(idProducto);
                if (producto == null || !producto.Activo.GetValueOrDefault())
                {
                    return false;
                }

                // Obtener cantidad actual en el carrito
                var cantidadEnCarrito = await _context.Carritos
                    .Where(c => c.IdUsuario == idCliente && c.IdProducto == idProducto)
                    .SumAsync(c => c.Cantidad ?? 0);

                var stockDisponible = (producto.Stock ?? 0) - cantidadEnCarrito;
                return stockDisponible >= cantidad;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ActualizarCantidadAsync(int idCliente, int idProducto, int nuevaCantidad)
        {
            try
            {
                var itemCarrito = await _context.Carritos
                    .FirstOrDefaultAsync(c => c.IdUsuario == idCliente && c.IdProducto == idProducto);

                if (itemCarrito == null)
                {
                    return false;
                }

                if (nuevaCantidad <= 0)
                {
                    
                    await EliminarProductoAsync(idCliente, idProducto);
                    return true;
                }

                // Validar stock disponible
                var producto = await _context.Productos.FindAsync(idProducto);
                if (producto == null || (producto.Stock ?? 0) < nuevaCantidad)
                {
                    return false;
                }

                itemCarrito.Cantidad = nuevaCantidad;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
