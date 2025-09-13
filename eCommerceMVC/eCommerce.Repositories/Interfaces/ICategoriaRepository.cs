using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Entities;


namespace eCommerce.Repositories.Interfaces

{
    public interface ICategoriaRepository
    {
        IQueryable<Categoria> Query();
        Task<IEnumerable<Categoria>> GetAllAsync();

        Task<IEnumerable<Categoria>> GetAllAsyncNoTracking();
        Task<Categoria> GetByIdAsync(int id);
        Task AddAsync(Categoria categoria);
        Task UpdateAsync(Categoria categoria);
        Task<bool> DeleteAsync(int id);
    }

}