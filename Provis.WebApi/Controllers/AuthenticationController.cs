using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.userDTO;
using Provis.Core.Interfaces.Services;
using Provis.Core.Entities;
using Provis.Core.Roles;
using Provis.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> RegistrationAsyns([FromBody] UserRegDTO RegDTO)
        {
            var User = new User()
            {
                UserName = RegDTO.Name,
                Surname = RegDTO.Surname,
                
                Email = RegDTO.Email
            };

            await authenticationService.RegistrationAsync(User, RegDTO.Password, SystemRoles.User);

            return Ok();
        }
    }
}
