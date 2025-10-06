using eCommerce.Data;
using eCommerce.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eCommerce.Areas.Negocio.Controllers
{
    [Authorize]
    public class PerfilController : BaseNegocioController
    {
        private readonly DbecommerceContext _context;

        public PerfilController(DbecommerceContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var idCliente = GetClienteId(); 

                if (idCliente == 0)
                {
                    TempData["Error"] = "No se pudo identificar el cliente";
                    return RedirectToAction("Index", "Catalogo");
                }

                var cliente = await _context.Clientes
                    .Include(c => c.DireccionesEnvio.Where(d => d.Activo == true))
                    .Include(c => c.Venta)
                    .FirstOrDefaultAsync(c => c.IdCliente == idCliente);

                if (cliente == null)
                {
                    TempData["Error"] = "No se encontró el perfil del usuario";
                    return RedirectToAction("Index", "Catalogo");
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el perfil: {ex.Message}";
                return RedirectToAction("Index", "Catalogo");
            }
        }

        public async Task<IActionResult> Editar()
        {
            try
            {
                var idCliente = GetClienteId(); 

                if (idCliente == 0)
                {
                    TempData["Error"] = "No se pudo identificar el cliente";
                    return RedirectToAction("Index", "Catalogo");
                }

                var cliente = await _context.Clientes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.IdCliente == idCliente);

                if (cliente == null)
                {
                    TempData["Error"] = "No se encontró el perfil del usuario";
                    return RedirectToAction("Index");
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Cliente cliente)
        {
            try
            {
                var idCliente = GetClienteId(); 

                if (idCliente == 0)
                {
                    TempData["Error"] = "No se pudo identificar el cliente";
                    return RedirectToAction("Index", "Catalogo");
                }

                ModelState.Remove("Contraseña");
                ModelState.Remove("FechaRegistro");

                var clienteExistente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.IdCliente == idCliente);

                if (clienteExistente == null)
                {
                    TempData["Error"] = "No se encontró el perfil del usuario";
                    return RedirectToAction("Index");
                }

                clienteExistente.Nombres = cliente.Nombres?.Trim();
                clienteExistente.Apellidos = cliente.Apellidos?.Trim();
                clienteExistente.Correo = cliente.Correo?.Trim();

                await _context.SaveChangesAsync();

                var usuarioAsociado = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.IdCliente == idCliente);

                if (usuarioAsociado != null)
                {
                    usuarioAsociado.Nombres = clienteExistente.Nombres;
                    usuarioAsociado.Apellidos = clienteExistente.Apellidos;
                    usuarioAsociado.Correo = clienteExistente.Correo;

                    _context.Update(usuarioAsociado);
                    await _context.SaveChangesAsync();

                    await RefreshClaims(usuarioAsociado);
                }

                TempData["Success"] = "Perfil actualizado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException?.Message ?? ex.Message;
                TempData["Error"] = $"Error: {errorMsg}";
                return View(cliente);
            }
        }

        public async Task<IActionResult> MisCompras()
        {
            try
            {
                var idCliente = GetClienteId(); 

                if (idCliente == 0)
                {
                    TempData["Error"] = "No se pudo identificar el cliente";
                    return RedirectToAction("Index", "Catalogo");
                }

                var compras = await _context.Ventas
                    .Include(v => v.DetalleVenta)
                        .ThenInclude(d => d.IdProductoNavigation)
                    .Include(v => v.IdEstadoPedidoNavigation)
                    .Include(v => v.IdDireccionEnvioNavigation)
                    .Where(v => v.IdCliente == idCliente)
                    .OrderByDescending(v => v.FechaVenta)
                    .ToListAsync();

                return View(compras);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar las compras: " + ex.Message;
                return RedirectToAction("Index", "Catalogo");
            }
        }

        public async Task<IActionResult> DetalleCompra(int id)
        {
            try
            {
                var idCliente = GetClienteId(); 

                if (idCliente == 0)
                {
                    TempData["Error"] = "No se pudo identificar el cliente";
                    return RedirectToAction("Index", "Catalogo");
                }

                var venta = await _context.Ventas
                    .Include(v => v.DetalleVenta)
                        .ThenInclude(d => d.IdProductoNavigation)
                            .ThenInclude(p => p.IdMarcaNavigation)
                    .Include(v => v.IdEstadoPedidoNavigation)
                    .Include(v => v.IdDireccionEnvioNavigation)
                    .Include(v => v.IdMetodoPagoNavigation)
                    .Include(v => v.IdCuponNavigation)
                    .Include(v => v.HistorialPedidos)
                        .ThenInclude(h => h.IdEstadoPedidoNavigation)
                    .FirstOrDefaultAsync(v => v.IdVenta == id && v.IdCliente == idCliente);

                if (venta == null)
                {
                    TempData["Error"] = "Compra no encontrada";
                    return RedirectToAction("MisCompras");
                }

                return View(venta);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el detalle: " + ex.Message;
                return RedirectToAction("MisCompras");
            }
        }

        public async Task<IActionResult> MisDirecciones()
        {
            try
            {
                var idCliente = GetClienteId(); 

                if (idCliente == 0)
                {
                    TempData["Error"] = "No se pudo identificar el cliente";
                    return RedirectToAction("Index", "Catalogo");
                }

                var direcciones = await _context.DireccionesEnvio
                    .Where(d => d.IdCliente == idCliente && d.Activo == true)
                    .OrderByDescending(d => d.EsDireccionPrincipal)
                    .ThenByDescending(d => d.FechaRegistro)
                    .ToListAsync();

                return View(direcciones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar las direcciones: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarDireccion(int id)
        {
            try
            {
                var idCliente = GetClienteId(); 

                if (idCliente == 0)
                {
                    TempData["Error"] = "No se pudo identificar el cliente";
                    return RedirectToAction("Index", "Catalogo");
                }

                var direccion = await _context.DireccionesEnvio
                    .FirstOrDefaultAsync(d => d.IdDireccionEnvio == id && d.IdCliente == idCliente);

                if (direccion == null)
                {
                    TempData["Error"] = "Dirección no encontrada";
                    return RedirectToAction("MisDirecciones");
                }

                direccion.Activo = false;
                _context.Update(direccion);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Dirección eliminada exitosamente";
                return RedirectToAction("MisDirecciones");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la dirección: " + ex.Message;
                return RedirectToAction("MisDirecciones");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EstablecerPredeterminada(int id)
        {
            try
            {
                var idCliente = GetClienteId(); 

                if (idCliente == 0)
                {
                    TempData["Error"] = "No se pudo identificar el cliente";
                    return RedirectToAction("Index", "Catalogo");
                }

                var direccionesCliente = await _context.DireccionesEnvio
                    .Where(d => d.IdCliente == idCliente && d.Activo == true)
                    .ToListAsync();

                foreach (var dir in direccionesCliente)
                {
                    dir.EsDireccionPrincipal = false;
                }

                var direccion = direccionesCliente.FirstOrDefault(d => d.IdDireccionEnvio == id);
                if (direccion != null)
                {
                    direccion.EsDireccionPrincipal = true;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Dirección establecida como predeterminada";
                }
                else
                {
                    TempData["Error"] = "Dirección no encontrada";
                }

                return RedirectToAction("MisDirecciones");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al establecer dirección predeterminada: " + ex.Message;
                return RedirectToAction("MisDirecciones");
            }
        }

        #region Métodos Helper

        
        private async Task RefreshClaims(Usuario usuario)
        {
            try
            {
                if (!usuario.IdCliente.HasValue) return;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombres),
                    new Claim(ClaimTypes.Email, usuario.Correo),
                    new Claim(ClaimTypes.Role, usuario.Rol),
                    new Claim("IdCliente", usuario.IdCliente.Value.ToString()),
                    new Claim("IdUsuario", usuario.IdUsuario.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties { IsPersistent = true }
                );
            }
            catch (Exception)
            {
                
            }
        }

        #endregion
    }
}