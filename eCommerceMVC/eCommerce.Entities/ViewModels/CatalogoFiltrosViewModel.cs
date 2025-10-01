namespace eCommerce.Entities.ViewModels
{
    public class CatalogoFiltrosViewModel
    {
        // Productos filtrados
        public List<DetalleProductoViewModel> Productos { get; set; } = new();

        // Filtros aplicados
        public string BusquedaTexto { get; set; }
        public int? CategoriaId { get; set; }
        public int? MarcaId { get; set; }
        public decimal? PrecioMin { get; set; }
        public decimal? PrecioMax { get; set; }
        public string Ordenamiento { get; set; } 

        // Datos para los filtros
        public List<FiltroItem> Categorias { get; set; } = new();
        public List<FiltroItem> Marcas { get; set; } = new();
        public decimal PrecioMinimoDisponible { get; set; }
        public decimal PrecioMaximoDisponible { get; set; }

        // Información adicional
        public string CategoriaNombre { get; set; }
        public int TotalResultados { get; set; }
    }

    public class FiltroItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; } // Cantidad de productos en esa categoría/marca
    }
}

