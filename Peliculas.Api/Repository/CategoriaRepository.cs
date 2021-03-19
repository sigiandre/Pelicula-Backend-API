using System;
using System.Collections.Generic;
using System.Linq;
using Peliculas.Api.Data;
using Peliculas.Api.Models;
using Peliculas.Api.Repository.IRepository;

namespace Peliculas.Api.Repository
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ActualizarCategoria(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            _context.Categorias.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(string nombre)
        {
            bool valor = _context.Categorias.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }

        public bool ExisteCategoria(int id)
        {
            return _context.Categorias.Any(c => c.Id == id);
        }

        public Categoria GetCategoria(int CategoriaId)
        {
            return _context.Categorias.FirstOrDefault(c => c.Id == CategoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _context.Categorias.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }
    }
}