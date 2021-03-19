using System;
using System.ComponentModel.DataAnnotations;

namespace Peliculas.Api.Models.Dtos
{
    public class UsuarioAuthLoginDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "El Password es obligatorio")]
        public string Password { get; set; }
    }
}
