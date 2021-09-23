using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;

namespace Api.Services
{
    public class UserInfo : IUserInfo
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserInfo(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public async Task<UserInfoResponse> GetUserInfo()
        {
            var authHeader = _contextAccessor.HttpContext.Request.Headers["Authorization"];

            var accessToken = authHeader.ToString().Split("Bearer")[1].Trim();

            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

            if (disco.IsError)
            {
                throw new System.Exception(disco.Error);
            }

            var userInfoRequest = new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = accessToken
            };

            var userInfoResponse = await client.GetUserInfoAsync(userInfoRequest);
            return userInfoResponse;
        }
    }
}