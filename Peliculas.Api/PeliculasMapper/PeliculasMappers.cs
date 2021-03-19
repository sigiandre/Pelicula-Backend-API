using System;
using AutoMapper;
using Peliculas.Api.Models;
using Peliculas.Api.Models.Dtos;

namespace Peliculas.Api.PeliculasMapper
{
    public class PeliculasMappers : Profile
    {
        public PeliculasMappers()
        {
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
            CreateMap<Pelicula, PeliculaCreateDto>().ReverseMap();
            CreateMap<Pelicula, PeliculaUpdateDto>().ReverseMap();
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, UsuarioAuthDto>().ReverseMap();
            CreateMap<Usuario, UsuarioAuthLoginDto>().ReverseMap();
        }
    }
}
