using Microsoft.AspNetCore.Authorization;

namespace AnimalCrossingAPI.Auth
{
    public class ScopeRequirement : IAuthorizationRequirement
    {
        public string RequiredScope { get; }

        public ScopeRequirement(string requiredScope)
        {
            RequiredScope = requiredScope;
        }
    }
}
