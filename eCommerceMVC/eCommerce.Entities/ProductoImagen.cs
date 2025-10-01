using System;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Entities
{
    public partial class ProductoImagen
    {
        public int IdImagen { get; set; }

        [Required]
        public int IdProducto { get; set; }

        [MaxLength(200)]
        public string? RutaImagen { get; set; }

        [MaxLength(100)]
        public string? NombreImagen { get; set; }

        public int? Orden { get; set; }

        public bool? EsPrincipal { get; set; }

        public bool? Activo { get; set; }

        public DateTime? FechaRegistro { get; set; }

        
        public virtual Producto? IdProductoNavigation { get; set; }
    }
}