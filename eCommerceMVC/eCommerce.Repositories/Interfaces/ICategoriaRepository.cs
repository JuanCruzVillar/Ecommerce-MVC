using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Entities;


namespace eCommerce.Repositories.Interfaces

{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetAllAsync();

        Task<IEnumerable<Categoria>> GetAllAsyncNoTracking();
        Task<Categoria> GetByIdAsync(int id);
        Task AddAsync(Categoria categoria);
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(int id);
    }

}