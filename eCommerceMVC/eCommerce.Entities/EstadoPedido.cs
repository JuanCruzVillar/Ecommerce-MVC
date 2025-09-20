using eCommerce.Entities;

namespace eCommerce.Entities
{
    public class EstadoPedido
    {
        public int IdEstadoPedido { get; set; }

        public string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public int Orden { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Navegación
        public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
        public virtual ICollection<HistorialPedido> HistorialPedidos { get; set; } = new List<HistorialPedido>();
    }
}
