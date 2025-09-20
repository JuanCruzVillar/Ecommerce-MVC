using eCommerce.Entities;

namespace eCommerce.Entities
{
    public class HistorialPedido
    {
        public int IdHistorialPedido { get; set; }

        public int IdVenta { get; set; }

        public int IdEstadoPedido { get; set; }

        public string? Comentarios { get; set; }

        public DateTime FechaCambio { get; set; } = DateTime.Now;

        public int? IdUsuario { get; set; }

        // Navegación
        public virtual Venta IdVentaNavigation { get; set; }
        public virtual EstadoPedido IdEstadoPedidoNavigation { get; set; }
        public virtual Usuario? IdUsuarioNavigation { get; set; }
    }
}
