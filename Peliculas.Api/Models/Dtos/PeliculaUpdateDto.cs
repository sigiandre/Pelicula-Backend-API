using System;
using System.ComponentModel.DataAnnotations;
using static Peliculas.Api.Models.Pelicula;

namespace Peliculas.Api.Models.Dtos
{
    public class PeliculaUpdateDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "La Descripcion es obligatorio")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "La duracion es obligatorio")]
        public string Duracion { get; set; }
        public TipoClasificacion Clasificacion { get; set; }
        public int categoriaId { get; set; }
    }
}
