using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests
{
    internal class usuarioFalsoFiltro : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "ejemplo@ejemplo.com"),
                new Claim(ClaimTypes.Email, "ejemplo@ejemplo.com"),
                new Claim(ClaimTypes.NameIdentifier, "9722b56a-77ea-941d-e319b6eb3712")
            }));

            await next();
        }
    }
}
