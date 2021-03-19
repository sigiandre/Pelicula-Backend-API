using System;
using System.ComponentModel.DataAnnotations;

namespace Peliculas.Api.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string UsuarioA { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
