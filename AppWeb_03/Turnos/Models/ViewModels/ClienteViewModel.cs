using System.ComponentModel.DataAnnotations;

namespace Turnos.Models.ViewModels
{
    public class ClienteViewModel
    {
        [Required]
        [MaxLength(12)]
        [Display(Name = "Código")]
        public string CodigoCliente { get; set; }

        [Required]
        [MaxLength(20)] // Máximo 20 caracteres
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre no puede contener números")]
        [Display(Name = "Nombre")]
        public string NombreCliente { get; set; }

        [Required]
        [MaxLength(20)] // Máximo 20 caracteres
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El apellido no puede contener números")]
        [Display(Name = "Apellido")]
        public string ApellidoCliente { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [Display(Name = "Email")]
        public string EmailCliente { get; set; }

        [Required]
        [Display(Name = "Obra social")]
        public int IdObraSocialCliente { get; set; }

    }
}
