using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Entities;

namespace eCommerce.Services.Interfaces
{
    public interface IMarcaService
    {
        Task<IEnumerable<Marca>> GetAllAsync();
        Task<Marca> GetByIdAsync(int id);
        Task<bool> CreateAsync(Marca marca);
        Task<bool> UpdateAsync(Marca marca);
        Task<bool> DeleteAsync(int id);
    }
}
