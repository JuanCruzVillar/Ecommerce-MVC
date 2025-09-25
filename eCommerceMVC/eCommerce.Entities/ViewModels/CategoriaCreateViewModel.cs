namespace eCommerceMVC.eCommerce.Entities.ViewModels
{
    public class CategoriaCreateViewModel
    {

        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
        public int? IdCategoriaPadre { get; set; }

       
        public List<string> SubCategorias { get; set; } = new();
    }
}
