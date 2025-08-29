using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eCommerce.Entities;
using eCommerce.Data;
using eCommerce.Repositories.Interfaces;

namespace eCommerce.Repositories.Implementations
{
    public class MarcaRepository : IMarcaRepository
    {
        private readonly DbecommerceContext _context;

        public MarcaRepository(DbecommerceContext context)
        {
            _context = context;
        }

        // Listado de marcas rastreadas (para editar o eliminar)
        public async Task<IEnumerable<Marca>> GetAllAsync()
        {
            return await _context.Marcas.ToListAsync();
        }

        // Listado de marcas sin tracking (para validaciones de duplicados)
        public async Task<IEnumerable<Marca>> GetAllAsyncNoTracking()
        {
            return await _context.Marcas.AsNoTracking().ToListAsync();
        }

        // Obtener marca por Id incluyendo productos
        public async Task<Marca> GetByIdAsync(int id)
        {
            return await _context.Marcas
                                 .Include(m => m.Productos)
                                 .FirstOrDefaultAsync(m => m.IdMarca == id);
        }

        // Agregar marca
        public async Task AddAsync(Marca marca)
        {
            await _context.Marcas.AddAsync(marca);
            await _context.SaveChangesAsync();
        }

        // Actualizar marca de forma segura evitando conflicto de tracking
        public async Task UpdateAsync(Marca marca)
        {
            var marcaDb = await _context.Marcas.FindAsync(marca.IdMarca);
            if (marcaDb == null) return;

            // Copiar propiedades necesarias
            marcaDb.Descripcion = marca.Descripcion;
            marcaDb.Activo = marca.Activo;

            await _context.SaveChangesAsync();
        }

        // Eliminar marca
        public async Task DeleteAsync(int id)
        {
            var marca = await GetByIdAsync(id);
            if (marca != null)
            {
                _context.Marcas.Remove(marca);
                await _context.SaveChangesAsync();
            }
        }
    }
}
