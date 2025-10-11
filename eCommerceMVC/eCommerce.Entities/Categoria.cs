using System;
using System.Collections.Generic;

namespace eCommerce.Entities;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime? FechaRegistro { get; set; }

    // Foreign key para la categoría padre
    public int? IdCategoriaPadre { get; set; }

    // Navegación a la categoría padre (relación autorreferenciada)
    public virtual Categoria? CategoriaPadre { get; set; }

    // Colección de subcategorías (hijas)
    public virtual ICollection<Categoria> SubCategorias { get; set; } = new List<Categoria>();

    // Relación con productos
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}