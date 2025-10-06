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
                .Where(c => c.IdCliente == clienteId.Value)
                .Include(c => c.IdProductoNavigation)
                .ToListAsync();

            return carritos;
        }

        public async Task AgregarProductoAsync(int? clienteId, int productoId, int cantidad)
        {
            try
            {
                if (!clienteId.HasValue)
                {
                    throw new Exception("Cliente no identificado");
                }

                var item = await _context.Carritos
                    .FirstOrDefaultAsync(c => c.IdCliente == clienteId.Value && c.IdProducto == productoId);

                if (item != null)
                {
                    item.Cantidad += cantidad;
                }
                else
                {
                    var nuevoItem = new Carrito
                    {
                        IdCliente = clienteId.Value,
                        IdProducto = productoId,
                        Cantidad = cantidad
                    };
                    _context.Carritos.Add(nuevoItem);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar producto: {ex.Message}", ex);
            }
        }

        public async Task EliminarProductoAsync(int? clienteId, int productoId)
        {
            if (!clienteId.HasValue) return;

            var item = await _context.Carritos
                .FirstOrDefaultAsync(c => c.IdCliente == clienteId.Value && c.IdProducto == productoId);

            if (item != null)
            {
                _context.Carritos.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task VaciarCarritoAsync(int? clienteId)
        {
            if (!clienteId.HasValue) return;

            var items = _context.Carritos.Where(c => c.IdCliente == clienteId.Value);
            _context.Carritos.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CarritoItemViewModel>> ObtenerItemsCarritoAsync(int clienteId)
        {
            try
            {
                var items = await _context.Carritos
                    .Include(c => c.IdProductoNavigation)
                        .ThenInclude(p => p.IdMarcaNavigation)
                    .Include(c => c.IdProductoNavigation)
                        .ThenInclude(p => p.IdCategoriaNavigation)
                    .Where(c => c.IdCliente == clienteId)
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

        public async Task<int> ObtenerCantidadItemsAsync(int clienteId)
        {
            try
            {
                var cantidadTotal = await _context.Carritos
                    .Where(c => c.IdCliente == clienteId)
                    .SumAsync(c => c.Cantidad ?? 0);

                return cantidadTotal;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> ObtenerTotalCarritoAsync(int clienteId)
        {
            try
            {
                var total = await _context.Carritos
                    .Include(c => c.IdProductoNavigation)
                    .Where(c => c.IdCliente == clienteId)
                    .SumAsync(c => (c.IdProductoNavigation.Precio ?? 0) * (c.Cantidad ?? 0));

                return total;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> ValidarStockDisponibleAsync(int clienteId, int idProducto, int cantidad)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(idProducto);
                if (producto == null || !producto.Activo.GetValueOrDefault())
                {
                    return false;
                }

                var cantidadEnCarrito = await _context.Carritos
                    .Where(c => c.IdCliente == clienteId && c.IdProducto == idProducto)
                    .SumAsync(c => c.Cantidad ?? 0);

                var stockDisponible = (producto.Stock ?? 0) - cantidadEnCarrito;
                return stockDisponible >= cantidad;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ActualizarCantidadAsync(int clienteId, int idProducto, int nuevaCantidad)
        {
            try
            {
                var itemCarrito = await _context.Carritos
                    .FirstOrDefaultAsync(c => c.IdCliente == clienteId && c.IdProducto == idProducto);

                if (itemCarrito == null)
                {
                    return false;
                }

                if (nuevaCantidad <= 0)
                {
                    await EliminarProductoAsync(clienteId, idProducto);
                    return true;
                }

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