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
        [Route("changename")]
        public async Task ChangeNameAsync([FromBody] UserChangeNameDTO userChangeNameDTO)
        {
            await _userService.ChangeNameAsync(UserId, userChangeNameDTO);
        }

        [HttpPut]
        [Authorize]
        [Route("changesurname")]
        public async Task ChangeSurnameAsync([FromBody] UserChangeSurnameDTO userChangeSurnameDTO)
        {
            await _userService.ChangeSurnameAsync(UserId, userChangeSurnameDTO);
        }

        [HttpPut]
        [Authorize]
        [Route("changeusername")]
        public async Task ChangeUsernameAsync([FromBody] UserChangeUsernameDTO userChangeUsernameDTO)
        {
            await _userService.ChangeUsernameAsync(UserId, userChangeUsernameDTO);
        }

    }
}
