using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.userDTO;
using Provis.Core.Interfaces.Services;
using Provis.Core.Entities;
using Provis.Core.Roles;
using System.Threading.Tasks;

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
        public async Task<IActionResult> LoginAsyns([FromBody] UserLogDTO logDTO)
        {
            var token = await authenticationService.LoginAsync(logDTO.Email, logDTO.Password);

            return Ok(new { token = token });
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
    }
}
