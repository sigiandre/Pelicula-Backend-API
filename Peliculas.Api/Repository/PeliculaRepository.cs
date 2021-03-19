using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Peliculas.Api.Data;
using Peliculas.Api.Models;
using Peliculas.Api.Repository.IRepository;

namespace Peliculas.Api.Repository
{
    public class PeliculaRepository : IPeliculaRepository
    {
        private readonly ApplicationDbContext _context;

        public PeliculaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ActualizarPelicula(Pelicula pelicula)
        {
            _context.Peliculas.Update(pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula pelicula)
        {
            _context.Peliculas.Remove(pelicula);
            return Guardar();
        }

        public IEnumerable<Pelicula> BuscarPelicula(string nombre)
        {
            IQueryable<Pelicula> query = _context.Peliculas;

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre));
            }
            return query.ToList();
        }

        public bool CrearPelicula(Pelicula pelicula)
        {
            _context.Peliculas.Add(pelicula);
            return Guardar();
        }

        public bool ExistePelicula(string nombre)
        {
            bool valor = _context.Categorias.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;

        }

        public bool ExistePelicula(int id)
        {
            bool valor = _context.Peliculas.Any(c => c.Id == id);
            return valor;

        }

        public Pelicula GetPelicula(int PeliculaId)
        {
            return _context.Peliculas.FirstOrDefault(c => c.Id == PeliculaId);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return _context.Peliculas.OrderBy(c => c.Nombre).ToList();
        }

        public ICollection<Pelicula> GetPeliculasEnCategoria(int CatId)
        {
            return _context.Peliculas.Include(ca => ca.Categoria).Where(ca => ca.categoriaId ==  CatId).ToList();
        }

        public bool Guardar()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }
    }
}
