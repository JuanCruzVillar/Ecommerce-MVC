using eCommerce.Entities;

namespace eCommerce.Entities
{
    public class Cupon
    {
        public int IdCupon { get; set; }

        public string Codigo { get; set; }

        public string? Descripcion { get; set; }

        public decimal DescuentoFijo { get; set; } = 0;

        public decimal DescuentoPorcentaje { get; set; } = 0;

        public decimal MontoMinimo { get; set; } = 0;

        public int UsosMaximos { get; set; } = 1;

        public int UsosActuales { get; set; } = 0;

        public DateTime FechaInicio { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Navegación
        public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
