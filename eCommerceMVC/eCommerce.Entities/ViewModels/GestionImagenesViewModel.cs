namespace eCommerce.Entities.ViewModels
{
    public class GestionImagenesViewModel
    {

        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public List<ImagenViewModel> Imagenes { get; set; } = new();
    }
}
