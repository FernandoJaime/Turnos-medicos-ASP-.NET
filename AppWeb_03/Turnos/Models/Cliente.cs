using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Turnos.Models;

public partial class Cliente
{
    public int IdClientes { get; set; }

    [Required]
    [MaxLength(12)]
    [Display(Name = "Código")]
    public string CodigoCliente { get; set; } = null!;

    [Required]
    [MaxLength(20)] // Máximo 20 caracteres
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre no puede contener números")]
    [Display(Name = "Nombre")]
    public string NombreCliente { get; set; } = null!;

    [Required]
    [MaxLength(20)] // Máximo 20 caracteres
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El apellido no puede contener números")]
    [Display(Name = "Apellido")]
    public string ApellidoCliente { get; set; } = null!;

    [Required]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [Display(Name = "Email")]
    public string? EmailCliente { get; set; }

    [Required]
    [Display(Name = "Obra social")]
    public int IdObraSocialCliente { get; set; }

    public virtual Obrasocial IdObraSocialClienteNavigation { get; set; } = null!;

    public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
