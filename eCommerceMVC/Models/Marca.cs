using System;
using System.Collections.Generic;

namespace eCommerceMVC.Models;

public partial class Marca
{
    public int IdMarca { get; set; }

    public string? Descripcion { get; set; }

    public bool? Activo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Productos> Productos { get; set; } = new List<Productos>();
}
