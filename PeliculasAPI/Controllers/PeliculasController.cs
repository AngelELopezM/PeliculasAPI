using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using PeliculasAPI.Servicios;
using System.Linq.Dynamic.Core;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculasController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAlmacenadorArchivos _almacenadorArchivos;
        private readonly string _contenedor = "peliculas";

        public PeliculasController(ApplicationDbContext context,
                                    IMapper mapper,
                                    IAlmacenadorArchivos almacenadorArchivos)
                                    :base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<PeliculasIndexDTO>> Get()
        {
            var top = 5;
            var hoy = DateTime.Today;

            var proximosEstrenos = await _context.Peliculas.Where(x => x.FechaEstreno > hoy)
                                                            .OrderBy(x => x.FechaEstreno)
                                                            .Take(top)
                                                            .ToListAsync();

            var enCines = await _context.Peliculas.Where(x => x.EnCines)
                                                  .Take(top)
                                                  .ToListAsync();

            var resultado = new PeliculasIndexDTO();
            resultado.FuturosEstrenos = _mapper.Map<List<PeliculaDTO>>(proximosEstrenos);
            resultado.EnCines = _mapper.Map<List<PeliculaDTO>>(enCines);

            return resultado;
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] FiltroPeliculasDTO filtroPeliculas)
        {
            var peliculasQueryable = _context.Peliculas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroPeliculas.Titulo))
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculas.Titulo));

            if (filtroPeliculas.EnCines)
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);

            if (filtroPeliculas.ProximosEstrenos)
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > DateTime.Today);

            if (filtroPeliculas.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.PeliculasGeneros.Select(y => y.GeneroId)
                                                        .Contains(filtroPeliculas.GeneroId));
            }
            if (!string.IsNullOrWhiteSpace(filtroPeliculas.CampoOrdenar))
            {
                var tipoOrder = filtroPeliculas.OrdenAscendente ? "ascending" : "descending";
                try
                {
                    //Aqui utilizamos la libreria de System.linq.Dynamic.Core para hacer este orderby mas corto
                    peliculasQueryable = peliculasQueryable.OrderBy($"{filtroPeliculas.CampoOrdenar} {tipoOrder}");
                }
                catch (Exception) { }
            }

            await HttpContext.InsertarParametrosPaginacion(peliculasQueryable,
                                                           filtroPeliculas.CantidadRegistrosPorPagina);

            var peliculas = await peliculasQueryable.Paginar(filtroPeliculas.Paginacion).ToListAsync();

            return _mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpGet("{id:int}", Name = "obtenerPelicula")]
        public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            var pelicula = await _context.Peliculas
                                         .Include(x => x.PeliculasActores).ThenInclude(x => x.Actor)
                                         .Include(x => x.PeliculasGeneros).ThenInclude(x => x.Genero)
                                         .FirstOrDefaultAsync(x => x.Id == id);
            if (pelicula is null)
                return NotFound();

            return _mapper.Map<PeliculaDetallesDTO>(pelicula);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculasCreacionDTO peliculaToCreate)
        {
            var pelicula = _mapper.Map<Pelicula>(peliculaToCreate);

            if (peliculaToCreate.Poster != null)
            {
                using var memoryStream = new MemoryStream();
                await peliculaToCreate.Poster.CopyToAsync(memoryStream);
                var contenido = memoryStream.ToArray();
                var extension = Path.GetExtension(peliculaToCreate.Poster.FileName);
                pelicula.Poster = await _almacenadorArchivos.GuardarArchivo(contenido,
                                                                            extension,
                                                                            _contenedor,
                                                                            peliculaToCreate.Poster.ContentType);
            }

            _context.Peliculas.Add(pelicula);
            await _context.SaveChangesAsync();
            var peliculaDTO = _mapper.Map<PeliculaDTO>(pelicula);

            return new CreatedAtRouteResult("obtenerPelicula", new { id = pelicula.Id }, peliculaDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculasCreacionDTO peliculaToCreate)
        {
            var peliculaDB = await _context.Peliculas.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (peliculaDB == null)
                return NotFound();

            peliculaDB = _mapper.Map(peliculaToCreate, peliculaDB);

            if (peliculaDB.Poster != null)
            {
                using var memoryStream = new MemoryStream();
                await peliculaToCreate.Poster.CopyToAsync(memoryStream);
                var contenido = memoryStream.ToArray();
                var extension = Path.GetExtension(peliculaToCreate.Poster.FileName);
                peliculaDB.Poster = await _almacenadorArchivos.EditarArchivo(contenido, extension,
                                                                        _contenedor,
                                                                        peliculaDB.Poster,
                                                                        peliculaToCreate.Poster.ContentType);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        => await Delete<Pelicula>(id);
    }
}
