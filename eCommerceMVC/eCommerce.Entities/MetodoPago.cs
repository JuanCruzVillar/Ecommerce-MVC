using eCommerce.Entities;

namespace eCommerce.Entities
{
    public class MetodoPago
    {
        public int IdMetodoPago { get; set; }

        public string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public bool RequiereDatosAdicionales { get; set; } = false;

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Navegación
        public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
