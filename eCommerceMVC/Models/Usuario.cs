using System;
using System.Collections.Generic;

namespace eCommerceMVC.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int? IdCliente { get; set; }

    public string? Nombres { get; set; }

    public string? Apellidos { get; set; }

    public string? Correo { get; set; }

    public string? Contraseña { get; set; }

    public bool? Restablecer { get; set; }

    public bool? Activo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }
}
