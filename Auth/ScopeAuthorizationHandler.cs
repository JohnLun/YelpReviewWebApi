using AnimalCrossingAPI.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AnimalCrossingAPI.Auth
{
    public class ScopeAuthorizationHandler : AuthorizationHandler<ScopeRequirement, Review>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement, Review resource)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var scopeClaim = context.User.FindFirst("scope")?.Value;

            if (scopeClaim != null)
            {
                var scopes = scopeClaim.Split(' ');
                if (scopes.Contains(requirement.RequiredScope) && userId == resource.CreatedBy)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
