using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Entities;

namespace eCommerce.Repositories.Interfaces
{
    public interface IProductoRepository
    {

        Task<IEnumerable<Producto>> GetAllWithCategoriasAsync();

        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto> GetByIdAsync(int id);
        Task AddAsync(Producto producto);
        Task UpdateAsync(Producto producto);
        Task DeleteAsync(int id);
    }
}
