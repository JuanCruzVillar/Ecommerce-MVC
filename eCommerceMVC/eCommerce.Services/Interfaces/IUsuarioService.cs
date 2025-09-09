using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Entities;

namespace eCommerce.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario> GetByIdAsync(int id);
        Task<bool> CreateAsync(Usuario usuario);
        Task<bool> UpdateAsync(Usuario usuario);
        Task<bool> DeleteAsync(int id);
        Task<Usuario> ValidarUsuario(string correo, string contraseña);

        Task<Usuario> GetByCorreoAsync(string correo);

    }
}
