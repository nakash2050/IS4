using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Services
{
    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        private readonly IUserInfo _userInfo;

        public CustomJwtBearerEvents(IUserInfo userInfo)
        {
            _userInfo = userInfo;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            JsonElement permissions;
            var userInfo = await _userInfo.GetUserInfo();

            userInfo.Json.TryGetProperty("permission", out permissions);

            if (permissions.ValueKind != JsonValueKind.Undefined)
            {
                var permissionClaims = new List<Claim>();

                if (permissions.ValueKind == JsonValueKind.Array)
                {
                    IEnumerable<JsonElement> arrPermissions = permissions.EnumerateArray().GetEnumerator();

                    permissionClaims.AddRange(arrPermissions.Select(permission => new Claim("permissions", permission.GetString())));
                }
                else
                {
                    permissionClaims.Add(new Claim("permissions", permissions.GetString()));
                }

                var permissionIdentity = new ClaimsIdentity(permissionClaims);

                context.Principal.AddIdentity(permissionIdentity);
            }
        }

        // public override async Task Forbidden(ForbiddenContext context)
        // {
        //     var response = context.Response;

        //     var error = new ErrorResponse { Error = "forbidden", ErrorDescription = context.Result.Failure.Message };

        //     byte[] data = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(error));

        //     response.ContentType = "application/json";
        //     await response.Body.WriteAsync(data, 0, data.Length);
        //     await response.Body.FlushAsync();
        // }

        
    }
}