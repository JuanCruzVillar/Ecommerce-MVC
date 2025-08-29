using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Entities;
using eCommerce.Repositories.Interfaces;
using eCommerce.Services.Interfaces;

namespace eCommerce.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            return await _categoriaRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Categoria>> GetAllAsyncNoTracking()
        {
            return await _categoriaRepository.GetAllAsyncNoTracking();
        }

        public async Task<Categoria> GetByIdAsync(int id)
        {
            return await _categoriaRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(Categoria categoria)
        {
            var categorias = await _categoriaRepository.GetAllAsyncNoTracking();

            if (categorias.Any(c => c.Descripcion.ToLower() == categoria.Descripcion.ToLower()))
                return false;

            await _categoriaRepository.AddAsync(categoria);
            return true;
        }

        public async Task<bool> UpdateAsync(Categoria categoria)
        {
            var categorias = await _categoriaRepository.GetAllAsyncNoTracking();

            if (categorias.Any(c => c.Descripcion.ToLower() == categoria.Descripcion.ToLower()
                                    && c.IdCategoria != categoria.IdCategoria))
                return false;

            await _categoriaRepository.UpdateAsync(categoria);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);

            if (categoria == null)
                return false;

            if (categoria.Productos != null && categoria.Productos.Any())
                return false;

            await _categoriaRepository.DeleteAsync(id);
            return true;
        }
    }
}
