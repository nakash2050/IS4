using System.Linq;
using System.Threading.Tasks;
using Api.Services;
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

        [HttpGet("userinfo/customer")]
        [Authorize(Policy = "customer")]
        public async Task<IActionResult> GetUserInfoForCustomer()
        {
            var userInfo = await _userInfo.GetUserInfo();
            return Ok(userInfo.Json);
        }

        [HttpGet("userinfo/invoice")]
        [Authorize(Policy = "invoice")]
        public async Task<IActionResult> GetUserInfoForInvoice()
        {
            var userInfo = await _userInfo.GetUserInfo();
            return Ok(userInfo.Json);
        }
    }
}