using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Entities;

namespace eCommerce.Services.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<Cliente> GetByIdAsync(int id);
        Task<bool> CreateAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task<bool> DeleteAsync(int id);
    }
}
