using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace PeliculasAPI.Helpers
{
    public class PeliculaExisteAttibute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDbContext _context;

        public PeliculaExisteAttibute(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var peliculaIdObject = context.HttpContext.Request.RouteValues["peliculaId"];

            if (peliculaIdObject is null)
                return;

            var peliculaId = int.Parse(peliculaIdObject.ToString());

            var existePelicula = await _context.Peliculas.AnyAsync(x => x.Id == peliculaId);
            if (!existePelicula)
                context.Result = new NotFoundResult();
            else
                await next();
        }
    }
}
