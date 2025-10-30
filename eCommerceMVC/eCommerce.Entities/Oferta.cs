using System;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Entities
{
    public class Oferta
    {
        public int IdOferta { get; set; }

        [Required]
        public int IdProducto { get; set; }

        [Required]
        public decimal PrecioOferta { get; set; }

        public int? PorcentajeDescuento { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Navegación
        public virtual Producto IdProductoNavigation { get; set; }

        // Métodos helper
        public bool EstaVigente()
        {
            var ahora = DateTime.Now;
            return Activo && ahora >= FechaInicio && ahora <= FechaFin;
        }

        public decimal CalcularDescuento(decimal precioOriginal)
        {
            return precioOriginal - PrecioOferta;
        }
    }
}
