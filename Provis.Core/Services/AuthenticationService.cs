using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Provis.Core.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Provis.Core.Helpers;
using Provis.Core.Entities;
using Task = System.Threading.Tasks.Task;
using Provis.Core.Exceptions;

namespace Provis.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly SignInManager<User> _signInManager;
        protected readonly IOptions<JwtOptions> _jwtOptions;

        public AuthenticationService(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<JwtOptions> jwtOptions)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._jwtOptions = jwtOptions;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user == null)
            {
                throw new HttpStatusException(System.Net.HttpStatusCode.Unauthorized, "Incorrect email!");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);

            if (!result.Succeeded)
            {
                throw new HttpStatusException(System.Net.HttpStatusCode.Unauthorized, "Incorrect login or password!");
            }

            return GenerateWebToken(user);
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task RegistrationAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                StringBuilder errorMessage = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    errorMessage.Append(error.ToString() + " ");
                }
                throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest,errorMessage.ToString());
            }

        }

        private string GenerateWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                //new Claim(ClaimTypes.Role, "User"),//don't know what to do with roles
            };

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.Value.LifeTime),
                signingCredentials: credentials);
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
