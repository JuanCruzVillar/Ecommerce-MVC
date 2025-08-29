using eCommerce.Entities;
using eCommerce.Repositories;
using eCommerce.Repositories.Interfaces;
using eCommerce.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Services.Implementations
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _clienteRepository.GetAllAsync();
        }

        public async Task<Cliente> GetByIdAsync(int id)
        {
            return await _clienteRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(Cliente cliente)
        {
            var clientes = await _clienteRepository.GetAllAsync();
            if (clientes.Any(c => c.Correo?.ToLower() == cliente.Correo?.ToLower()))
                return false;

            await _clienteRepository.AddAsync(cliente);
            return true;
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null) return false;

            await _clienteRepository.DeleteAsync(id);
            return true;
        }
    }
}
