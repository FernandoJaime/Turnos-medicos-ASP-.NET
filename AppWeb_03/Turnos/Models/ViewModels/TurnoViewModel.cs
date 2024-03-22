using System.ComponentModel.DataAnnotations;

namespace Turnos.Models.ViewModels
{
    public class TurnoViewModel
    {
        [Required]
        [MaxLength(12)]
        [Display(Name = "Código:")]
        public string CodigoTurno { get; set; }

        [Required]
        [Display(Name = "Razon del turno:")]
        public string RazonDeTurno { get; set; }

        [Required]
        [DataType(DataType.Date, ErrorMessage = "El formato de la fecha no es válido.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true, NullDisplayText = "Ingrese una fecha válida.")]
        public DateTime FechaTurno { get; set; }

        [Required]
        [Display(Name = "Profesionales:")]
        public int ProfesionalId { get; set; }

        [Required]
        [Display(Name = "Clientes:")]
        public int ClienteId { get; set; }
    }
}
