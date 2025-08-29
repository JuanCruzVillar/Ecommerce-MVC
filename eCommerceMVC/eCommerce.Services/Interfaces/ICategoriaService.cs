using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Entities;

namespace eCommerce.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<Categoria>> GetAllAsync();               // Para uso normal
        Task<IEnumerable<Categoria>> GetAllAsyncNoTracking();     // Para lectura sin tracking
        Task<Categoria> GetByIdAsync(int id);
        Task<bool> CreateAsync(Categoria categoria);
        Task<bool> UpdateAsync(Categoria categoria);
        Task<bool> DeleteAsync(int id);
    }
}
