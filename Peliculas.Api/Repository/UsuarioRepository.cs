using System;
using System.Collections.Generic;
using System.Linq;
using Peliculas.Api.Data;
using Peliculas.Api.Models;
using Peliculas.Api.Repository.IRepository;

namespace Peliculas.Api.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ExisteUsuario(string usuario)
        {
            if (_context.Usuarios.Any(x => x.UsuarioA == usuario))
            {
                return true;
            }
            return false;
        }

        public Usuario GetUsuario(int UsuarioId)
        {
            return _context.Usuarios.FirstOrDefault(c => c.Id == UsuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _context.Usuarios.OrderBy(c => c.UsuarioA).ToList();
        }

        public bool Guardar()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }

        public Usuario Login(string usuario, string password)
        {
            var user = _context.Usuarios.FirstOrDefault(x => x.UsuarioA == usuario);

            if (user == null)
            {
                return null;
            }

            if (!VerificaPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;
        }

        public Usuario Registro(Usuario usuario, string password)
        {
            byte[] passwordHash, passwordSalt;

            CrearPasswordHash(password, out passwordHash, out passwordSalt);

            usuario.PasswordHash = passwordHash;
            usuario.PasswordSalt = passwordSalt;

            _context.Usuarios.Add(usuario);
            Guardar();

            return usuario;
        }

        private bool VerificaPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var hashComputado = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < hashComputado.Length; i++)
                {
                    if (hashComputado[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        private void CrearPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
