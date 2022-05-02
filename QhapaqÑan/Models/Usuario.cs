using System;
using System.ComponentModel.DataAnnotations;

namespace QhapaqÑan.Models
{
    public class Usuario
    {
        public string DNI { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Ap_Paterno { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Ap_Materno { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public DateTime Nacimiento { get; set; }

        public string RUC { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [EmailAddress(ErrorMessage = "El campo no es una dirección de correo electrónico válida")]
        public string Correo { get; set; }

        [MinLength(9, ErrorMessage = "Celular minimo 9 caracteres")]
        [MaxLength(9, ErrorMessage = "Celular máximo 9 caracteres")]
        public string Celular { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Contraseña minimo 6 caracteres")]
        [MaxLength(15, ErrorMessage = "Contraseña máximo 15 caracteres")]
        public string Contrasenia { get; set; }

        public string Recovery { get; set; }

        [Required(ErrorMessage = "Debe seleccionar uno")]
        public int Id_Rol { get; set; }
        public Roles Roles { get; set; }
    }
}
