using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Turnos.Models;

public partial class Obrasocial
{
    public int IdObraSocial { get; set; }

    [Required]
    [MaxLength(12)]
    public string CodigoObraSocial { get; set; } = null!;

    [Required]
    [MaxLength(20)] // Máximo 20 caracteres
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre no puede contener números")]
    [Display(Name = "Obra social")]
    public string NombreObraSocial { get; set; } = null!;

    [Required]
    [Range(0, 9999999.99)] // Validacion de regular expression no lo pongo porque ya lo tiene por defecto.
    [Display(Name = "Precio")]
    public decimal PrecioObraSocial { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
