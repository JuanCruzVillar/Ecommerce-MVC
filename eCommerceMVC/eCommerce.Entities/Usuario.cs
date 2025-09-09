using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Entities;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int? IdCliente { get; set; }
    [Required]
    [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "Solo se permiten letras en el nombre")]
    public string? Nombres { get; set; }
    [Required]
    [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "Solo se permiten letras en el apellido")]
    public string? Apellidos { get; set; }

    public string? Correo { get; set; }

    public string? Contraseña { get; set; }

    public bool? Restablecer { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime? FechaRegistro { get; set; }

    public string Rol { get; set; } = "Cliente";

    public virtual Cliente? IdClienteNavigation { get; set; }
}
