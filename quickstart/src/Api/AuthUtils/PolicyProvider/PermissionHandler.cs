using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.AuthUtils.PolicyProvider
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            List<string> missingScopes = new();

            if (requirement.PermissionOperator == PermissionOperator.And)
            {
                foreach (var permission in requirement.Permissions)
                {
                    if (!context.User.HasClaim(PermissionRequirement.ClaimType, permission))
                    {
                        // If the user lacks ANY of the required permissions
                        // we mark it as failed.
                        context.Fail();
                        return Task.CompletedTask;
                    }
                }

                // identity has all required permissions
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            foreach (var permission in requirement.Permissions)
            {
                if (context.User.HasClaim(PermissionRequirement.ClaimType, permission))
                {
                    // In the OR case, as soon as we found a matching permission
                    // we can already mark it as Succeed
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            // identity does not have any of the required permissions
            context.Fail();

        // SEND_ERROR:

        //     if (!_httpContextAccessor.HttpContext.Response.HasStarted)
        //     {
        //         missingScopes = requirement.Permissions.ToList();

        //         var missingScopesError = new ScopeError { Error = "Missing required scopes", Scopes = missingScopes.ToArray() };

        //         var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<ScopeError>(missingScopesError));
        //         var httpContext = _httpContextAccessor.HttpContext;
        //         // httpContext.Response.StatusCode = 405;
        //         httpContext.Response.ContentType = "application/json";
        //         httpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        //     }

            return Task.CompletedTask;
        }
    }

    public class ScopeError
    {
        public string Error { get; set; }
        public string[] Scopes { get; set; }
    }
}