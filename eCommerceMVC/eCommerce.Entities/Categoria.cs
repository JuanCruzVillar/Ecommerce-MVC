using System;
using System.Collections.Generic;

namespace eCommerce.Entities;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime? FechaRegistro { get; set; }

    public int? IdCategoriaPadre { get; set; }

    public virtual Categoria IdCategoriaPadreNavigation { get; set; }
    public virtual Categoria? CategoriaPadre { get; set; } 
    public virtual ICollection<Categoria> SubCategorias { get; set; } = new List<Categoria>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual ICollection<Categoria> CategoriasHijas { get; set; }
           = new List<Categoria>();
}
