using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eCommerce.Entities;
using eCommerce.Data;
using eCommerce.Repositories.Interfaces;

namespace eCommerce.Repositories.Implementations
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly DbecommerceContext _context;

        public CategoriaRepository(DbecommerceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            return await _context.Categorias.Include(c => c.Productos).ToListAsync();
        }

        public async Task<IEnumerable<Categoria>> GetAllAsyncNoTracking()
        {
            return await _context.Categorias.AsNoTracking().Include(c => c.Productos).ToListAsync();
        }

        public async Task<Categoria> GetByIdAsync(int id)
        {
            return await _context.Categorias
                .Include(c => c.Productos)          
                .Include(c => c.SubCategorias)      
                .Include(c => c.CategoriaPadre)     
                .FirstOrDefaultAsync(c => c.IdCategoria == id);
        }

        public async Task AddAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categoria = await _context.Categorias
                                          .Include(c => c.SubCategorias)
                                          .Include(c => c.Productos)
                                          .FirstOrDefaultAsync(c => c.IdCategoria == id);

            if (categoria == null)
                return false;

            // no borrar si tiene productos o subcategorías
            if ((categoria.SubCategorias != null && categoria.SubCategorias.Any()) ||
                (categoria.Productos != null && categoria.Productos.Any()))
            {
                return false; 
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return true; 
        }

        public IQueryable<Categoria> Query()
        {
            return _context.Categorias.AsQueryable();
        }
    }



}

