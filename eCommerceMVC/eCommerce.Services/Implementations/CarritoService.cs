using eCommerce.Data;
using eCommerce.Entities;
using eCommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
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

            return await _context.Carritos
                .Include(c => c.IdProductoNavigation)
                .Where(c => c.IdCliente == clienteId.Value)
                .ToListAsync();
        }

        public async Task AgregarProductoAsync(int? clienteId, int productoId, int cantidad)
        {
            if (!clienteId.HasValue) return;

            var item = await _context.Carritos
                .FirstOrDefaultAsync(c => c.IdCliente == clienteId.Value && c.IdProducto == productoId);

            if (item != null)
                item.Cantidad += cantidad;
            else
                _context.Carritos.Add(new Carrito
                {
                    IdCliente = clienteId.Value,
                    IdProducto = productoId,
                    Cantidad = cantidad
                });

            await _context.SaveChangesAsync();
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
    }
}
