using eCommerce.Entities;
using eCommerce.Repositories;
using eCommerce.Repositories.Interfaces;
using eCommerce.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Services.Implementations


{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            return await _usuarioRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(Usuario usuario)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            if (usuarios.Any(u => u.Correo.ToLower() == usuario.Correo.ToLower()))
                return false;

            await _usuarioRepository.AddAsync(usuario);
            return true;
        }

        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            if (usuarios.Any(u => u.Correo.ToLower() == usuario.Correo.ToLower() && u.IdUsuario != usuario.IdUsuario))
                return false;

            await _usuarioRepository.UpdateAsync(usuario);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return false;

            await _usuarioRepository.DeleteAsync(id);
            return true;
        }
    }
}
