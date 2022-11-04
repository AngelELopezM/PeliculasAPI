using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerosController : CustomBaseController
    {
        public GenerosController(ApplicationDbContext context, 
                                IMapper mapper):base(context, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> Get()
            => await Get<Genero, GeneroDTO>();

        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        => await Get<Genero, GeneroDTO>(id);

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoToCreate)
        => await Post<GeneroCreacionDTO, Genero, GeneroDTO>(generoToCreate, "obtenerGenero");

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generoToUpdate)
        => await Put<GeneroCreacionDTO, Genero>(id, generoToUpdate);

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="admin")]
        public async Task<ActionResult> Delete(int id)
        => await Delete<Genero>(id);
    }
}
