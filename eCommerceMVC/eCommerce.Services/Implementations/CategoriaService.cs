using eCommerce.Entities;
using eCommerce.Repositories.Interfaces;
using eCommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return await _categoriaRepository.Query()
                .Include(c => c.SubCategorias)
                .Include(c => c.CategoriaPadre)
                .ToListAsync();
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
            if (categorias.Any(c => c.Descripcion.ToLower() == categoria.Descripcion.ToLower() && c.IdCategoria != categoria.IdCategoria))
                return false;

            await _categoriaRepository.UpdateAsync(categoria);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null) return false;

            if (categoria.Productos != null && categoria.Productos.Any())
                return false;

            await _categoriaRepository.DeleteAsync(id);
            return true;
        }

        
        public async Task<IEnumerable<Categoria>> GetCategoriasPrincipalesAsync()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            return categorias.Where(c => c.IdCategoriaPadre == null && c.Activo).ToList();
        }

        public async Task<IEnumerable<Categoria>> GetSubCategoriasAsync(int idCategoriaPadre)
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            return categorias.Where(c => c.IdCategoriaPadre == idCategoriaPadre && c.Activo).ToList();
        }

        
        public async Task<IEnumerable<int>> ObtenerIdsConSubcategoriasAsync(int idCategoria)
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            var dic = categorias.ToDictionary(c => c.IdCategoria, c => c.SubCategorias);

            List<int> ObtenerRecursivo(int id)
            {
                var ids = new List<int> { id };
                if (dic.ContainsKey(id))
                {
                    foreach (var sub in dic[id])
                    {
                        ids.AddRange(ObtenerRecursivo(sub.IdCategoria));
                    }
                }
                return ids;
            }

            return ObtenerRecursivo(idCategoria);
        }
    }
}
