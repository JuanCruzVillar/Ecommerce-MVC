using eCommerce.Entities;

namespace eCommerce.Entities
{
    public class DireccionEnvio
    {
        public int IdDireccionEnvio { get; set; }

        public int IdCliente { get; set; }

        public string NombreCompleto { get; set; }

        public string Direccion { get; set; }

        public string? Referencias { get; set; }

        public string Ciudad { get; set; }

        public string Provincia { get; set; }

        public string CodigoPostal { get; set; }

        public string Telefono { get; set; }

        public bool EsDireccionPrincipal { get; set; } = false;

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Navegación
        public virtual Cliente IdClienteNavigation { get; set; }
    }
}
