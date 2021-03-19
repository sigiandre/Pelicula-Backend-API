using System;
using System.ComponentModel.DataAnnotations;

namespace Peliculas.Api.Models.Dtos
{
    public class CategoriaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
