using eCommerce.Entities;
using eCommerce.Repositories.Interfaces;
using eCommerce.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Services.Implementations
{
    public class MarcaService : IMarcaService
    {
        private readonly IMarcaRepository _marcaRepository;

        public MarcaService(IMarcaRepository marcaRepository)
        {
            _marcaRepository = marcaRepository;
        }

        public async Task<IEnumerable<Marca>> GetAllAsync()
        {
            return await _marcaRepository.GetAllAsync();
        }

        public async Task<Marca> GetByIdAsync(int id)
        {
            return await _marcaRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(Marca marca)
        {
            // Validación duplicados usando NoTracking
            var marcas = await _marcaRepository.GetAllAsyncNoTracking();
            if (marcas.Any(m => m.Descripcion.ToLower() == marca.Descripcion.ToLower()))
                return false;

            await _marcaRepository.AddAsync(marca);
            return true;
        }

        public async Task<bool> UpdateAsync(Marca marca)
        {
            // Validación duplicados usando NoTracking
            var marcas = await _marcaRepository.GetAllAsyncNoTracking();
            if (marcas.Any(m => m.Descripcion.ToLower() == marca.Descripcion.ToLower()
                                && m.IdMarca != marca.IdMarca))
                return false;

            await _marcaRepository.UpdateAsync(marca);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var marca = await _marcaRepository.GetByIdAsync(id);
            if (marca == null) return false;

            // No eliminar si tiene productos
            if (marca.Productos != null && marca.Productos.Any())
                return false;

            await _marcaRepository.DeleteAsync(id);
            return true;
        }
    }
}
