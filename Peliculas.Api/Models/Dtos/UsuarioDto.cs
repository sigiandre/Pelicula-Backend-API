using System;
namespace Peliculas.Api.Models.Dtos
{
    public class UsuarioDto
    {
        public string UsuarioA { get; set; }
        public byte[] PasswordHash { get; set; }
    }
}
