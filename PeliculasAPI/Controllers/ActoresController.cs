using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using PeliculasAPI.Servicios;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActoresController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAlmacenadorArchivos _almacenadorArchivos;
        //En azure el contenedor viene siendo el nombre de la carpeta
        private readonly string _contenedor = "fotosactores";

        public ActoresController(ApplicationDbContext context, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos) :base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        => await Get<Actor, ActorDTO>(paginacionDTO);

        [HttpGet("{id:int}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        => await Get<Actor, ActorDTO>(id);

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorToCreate)
        {
            var mapperActor = _mapper.Map<Actor>(actorToCreate);

            if (actorToCreate.Foto != null)
            {
                using var memoryStream = new MemoryStream();
                await actorToCreate.Foto.CopyToAsync(memoryStream);
                var contenido = memoryStream.ToArray();
                var extension = Path.GetExtension(actorToCreate.Foto.FileName);
                mapperActor.Foto = await _almacenadorArchivos.GuardarArchivo(contenido,
                                                                            extension,
                                                                            _contenedor,
                                                                            actorToCreate.Foto.ContentType);
            }

            _context.Actores.Add(mapperActor);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<ActorDTO>(mapperActor);

            return new CreatedAtRouteResult("obtenerActor", new { id = mapperActor.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Post(int id, [FromForm] ActorCreacionDTO actorToUpdate)
        {
            var actorDB = await _context.Actores.FirstOrDefaultAsync(x=> x.Id.Equals(id));
            if (actorDB == null)
                return NotFound();

            actorDB = _mapper.Map(actorToUpdate, actorDB);

            if (actorDB.Foto != null)
            {
                using var memoryStream = new MemoryStream();
                await actorToUpdate.Foto.CopyToAsync(memoryStream);
                var contenido = memoryStream.ToArray();
                var extension = Path.GetExtension(actorToUpdate.Foto.FileName);
                actorDB.Foto = await _almacenadorArchivos.EditarArchivo(contenido, extension,
                                                                        _contenedor,
                                                                        actorDB.Foto,
                                                                        actorToUpdate.Foto.ContentType);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        => await Patch<Actor, ActorPatchDTO>(id, patchDocument);

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        => await Delete<Actor>(id);

    }
}
