using System;
using System.Collections.Generic;

namespace eCommerceMVC.Models;

public partial class Categorias
{
    public int IdCategoria { get; set; }

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime? FechaRegistro { get; set; }


    public virtual ICollection<Productos> Productos { get; set; } = new List<Productos>();
}
