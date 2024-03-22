using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Turnos.Models;

public partial class Profesionale
{
    public int IdProfesional { get; set; }

    [Required]
    [MaxLength(12)]
    public string CodigoProfesional { get; set; } = null!;

    [Required]
    [MaxLength(20)] // Máximo 20 caracteres
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre no puede contener números")]
    public string NombreProfesional { get; set; } = null!;

    [Required]
    [MaxLength(20)] // Máximo 20 caracteres
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El apellido no puede contener números")]
    public string ApellidoProfesional { get; set; } = null!;

    [Required]
    [DataType(DataType.Date, ErrorMessage = "El formato de la fecha no es válido.")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true, NullDisplayText = "Ingrese una fecha válida.")]
    public DateTime FechaDeRecibimiento { get; set; }

    public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
