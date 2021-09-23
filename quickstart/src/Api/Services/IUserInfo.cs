using System.Threading.Tasks;
using IdentityModel.Client;

namespace Api.Services
{
    public interface IUserInfo
    {
        Task<UserInfoResponse> GetUserInfo();
    }
}