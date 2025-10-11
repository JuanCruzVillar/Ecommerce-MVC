using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Entities
{
    public partial class Producto
    {
        public int IdProducto { get; set; }

        public string? Nombre { get; set; }

        public string? Descripcion { get; set; }

        [Required]
        public int? IdMarca { get; set; }

        [Required]
        public int? IdCategoria { get; set; }

        public decimal? Precio { get; set; }

        public int? Stock { get; set; }

        public string? RutaImagen { get; set; }

        public string? NombreImagen { get; set; }

        public bool? Activo { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public virtual ICollection<Carrito> Carritos { get; set; } = new List<Carrito>();

        public virtual ICollection<DetalleVentas> DetalleVenta { get; set; } = new List<DetalleVentas>();

        public virtual Categoria? IdCategoriaNavigation { get; set; }

        public virtual Marca? IdMarcaNavigation { get; set; }

        public virtual ICollection<ProductoEspecificacion> Especificaciones { get; set; } = new List<ProductoEspecificacion>();

        public virtual ICollection<ProductoImagen> Imagenes { get; set; } = new List<ProductoImagen>();

        public virtual ICollection<ProductoEspecificacion> ProductoEspecificaciones { get; set; }
            = new List<ProductoEspecificacion>();
    }
}
