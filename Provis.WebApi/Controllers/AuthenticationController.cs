using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.userDTO;
using Provis.Core.Interfaces.Services;
using Provis.Core.Entities;
using Provis.Core.Roles;
using System.Threading.Tasks;
using Provis.Core.DTO.UserDTO;

namespace Provis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLogDTO logDTO)
        {
            var tokens = await authenticationService.LoginAsync(logDTO.Email, logDTO.Password);

            return Ok(tokens);
        }

        [HttpPost]
        [Route("logintwostep")]
        public async Task<IActionResult> LoginTwoStepAsync([FromBody] UserTwoFactorDTO twoFactorDTO)
        {
            var tokens = await authenticationService.LoginTwoStepAsync(twoFactorDTO);

            return Ok(tokens);
        }

        [HttpPost]
        [Route("registration")]
        public async Task<IActionResult> RegistrationAsync([FromBody] UserRegDTO regDTO)
        {
            var user = new User()
            {
                UserName = regDTO.Username,
                Surname = regDTO.Surname,
                Name = regDTO.Name,
                Email = regDTO.Email
            };

            await authenticationService.RegistrationAsync(user, regDTO.Password, SystemRoles.User);

            return Ok();
        }

        [HttpPost]
        [Route("refreshToken")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] UserAutorizationDTO userTokensDTO)
        {
            var tokens = await authenticationService.RefreshTokenAsync(userTokensDTO);

            return Ok(tokens);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> LogoutAsync([FromBody] UserAutorizationDTO userTokensDTO)
        {
            await authenticationService.LogoutAsync(userTokensDTO);

            return NoContent();
        }
    }
}
