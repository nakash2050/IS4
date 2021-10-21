using System.Linq;
using System.Threading.Tasks;
using Api.AuthUtils.PolicyProvider;
using Api.Services;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        private readonly IUserInfo _userInfo;

        public IdentityController(IUserInfo userInfo)
        {
            _userInfo = userInfo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            // HttpContext.Request.
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        [HttpGet("userinfo/alice")]
        [PermissionAuthorize(PermissionOperator.Or, "ride.view")]
        public async Task<IActionResult> GetUserInfoForAlice()
        {
            var userInfo = await _userInfo.GetUserInfo();
            return Ok(userInfo.Json);
        }

        [HttpGet("userinfo/bob")]
        [PermissionAuthorize(PermissionOperator.And, "ride.discard", "ride.book", "ride.view")]
        public async Task<IActionResult> GetUserInfoForBob()
        {
            var userInfo = await _userInfo.GetUserInfo();
            return Ok(userInfo.Json);
        }

        [HttpGet("userinfo/jan")]
        [PermissionAuthorize(PermissionOperator.And, "ride.book", "ride.view")]
        public async Task<IActionResult> GetUserInfoForJan()
        {
            var userInfo = await _userInfo.GetUserInfo();
            return Ok(userInfo.Json);
        }
    }
}