using System;
using System.Collections.Generic;

namespace eCommerce.Entities;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime? FechaRegistro { get; set; }


    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
