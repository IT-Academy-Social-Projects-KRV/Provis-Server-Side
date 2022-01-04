using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly SignInManager<User> _signInManager;
        protected readonly IJwtService _jwtService;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly IRepository<RefreshToken> _refreshTokenRepository;

        public AuthenticationService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtService jwtService,
            RoleManager<IdentityRole> roleManager,
            IRepository<RefreshToken> refreshTokenRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _roleManager = roleManager;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<UserTokensDTO> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.Unauthorized, "Incorrect login or password!");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);

            if (!result.Succeeded)
            {
                throw new HttpException(System.Net.HttpStatusCode.Unauthorized, "Incorrect login or password!");
            }

            var claims = _jwtService.SetClaims(user);

            var token = _jwtService.CreateToken(claims);
            var refeshToken = _jwtService.CreateRefreshToken();

            var refeshTokenFromDb = await _refreshTokenRepository.Query().FirstOrDefaultAsync(x => x.UserId == user.Id);

            RefreshToken rt = new RefreshToken()
            {
                Token = refeshToken,
                UserId = user.Id
             };

            await _refreshTokenRepository.AddAsync(rt);
            await _refreshTokenRepository.SaveChangesAsync();

            var tokens = new UserTokensDTO()
            {
                Token = token,
                RefreshToken = refeshToken
            };
            
            return tokens;
        }

        public async Task RegistrationAsync(User user, string password, string roleName)
        {
            user.CreateDate = DateTime.UtcNow;
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                StringBuilder errorMessage = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    errorMessage.Append(error.ToString() + " ");
                }
            }

            var findRole = await _roleManager.FindByNameAsync(roleName);

            if (findRole == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<UserTokensDTO> RefreshTokenAsync(UserTokensDTO userTokensDTO)
        {
            var refeshTokenFromDb = await _refreshTokenRepository.Query().FirstOrDefaultAsync(x=>x.Token == userTokensDTO.RefreshToken);

            if(refeshTokenFromDb == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "Invalid refrash token");
            }

            var claims = _jwtService.GetClaimsFromExpiredToken(userTokensDTO.Token);
            var newToken = _jwtService.CreateToken(claims);
            var newRefreshToken = _jwtService.CreateRefreshToken();

            refeshTokenFromDb.Token = newRefreshToken;
            await _refreshTokenRepository.UpdateAsync(refeshTokenFromDb);
            await _refreshTokenRepository.SaveChangesAsync();

            var tokens = new UserTokensDTO()
            { 
                Token = newToken,
                RefreshToken = newRefreshToken
            };

            return tokens;
        }

        public async Task LogoutAsync(UserTokensDTO userTokensDTO)
        {
            var refeshTokenFromDb = await _refreshTokenRepository.Query().FirstOrDefaultAsync(x => x.Token == userTokensDTO.RefreshToken);

            if (refeshTokenFromDb == null)
            {
                return;
            }

            await _refreshTokenRepository.DeleteAsync(refeshTokenFromDb);
            await _refreshTokenRepository.SaveChangesAsync();
        }
    }
}
