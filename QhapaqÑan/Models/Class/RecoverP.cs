using System.ComponentModel.DataAnnotations;

namespace QhapaqÑan.Models.Class
{
    public class RecoverP
    {
        public string Token { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Contraseña minimo 6 caracteres")]
        [MaxLength(25, ErrorMessage = "Contraseña máximo 25 caracteres")]
        public string Password { get; set; }

        [Compare("Password")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Contraseña minimo 6 caracteres")]
        [MaxLength(25, ErrorMessage = "Contraseña máximo 25 caracteres")]
        public string ConfirmPassword { get; set; }
    }
}
