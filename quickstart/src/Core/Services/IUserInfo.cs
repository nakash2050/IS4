using System.Threading.Tasks;
using IdentityModel.Client;

namespace Core.Services
{
    public interface IUserInfo
    {
         Task<UserInfoResponse> GetUserInfo();
    }
}