using eCommerce.Entities;
using eCommerce.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eCommerce.Areas.Negocio.Controllers
{
    [Area("Negocio")]
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly DbecommerceContext _context;

        public PerfilController(DbecommerceContext context)
        {
            _context = context;
        }

        // GET: Perfil/Index - Mostrar datos del perfil
        public async Task<IActionResult> Index()
        {
            try
            {
                var idCliente = GetClienteId();

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
                TempData["Error"] = "Error al cargar el perfil: " + ex.Message;
                return RedirectToAction("Index", "Catalogo");
            }
        }

        // GET: Perfil/Editar
        public async Task<IActionResult> Editar()
        {
            try
            {
                var idCliente = GetClienteId();

                var cliente = await _context.Clientes
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
        // POST: Perfil/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(eCommerce.Entities.Cliente cliente)
        {
            try
            {
                var idCliente = GetClienteId();

                var clienteExistente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.IdCliente == idCliente);

                if (clienteExistente == null)
                {
                    TempData["Error"] = "No se encontró el perfil del usuario";
                    return RedirectToAction("Index");
                }

                // Actualizar los campos permitidos 
                clienteExistente.Nombres = cliente.Nombres;
                clienteExistente.Apellidos = cliente.Apellidos;
                clienteExistente.Correo = cliente.Correo;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Perfil actualizado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el perfil: " + ex.Message;

               
                var idCliente = GetClienteId();
                var clienteReload = await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == idCliente);
                return View(clienteReload ?? cliente);
            }
        }

        // GET: Perfil/MisCompras - Historial de compras
        public async Task<IActionResult> MisCompras()
        {
            try
            {
                var idCliente = GetClienteId();

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

        // GET: Perfil/DetalleCompra/5
        public async Task<IActionResult> DetalleCompra(int id)
        {
            try
            {
                var idCliente = GetClienteId();

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

        // GET: Perfil/MisDirecciones
        public async Task<IActionResult> MisDirecciones()
        {
            try
            {
                var idCliente = GetClienteId();

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

        // POST: Perfil/EliminarDireccion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarDireccion(int id)
        {
            try
            {
                var idCliente = GetClienteId();

                var direccion = await _context.DireccionesEnvio
                    .FirstOrDefaultAsync(d => d.IdDireccionEnvio == id && d.IdCliente == idCliente);

                if (direccion == null)
                {
                    TempData["Error"] = "Dirección no encontrada";
                    return RedirectToAction("MisDirecciones");
                }

                
                direccion.Activo = false;
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

        // POST: Perfil/EstablecerPredeterminada/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EstablecerPredeterminada(int id)
        {
            try
            {
                var idCliente = GetClienteId();

                
                var direccionesCliente = await _context.DireccionesEnvio
                    .Where(d => d.IdCliente == idCliente && d.Activo == true)
                    .ToListAsync();

                foreach (var dir in direccionesCliente)
                {
                    dir.EsDireccionPrincipal = false;
                }

                // Establecer la nueva dirección predeterminada
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

        private int GetClienteId()
        {
            var usuarioIdClaim = User.FindFirst("IdUsuario")?.Value;
            if (int.TryParse(usuarioIdClaim, out int usuarioId))
            {
                
                var usuario = _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefault(u => u.IdUsuario == usuarioId);

                return usuario?.IdCliente ?? 0;
            }
            return 0;
        }

        #endregion
    }
}