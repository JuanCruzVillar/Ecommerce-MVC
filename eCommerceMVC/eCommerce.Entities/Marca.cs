using System;
using System.Collections.Generic;

namespace eCommerce.Entities;

public partial class Marca
{
    public int IdMarca { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }  // ⚠️ debe ser bool no bool?
    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
