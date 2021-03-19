using System;
using System.Collections.Generic;
using Peliculas.Api.Models;

namespace Peliculas.Api.Repository.IRepository
{
    public interface IPeliculaRepository
    {
        ICollection<Pelicula> GetPeliculas();
        ICollection<Pelicula> GetPeliculasEnCategoria(int CatId);
        Pelicula GetPelicula(int PeliculaId);
        bool ExistePelicula(string nombre);
        IEnumerable<Pelicula> BuscarPelicula(string nombre);
        bool ExistePelicula(int id);
        bool CrearPelicula(Pelicula pelicula);
        bool ActualizarPelicula(Pelicula pelicula);
        bool BorrarPelicula(Pelicula pelicula);
        bool Guardar();
    }
}
