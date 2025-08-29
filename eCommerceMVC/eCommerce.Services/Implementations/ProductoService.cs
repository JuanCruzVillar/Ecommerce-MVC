using eCommerce.Entities;
using eCommerce.Repositories;
using eCommerce.Repositories.Interfaces;
using eCommerce.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _productoRepository.GetAllAsync();
        }

        public async Task<Producto> GetByIdAsync(int id)
        {
            return await _productoRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(Producto producto)
        {
            await _productoRepository.AddAsync(producto);
            return true;
        }

        public async Task<bool> UpdateAsync(Producto producto)
        {
            await _productoRepository.UpdateAsync(producto);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            if (producto == null) return false;

            await _productoRepository.DeleteAsync(id);
            return true;
        }
    }
}
