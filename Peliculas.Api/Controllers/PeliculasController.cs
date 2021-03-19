using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Peliculas.Api.Models;
using Peliculas.Api.Models.Dtos;
using Peliculas.Api.Repository.IRepository;

namespace Peliculas.Api.Controllers
{
    [Authorize]
    [Route("api/Peliculas")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliculas")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PeliculasController(IPeliculaRepository peliculaRepository, IMapper mapper, IWebHostEnvironment hostEnvironment)
        {
            _peliculaRepository = peliculaRepository;
            _mapper = mapper;
            _hostingEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Obtener todas las peliculas
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<PeliculaDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = _peliculaRepository.GetPeliculas();
            var listaPeliculasDto = new List<PeliculaDto>();

            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(lista));
            }
            return Ok(listaPeliculasDto);
        }

        /// <summary>
        /// Obtener una pelicula individual
        /// </summary>
        /// <param name="peliculaId">Este es el id de la pelicula</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{peliculaId:int}", Name = "GetPelicula")]
        [ProducesResponseType(200, Type = typeof(PeliculaDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetPelicula(int peliculaId)
        {
            var itemPelicula = _peliculaRepository.GetPelicula(peliculaId);

            if (itemPelicula == null)
            {
                return NotFound();
            }

            var itemPeliculaDto = _mapper.Map<PeliculaDto>(itemPelicula);
            return Ok(itemPeliculaDto);
        }

        /// <summary>
        /// Obtener peliculas en categoria
        /// </summary>
        /// <param name="categoriaId">Este es el id de la pelicula en categoria</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetPeliculasEnCategoria/{categoriaId:int}")]
        [ProducesResponseType(200, Type = typeof(PeliculaDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetPeliculasEnCategoria(int categoriaId)
        {
            var listaPelicula = _peliculaRepository.GetPeliculasEnCategoria(categoriaId);

            if (listaPelicula == null)
            {
                return NotFound();
            }

            var itemPelicula = new List<PeliculaDto>();

            foreach (var item in listaPelicula)
            {
                itemPelicula.Add(_mapper.Map<PeliculaDto>(item));
            }
            return Ok(itemPelicula);
        }

        /// <summary>
        /// Buscar pelicula por nombre
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("Buscar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult Buscar(string nombre)
        {
            try
            {
                var resultado = _peliculaRepository.BuscarPelicula(nombre);

                if (resultado.Any())
                {
                    return Ok(resultado);
                }
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos de la aplicacion");
            }
        }

        /// <summary>
        /// Crear una nueva pelicula
        /// </summary>
        /// <param name="PeliculaDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PeliculaCreateDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearPelicula([FromForm] PeliculaCreateDto PeliculaDto)
        {
            if(PeliculaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_peliculaRepository.ExistePelicula(PeliculaDto.Nombre))
            {
                ModelState.AddModelError("", "La pelicula ya existe");
                return StatusCode(404, ModelState);
            }

            /*Subida de Archivos*/
            var archivo = PeliculaDto.Foto;
            string rutaPrincipal = _hostingEnvironment.WebRootPath;
            var archivos = HttpContext.Request.Form.Files;

            if(archivo.Length > 0)
            {
                //Nueva Imagen
                var nombreFoto = Guid.NewGuid().ToString();
                var subidas = Path.Combine(rutaPrincipal, @"fotos");
                var extension = Path.GetExtension(archivos[0].FileName);

                using (var fileStreams = new FileStream(Path.Combine(subidas, nombreFoto + extension), FileMode.Create))
                {
                    archivos[0].CopyTo(fileStreams);
                }
                PeliculaDto.RutaImagen = @"\fotos\" + nombreFoto + extension;
            }

            var pelicula = _mapper.Map<Pelicula>(PeliculaDto);

            if (!_peliculaRepository.CrearPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id}, pelicula);
        }

        /// <summary>
        /// Actualizar una pelicula existente
        /// </summary>
        /// <param name="peliculaId"></param>
        /// <param name="peliculaDto"></param>
        /// <returns></returns>
        [HttpPatch("{peliculaId:int}", Name = "ActualizarPelicula")]
        [ProducesResponseType(204, Type = typeof(PeliculaDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPelicula(int peliculaId, [FromBody] PeliculaDto peliculaDto)
        {
            if (peliculaDto == null || peliculaId != peliculaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);

            if (!_peliculaRepository.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {pelicula.Nombre} ");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        /// <summary>
        /// Borrar una pelicula existente
        /// </summary>
        /// <param name="peliculaId"></param>
        /// <returns></returns>
        [HttpDelete("{peliculaId:int}", Name = "BorrarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarCategoria(int peliculaId)
        {
            if (!_peliculaRepository.ExistePelicula(peliculaId))
            {
                return NotFound();
            }

            var pelicula = _peliculaRepository.GetPelicula(peliculaId);

            if (!_peliculaRepository.BorrarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro {pelicula.Nombre} ");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
