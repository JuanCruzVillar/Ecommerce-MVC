using System;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Entities
{
    public partial class ProductoEspecificacion
    {
        public int IdEspecificacion { get; set; }

        [Required]
        public int IdProducto { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Clave { get; set; }

        [Required]
        [MaxLength(500)]
        public string? Valor { get; set; }

        public int? Orden { get; set; }

        public bool? Activo { get; set; }

        public DateTime? FechaRegistro { get; set; }

        
        public virtual Producto? IdProductoNavigation { get; set; }
    }
}