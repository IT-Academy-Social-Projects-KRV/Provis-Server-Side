using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.userDTO;
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

        [HttpPut]
        [Authorize]
        [Route("changeinfo")]
        public async Task ChangeInfoAsync([FromBody] UserChangeInfoDTO userChangeInfoDTO)
        {
            await _userService.ChangeInfoAsync(UserId, userChangeInfoDTO);
        }
    }
}
