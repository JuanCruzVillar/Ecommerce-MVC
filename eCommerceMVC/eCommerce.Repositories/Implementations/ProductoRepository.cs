using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eCommerce.Entities;
using eCommerce.Data;
using eCommerce.Repositories.Interfaces;

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

    }
}
