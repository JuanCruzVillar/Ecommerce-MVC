using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Entities;

namespace eCommerce.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<Categoria>> GetAllAsync();               
        Task<IEnumerable<Categoria>> GetAllAsyncNoTracking();     
        Task<Categoria> GetByIdAsync(int id);
        Task<bool> CreateAsync(Categoria categoria);
        Task<bool> UpdateAsync(Categoria categoria);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<Categoria>> GetCategoriasPrincipalesAsync();  
        Task<IEnumerable<Categoria>> GetSubCategoriasAsync(int idCategoriaPadre); 
        Task<IEnumerable<int>> ObtenerIdsConSubcategoriasAsync(int idCategoria);
    }
}
