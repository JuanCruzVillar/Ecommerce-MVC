using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eCommerce.Entities;
using eCommerce.Data;
using eCommerce.Repositories.Interfaces;

namespace eCommerce.Repositories.Implementations
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DbecommerceContext _context;

        public UsuarioRepository(DbecommerceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios
                .Include(u => u.IdClienteNavigation)
                .ToListAsync();
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.IdClienteNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            System.Diagnostics.Debug.WriteLine("=== REPOSITORY UPDATE ===");
            System.Diagnostics.Debug.WriteLine($"Usuario a actualizar - ID: {usuario.IdUsuario}");
            System.Diagnostics.Debug.WriteLine($"Nombres: '{usuario.Nombres}'");
            System.Diagnostics.Debug.WriteLine($"Apellidos: '{usuario.Apellidos}'");
            System.Diagnostics.Debug.WriteLine($"Rol: '{usuario.Rol}'");

            // Detach cualquier instancia trackeada
            var local = _context.Set<Usuario>()
                .Local
                .FirstOrDefault(u => u.IdUsuario == usuario.IdUsuario);

            if (local != null)
            {
                System.Diagnostics.Debug.WriteLine("Entidad local encontrada, haciendo detach");
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(usuario).State = EntityState.Modified;

            System.Diagnostics.Debug.WriteLine("Llamando a SaveChangesAsync...");
            var changes = await _context.SaveChangesAsync();
            System.Diagnostics.Debug.WriteLine($"Cambios guardados: {changes}");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var usuario = await GetByIdAsync(id);
                if (usuario == null) return false;

               
                var carritos = _context.Carritos.Where(c => c.IdCliente == usuario.IdCliente);
                _context.Carritos.RemoveRange(carritos);

                
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}