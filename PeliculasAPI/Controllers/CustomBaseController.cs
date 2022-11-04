using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CustomBaseController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class
        {
            var entidades = await _context.Set<TEntidad>().AsNoTracking().ToListAsync();
            var dtos = _mapper.Map<List<TDTO>>(entidades);
            return dtos;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginacionDTO paginacionDTO) where TEntidad: class
        {
            var queryable = _context.Set<TEntidad>().AsQueryable();
            await HttpContext.InsertarParametrosPaginacion(queryable,
                                                        paginacionDTO.CantidadRegistrosPorPagina);
            var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();
            return _mapper.Map<List<TDTO>>(entidades);
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginacionDTO paginacionDTO,
            IQueryable<TEntidad> queryable)
            where TEntidad : class
        {
            await HttpContext.InsertarParametrosPaginacion(queryable,
                                                        paginacionDTO.CantidadRegistrosPorPagina);
            var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();
            return _mapper.Map<List<TDTO>>(entidades);
        }

        //Aqui le agregamos esa interfaz para poder utilizar el Linq y decirle a linq
        //que las entidades todas tienen un Id porque implementas de IId
        protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id) where TEntidad : class, IId
        {
            var entidad = await _context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (entidad is null)
                return NotFound();

            return _mapper.Map<TDTO>(entidad);
        }

        protected async Task<ActionResult> Post<TCreacion, TEntidad, TLectura>
           (TCreacion creacionDTO, string nombreRuta) where TEntidad : class, IId
        {
            var entidad = _mapper.Map<TEntidad>(creacionDTO);
            _context.Set<TEntidad>().Add(entidad);
            await _context.SaveChangesAsync();

            var dtoLectura = _mapper.Map<TLectura>(entidad);

            return new CreatedAtRouteResult(nombreRuta, new { id = entidad.Id }, dtoLectura);
        }

        protected async Task<ActionResult> Put<TUpdate, TEntidad>(int id, TUpdate generoToUpdate) where TEntidad : class, IId
        {
            var entidad = _mapper.Map<TEntidad>(generoToUpdate);
            entidad.Id = id;
            _context.Set<TEntidad>().Update(entidad);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntidad, TDTO>(int id, JsonPatchDocument<TDTO> patchDocument)
            where TDTO : class
            where TEntidad : class, IId
        {
            if (patchDocument is null)
                return BadRequest();

            var entidad = await _context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (entidad is null)
                return NotFound();

            var entidadDTO = _mapper.Map<TDTO>(entidad);

            patchDocument.ApplyTo(entidadDTO, ModelState);

            var esValido = TryValidateModel(entidadDTO);

            if (!esValido)
                return BadRequest(ModelState);

            _mapper.Map(entidadDTO, entidad);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Aqui utilizamos el new() del final para indicarle que vamos a tener un contructor vacio
        protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad : class, IId, new()
        {
            var existe = await _context.Set<TEntidad>().AnyAsync(x => x.Id.Equals(id));
            if (!existe)
                return NotFound();

            _context.Set<TEntidad>().Remove(new TEntidad() { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
