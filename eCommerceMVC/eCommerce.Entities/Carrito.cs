using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Entities
{
    public partial class Carrito
    {
        [Key]
        public int IdCarrito { get; set; }

        public int? IdCliente { get; set; }

        public int? IdProducto { get; set; }

        public int? Cantidad { get; set; }


        public virtual Producto? IdProductoNavigation { get; set; }

        public virtual Cliente? IdClienteNavigation { get; set; }
    }
}