using Microsoft.AspNetCore.Authorization;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    internal class AllowAnonymousHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (var requirement in context.PendingRequirements.ToList())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
