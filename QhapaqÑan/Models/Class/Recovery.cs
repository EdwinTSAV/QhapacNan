using System.ComponentModel.DataAnnotations;

namespace QhapaqÑan.Models.Class
{
    public class Recovery
    {
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [EmailAddress(ErrorMessage = "El campo no es una dirección de correo electrónico válida")]
        public string Email { get; set; }
    }
}
