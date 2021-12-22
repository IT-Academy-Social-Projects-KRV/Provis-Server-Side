using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.Interfaces.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Provis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        [Route("getpersonalinfo")]
        public async Task<IActionResult> GetUserPersonalInfoAsync()
        {
            var userInfo = await _userService.GetUserPersonalInfoAsync(UserId);

            return Ok(userInfo);
        }

        [HttpGet]
        [Authorize]
        [Route("invite")]
        public async Task<IActionResult> GetUserInviteInfoAsync()
        {
            var userInviteList = await _userService.GetUserInviteInfoListAsync(UserId);

            return Ok(userInviteList);
        }
 
    }
}
