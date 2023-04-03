using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Claims;

namespace SwaggerSecurityTrimmingV1.FilterSwagger
{
    public class SecurityTrimming : IDocumentFilter
    {
        private readonly IServiceProvider _provider;

        public SecurityTrimming(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            IHttpContextAccessor http = _provider.GetRequiredService<IHttpContextAccessor>();
            IAuthorizationService auth = _provider.GetRequiredService<IAuthorizationService>();

            foreach (Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription? description in context.ApiDescriptions)
            {
                IEnumerable<AuthorizeAttribute> authAttributes = description.CustomAttributes().OfType<AuthorizeAttribute>();
                IEnumerable<object> allAttributes = description.CustomAttributes();

                bool anonymousAllowed = allAttributes.Any(attr => attr.GetType() == typeof(AllowAnonymousAttribute));

                bool show = true;

                if (!anonymousAllowed && (isForbiddenDueAnonymous(http, authAttributes) || isForbiddenDueRole(http, auth, authAttributes)))
                {
                    show = false;
                }

                if (show)
                    continue; // user passed all permissions checks

                string route = "/" + description.RelativePath.TrimEnd('/');
                OpenApiPathItem path = swaggerDoc.Paths[route];

                // remove method or entire path (if there are no more methods in this path)
                OperationType operation = Enum.Parse<OperationType>(description.HttpMethod, true);
                path.Operations.Remove(operation);
                if (path.Operations.Count == 0)
                {
                    swaggerDoc.Paths.Remove(route);
                }
            }
        }

        private static bool isForbiddenDueRole(
            IHttpContextAccessor http,
            IAuthorizationService auth,
            IEnumerable<AuthorizeAttribute> attributes)
        {
            bool isForbidden = true;

            IEnumerable<Claim> rolesFromClaim = http.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role);

            IEnumerable<string?> rolesFromAttribute = attributes
                        .OfType<AuthorizeAttribute>()
                        .Select(a => a.Roles)
                        .Distinct();

            if (rolesFromAttribute.Any())
            {
                string? roleAttribute = rolesFromAttribute.FirstOrDefault();

                if (roleAttribute != null)
                {
                    string[] roles = roleAttribute.Split(',').Select(rs => rs.Trim()).ToArray();
                    // if roles contains the given role then it mean i have to keep this path

                    bool roleExists = rolesFromClaim.Any(rfc => roles.Contains(rfc.Value));

                    if (roleExists)
                    {
                        isForbidden = false;
                    }
                }
            }

            return isForbidden;
        }

        private static bool isForbiddenDuePolicy(
            IHttpContextAccessor http,
            IAuthorizationService auth,
            IEnumerable<AuthorizeAttribute> attributes)
        {
            IEnumerable<string?> policies = attributes
                .Where(p => !String.IsNullOrEmpty(p.Policy))
                .Select(a => a.Policy)
                .Distinct();

            AuthorizationResult[] result = Task.WhenAll(policies.Select(p => auth.AuthorizeAsync(http.HttpContext.User, p))).Result;
            return result.Any(r => !r.Succeeded);
        }

        private static bool isForbiddenDueAnonymous(
            IHttpContextAccessor http,
            IEnumerable<AuthorizeAttribute> attributes)
        {
            bool forbidden = attributes.Any() && !http.HttpContext.User.Identity.IsAuthenticated;

            return forbidden;
        }
    }
}
